using Dakoq.WebApp.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace Dakoq.WebApp
{
    public class Program
    {
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
                services.PostConfigure<AppConfiguration>(options =>
                {
                    var config = builder.Configuration;

                    KnoqAuthenticationInfo knoqAuth = new()
                    {
                        TraqUsername = config.GetValue<string>("TRAQ_USERNAME"),
                        TraqPassword = config.GetValue<string>("TARQ_PASSWORD")
                    };

                    MySqlConnectionStringBuilder csb = new()
                    {
                        Server = config.GetValue<string>("NS_MARIADB_HOSTNAME"),
                        Port = uint.TryParse(config.GetValue<string>("NS_MARIADB_PORT"), out var _port) ? _port : default,
                        UserID = config.GetValue<string>("NS_MARIADB_USER"),
                        Password = config.GetValue<string>("NS_MARIADB_PASSWORD"),
                        Database = config.GetValue<string>("NS_MARIADB_DATABASE")
                    };

                    options.DakoqBaseAddress = Uri.TryCreate(config.GetValue<string>("DAKOQ_SERVER"), UriKind.RelativeOrAbsolute, out var _uri) ?_uri : null;
                    options.DbConnectionString = csb.ConnectionString;
                    options.KnoqAuthInfo = knoqAuth;
                    options.TraqApiBaseAddress = Uri.TryCreate(config.GetValue<string>("TRAQ_API_SERVER"), UriKind.RelativeOrAbsolute, out _uri) ? _uri : null;
                });

                // Database context
                services.AddDbContextFactory<Repository.RepositoryContext>((services, options) => {
                    var conf = services.GetRequiredService<IOptions<AppConfiguration>>();
                    options.UseMySQL(conf.Value.DbConnectionString!);
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
