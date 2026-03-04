namespace Resonance.Api.AppStart.Extensions
{
    public static class CorsExtensions
    {
        private const string AllowAllPolicy = "AllowAll";
        private const string AllowSpecificOriginPolicy = "AllowSpecificOrigin";

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.SetIsOriginAllowed(origin => true) // Разрешить любые источники
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // SignalR требует AllowCredentials
                });
            });
        }

        public static void ApplyCors(this WebApplication app)
        {
            app.UseCors(); // Используем для теста
        }
    }
}
