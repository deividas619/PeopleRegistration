using Microsoft.Extensions.DependencyInjection;
using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.BusinessLogic.Services;

namespace PeopleRegistration.BusinessLogic
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPersonInformationService, PersonInformationService>();
            services.AddTransient<IJwtService, JwtService>();

            return services;
        }
    }
}
