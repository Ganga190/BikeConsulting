namespace BikeConsulting.Api.Model.Auth
{
    public class UserLoginModel
    {
        public string UserName { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public bool isEncrypted { get; set; }
        public string? otp { get; set; } = string.Empty;
    }
}
