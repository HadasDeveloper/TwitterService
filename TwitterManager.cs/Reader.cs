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
        private const string TimelineFormatFirstTime = "https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={0}&amp;include_rts={1}&amp;exclude_replies={2}{3}&amp;count={4}";
        private const string TimelineFormatNotFirstTime = "https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={0}&amp;include_rts={1}&amp;exclude_replies={2}&amp;since_id={3}&amp;count={4}";

        public void Read()
        {
            authenticateMessageCounter = 0;

            //This function will get the twitter authentication data for this host name from the db
            oAuthData = DataContext.GetoAuthData();
            if (oAuthData == null)
                return;

            //This function will get the twitter accounts from the db
            List<ScreenNameToLoad> screenNamesToLoad = DataContext.GetScreenNames();//

            //List<ScreenNameToLoad> screenNamesToLoad = new List<ScreenNameToLoad>();//
            //ScreenNameToLoad ScreenName = new ScreenNameToLoad();//
            //ScreenName.ScreenName = "dailyfutures";//
            //ScreenName.IsFirstTime = true;//
            //screenNamesToLoad.Add(ScreenName);//

            if (screenNamesToLoad.Count == 0)
                return;

            runId = DataContext.GetMaxRunID() + 1;

            foreach (var screenNameToLoad in screenNamesToLoad)
            {
                Console.WriteLine(screenNameToLoad.ScreenName);
                if (done)
                    break;

                currentScreenName = screenNameToLoad.ScreenName;
              
                ReadTweets(screenNameToLoad.ScreenName, screenNameToLoad.IsFirstTime);

                while (!finishedFlag)
                    Thread.Sleep(500);

                finishedFlag = false;
                finishedToProcessScreenName = false;
                pageCounter = 0;
            }

            DataContext.InsertRowsUpdateLog(Environment.MachineName, statusCounter, authenticateMessageCounter);            
        }

        public void ReadTweets(string screenName, bool isFirstTime)
        {
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

                    string maxId = pageCounter == 1 ? "" : "&amp;max_id=" + maxIdInt;
                    timelineUrl = string.Format(TimelineFormatFirstTime, screenName, IncludeRts, ExcludeReplies, maxId, Count);
                }
                else
                {
                    string sinceId = DataContext.GetMaxMessageIdForUser(screenName);
                    timelineUrl = string.Format(TimelineFormatNotFirstTime, screenName, IncludeRts, ExcludeReplies, sinceId, Count);
                }

                OAuthTwitterWrapper.OAuthTwitterWrapper oAuthT = new OAuthTwitterWrapper.OAuthTwitterWrapper();

                TwitterData tData = new TwitterData();

                tData = oAuthT.GetMyTimeline(timelineUrl, oAuthData);
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
                    DataContext.UpdateScreenNames_LastUpdated(currentScreenName);
                }
                else//insert to DB
                    DataContext.InsertTwitterItems(items);

                Console.WriteLine("stutus counter = " + items.Count);

                authenticateMessageCounter++;               

                if (authenticateMessageCounter >= 100)
                {
                    Console.WriteLine(" authenticate Message Counter = " + authenticateMessageCounter);
                    DataContext.UpdateScreenNames_LastUpdated(currentScreenName);
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
