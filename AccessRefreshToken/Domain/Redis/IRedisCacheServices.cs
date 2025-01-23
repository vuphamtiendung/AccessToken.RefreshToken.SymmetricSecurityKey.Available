using AccessRefreshToken.Model.AccessModel;
using AccessRefreshToken.Model.TokenModel;

namespace AccessRefreshToken.Domain.Redis
{
    public interface IRedisCacheServices
    {
        public T GetCacheData<T>(string refreshToken);
        public void SetCacheData(string tokenModel, RefreshTokenModel refreshTokenModel, TimeSpan cacheDuration);
        public void RemoveData(TokenModel tokenModel);
    }
}
