using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccessRefreshToken.Domain.AccessToken
{
    public class AccessToken : IAccessToken
    {
        private readonly IConfiguration _configuration;
        public AccessToken(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateAccessToken(string userId)
        {
            var jwtKey = _configuration["Token:Key"];
            var jwtIssuer = _configuration["Token:Issuer"];
            var jwtAudience = _configuration["Token:Audience"];

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var signinCredential = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Iss, jwtIssuer),
                new Claim(JwtRegisteredClaimNames.Aud, jwtAudience),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = new JwtSecurityToken(
                                   jwtKey,
                                   jwtIssuer,
                                   claims,
                                   expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["Token:AccessTokenExpirationMinutes"])),
                                   signingCredentials: signinCredential
                              );
            return jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
        }
    }
}
