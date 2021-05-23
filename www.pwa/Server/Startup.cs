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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            
            context.Database.Migrate();
            
            CreateRoles(serviceProvider, Configuration["Auth:Admin"], Configuration["Auth:Credential"]);
            
            if (!context.wwwWalks.Any()) {
                WwwWalk nepalWalk = new WwwWalk() {
                    Name = "Sponsorenlauf für Nepal",
                    Guid = new Guid("7A40C465-BDC8-4373-B6BE-6E49C10D5ECA"),
                    Description = "",
                    TotalDistance = 8400.0f,
                    isActive = true,
                    Credential = Configuration["DefaultCredential"],
                    WwwSchools = new List<WwwSchool>() {
                        new WwwSchool() {
                            Name = "Gymnasium Geretsried",
                            WwwClasses = new List<WwwClass>(WwwData.s_classes.Select(s => new WwwClass() {
                                Name = s,
                                Year = DbService.GetClassYear(s)
                            }))
                        }
                    },
                    Points = new List<WwwWalkData>() {
                        new WwwWalkData() {
                            Name = "Geretsried",
                            Description = "Start",
                            Position = 1,
                            Latitude = 47.8583,
                            Longitude = 11.478
                        },
                        new WwwWalkData() {
                            Name = "Graz",
                            Description = "Österreich",
                            Position = 2,
                            Latitude = 46.995,
                            Longitude = 15.469
                        },
                        new WwwWalkData() {
                            Name = "Novi Sad",
                            Description = "Нови Сад, Serbien",
                            Position = 3,
                            Latitude = 45.2995,
                            Longitude = 19.8653
                        },
                        new WwwWalkData() {
                            Name = "Plovdiv",
                            Description = "Пловдив, Bulgarien",
                            Position = 4,
                            Latitude = 42.2097,
                            Longitude = 24.7302
                        },
                        new WwwWalkData() {
                            Name = "Istanbul",
                            Description = "Türkei",
                            Position = 5,
                            Latitude = 41.0027,
                            Longitude = 29.0121
                        },
                        new WwwWalkData() {
                            Name = "Ankara",
                            Description = "Türkei",
                            Position = 6,
                            Latitude = 41.0097,
                            Longitude = 32.8931
                        },
                        new WwwWalkData() {
                            Name = "Teheran",
                            Description = "تهران, Iran",
                            Position = 7,
                            Latitude = 35.648,
                            Longitude = 51.405
                        },
                        new WwwWalkData() {
                            Name = "Aşgabat",
                            Description = "Turkmenistan",
                            Position = 8,
                            Latitude = 37.9269,
                            Longitude = 58.4061
                        },
                        new WwwWalkData() {
                            Name = "Duschanbe",
                            Description = "Душанбе, Tajikistan",
                            Position = 9,
                            Latitude = 38.5589,
                            Longitude = 68.784
                        },
                        new WwwWalkData() {
                            Name = "Rawalpindi",
                            Description = "راولپنڈی‎, Pakistan",
                            Position = 10,
                            Latitude = 33.5883,
                            Longitude = 73.0666
                        },
                        new WwwWalkData() {
                            Name = "Neu-Delhi",
                            Description = "नई दिल्ली, Indien",
                            Position = 11,
                            Latitude = 28.6014,
                            Longitude = 77.2408
                        },
                        new WwwWalkData() {
                            Name = "Manang",
                            Description = "Partnerschule Lophel Ling Boarding School (LBS) in Nepal",
                            Position = 12,
                            Latitude = 28.6419,
                            Longitude = 84.0903
                        },

                    }
                };

                double distance = 0;
                WwwWalkData oldpoint = null;
                foreach (var point in nepalWalk.Points.OrderBy(o => o.Position)) {
                    if (oldpoint != null)
                        distance += RunService.GetDistance(oldpoint.Latitude, oldpoint.Longitude, point.Latitude, point.Longitude);
                    point.Distance = Math.Round(distance / 1000, 2);
                    oldpoint = point;
                }
                nepalWalk.TotalDistance = (float)Math.Round(distance / 1000, 2);
                context.wwwWalks.Add(nepalWalk);
                context.SaveChanges();
            }
            
            // string basePath = Environment.GetEnvironmentVariable("ASPNETCORE_BASEPATH");
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

        private void CreateRoles(IServiceProvider serviceProvider, string email, string securePassword)
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
