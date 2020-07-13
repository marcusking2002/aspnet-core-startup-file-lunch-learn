using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ExampleWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IConnectionManager, ConnectionManager>(); //instantiation per app lifetime
            services.AddScoped<IDataRepo, DataRepo>(); // instantiation per web request 
            //services.AddTransient<IDataRepo, DataRepo>(); // instantiation every time one is needed.
        }

        public void ConfigureStaging3Services(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IConnectionManager, ConnectionManager>(); //instantiation per app lifetime
            services.AddScoped<IDataRepo, DataRepoOther>();
        }

        public void ConfigureStaging3(IApplicationBuilder app, IWebHostEnvironment env) => Configure(app, env);

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsEnvironment("DEVELOPment"))
            {
                app.UseStaticFiles();
            }
           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            ////you interrupted the pipeline here.
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Cookies.Append("MORE_COOKIE", DateTime.Now.Ticks.ToString());

            //    await context.Response.WriteAsync("You have total control over the request pipeline inline.");

            //    await next.Invoke();
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ////too late
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Cookies.Append("MORE_COOKIE", DateTime.Now.Ticks.ToString());

            //    await context.Response.WriteAsync("You have total control over the request pipeline inline.");

            //    await next.Invoke();
            //});
        }
    }
}
