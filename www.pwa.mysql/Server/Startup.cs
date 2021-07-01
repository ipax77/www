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
            // services.AddDbContext<ApplicationDbContext>(options =>
            //     options.UseSqlite(
            //         Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<ApplicationDbContext>(options =>
                options
                .UseMySql(Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new System.Version(5, 7, 34)),
                x => x.EnableRetryOnFailure()
            ));

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
            // dbService.SeedSponsors(context);

            CreateRoles(serviceProvider, Configuration["Auth:Admin"], Configuration["Auth:Credential"], Configuration["Auth:Admin2"], Configuration["Auth:Credential2"]);
            
            if (!context.wwwWalks.Any()) {
                WwwWalk nepalWalk = new WwwWalk() {
                    Name = "Sponsorenlauf für Nepal",
                    Guid = new Guid("7A40C465-BDC8-4373-B6BE-6E49C10D5ECA"),
                    Description = "",
                    TotalDistance = 7257.5f,
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
                            Longitude = 11.478,
                            ImageCopyRight = "Bild: GymGer",
                            LongDescription = "Das Gymnasium Geretsried ist ein Naturwissenschaftlich-technologisches und Sprachliches Gymnasium. Besonderheit: Mit der Wahl der 2.Fremdsprache(Latein, Französisch) ist die Ausbildungsrichtung noch nicht festgelegt. Im Sprachlichen Gymnasium sind drei moderne Fremdsprachen möglich(Englisch, Französisch, Spanisch)."
                        },
                        new WwwWalkData() {
                            Name = "Graz",
                            Description = "Österreich",
                            Position = 2,
                            Latitude = 46.995,
                            Longitude = 15.469,
                            LongDescription = "Graz liegt umgeben von Hügeln im Westen, Norden und Osten im so genannten Grazer Becken. Einerseits ist es toll, von Bergen umringt zu sein, andererseits führt das aber auch zur Einkesselung. Daher hat Graz im Herbst und Winter oft Nebel. Im Frühling und Sommer hingegen profitiert Graz von der Öffnung nach Süden hin. Mit 2100 Sonnenstunden im Jahr kann man getrost von mediterranem Klima sprechen. Da sitzt man dann im Sommer im Lendviertel und schlürft Aperol wie in Italien!"
                        },
                        new WwwWalkData() {
                            Name = "Novi Sad",
                            Description = "Нови Сад, Serbien",
                            Position = 3,
                            Latitude = 45.2995,
                            Longitude = 19.8653,
                            LongDescription = "Novi Sad ist Serbiens zweitgrößte Stadt. Die kulturell vielfältige Stadt Novi Sad, in der Provinz Vojvodina in Nordserbien gelegen, ist die erste Stadt außerhalb der EU, die den Titel Europäische Kulturhauptstadt erhalten hat. Die vielen Nationen, die hier seit drei Jahrhunderten zusammenleben, machen die Region heute zu einem Ort aktiver Kulturbegegnungen und verschiedener kultureller Identitäten. Dezentrale Kulturstationen in der ganzen Stadt engagieren sich für den Erhalt des kulturellen Erbes."
                        },
                        new WwwWalkData() {
                            Name = "Plovdiv",
                            Description = "Пловдив, Bulgarien",
                            Position = 4,
                            Latitude = 42.2097,
                            Longitude = 24.7302,
                            LongDescription = "Plovdiv als zweitgrößte bulgarische Stadt hat eine äußerst wechselvolle Geschichte. Im Laufe der inzwischen rund 6.000 Jahre währenden Siedlungsgeschichte eroberten makedonische Könige, thrakische Herrscher, die Römer, Kreuzritter und Osmanen die Stadt. Schlenderst du durch die Altstadt, wirst du an zahlreichen Sehenswürdigkeiten aus den verschiedenen Epochen vorbeikommen.Die Römer hinterließen ein sehr gut erhaltendes antikes Theater. Es liegt hoch oben auf einem der sieben Hügel von Plovdiv. "
                        },
                        new WwwWalkData() {
                            Name = "Istanbul",
                            Description = "Türkei",
                            Position = 5,
                            Latitude = 41.0027,
                            Longitude = 29.0121,
                            LongDescription = "Istanbul ist eine Stadt, die sich mit ihrer spektakulären Topographie am Wasser und auf zwei Kontinenten immer wieder neu erfindet. Der Bosporus, die Meerenge, teilt das Stadtgebiet in einen europäischen und in einen asiatischen Teil. Im Süden liegt das Marmarameer und im Norden das Schwarze Meer. Aus Byzantion, einer Stadt aus dem 7. Jahrhundert vor Chr., entstand Konstantinopolis im 4. Jahrhundert als Neugründung durch Kaiser Konstantin. Bauten wie die Hagia Sophia und andere Kirchen, der Hippodrom, Kaiserpalast oder die Stadtmauern sind heute noch Zeugen dieser Zeit. Nach 1600 Jahren Hauptstadtdasein verlor Istanbul diese Rolle mit der Gründung der Republik 1923 an Ankara. Blieben die Veränderungen bis in die Mitte des 20. Jahrhunderts eher im städtischen Zentralbereich, entwickelte sich die Stadt mit den 1950er Jahren in einem immer schneller werdenden Tempo von einer großen Stadt zu einer Megametropole."
                        },
                        new WwwWalkData() {
                            Name = "Ankara",
                            Description = "Türkei",
                            Position = 6,
                            Latitude = 39.95165524923777,
                            Longitude = 32.841101549499875,
                            LongDescription = "Das Mausoleum Atatürks ist die wohl bekannteste Sehenswürdigkeit Ankaras. Es befindet sich auf dem Hügel Rasattepe, der im 12. Jahrhundert vor Christus von den Phrygern, die in Anatolien ein großes Reich hatten, als Grabhügel errichtet wurde. Im Anıtkabir ruht der Staatsgründer der Türkei, Mustafa Kemal Atatürk. Das Anıtkabir wird für türkische Staatsakte genutzt und sowohl von Touristen als auch von Einheimischen gern besucht. Neben dem Mausoleum kann man auf dem Gelände unter anderem ein Museum und den Friedenspark besuchen. Im Museumsbereich sind private Gegenstände Atatürks zu besichtigen, darunter Säbel und Gewehre, aber auch Gemälde und die private Bibliothek des ehemaligen türkischen Präsidenten. Im Friedenspark gibt es Bäume aus 24 Nationen und verschiedene weitere Gewächse zu sehen."
                        },
                        new WwwWalkData() {
                            Name = "Teheran",
                            Description = "تهران, Iran",
                            Position = 7,
                            Latitude = 35.648,
                            Longitude = 51.405,
                            LongDescription = "Teheran liegt in einer erdbebengefährdeten Zone. Die Region im Iranischen Hochland ist tektonisch sehr aktiv. Hier stoßen die Indisch-Australische und die Arabische Kontinentalplatte auf die Eurasische Platte. Mehrmals im Jahr kommt es zu leichten Erdstößen. Am 27. März 1830 erschütterte ein Beben von 7,0 Punkten auf der Richterskala die Stadt. Fast alle Gebäude der Hauptstadt wurden zerstört. In der gesamten Region starben schätzungsweise 45.000 Menschen. Der Fluss Karadsch, der im Westen an Teheran vorbeifließt, und der Fluss Djadjurd, der östlich liegt, sind die Hauptversorgungsquelle für Wasser. Das Wetter variiert stark von der jeweiligen Höhe des Gebietes. So sind die Temperaturen im höher gelegenen Norden wesentlich kühler als im Süden der Stadt. Im Süden sind die Sommer trocken und heiß. Hier werden im Juli Spitzentemperaturen von über 30° C erreicht. Der Norden erreicht zu dieser Zeit Höchsttemperaturen von etwa 25° C."
                        },
                        new WwwWalkData() {
                            Name = "Aşgabat",
                            Description = "Turkmenistan",
                            Position = 8,
                            Latitude = 37.9269,
                            Longitude = 58.4061,
                            LongDescription = "Aşgabat ist die Hauptstadt und größte Stadt Turkmenistans. Die Stadt befindet sich im Süden des Landes nahe der Grenze zum Iran am Kreuzungspunkt alter Karawanenrouten in der Karakumwüste. Der Name der Stadt stammt wohl aus dem Persischen und bedeutet die liebliche Stadt. Von 1819 bis 1927 trug sie den Namen Poltorazk, da sie von Russen gegründet wurde. Seit der Unabhängigkeit Turkmenistans am 27. Oktober 1991 ist sie auch dessen Hauptstadt. Mehrere Erdbeben, insbesondere das vom 5. Oktober 1948, haben die Stadt stark in Mitleidenschaft gezogen. So findet man hier heutzutage kaum noch frühere Bausubstanz. Das heutige Stadtbild wird von modernen Bauten aus sowjetischer Zeit und den 1990er-Jahren geprägt. Aşgabat ist hauptsächlich Regierungs- und Verwaltungszentrum. Auch sind hier Industrie, eine Universität und Hochschulen angesiedelt."
                        },
                        new WwwWalkData() {
                            Name = "Duschanbe",
                            Description = "Душанбе, Tajikistan",
                            Position = 9,
                            Latitude = 38.5589,
                            Longitude = 68.784,
                            LongDescription = "Duschanbe wird auf dem endenden e betont ausgesprochen. Seit den 2000er Jahren vollzieht sich in Duschanbe der radikale Umbau des Stadtzentrums. Sowjetische Architektur wird abgerissen und ersetzt durch eine „tadschikische” Moderne, einen an den Baustil Dubais und der Emirate angelehnten „islamischen” Internationalismus. Die alten Bazare und Wohnviertel mit ihren durch Mauern nach außen abgeschotteten Gehöften (samt Gärten und eingeschossiger Bebauung) sowie die zwei- und mehrstöckigen Wohn- und Verwaltungsgebäude der 1920er bis 1980er Jahre werden ersetzt durch luxuriöse Apartmentblocks und Hotelanlagen, verglaste Bürogebäude und gigantomanische Neubauten, die in der Regel von türkischen, iranischen, chinesischen und golfarabischen Investoren finanziert werden. Der Kontrast zum Rest des Landes könnte kaum größer sein."
                        },
                        new WwwWalkData() {
                            Name = "Rawalpindi",
                            Description = "راولپنڈی‎, Pakistan",
                            Position = 10,
                            Latitude = 33.5883,
                            Longitude = 73.0666,
                            ImageCopyRight = "Bild: Naseerabbas, CC BY-SA 3.0 <LINK> via Wikimedia Commons",
                            ImageCopyRightLink = "https://creativecommons.org/licenses/by-sa/3.0",
                            LongDescription = "Der Distrikt Rāwalpindi befindet sich im äußersten Norden der Provinz Punjab, die sich im Westen von Pakistan befindet, und grenzt an die Hauptstadt Islamabad an. Flüsse, die durch das Gebiet fließen, sind der Indus und der Jhelum. Bei der Volkszählung 1901 durch die Briten hatte der Distrikt 558.699 Einwohner. Mit der Gründung von Islamabad als Hauptstadt Pakistans in unmittelbarer Nähe erlebte das Gebiet einen starken Aufschwung und Bevölkerungszuwachs. Der Distrikt zählt heute zu den am stärksten urbanisierten Regionen Pakistans."
                        },
                        new WwwWalkData() {
                            Name = "Neu-Delhi",
                            Description = "नई दिल्ली, Indien",
                            Position = 11,
                            Latitude = 28.6014,
                            Longitude = 77.2408,
                            LongDescription = "Neu Delhi ist die Hauptstadt Indiens, Sitz der indischen Regierung, des Parlaments und der obersten Gerichte. Es ist mit 28,125 Mill. Einwohnern die drittgrößte Metropolregion der Welt (Stand 2019). Der Grundstein wurde während der britischen Kolonialzeit am 15. Dezember 1911 gelegt. Dadurch sollte die traditionelle Hauptstadt Kalkutta abgelöst werden. Den Namen New Delhi erhielt der neue Stadtteil erst im Jahre 1927. Neu-Delhi ist in den Sommermonaten regelmäßig von starker Hitze betroffen. In den letzten Jahren häufen sich jedoch Extremtemperaturen, die an 45°C heranreichen. Im Sommer 2019 wurde mit 48°C der Temperaturrekord für Neu-Delhi gebrochen. Schwerwiegende Veränderungen des Klimas haben in den letzten Jahren in weiten Teilen Indiens zu einer drastischen Verknappung des Trinkwassers geführt. Dies ist in besonderem Maße in Neu-Delhi spürbar. So gehört die Stadt zu jenen 21 bedeutenden indischen Städten, deren Grundwasserreserven nach Berechnungen der Regierungsagentur NITI Aayog bald vollständig aufgezehrt sein werden."
                        },
                        new WwwWalkData() {
                            Name = "Manang",
                            Description = "Partnerschule Lophel Ling Boarding School (LBS) in Nepal",
                            Position = 12,
                            Latitude = 28.6419,
                            Longitude = 84.0903,
                            LongDescription = "Manang ist ein Dorf und ein Village Development Committee in der Annapurnaregion in Nepal und ca. 45 km nördlich von Pokhara gelegen. Die Einwohnerzahl betrug 1991  391 Personen in 120 Haushalten. Bei der Volkszählung 2011 hatte Manang 630 Einwohner (davon 373 männlich) in 131 Haushalten. Manang liegt im breiten Manangtal am Ufer des Marsyangdi und nördlich des Annapurna Himals auf einer Höhe von 3519 m.  Westlich von Manang liegt auf einer Höhe von 4920 m der Tilicho Lake, nordwestlich befindet sich der 5416 m hohe Pass Thorong La und im Norden ragt der 6584 m hohe Chulu empor."
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
