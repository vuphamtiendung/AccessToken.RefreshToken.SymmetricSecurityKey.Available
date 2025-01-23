using AccessRefreshToken.Logging;

namespace AccessRefreshToken.Extension
{
    public static class LoggingExtension
    {
        public static void Logging(this IServiceCollection services)
        {
            services.AddSingleton<ILoggingServices, LoggingServices>();
        }
    }
}
