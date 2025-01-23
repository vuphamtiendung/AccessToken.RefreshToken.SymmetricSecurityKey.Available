namespace AccessRefreshToken.Extension
{
    public static class CorsExtension
    {
        public static void Cors(this IServiceCollection services)
        {
            services.AddCors(option =>
            {
                option.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://127.0.0.1:5500", "http://127.0.0.1:5500/index.html")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });
        }
    }
}
