using System;
using System.Xml.Linq;
using Logger;

namespace TwitterManager.Models
{
    public class TwitterItem
    {
        public decimal MessageId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        public string ScreenName { get; set; }
        public string Language { get; set; }
        public decimal InReplyToStatusId { get; set; }
        public int InReplyToUserId { get; set; }
        public string InReplyToScreenName { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }

        public TwitterItem (XElement tweet)
        {
            EventLogWriter logWriter = new EventLogWriter("TwitterManager");

            //Message Id
            try
            {
                MessageId = decimal.Parse(tweet.Element("id").Value);
            }
            catch (Exception ex)
            {
                logWriter.WriteErrorToEventLog(string.Format("TwitterItem: Exception = {0}", ex.Message));
            }

            //Created At
            try
            {
                string date = tweet.Element("created_at").Value;
                CreatedAt = DateTime.Parse(date.Substring(date.Length - 4, 4) + date.Substring(4, 19));
            }
            catch (Exception ex)
            {
                logWriter.WriteErrorToEventLog(string.Format("TwitterItem: Exception = {0}", ex.Message));
            }

            //User Id
            try
            {
                UserId = int.Parse(tweet.Element("user").Element("id").Value);
            }
            catch (Exception ex)
            {
                logWriter.WriteErrorToEventLog(string.Format("TwitterItem: Exception = {0}", ex.Message));
            }

            //Message
            Message = tweet.Element("text").Value;

            //Screen Name
            ScreenName = tweet.Element("user").Element("screen_name").Value;

            //Language
            Language = tweet.Element("user").Element("lang").Value;

            //InReplyToStatusId
            try
            {
                InReplyToStatusId = tweet.Element("in_reply_to_status_id").Value == "" ?0 :decimal.Parse(tweet.Element("in_reply_to_status_id").Value);
            }
            catch (Exception ex)
            {
                logWriter.WriteErrorToEventLog(string.Format("TwitterItem: Exception = {0}", ex.Message));
            }

            //InReplyToUserId
            try
            {
                InReplyToUserId = tweet.Element("in_reply_to_user_id").Value == "" ?0 :int.Parse(tweet.Element("in_reply_to_user_id").Value);
            }
            catch (Exception ex)
            {
                logWriter.WriteErrorToEventLog(string.Format("TwitterItem: Exception = {0}", ex.Message));
            }

            //InReplyToScreenName
            InReplyToScreenName = tweet.Element("in_reply_to_screen_name").Value;
            
            UploadedBy = Environment.MachineName;
            
            UploadedAt = DateTime.Now;
        }
    }
}
