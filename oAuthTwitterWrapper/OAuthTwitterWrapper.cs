using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;
using oAuthTwitterWrapper;

namespace OAuthTwitterWrapper
{
    public class OAuthTwitterWrapper
    {
        private const string OAuthConsumerKey = "dlwIzwzXwBTY0BiPei3Yg";
        private const string OAuthConsumerSecret = "dorC5hwB8t0a3HlYbMkuW0tN2QClgdI8pahfRMEcJ8";
        private const string OAuthUrl = "https://api.twitter.com/oauth2/token";
        private string timelineUrl; 
        private const string SearchFormat = "https://api.twitter.com/1.1/search/tweets.json?q={0}";
        private const string SearchQuery = "%23test";
		private readonly string searchUrl = string.Format(SearchFormat, SearchQuery);

        public List<XmlDocument> GetMyTimeline(string url,OAuthData oAuthData)
        {
            timelineUrl = url;
            var authenticate = new Authenticate();
            TwitAuthenticateResponse twitAuthResponse = authenticate.AuthenticateMe(oAuthData.OAuthConsumerKey, oAuthData.OAuthConsumerSecret, OAuthUrl);

            // Do the timeline
			var timeLineJson = new Utility().RequstJson(timelineUrl, twitAuthResponse.token_type, twitAuthResponse.access_token);

            List<XmlDocument> xmls = new List<XmlDocument>();

            if (timeLineJson == string.Empty || timeLineJson == "[]")
                return xmls;

            timeLineJson = timeLineJson.Substring(3, timeLineJson.Length - 6);
            var timeLines = timeLineJson.Split(new [] { "\"},{\"" }, System.StringSplitOptions.None);

            foreach (string line in timeLines)
                xmls.Add(JsonConvert.DeserializeXmlNode("{\"root\":[{\"" + line + "\"}]}"));              

            return xmls;
        }

		public string GetSearch()
		{
			var searchJson = string.Empty;
			var authenticate = new Authenticate();
			TwitAuthenticateResponse twitAuthResponse = authenticate.AuthenticateMe(OAuthConsumerKey, OAuthConsumerSecret, OAuthUrl);

			// Do the timeline
			var utility = new Utility();
			searchJson = utility.RequstJson(searchUrl, twitAuthResponse.token_type, twitAuthResponse.access_token);

			return searchJson;
		}
    }
}
