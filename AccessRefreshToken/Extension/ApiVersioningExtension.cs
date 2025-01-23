using Microsoft.AspNetCore.Mvc;

namespace AccessRefreshToken.ExtensionMethod
{
    public static class ApiVersioningExtension
    {
        public static void ApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });
        }
    }
}
