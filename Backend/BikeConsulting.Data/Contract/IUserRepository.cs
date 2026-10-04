using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeConsulting.Data.Contract
{
    public interface IUserRepository
    {
        #region Users
        string FetchUsers(int? userId = null, string searchText = "", int? pageNo = null, int? recordCount = null);
        int SaveUser(string inputVal, int recordType);
        #endregion

        #region Auth Validate User
        string ValidateUser(string username, string password = "", string otp = "");
        #endregion

    }
}
