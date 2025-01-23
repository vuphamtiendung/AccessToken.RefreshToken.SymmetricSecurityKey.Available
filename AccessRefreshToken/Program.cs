using AccessRefreshToken.Domain.AccessToken;
using AccessRefreshToken.Domain.Common;
using AccessRefreshToken.Domain.Redis;
using AccessRefreshToken.Domain.RefreshToken;
using AccessRefreshToken.Extension;
using AccessRefreshToken.ExtensionMethod;
using AccessRefreshToken.Model.UserModel;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();

        builder.Services.ApiVersioning();
        builder.Services.AddSingleton<IValidateToken, ValidateToken>();
        builder.Services.AddSingleton<IValidateUser, ValidateUser>();
        builder.Services.AddScoped<IRedisCacheServices, RedisCacheServices>();
        builder.Services.AddSingleton<IAccessToken, AccessToken>();
        builder.Services.AddSingleton<IRefreshToken, RefreshToken>();
        builder.Services.AddSingleton<ListUser>();

        builder.Services.AddAuthentication(builder.Configuration);
        builder.Services.AddAuthorization();
        builder.Services.RedisCache(builder.Configuration);
        builder.Services.Logging();
        builder.Services.Cors();
        
        builder.Services.AddOpenApi();

        // MiddleWare pipeline
        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRouting();
        app.UseCors("AllowFrontend");
        app.MapControllers();
        app.Run();
    }
}