using Dakoq.WebApp.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace Dakoq.WebApp
{
    public class Program
    {
        public static Uri TraqApiBaseAddress = new("https://q.trap.jp/api/v3");

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure services
            {
                var services = builder.Services;

                // API controllers
                services.AddControllers();

                // Razor (View)
                services.AddRazorComponents()
                    .AddInteractiveServerComponents()
                    .AddInteractiveWebAssemblyComponents();

                // Authentication
                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie();
                services
                    .AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>()
                    .AddCascadingAuthenticationState();

                // Http client
                Uri traqApiBaseAddress = new(builder.Configuration.GetValue<string>("TARQ_API_SERVER") ?? "https://q.trap.jp/api/v3");
                services.AddHttpClient("traQ", c => c.BaseAddress = traqApiBaseAddress);

                // Configuration
                services.Configure<AppConfiguration>(options =>
                {

                });
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseAntiforgery();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);
            app.MapControllers();

            app.Run();
        }
    }
}
