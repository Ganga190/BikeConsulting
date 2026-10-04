using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeConsulting.Dto.User
{
    public class UserDto
    {
        public UserDto() { }
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int Age { get; set; }
        public int RoleID { get; set; }
        public string MPIN { get; set; } = string.Empty;
        public bool Active { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public List<UserProfileDto> UserProfile { get; set; } = new();
    }

    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int UserRoleID { get; set; }
        public bool HasMpin { get; set; }
    }
}
