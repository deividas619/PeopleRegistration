using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonRegistration.Database.Interfaces;
using PersonRegistration.Database.Repositories;

namespace PersonRegistration.Database
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            return services;
        }
    }
}
