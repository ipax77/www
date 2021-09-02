using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using www.pwa.Server.Data;
using www.pwa.Server.Models;
using System.Collections.Generic;
using www.pwa.Shared;
using www.pwa.Server.Services;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Server.Data;

namespace www.pwa.Server
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "wwwOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            WwwData.DefaultWalk = Configuration["DefaultWalk"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddDbContext<ApplicationDbContext>(options =>
            //     options.UseSqlite(
            //         Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<ApplicationDbContext>(options =>
                options
                .UseMySql(Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new System.Version(5, 7, 34)),
                x =>
                {
                    x.EnableRetryOnFailure();
                    x.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                }));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
                    {
                        options.IdentityResources["openid"].UserClaims.Add("role");
                        options.ApiResources.Single().UserClaims.Add("role");
                    }
                );
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler
                .DefaultInboundClaimTypeMap.Remove("role");

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                builder =>
                                {
                                    builder.WithOrigins("https://www.pax77.org",
                                                        "https://localhost:5001"
                                                    );
                                });
            });

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSingleton<DbService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context, IServiceProvider serviceProvider, DbService dbService)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            
            context.Database.Migrate();

            CreateRoles(serviceProvider, Configuration["Auth:Admin"], Configuration["Auth:Credential"], Configuration["Auth:Admin2"], Configuration["Auth:Credential2"]);

            // dbService.SeedSponsors(context).GetAwaiter().GetResult();

            var classes = context.wwwClasses.Select(s => new SchoolClass() { Name = s.Name });
            Extensions.SortClasses(classes.ToList());

            if (!context.wwwWalks.Any()) {

                var walk = Walks.GetSeenWalk(Configuration["FormCredential"]);

                double distance = 0;
                WwwWalkData oldpoint = null;
                foreach (var point in walk.Points.OrderBy(o => o.Position)) {
                    if (oldpoint != null)
                        distance += RunService.GetDistance(oldpoint.Latitude, oldpoint.Longitude, point.Latitude, point.Longitude);
                    point.Distance = Math.Round(distance / 1000, 2);
                    oldpoint = point;
                }
                walk.TotalDistance = (float)Math.Round(distance / 1000, 2);
                context.wwwWalks.Add(walk);
                context.SaveChanges();
            }
            
            // string basePath = Environment.GetEnvironmentVariable("ASPNETCORE_BASEPATH");
            string basePath = "/worldwidewalk";
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
                app.UseMigrationsEndPoint();
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

            app.UseCors(MyAllowSpecificOrigins);

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }

        private void CreateRoles(IServiceProvider serviceProvider, string email, string securePassword, string email2, string securePassword2)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            Task<IdentityResult> roleResult;

            foreach (var role in Enum.GetValues(typeof(Role)))
            {
                Task<bool> roleExists = roleManager.RoleExistsAsync(role.ToString());
                roleExists.Wait();

                if (!roleExists.Result)
                {
                    roleResult = roleManager.CreateAsync(new IdentityRole(role.ToString()));
                    roleResult.Wait();
                }
            }

            CreateUser(userManager, email, securePassword);
            CreateUser(userManager, email2, securePassword2);
            
        }

        private void CreateUser(UserManager<ApplicationUser> userManager, string email, string securePassword) {
            Task<ApplicationUser> adminUser = userManager.FindByEmailAsync(email);
            adminUser.Wait();

            if (adminUser.Result == null)
            {
                var admin = new ApplicationUser();
                admin.Email = email;
                admin.UserName = email;

                Task<IdentityResult> newUser = userManager.CreateAsync(admin, securePassword);
                newUser.Wait();
            }

            var createdAdminUser = userManager.FindByEmailAsync(email);
                createdAdminUser.Wait();
            createdAdminUser.Result.EmailConfirmed = true; // confirm email so we can login
            Task<IdentityResult> newUserRoleAssignment = userManager.AddToRoleAsync(createdAdminUser.Result, Role.Administrator.ToString());
            newUserRoleAssignment.Wait();            
        }
    }
}
