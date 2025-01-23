using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System.Diagnostics.SymbolStore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccessRefreshToken.Domain.RefreshToken
{
    public class RefreshToken : IRefreshToken
    {
        private readonly IConfiguration _configuration;
        public RefreshToken(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateRefreshToken(string userId)
        {
            string jwtKey = _configuration["Token:Key"];
            string jwtIssuer = _configuration["Token:Issuer"];
            string jwtAudience = _configuration["Token:Audience"];

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var signinCredential = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Iss, jwtIssuer),
                    new Claim(JwtRegisteredClaimNames.Aud, jwtAudience),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                SigningCredentials = signinCredential
            };
            var token = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            string tokenString = jwtSecurityTokenHandler.WriteToken(token);
            return tokenString;

        }
    }
}
