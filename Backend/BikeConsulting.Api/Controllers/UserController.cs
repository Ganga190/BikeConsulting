using AutoMapper;
using BikeConsulting.Api.Model.User;
using BikeConsulting.Business.Contract;
using BikeConsulting.Dto.User;
using BikeConsulting.Framework.BikeConsultingConstants;
using BikeConsulting.Framework.Utility;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BikeConsulting.Api.Controllers
{

    [ApiController]
    public class UserController : ControllerBase
    {
        #region Declaration
        private readonly IUserComponent _userComponent;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IValidator<UserModel> _validator;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor to initialiaze the business object
        /// </summary>
        /// <param name="userComponent"></param>
        public UserController(IUserComponent userComponent,
                              ILogger<UserController> logger,
                              IMapper mapper,
                              IValidator<UserModel> validator)
        {
            _userComponent = userComponent;
            _logger = logger;
            _mapper = mapper;
            _validator = validator;
        }
        #endregion

        #region User

        #region FetchUserInfo
        /// <summary>
        /// Retrieves user list to display the User Management Grid.
        /// </summary>
        /// <remarks>
        /// It supports pagination and search functionality to efficiently manage and display user data.
        /// This API is also used to fetch user details of selected user in the grid
        /// </remarks>

        /// <param name="userId">To display the user information for edit and duplicate. Can be null or empty to fetch users list.</param>
        /// <param name="searchText">The text used to filter user information based on name, email, or other criteria.</param>
        /// <param name="pageNo">The page number for pagination, used to retrieve a specific subset of data.</param>
        /// <param name="recordCount">The number of records to fetch per page, adding in paginated data retrieval.</param>
        [Route((BikeConsultingConstants.ROUTE_URL_USER))]
        [HttpGet]
        [Authorize]
        public string FetchUsers(int? userId = null, string searchText = "", int? pageNo = null, int? recordCount = null)
        {
            return _userComponent.FetchUsers(userId, searchText, pageNo, recordCount);
        }
        #endregion

        #region SaveUser
        /// <summary>
        /// This API is used to save (ie CRUD) user details into the database
        /// </summary>
        /// <remarks>
        /// Specifies the type of CRUD (Record Type: 1-Create, 2-Update, 3-Delete). <br />
        /// Mandatory fields for External (Customer) user create : First Name, Last Name, Email, Password. <br />
        /// Mandatory fields for AD (Internal) user  create : First Name, Email. <br />
        /// Mandatory fields for User update : UserID. <br />
        /// </remarks>

        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost, Route(BikeConsultingConstants.ROUTE_URL_SAVE_USER)]
        [Authorize]
        public int SaveUser([FromBody] UserModel model)
        {
            int userId = -1;
            var result = _validator.Validate(model);
            if (result.IsValid)
            {
                UserDto userDto = _mapper.Map<UserDto>(model);
                userDto.CreatedBy = model.SubmittedBy;
                string inputJson = JsonConvert.SerializeObject(userDto);
                userId = _userComponent.SaveUser(inputJson, (int)Enumerations.ActionEnum.Save);
            }
            return userId;

        }
        #endregion

        #region UpdateUser
        [Authorize]
        [HttpPut(BikeConsultingConstants.ROUTE_URL_UPDATE_USER)]
        public int UpdateUser(int id, [FromBody] UserModel model)
        {
            int userId = -1;
            var result = _validator.Validate(model);
            if (result.IsValid)
            {
                UserDto user = _mapper.Map<UserDto>(model);
                user.UserID = id;
                user.UpdatedBy = model.SubmittedBy;
                string inputJson = JsonConvert.SerializeObject(user);
                userId = _userComponent.SaveUser(inputJson, (int)Enumerations.ActionEnum.Update);
            }
            return userId;
        }
        #endregion

        #region DeleteUser
        [HttpDelete(BikeConsultingConstants.ROUTE_URL_DELETE_USER)]
        public int DeleteUser(int id, [FromBody] UserModel model)
        {
            int userId = -1;
            var result = _validator.Validate(model);
            if (result.IsValid)
            {
                UserDto nominationDto = _mapper.Map<UserDto>(model);
                nominationDto.UserID = id;

                string inputJson = JsonConvert.SerializeObject(nominationDto);
                userId = _userComponent.SaveUser(inputJson, (int)Enumerations.ActionEnum.Delete);
            }
            return userId;
        }
        #endregion


        #endregion
    }
}
