using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExploreCalifornia
{
    public class Startup
    {
        private readonly IConfigurationRoot configuration;
        public Startup(IHostingEnvironment env)
        {
               configuration = new ConfigurationBuilder()
                              .AddEnvironmentVariables()
                              .AddJsonFile(env.ContentRootPath + "/config.json")
                              .AddJsonFile(env.ContentRootPath + "/config.development.json", true)
                              .Build();

            
        }        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc();
            services.AddTransient < FeatureToggles>(x => new FeatureToggles
            {
                EnableDeveloperException =
                    configuration.GetValue<bool>("FeatureToggles:EnableDeveloperException")
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IHostingEnvironment env,
                              FeatureToggles features
                             )
        {

            
            app.UseExceptionHandler("/error.html");
          

            //if (configuration.GetValue<bool>("FeatureToggles:EnableDeveloperException"))
            if(features.EnableDeveloperException)
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.Contains("invalid"))
                    throw new Exception("Error");
                await next();
            });
            //else
            //{
            //    app.UseExceptionHandler("/Error");
            //}

            //app.UseStaticFiles();

            //app.UseMvc();
            app.UseFileServer();
        }
    }
}
