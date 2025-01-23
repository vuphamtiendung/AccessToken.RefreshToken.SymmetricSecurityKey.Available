using AccessRefreshToken.Domain.AccessToken;
using AccessRefreshToken.Domain.Common;
using AccessRefreshToken.Domain.Redis;
using AccessRefreshToken.Domain.RefreshToken;
using AccessRefreshToken.Logging;
using AccessRefreshToken.Model.AccessModel;
using AccessRefreshToken.Model.TokenModel;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccessRefreshToken.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IValidateToken _validateToken;
        private readonly IValidateUser _validateUser;
        private IRedisCacheServices _redisCacheServices;
        private readonly IAccessToken _accessToken;
        private readonly IRefreshToken _refreshToken;
        private readonly ILoggingServices _loggingServices;
        private readonly IConfiguration _configuration;

        public AuthenticateController(IValidateToken validateToken, IValidateUser validateUser, 
            IRedisCacheServices redisCacheServices, IAccessToken accessToken, IRefreshToken refreshToken,
            ILoggingServices loggingServices, IConfiguration configuration)
        {
            _validateToken = validateToken;
            _validateUser = validateUser;
            _redisCacheServices = redisCacheServices;
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            _loggingServices = loggingServices;
            _configuration = configuration;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel)
        {
            // check and validate 
            if (string.IsNullOrEmpty(tokenModel.RefreshToken))
                return BadRequest(new { message = "RefreshToken is invalid" });

            var principal = _validateToken.ValidateToken(tokenModel.RefreshToken);
            if (principal != null) return BadRequest(new { message = "Refresh token is still valid, no need to refresh" });

            RefreshTokenModel storeRefreshToken = _redisCacheServices.GetCacheData<RefreshTokenModel>(tokenModel.RefreshToken);

            if (storeRefreshToken == null) return Unauthorized("Invalid Refresh Token");
            if (storeRefreshToken.Expiration < DateTime.UtcNow) return Unauthorized("Refresh Token had expired");
            if (storeRefreshToken.IsRevoke) return Unauthorized("Refresh Token had revoked");

            storeRefreshToken.IsRevoke = true;
            _redisCacheServices.SetCacheData(tokenModel.RefreshToken, storeRefreshToken, TimeSpan.FromMinutes(30));

            // Create new access token and refresh token
            string ? userId = storeRefreshToken.UserId;
            string ? newAccessToken = _accessToken.GenerateAccessToken(userId);
            string ? newRefreshToken = _refreshToken.GenerateRefreshToken(userId);

            var refreshTokenModel = new RefreshTokenModel()
            {
                Token = newRefreshToken,
                UserId = userId,
                Expiration = DateTime.UtcNow.AddDays(7),
                IsRevoke = false
            };
            _redisCacheServices.SetCacheData(newRefreshToken, refreshTokenModel, TimeSpan.FromMinutes(30));
            _loggingServices.LogInfor("Refresh token had been create success");
            return Ok(new TokenModel
            {
                AccessToken = newAccessToken,   
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = _validateUser.ValidateUser(loginModel.UserName, loginModel.Password);
            if (user == null) return Unauthorized("Invalid username or password");
            if(loginModel.UserName == user.UserName && loginModel.Password == user.Password)
            {
                string accessToken = _accessToken.GenerateAccessToken(loginModel.UserId);
                string refreshToken = _refreshToken.GenerateRefreshToken(loginModel.UserId);

                var refreshTokenModel = new RefreshTokenModel()
                {
                    Token = refreshToken,
                    UserId = user.Id.ToString(),
                    Expiration = DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["Token:RefreshTokenExpirationDay"])),
                    IsRevoke = false
                };

                _redisCacheServices.SetCacheData(refreshToken, refreshTokenModel, TimeSpan.FromMinutes(30));
                _loggingServices.LogInfor("User had been login success");
                return Ok(new TokenModel
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            else
            {
                return Unauthorized();
                _loggingServices.LogError("User had been loging failed");
            }
        }
    }
}
