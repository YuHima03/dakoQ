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

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddControllers();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            builder.Services
                .AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>()
                .AddCascadingAuthenticationState();

            builder.Services.AddHttpClient("traQ", client =>
            {
                client.BaseAddress = TraqApiBaseAddress;
            });

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
