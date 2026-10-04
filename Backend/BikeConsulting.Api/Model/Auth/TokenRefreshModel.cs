using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeConsulting.Api.Model.Auth
{
    public class TokenRefreshModel
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddDays(7);
    }
}
