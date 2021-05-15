using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.HttpOverrides;
using www.pwa.Server.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using www.pwa.Server.Services;
using www.pwa.Shared;

namespace www.pwa.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            WwwData.Admin = Configuration["Auth:Admin"];
            WwwData.AdminCredential = Configuration["Auth:Credential"];
            WwwData.Credential = Configuration["FormCredential"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<WwwContext>(options =>
                options
                    .UseMySql(Configuration["ConnectionStrings:DefaultConnection"], new MySqlServerVersion(new System.Version(5, 7, 31)),
                    x => x.EnableRetryOnFailure()
                ));
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<WwwContext>();

            services.AddSingleton<DbService>();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, WwwContext context, DbService db)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            context.Database.Migrate();
            db.Init(context);

             string basePath = "/www";
             if (!string.IsNullOrEmpty(basePath))
             {
                 app.Use((context, next) =>
                 {
                     context.Request.Scheme = "https";
                     return next();
                 });

                 app.Use((context, next) =>
                 {
                     context.Request.PathBase = new PathString(basePath);
                     if (context.Request.Path.StartsWithSegments(basePath, out var remainder))
                     {
                         context.Request.Path = remainder;
                     }
                     return next();
                 });
             }
             else
                 basePath = String.Empty;

             app.UseForwardedHeaders(new ForwardedHeadersOptions
             {
                 ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
             });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
