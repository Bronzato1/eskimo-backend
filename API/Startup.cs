using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowCredentials();
            corsBuilder.WithOrigins("http://localhost:9000", "https://eskimoo.z28.web.core.windows.net");
            //corsBuilder.AllowAnyOrigin();

            services.AddCors(options =>
            {
                options.AddPolicy("AureliaSPA", corsBuilder.Build());
            });

            services.AddAuthorization();

            // Add framework services.
            services.AddMvc().AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
                
            string appRoot = Environment.CurrentDirectory;
            string database = System.IO.Path.Combine(appRoot + @"\", @"App_Data\blogging.db");
            services.AddDbContext<BlogContext> (options => options.UseSqlite("Data Source=" + database));
            services.AddScoped<IBlogRepository, BlogRepository>();
            
            //Transient objects are always different; a new instance is provided to every controller and every service.
            //Scoped objects are the same within a request, but different across different requests.
            //Singleton objects are the same for every object and every request.

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            // loggerFactory.AddDebug();
            app.UseStaticFiles();
            app.UseCors("AureliaSPA");
            app.UseMvc();
        }
    }
}
