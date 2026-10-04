using BikeConsulting.Data.Contract;
using BikeConsulting.Dto.User;
using BikeConsulting.Entities.DbContexts;
using BikeConsulting.Framework.BikeConsultingConstants;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BikeConsulting.Data.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly BikeConsultingContext _exContext;
        private IDbConnection DbConnection => _exContext.Database.GetDbConnection();

        public UserRepository(BikeConsultingContext exContext)
        {
            this._exContext = exContext;
        }

        #region User

        #region FetchUser
        /// <summary>
        /// Fetch to display the EmployeeDetails table value using empId,pageNo & recordCount using Dapper
        /// </summary>        
        /// <param name="userId"></param>
        /// <param name="searchText"></param>
        /// <param name="pageNo"></param>
        /// <param name="recordCount"></param>
        /// <returns>EmployeeDetail table values (JSON)</returns>
        public string FetchUsers(int? userId = null, string searchText = "", int? pageNo = null, int? recordCount = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add(BikeConsultingConstants.SP_PARAM_USER_ID, userId);
            parameters.Add(BikeConsultingConstants.SP_PARAM_SEARCH_TEXT, string.IsNullOrEmpty(searchText) ? null : searchText);
            parameters.Add(BikeConsultingConstants.SP_PARAM_PAGE_NO, pageNo);
            parameters.Add(BikeConsultingConstants.SP_PARAM_RECORD_COUNT, recordCount);
            parameters.Add(BikeConsultingConstants.SP_PARAM_RETURN_VALUE, dbType: DbType.String, direction: ParameterDirection.Output, size: -1);

            DbConnection.Execute("[User].[ProcFetchUser]", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<string>(BikeConsultingConstants.SP_PARAM_RETURN_VALUE) ?? string.Empty;
        }
        #endregion

        #region SaveUser
        /// <summary>
        /// Save, update, or delete user record
        /// </summary>
        /// <param name="inputVal"></param>
        /// <param name="recordType"></param>
        /// <returns></returns>
        public int SaveUser(string inputVal, int recordType)
        {
            var parameters = new DynamicParameters();
            parameters.Add(BikeConsultingConstants.SP_PARAM_SAVE_USER, inputVal);
            parameters.Add(BikeConsultingConstants.SP_PARAM_RECORD_TYPE, recordType);
            parameters.Add(BikeConsultingConstants.SP_PARAM_RETURN_VAL, dbType: DbType.Int32, direction: ParameterDirection.Output);

            DbConnection.Execute("[User].[ProcSaveUser]", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<int>(BikeConsultingConstants.SP_PARAM_RETURN_VAL);
        }
        #endregion

        #endregion

        #region Validate User      
        /// <summary>
        /// Validate user login credentials using Dapper
        /// </summary>
        public string ValidateUser(string username, string password = "", string otp = "")
        {
            var parameters = new DynamicParameters();
            parameters.Add(BikeConsultingConstants.SP_USERNAME, username);
            parameters.Add(BikeConsultingConstants.SP_PASSWORD, (string.IsNullOrEmpty(password) || password == "string") ? null : password);
            parameters.Add(BikeConsultingConstants.SP_PARAM_OTP, string.IsNullOrEmpty(otp) ? null : otp);
            parameters.Add(BikeConsultingConstants.SP_PARAM_RETURN_VALUE, dbType: DbType.String, direction: ParameterDirection.Output, size: -1);

            DbConnection.Execute("[User].[ProcValidateUserLogin]", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<string>(BikeConsultingConstants.SP_PARAM_RETURN_VALUE) ?? string.Empty;
        }
        #endregion

    }
}
