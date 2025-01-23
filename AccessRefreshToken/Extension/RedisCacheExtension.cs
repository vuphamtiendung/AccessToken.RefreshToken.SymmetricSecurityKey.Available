namespace AccessRefreshToken.Extension
{
    public static class RedisCacheExtension
    {
        public static void RedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = configuration["ConnectionStrings:RedisConnectionString"];
                option.InstanceName = "AuthenticationCatalog_";
            });
        }
    }
}
