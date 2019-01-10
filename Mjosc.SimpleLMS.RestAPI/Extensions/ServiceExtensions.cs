using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Mjosc.SimpleLMS.Entities.Models;
using Mjosc.SimpleLMS.RestAPI.Helpers;

namespace Mjosc.SimpleLMS.RestAPI.Extensions
{
    // -------------------------------------------------------------------
    // All extension methods used within Startup.cs. Anything more complex 
    // than a single method call with zerof arguments has been moved here.
    // -------------------------------------------------------------------

    public static class ServiceExtensions
    {

        // Retrieves the authentication strings stored in appsettings.json.
        // This is required for JWT configuration.
        public static IConfigurationSection ConfigureAuthenticationStrings(
            this IServiceCollection services, IConfiguration config)
        {
            IConfigurationSection authStrings = config.GetSection("AuthenticationStrings");
            services.Configure<AuthenticationStrings>(authStrings);

            // This is returned in order for it to be passed to AddJwtAuthentication.
            return authStrings;
        }

        // Configures JWT authentication. See Startup.cs and SecurityUtil.cs
        // for additional components involved in this authentication scheme.
        public static void AddJwtAuthentication(this IServiceCollection services, 
            IConfiguration config, IConfigurationSection configSection)
        {
            byte[] secretKey = Encoding.ASCII.GetBytes(configSection["JwtSecretKey"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // Required to connect to the MySQL database containing the supported
        // LMS tables.
        public static void AddLmsDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<LmsDbContext>(options =>
                options.UseMySql(config.GetConnectionString("SimpleLmsDatabase"))
            );
        }

        public static void EnableCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                // TODO: This is a very generous policy. Use WithOrigins, 
                // WithMethods, and WithHeaders to narrow external access.
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }
    }
}
