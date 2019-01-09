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
    public static class ServiceExtensions
    {

        public static IConfigurationSection ConfigureAuthenticationStrings(
            this IServiceCollection services, IConfiguration config)
        {
            IConfigurationSection authStrings = config.GetSection("AuthenticationStrings");
            services.Configure<AuthenticationStrings>(authStrings);

            // Make this available to the AddJwtAuthentication method.
            return authStrings;
        }

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

        public static void AddLmsDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<LmsDbContext>(options =>
                options.UseMySql(config.GetConnectionString("SimpleLmsDatabase"))
            );
        }
    }
}
