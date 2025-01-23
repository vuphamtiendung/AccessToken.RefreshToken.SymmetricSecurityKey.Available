using Microsoft.AspNetCore.Routing.Constraints;

namespace AccessRefreshToken.Model.TokenModel
{
    public class AccessTokenModel
    {
        public string ? SecretKey { get; set; }
        public string ? Issuer { get; set; }
        public string ? Audience { get; set; }
        public int AccessTokenExpireTimeMinutes { get; set; }
        public int RefreshTokenExpireDays { get; set; }  
    }
}
