using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PeopleRegistration.BusinessLogic;
using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.BusinessLogic.Services;
using PeopleRegistration.Database;
using Serilog;

/*
46310186868,1963-10-18,female
47912273317,1979-12-27,female
31505236180,1915-05-23,male
61407050885,2014-07-05,female
51301169746,2013-01-16,male
47208018929,1972-08-01,female
51212086740,2012-12-08,male
47904102653,1979-04-10,female
61508168620,2015-08-16,female
33105040446,1931-05-04,male
*/
namespace PeopleRegistration.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddResponseCaching();
            builder.Services.AddBusinessLogic();
            builder.Services.AddDatabase(builder.Configuration.GetConnectionString("Database"));
            builder.Services.AddTransient<IJwtService, JwtService>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                    ClockSkew = TimeSpan.FromSeconds(0)
                };
            });
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "PersonRegistrationAPI", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                if (!db.Users.Any(u => u.Username == "admin"))
                {
                    userService.Register("admin", "admin");
                }
                if (!db.Users.Any(u => u.Username == "test"))
                {
                    userService.Register("test", "test");
                }
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseResponseCaching();

            app.MapControllers();

            app.Run();
        }
    }
}
