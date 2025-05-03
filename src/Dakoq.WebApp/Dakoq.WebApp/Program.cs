using Dakoq.WebApp.Components;
using Knoq;
using Knoq.Extensions.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Security.Claims;
using Traq;

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

                services.AddLogging(lb =>
                {
                    lb.AddFilter(builder.Environment.IsDevelopment()
                        ? lv => lv >= LogLevel.Information
                        : lv => lv >= LogLevel.Warning);
                    lb.AddSimpleConsole(o =>
                    {
                        o.IncludeScopes = true;
                        o.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                    });
                });

                // API controllers
                services.AddControllers();

                // Razor (View)
                services.AddRazorComponents()
                    .AddInteractiveServerComponents();

                // Authentication
                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(static options =>
                    {
                        options.LoginPath = "/login";
                        options.LogoutPath = "/logout";
                    });
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

                    if (config.GetValue<string>("env-files") is string envFiles && !string.IsNullOrWhiteSpace(envFiles))
                    {
                        foreach (var path in envFiles.Split(';'))
                        {
                            config.AddIniStream(File.OpenRead(path));
                        }
                    }

                    KnoqAuthenticationInfo knoqAuth = new()
                    {
                        TraqUsername = config.GetValue<string>("TRAQ_USERNAME"),
                        TraqPassword = config.GetValue<string>("TRAQ_PASSWORD")
                    };

                    TraqOAuthClientInfo traqOAuthClient = new()
                    {
                        ClientId = config.GetValue<string>("TRAQ_CLIENT_ID"),
                        ClientSecret = config.GetValue<string>("TRAQ_CLIENT_SECRET")
                    };

                    MySqlConnectionStringBuilder csb = new()
                    {
                        Server = config.GetValue<string>("NS_MARIADB_HOSTNAME"),
                        Port = uint.TryParse(config.GetValue<string>("NS_MARIADB_EXPOSE_PORT") ?? config.GetValue<string>("NS_MARIADB_PORT"), out var _port) ? _port : default,
                        UserID = config.GetValue<string>("NS_MARIADB_USER"),
                        Password = config.GetValue<string>("NS_MARIADB_PASSWORD"),
                        Database = config.GetValue<string>("NS_MARIADB_DATABASE")
                    };

                    options.DakoqBaseAddress = Uri.TryCreate(config.GetValue<string>("DAKOQ_SERVER"), UriKind.RelativeOrAbsolute, out var _uri) ? _uri : null;
                    options.DbConnectionString = csb.ConnectionString;
                    options.KnoqApiBaseAddress = Uri.TryCreate(config.GetValue<string>("KNOQ_API_SERVER"), UriKind.RelativeOrAbsolute, out _uri) ? _uri : null;
                    options.KnoqAuthInfo = knoqAuth;
                    options.TraqApiBaseAddress = Uri.TryCreate(config.GetValue<string>("TRAQ_API_SERVER"), UriKind.RelativeOrAbsolute, out _uri) ? _uri : null;
                    options.TraqOAuthClientInfo = traqOAuthClient;
                });

                // Database context
                services.AddDbContextFactory<Repository.RepositoryContext>(static (services, options) =>
                {
                    var conf = services.GetRequiredService<IOptions<AppConfiguration>>();
                    options.UseMySQL(conf.Value.DbConnectionString!);
                });
                services.AddDbContextFactory<Infrastructure.Repository.Repository>(static (services, options) =>
                {
                    var config = services.GetRequiredService<IOptions<AppConfiguration>>().Value;
                    options.UseMySQL(config.DbConnectionString!);
                });

                // Authenticated user information
                services.AddScoped(static s =>
                {
                    var conf = s.GetRequiredService<IOptions<AppConfiguration>>();
                    var authStateProvider = s.GetRequiredService<AuthenticationStateProvider>();

                    return authStateProvider.GetAuthenticationStateAsync().ContinueWith<AuthUserInfo?>(t =>
                    {
                        var u = t.Result.User;

                        string? token = null;
                        string? username = null;
                        Guid? userId = null;
                        DateTimeOffset? tokenExpiresAt = null;

                        foreach (var c in u.Claims)
                        {
                            switch (c.Type)
                            {
                                case ClaimTypes.Expiration:
                                    if (tokenExpiresAt is null && DateTimeOffset.TryParse(c.Value, out var dt))
                                    {
                                        tokenExpiresAt = dt;
                                    }
                                    break;
                                case ClaimTypes.Name:
                                    if (username is null && !string.IsNullOrWhiteSpace(c.Value))
                                    {
                                        username = c.Value;
                                    }
                                    break;
                                case ClaimTypes.NameIdentifier:
                                    if (userId is null && Guid.TryParse(c.Value, out var guid))
                                    {
                                        userId = guid;
                                    }
                                    break;
                                case ClaimTypes.UserData:
                                    if (token is null && !string.IsNullOrWhiteSpace(c.Value))
                                    {
                                        token = c.Value;
                                    }
                                    break;
                            }
                        }

                        if (token is null || username is null || userId is null || tokenExpiresAt is null)
                        {
                            return null;
                        }
                        return new AuthUserInfo()
                        {
                            AccessToken = token,
                            Id = userId.Value,
                            Name = username,
                            TokenExpiresAt = tokenExpiresAt.Value,
                        };
                    });
                });

                services.AddOptions()
                    .AddSingleton<IConfigureOptions<TraqApiClientOptions>>(static sp => new ConfigureNamedOptions<TraqApiClientOptions>(Options.DefaultName, options =>
                        {
                            var conf = sp.GetRequiredService<IOptions<AppConfiguration>>().Value;
                            options.BaseAddress = conf.TraqApiBaseAddress?.ToString() ?? "https://q.trap.jp/api/v3";
                        }));
                services.AddAuthenticatedKnoqApiClient(
                    static (sp, options) =>
                    {
                        var conf = sp.GetRequiredService<IOptions<AppConfiguration>>().Value;
                        options.BaseAddress = conf.KnoqApiBaseAddress?.ToString() ?? "https://knoq.trap.jp";
                    },
                    static (sp, authInfo) =>
                    {
                        var conf = sp.GetRequiredService<IOptions<AppConfiguration>>().Value;
                        authInfo.Username = conf.KnoqAuthInfo?.TraqUsername;
                        authInfo.Password = conf.KnoqAuthInfo?.TraqPassword;
                    });

                services.AddScoped<Task<Traq.ITraqApiClient?>>(static async s =>
                {
                    var u = await s.GetRequiredService<Task<AuthUserInfo?>>()
                       ?? throw new Exception("The user is not authenticated.");
                    var o = s.GetRequiredService<IOptions<AppConfiguration>>().Value;
                    return new Traq.TraqApiClient(Options.Create(new Traq.TraqApiClientOptions()
                    {
                        BaseAddress = o.TraqApiBaseAddress?.ToString() ?? "https://q.trap.jp/api/v3",
                        BearerAuthToken = u.AccessToken
                    }));
                });

                services.AddHostedService<Services.KnoqSyncService>();
                services.Configure<Services.KnoqSyncServiceOptions>(static o =>
                {
                    o.FetchInterval = TimeSpan.FromMinutes(5);
                });

                services.AddHostedService<Services.MemoryMonitorService>();
                services.Configure<Services.MemoryMonitorServiceOptions>(static o =>
                {
                    o.CheckInterval = TimeSpan.FromSeconds(10);
                    o.ThresholdBytes = 100 * 1024 * 1024;
                });

                services.AddSingleton(TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));
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
                .AddInteractiveServerRenderMode();
            app.MapControllers();

            app.Run();
        }
    }
}
