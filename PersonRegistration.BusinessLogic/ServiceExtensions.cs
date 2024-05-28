using Microsoft.Extensions.DependencyInjection;
using PersonRegistration.BusinessLogic.Interfaces;
using PersonRegistration.BusinessLogic.Services;

namespace PersonRegistration.BusinessLogic
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<IJwtService, JwtService>();

            return services;
        }
    }
}
