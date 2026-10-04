namespace BikeConsulting.Business.Contract
{
    public interface IUserComponent
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
