#region Namespaces
using BikeConsulting.Api.Model.Auth;
using BikeConsulting.Business.Contract;
using BikeConsulting.Dto.User;
using BikeConsulting.Framework.BikeConsultingConstants;
using FrameworkUtility = BikeConsulting.Framework.Utility.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
#endregion


namespace BikeConsulting.Api.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {

        #region Declaration
        private readonly IUserComponent _userComponent;
        private readonly IConfiguration _config;
        private static List<TokenRefreshModel> refreshTokens = new List<TokenRefreshModel>();
        private readonly ILogger<AuthController> _logger;

        #endregion

        #region Constructor
        public AuthController(IConfiguration config,
                               ILogger<AuthController> logger,
                               IUserComponent userComponent)
        {
            _userComponent = userComponent;
            _config = config;
            _logger = logger;
        }
        #endregion

        #region Generates Token & Gets User data
        /// <summary>
        /// Generates token by using HS256 algorithm and validates user.
        /// Gets user data, token and OTP for the given username and password.
        /// </summary>
        /// <remarks>
        ///  Both Username i.e Email and Password is mandatory for External Users
        /// Only Username i.e Email is mandatory for AD Users
        /// </remarks>
        /// <param name="user"></param>
        /// <returns>token, OTP and user information</returns>
        [HttpPost("api/login")]
        public IActionResult GenerateJwtToken([FromBody] UserLoginModel user)
        {
            UserDto userDto = new UserDto();
            string userData = string.Empty;
            string tokenString = string.Empty;
            string refreshToken = string.Empty;
            string pwd = user.Password ?? string.Empty;

            if (!user.isEncrypted && !string.IsNullOrEmpty(pwd))
            {
                pwd = FrameworkUtility.EncryptPassword(pwd);
            }

            try
            {
                userData = _userComponent.ValidateUser(user.UserName, pwd);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
            if (userData == "INACTIVE")
            {
                return Unauthorized(new { message = "Your account is inactive. Please contact the administrator." });
            }
            if (string.IsNullOrEmpty(userData) || userData == "{}")
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            userDto = JsonConvert.DeserializeObject<UserDto>(userData);
            var profile = userDto?.UserProfile?.FirstOrDefault();

            if (profile == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            string secret = _config["Jwt:Key"] ?? _config["JwtConfig:secret"] ?? throw new InvalidOperationException("JWT Secret is not configured.");
            double exp = Convert.ToDouble(_config["Jwt:ExpirationInMinutes"] ?? _config["JwtConfig:expirationInMinutes"] ?? "60");

            var key = Encoding.UTF8.GetBytes(secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, profile.Email),
                    new Claim(ClaimTypes.NameIdentifier, profile.UserId.ToString()),
                    new Claim(ClaimTypes.Role, profile.RoleName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(exp),
                Issuer = _config["Jwt:Issuer"] ?? _config["JwtConfig:Issuer"],
                Audience = _config["Jwt:Audience"] ?? _config["JwtConfig:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            tokenString = tokenHandler.WriteToken(token);

            refreshToken = GenerateRefreshToken();

            refreshTokens.Add(new TokenRefreshModel
            {
                Token = refreshToken,
                Username = user.UserName,
                Password = user.Password
            });

            var userDataObject = JsonConvert.DeserializeObject(userData);

            return Ok(new
            {
                token = tokenString,
                refreshtoken = refreshToken,
                userid = profile.UserId,
                email = profile.Email,
                username = profile.FirstName,
                role = profile.RoleID,
                roleName = profile.RoleName,
                userRoleId = profile.UserRoleID,
                hasMpin = profile.HasMpin
                //data = userDto
            });
        }

        #endregion

        #region Refresh Token
        /// <summary>
        /// Genrate token with username and password if the token is expired
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] TokenRequestModel model)
        {
            var refreshToken = refreshTokens.Find(rt => rt.Token == model.RefreshToken);

            if (refreshToken == null || refreshToken.ExpiryDate <= DateTime.UtcNow)
            {
                return Unauthorized();
            }
            UserLoginModel user = new UserLoginModel { UserName = refreshToken.Username, Password = refreshToken.Password };
            var token = GenerateJwtToken(user);
            dynamic tn = JsonConvert.SerializeObject(token);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(tn);
            return Ok(new { Token = data.Value.token.ToString() });

        }
        #endregion

        #region Private Methods
        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
        #endregion

    }
}
