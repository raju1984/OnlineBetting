using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using QuickBetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Env { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false);
            if (Env.IsDevelopment())
            {
                services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver =
                    new DefaultContractResolver());
                services.AddRazorPages().AddRazorRuntimeCompilation();
            }
            else
            {
                services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver =
                    new DefaultContractResolver());
                services.AddRazorPages();
            }

            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
            });
            services.AddDistributedMemoryCache();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(120);//You can set Time  
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            //services.AddHostedService<TaskRunner>();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials()
              );
            app.UseStaticFiles();
            app.UseRobots(env);
            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                 name: "User",
                 areaName: "User",
                 pattern: "User/{controller=UserDashboard}/{action=Index}");

                endpoints.MapAreaControllerRoute(
                 name: "Admin",
                 areaName: "Admin",
                 pattern: "Admin/{controller}/{action}");

                endpoints.MapAreaControllerRoute(
                 name: "Agent",
                 areaName: "Agent",
                 pattern: "Agent/{controller}/{action}");

                endpoints.MapAreaControllerRoute(
                 name: "SuperAgent",
                 areaName: "SuperAgent",
                 pattern: "SuperAgent/{controller}/{action}");


                endpoints.MapAreaControllerRoute(
               name: "Nationallottery",
               areaName: "Nationallottery",
               pattern: "Nationallottery/{controller}/{action}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
