using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Organizer.Models;

namespace Organizer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("DBConn");
            services.AddDbContext<ModelContext>(options =>
                options.UseSqlServer(connection));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    options.LoginPath = new PathString("/Account/login");
                    //options.AccessDeniedPath = new PathString("Account/AccessDenied");
                });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
                .AddDataAnnotationsLocalization()
                .AddViewLocalization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            var supportedCultures = new[]
            {
                new CultureInfo("uk"),
                new CultureInfo("ru"),
                new CultureInfo("en")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=user}/{action=home}/{id?}");
                routes.MapRoute( // костиль, надо исправить в отдельную страницю для вывода ошибок
                    name: "error403",
                    template: "Account/AccessDenied",
                    defaults: new { controller = "user", action = "home" });
            });
        }
    }
}
