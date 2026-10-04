namespace BikeConsulting.Api.Model.User
{
    public class UserModel
    {
        public UserModel() { }
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }
        public int RoleID { get; set; }
        public string MPIN { get; set; }
        public bool Active { get; set; }
        public int SubmittedBy { get; set; }

    }
}
