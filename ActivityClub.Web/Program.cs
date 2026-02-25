using ActivityClub.Web.Services.Implementations;
using ActivityClub.Web.Services.Interfaces;
using ActivityClub.Web.Handlers;
using Microsoft.AspNetCore.Authentication;

namespace ActivityClub.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Needed so JwtForwardingHandler can read the current request cookie
            builder.Services.AddHttpContextAccessor();

            // Register the outgoing request handler (adds Authorization: Bearer <token> (authorization header) )
            builder.Services.AddTransient<JwtForwardingHandler>();

            // Register MVC authentication scheme that reads JWT from cookie and builds HttpContext.User
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "JwtCookie";
                    options.DefaultChallengeScheme = "JwtCookie";
                })
                .AddScheme<AuthenticationSchemeOptions, JwtCookieAuthenticationHandler>("JwtCookie", _ => { });

            builder.Services.AddAuthorization(); //Enables [Authorize] and role-based authorization in MVC

            //Register ApiSettings + HttpClientFactory
            builder.Services.Configure<ActivityClub.Web.Configurations.ApiSettings>(
                builder.Configuration.GetSection("ApiSettings"));

            var apiSettings = builder.Configuration
                .GetSection("ApiSettings")
                .Get<ActivityClub.Web.Configurations.ApiSettings>();

            builder.Services.AddHttpClient("ActivityClubApi", client =>
            {
                client.BaseAddress = new Uri(apiSettings!.BaseUrl);
            })
            .AddHttpMessageHandler<JwtForwardingHandler>(); //automatically attaches Authorization: Bearer <token> from cookie


            // Api clients
            builder.Services.AddScoped<IEventApiClient, EventApiClient>();
            builder.Services.AddScoped<IAuthApiClient, AuthApiClient>();
            builder.Services.AddScoped<ILookupApiClient, LookupApiClient>();
            builder.Services.AddScoped<IGuideApiClient, GuideApiClient>();
            builder.Services.AddScoped<IProfileApiClient, ProfileApiClient>();
            builder.Services.AddScoped<IMemberProfileApiClient, MemberProfileApiClient>();
            // UI services
            builder.Services.AddScoped<IEventsUiService, EventsUiService>();
            builder.Services.AddScoped<ILookupUiService, LookupUiService>();
            builder.Services.AddScoped<IAuthUiService, AuthUiService>();
            builder.Services.AddScoped<IGuidesUiService, GuidesUiService>();
            builder.Services.AddScoped<IProfileUiService, ProfileUiService>();
            builder.Services.AddScoped<IMemberProfileUiService, MemberProfileUiService>();


            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
