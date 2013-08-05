
namespace TwitterManager.Models
{
    public class StoredProcedures
    {
        public const string SqlInsertTwitterItems = "usp_TService_insert_twitter_item";
        public const string SqlGetoAuthData = "usp_TService_getOAuthTokens '{0}'";
        public const string SqlGetMinMessageIdForUser = "select convert(nvarchar(18), min(messageid)) minId from TService_rowdata where ScreenName = '{0}'";
        public const string SqlGetMaxMessageIdForUser = "select convert(nvarchar(18), max(messageid)) maxId from TService_rowdata where ScreenName = '{0}'";
        public const string SqlGetScreenNames = "usp_TService_get_screen_names";
        public const string SqlGetMaxRunId = "select Max(RunId)RunId from TService_QueriesTracking";
        public const string SqlUpdateQueriesTracking = "usp_TService_Update_Queries_Tracking '{0}' ,'{1}' ,'{2: yyyy/MM/dd HH:mm:ss}', '{3}', {4}, {5}";
        public const string SqlUpdateScreenNames_LastUpdated = "update TService_ScreenNamesToLoad set lastupdated = '{0: yyyy/MM/dd HH:mm:ss}' where screenname = '{1}'";
        public const string SqlUpdateScreenNames_Deactivate = "update TService_ScreenNamesToLoad set lastupdated = '{0: yyyy/MM/dd HH:mm:ss}' , active = 0 where screenname = '{1}'";
        public const string SqlInsertRowsUpdateLog = "insert into tservice_rowsupdatelog(hostName,updatedate,numofrowes,OAuth)values('{0}','{1: yyyy/MM/dd HH:mm:ss}',{2},{3})";
    }
}
