using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mjosc.SimpleLMS.RestAPI.Extensions;
using Mjosc.SimpleLMS.RestAPI.Services;
using AutoMapper;

namespace Mjosc.SimpleLMS.RestAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. 
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // -------------------------------------------------------------------
            // .NET Core provided configs
            // -------------------------------------------------------------------

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // -------------------------------------------------------------------
            // Custom configurations
            // -------------------------------------------------------------------

            services.AddCors();
            services.AddLmsDbContext(Configuration);

            IConfigurationSection configSection =
                services.ConfigureAuthenticationStrings(Configuration);

            services.AddJwtAuthentication(Configuration, configSection);

            services.AddScoped<IUserService, UserService>();
            services.AddAutoMapper();
        }

        // This method gets called by the runtime. 
        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(options => options
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
