using System;
using System.Text;
using System.Net;
using OAuthTwitterWrapper;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.IO;
using Logger;

namespace oAuthTwitterWrapper
{
	public class Authenticate
	{
		public TwitAuthenticateResponse AuthenticateMe(string oAuthConsumerKey, string oAuthConsumerSecret, string oAuthUrl)
		{
            EventLogWriter logWriter = new EventLogWriter("oAuthTwitterWrapper");

			TwitAuthenticateResponse twitAuthResponse;
			// Do the Authenticate
			const string authHeaderFormat = "Basic {0}";

			var authHeader = string.Format(authHeaderFormat,
										   Convert.ToBase64String(
											   Encoding.UTF8.GetBytes(Uri.EscapeDataString(oAuthConsumerKey) + ":" +

																	  Uri.EscapeDataString((oAuthConsumerSecret)))

											   ));
			const string postBody = "grant_type=client_credentials";
			HttpWebRequest authRequest = (HttpWebRequest)WebRequest.Create(oAuthUrl);

			authRequest.Headers.Add("Authorization", authHeader);
			authRequest.Method = "POST";
			authRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
			authRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			using (Stream stream = authRequest.GetRequestStream())
			{
				byte[] content = Encoding.ASCII.GetBytes(postBody);
				stream.Write(content, 0, content.Length);
			}
			authRequest.Headers.Add("Accept-Encoding", "gzip");
		    try
		    {
                 WebResponse authResponse = authRequest.GetResponse();
                // deserialize into an object
			    using (authResponse)
			    {
				    using (var reader = new StreamReader(authResponse.GetResponseStream()))
				    {
					    JavaScriptSerializer js = new JavaScriptSerializer();
					    var objectText = reader.ReadToEnd();
					    twitAuthResponse = JsonConvert.DeserializeObject<TwitAuthenticateResponse>(objectText);    
				    }
			    }

			    return twitAuthResponse;

		    }
		    catch (WebException e)
		    {
                logWriter.WriteErrorToEventLog(string.Format("Authenticate.TwitAuthenticateResponse: {0}", e.Message)); 
		    }

            return new TwitAuthenticateResponse();
		}
	}
}
