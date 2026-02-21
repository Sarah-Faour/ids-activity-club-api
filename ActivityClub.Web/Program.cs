using ActivityClub.Web.Services.Implementations;
using ActivityClub.Web.Services.Interfaces;

namespace ActivityClub.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Register ApiSettings + HttpClientFactory
            builder.Services.Configure<ActivityClub.Web.Configurations.ApiSettings>(
                builder.Configuration.GetSection("ApiSettings"));

            var apiSettings = builder.Configuration
                .GetSection("ApiSettings")
                .Get<ActivityClub.Web.Configurations.ApiSettings>();

            builder.Services.AddHttpClient("ActivityClubApi", client =>
            {
                client.BaseAddress = new Uri(apiSettings!.BaseUrl);
            });


            // Api clients
            builder.Services.AddScoped<IEventApiClient, EventApiClient>();
            builder.Services.AddScoped<IAuthApiClient, AuthApiClient>();
            builder.Services.AddScoped<ILookupApiClient, LookupApiClient>();
            // UI services
            builder.Services.AddScoped<IEventsUiService, EventsUiService>();
            builder.Services.AddScoped<ILookupUiService, LookupUiService>();
            builder.Services.AddScoped<IAuthUiService, AuthUiService>();


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
