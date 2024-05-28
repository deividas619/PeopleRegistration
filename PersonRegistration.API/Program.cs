using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PersonRegistration.BusinessLogic;
using PersonRegistration.BusinessLogic.Interfaces;
using PersonRegistration.BusinessLogic.Services;
using PersonRegistration.Database;
using Serilog;

//smaller responses from controller instead of full object use dto
//force to change the password if it's older than 90 days
//add [ProducesResponseType(http_code)] to controllers
//add fixture and specimen for tests
//maybe always resize image to specific size
//create custom exceptions instead of note/category object guid id error code
//look into attaching user table to the rest of the tables for fetch speed
//how to build docker image with swager endpoints and ui
//add remove image method
//add password complexity creating account/changing password
//attribute for unauthorized access to register user
//onmodelcreation deal with role enum https://stackoverflow.com/questions/32542356/how-to-save-enum-in-database-as-string

namespace PersonRegistration.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

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
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "NotesAPI", Version = "v1" });
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
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseResponseCaching();

            app.MapControllers();

            app.Run();
        }
    }
}
