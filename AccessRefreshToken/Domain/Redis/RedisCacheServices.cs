using AccessRefreshToken.Model.AccessModel;
using AccessRefreshToken.Model.TokenModel;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using static System.Console;

namespace AccessRefreshToken.Domain.Redis
{
    public class RedisCacheServices : IRedisCacheServices
    {
        private readonly IDistributedCache _distributeCache;
        public RedisCacheServices(IDistributedCache distributeCache)
        {
            _distributeCache = distributeCache;
        }

        public T GetCacheData<T>(string refreshToken)
        {
            try
            {
                var jsonData = _distributeCache.GetString(refreshToken);
                if (jsonData == null) return default(T);
                return JsonSerializer.Deserialize<T>(jsonData);
            }
            catch (Exception ex)
            {
                WriteLine($"Error deserializing cache for key {refreshToken}: {ex.Message}");
                return default(T);
            }
        }

        public void RemoveData(TokenModel tokenModel)
        {
            _distributeCache.Remove(tokenModel.RefreshToken);
        }

        public void SetCacheData(string tokenModel, RefreshTokenModel refreshTokenModel, TimeSpan cacheDuration)
        {
            var distributeCacheOption = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration,
                SlidingExpiration = cacheDuration
            };
            var jsonData = JsonSerializer.Serialize(refreshTokenModel);
            _distributeCache.SetString(tokenModel, jsonData, distributeCacheOption);
        }
    }
}
