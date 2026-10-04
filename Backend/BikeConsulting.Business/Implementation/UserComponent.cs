using BikeConsulting.Business.Contract;
using BikeConsulting.Data.Contract;

namespace BikeConsulting.Business.Implementation
{
    public class UserComponent : IUserComponent
    {
        #region Declaration
        private readonly IUserRepository userRepository;
        #endregion

        #region Constructor
        public UserComponent(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        #endregion


        #region User

        #region FetchUserInfo
        public string FetchUsers(int? userId = null, string searchText = "", int? pageNo = null, int? recordCount = null)
        {
            return userRepository.FetchUsers(userId, searchText, pageNo, recordCount);
        }

        #endregion

        #region SaveUser
        public int SaveUser(string inputVal, int recordType)
        {
            return userRepository.SaveUser(inputVal, recordType);
        }

        #endregion

        #endregion

        #region Auth Validate User
        #region ValidateUser
        public string ValidateUser(string username, string password = "", string otp = "")
        {
            return userRepository.ValidateUser(username, password, otp);
        }
        #endregion
        #endregion


    }
}
