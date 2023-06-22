using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;

namespace TCC.Front_end
{
    public class Startup : IStartup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 443;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;

            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                var url = this.Configuration.GetValue<string>("tcc-api:autenticacao");

                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // cookie middle setup above
                options.SignedOutRedirectUri = "/";
#if DEBUG
                //options.Authority = "http://localhost:3000"; // Auth Server
                options.RequireHttpsMetadata = false; // only for development 
#else
                //options.Authority = "https://api-tcc-autenticacao.azurewebsites.net"; // Auth Server

                
                options.RequireHttpsMetadata = true; // only for development 
#endif

                options.Authority = url;
                options.ClientId = "TCC-auth"; // client setup in Auth Server
                options.ClientSecret = "secret";
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                options.Scope.Add("TCC-auth");
                options.Scope.Add("offline_access");
                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = true;

                options.CallbackPath = new PathString("/Home");
                options.CorrelationCookie.SameSite = SameSiteMode.None;

            });

            services.AddControllersWithViews(options =>
            {
                options.EnableEndpointRouting = false;

            }).AddRazorRuntimeCompilation();
        }

        public void Configure(WebApplication app, IWebHostEnvironment environment)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseDeveloperExceptionPage();
            app.UseCookiePolicy();

            
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }

    }

    public interface IStartup
    {
        IConfiguration Configuration { get; }
        void Configure(WebApplication app, IWebHostEnvironment environment);
        void ConfigureServices(IServiceCollection services);
    }

    public static class StartupExtensions
    {
        public static WebApplicationBuilder UseStartup<TStartup>(this WebApplicationBuilder WebAppBuilder) where TStartup : IStartup
        {
            var startup = Activator.CreateInstance(typeof(TStartup), WebAppBuilder.Configuration) as IStartup;
            if (startup == null) throw new ArgumentException("Classe Startup.cs inválida!");

            startup.ConfigureServices(WebAppBuilder.Services);

            var app = WebAppBuilder.Build();
            startup.Configure(app, app.Environment);
            app.Run();

            return WebAppBuilder;
        }
    }
}
