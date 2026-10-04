using System.Data;

namespace BikeConsulting.Framework.BikeConsultingConstants

{
    public static class BikeConsultingConstants
    {

        #region Route Parameters

        //User
        public const string ROUTE_URL_USER = "api/users";
        public const string ROUTE_URL_USER_BY_ID = "api/users/{id}";
        public const string ROUTE_URL_SAVE_USER = "api/users";
        public const string ROUTE_URL_UPDATE_USER = "api/users/{id}";
        public const string ROUTE_URL_DELETE_USER = "api/users/{id}";

        #endregion

        #region Stored Procedure
        //Validate User
        public static string SP_SQL_VALIDATE_USER = "exec [User].[ProcValidateUserLogin] @UserName,@Password,@OTP,@ReturnValue OUTPUT";

        //User
        public static string SP_SQL_SAVE_USER = "exec [User].[ProcSaveUser] @JsonSaveUser,@RecordType,@ReturnVal";
        public static string SP_SQL_FETCH_USER = "exec [User].[ProcFetchUser] @UserID,@SearchText,@pPageNo,@pRecordCount,@ReturnValue OUTPUT";

        #endregion

        #region SQL Parameters

        public static string SP_PARAM_SEARCH_TEXT = "@SearchText";
        public static string SP_PARAM_RETURN_VALUE = "@ReturnValue";
        public static string SP_PARAM_PAGE_NO = "@pPageNo";
        public static string SP_PARAM_RECORD_COUNT = "@pRecordCount";
        public static string SP_PARAM_RECORD_TYPE = "@RecordType";
        public static string SP_PARAM_RETURN_VAL = "@ReturnVal";

        public static string SP_USERNAME = "@UserName";
        public static string SP_PASSWORD = "@Password";
        public static string SP_PARAM_OTP = "@OTP";

        //User
        public static string SP_PARAM_USER_ID = "@UserID";
        public static string SP_PARAM_SAVE_USER = "@JsonSaveUser";

        #endregion

    }
}
