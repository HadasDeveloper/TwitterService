using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using oAuthTwitterWrapper;
using TwitterManager.Helper;
using TwitterManager.Models;

namespace TwitterManager
{
    public class Reader
    {  
        private bool finishedFlag;
        private bool finishedToProcessScreenName;
        private int pageCounter;
        private int authenticateMessageCounter;
        private int statusCounter;
        private int runId;
        private bool done;
        private string currentScreenName = "";
        private string timelineUrl;
        private OAuthData oAuthData = new OAuthData();

        private const string IncludeRts = "1";
        private const string ExcludeReplies = "0";
        private const string Count = "200";
        private const string TimelineFormatFirstTime = 
            "https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={0}&amp;include_rts={1}&amp;exclude_replies={2}{3}&amp;count={4}";
        private const string TimelineFormatNotFirstTime = 
            "https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={0}&amp;include_rts={1}&amp;exclude_replies={2}&amp;since_id={3}&amp;count={4}";

        public void Read()
        {
            authenticateMessageCounter = 0;

            //This function will get the twitter authentication data for this host name from the db
            oAuthData = DataContext.GetoAuthData();
            if (oAuthData.OAuthConsumerKey == null || oAuthData.OAuthConsumerSecret == null)
                return;

            //This function will get the twitter accounts from the db
            List<ScreenNameToLoad> screenNamesToLoad = DataContext.GetScreenNames();

            if (screenNamesToLoad.Count == 0)
                return;

            runId = DataContext.GetMaxRunID() + 1;

            foreach (var screenNameToLoad in screenNamesToLoad)
            {
                //Console.WriteLine(screenNameToLoad.ScreenName);
                if (done)
                    break;

                currentScreenName = screenNameToLoad.ScreenName;

                /*-----*/      
                string maxid = DataContext.GetMaxMessageIdForUser(currentScreenName);

                try
                {
                    ReadTweets(screenNameToLoad.ScreenName, screenNameToLoad.IsFirstTime);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in foreach(var screenNameToLoad ... : ReadTweets --> " + e.Message );
                }

                while (!finishedFlag)
                    Thread.Sleep(500);

                finishedFlag = false;
                finishedToProcessScreenName = false;
                pageCounter = 0;

                /*-----*/
                Console.WriteLine(string.Format("{0, 5}  {1, 5}  {2, 5}", currentScreenName, maxid, DataContext.GetMaxMessageIdForUser(currentScreenName)));
                Console.WriteLine(string.Format("aouth: {0}", authenticateMessageCounter));
            }
            Console.ReadKey();
            DataContext.InsertRowsUpdateLog(Environment.MachineName, statusCounter, authenticateMessageCounter);            
        }

        public void ReadTweets(string screenName, bool isFirstTime)
        {
            bool multipleFlag = false;
            string maxId = "";
            string sinceId = "";

            while (!finishedToProcessScreenName && pageCounter < 16)
            {
                pageCounter++;

                if (isFirstTime)
                {
                    string maxIdInDB = DataContext.GetMinMessageIdForUser(screenName);
                    decimal maxIdInt = 0;
                    try
                    {
                        maxIdInt = maxIdInDB == null ? 0 : decimal.Parse(maxIdInDB) - 1;
                    }
                    catch (Exception e )
                    {
                        Console.WriteLine(e.Message);
                    }

                    maxId = pageCounter == 1 ? "" : "&amp;max_id=" + maxIdInt;
                    timelineUrl = string.Format(TimelineFormatFirstTime, screenName, IncludeRts, ExcludeReplies, maxId, Count);
                }
                else
                {   //if first 200 rows get sinceid from DB alse use the original and the id of the oldest new tweet as maxid 
                    string id = multipleFlag == false ? (sinceId = DataContext.GetMaxMessageIdForUser(screenName)) : sinceId + "&amp;max_id=" + maxId;
                    timelineUrl = string.Format(TimelineFormatNotFirstTime, screenName, IncludeRts, ExcludeReplies, id, Count);
                }

                OAuthTwitterWrapper.OAuthTwitterWrapper oAuthT = new OAuthTwitterWrapper.OAuthTwitterWrapper();

                TwitterData tData = oAuthT.GetMyTimeline(timelineUrl, oAuthData);
                List<TwitterItem> items = new List<TwitterItem>();
                
                // convert xml to twitter item
                
                foreach (XmlDocument status in tData.xmls)
                {
                    XElement xml = XElement.Load(new XmlNodeReader(status));
                    items.Add(new TwitterItem(xml));

                    statusCounter++;
                }

                if (items.Count < 200)
                {
                    finishedToProcessScreenName = true;
                    
                    //insert rows to DB
                    if (items.Count > 0)
                        DataContext.InsertTwitterItems(items);
                    
                    DataContext.UpdateScreenNames_LastUpdated(currentScreenName);
                }
                //insert rows to DB
                else
                {
                    Console.WriteLine(String.Format("sinceid = {0, 2}   maxid = {1, 2} ", sinceId,maxId));
                    DataContext.InsertTwitterItems(items);
                    multipleFlag = true;
                    
                    //get the oldest new tweet's id (maxid)
                    maxId = items[items.Count - 1].MessageId.ToString();
                    DataContext.UpdateScreenNames_LastUpdated(currentScreenName);
                }

                Console.WriteLine("stutus counter = " + items.Count);

                authenticateMessageCounter++;               

                if (authenticateMessageCounter >= 120)
                {
                    Console.WriteLine(" authenticate Message Counter = " + authenticateMessageCounter);
                    //DataContext.UpdateScreenNames_LastUpdated(currentScreenName);
                    DataContext.UpdateQueriesTracking(screenName, tData.error, runId, items.Count);
                    done = true;
                    break;
                }

                if (tData.error == null)
                    tData.error = "HTTP/1.1 200 OK";

                DataContext.UpdateQueriesTracking(screenName, tData.error, runId, items.Count);
            }

            finishedFlag = true;
        }

    }
}
