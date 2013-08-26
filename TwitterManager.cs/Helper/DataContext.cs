using System;
using System.Collections.Generic;
using System.Data;
using oAuthTwitterWrapper;
using TwitterManager.Models;

namespace TwitterManager.Helper
{
    public class DataContext
    {
        public static void InsertTwitterItems(List<TwitterItem> items)
        {
            DataHelper.InsertTwitterItems(items);
        }

        public static void InsertRowsUpdateLog(string hostName, int numofrowes, int oauth)
        {
            DataHelper.InsertRowsUpdateLog(hostName, numofrowes, oauth);
        }

        public static string GetMinMessageIdForUser(string screenName)
        {
            DataTable table = DataHelper.GetMinMessageIdForUser(screenName);
            string min = table.Rows.Count > 0 ? table.Rows[0].Field<string>("minId") : "";
            return min;
        }
        
        public static string GetMaxMessageIdForUser(string screenName)
        {
            DataTable table = DataHelper.GetMaxMessageIdForUser(screenName);
            string max = table.Rows.Count > 0 ? table.Rows[0].Field<string>("maxId") : "";
            return max;
        }
        
        public static int GetMaxRunID()
        {
            DataTable table = DataHelper.GetMaxRunID();
            int max = table.Rows.Count > 0 ? table.Rows[0].Field<int>("RunId") : 0;
            return max;
        }

        public static void UpdateScreenNames_LastUpdated(string screenName)
        {
            DataHelper.UpdateScreenNames_LastUpdated(screenName);
        }
       
        public static void UpdateScreenNames_Deactivate(string screenName)
        {
            DataHelper.UpdateScreenNames_Deactivate(screenName);
        }

        public static void UpdateQueriesTracking(string screenName, string massage, int runId, int rowesCount)
        {
            DataHelper.UpdateQueriesTracking(screenName, massage, Environment.MachineName, runId, rowesCount);
        }

        public static OAuthData GetoAuthData()
        {
            DataTable table = DataHelper.GetoAuthData(Environment.MachineName);
            OAuthData data = new OAuthData();

            foreach (DataRow row in table.Rows)
            {
                data.OAuthConsumerKey = row.Field<string>("ConsumerKey");
                data.OAuthConsumerSecret = row.Field<string>("ConsumerSecret");
            }
            return data;
        }

        public static List<ScreenNameToLoad> GetScreenNames()
        {
            DataTable table = DataHelper.GetScreenNames();
            List<ScreenNameToLoad> names = new List<ScreenNameToLoad>();

            foreach (DataRow row in table.Rows)
            {
                ScreenNameToLoad name = new ScreenNameToLoad
                                            {
                                                ScreenName = row.Field<string>("screenname"),
                                                IsFirstTime = row.Field<int>("count") > 0 ? false : true                                            
                                            };
                names.Add(name);
            }

            return names;
        }
       
    }
}

