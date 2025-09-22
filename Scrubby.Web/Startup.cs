using System;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using RestEase.HttpClientFactory;
using Scrubby.Web.Security;
using Scrubby.Web.Services;
using Scrubby.Web.Services.Interfaces;
using Scrubby.Web.Services.SQL;
using Scrubby.Web.Stockpile;
using Tgstation.Auth;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

namespace Scrubby.Web;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add status service
        var statusService = new AppStatusService();
        services.AddSingleton<IAppStatusService>(statusService);
        
        services.AddDistributedMemoryCache();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(5);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddMvc()
            .AddRazorRuntimeCompilation()
            .AddNewtonsoftJson();
        services.AddWebOptimizer(pipeline =>
        {
            pipeline.AddCssBundle("/css/bundle.css",
                "/lib/bootstrap/css/bootstrap.css",
                "/lib/bootstrap-toggle/css/bootstrap4-toggle.css",
                "/lib/font-awesome/css/all.css",
                "/lib/datatables/datatables.css",
                "/lib/bootstrap-select/css/bootstrap-select.css",
                "/lib/tagify/tagify.css",
                "/lib/prism/prism.css",
                "/css/site.css",
                "/css/file.css");

            pipeline.AddJavaScriptBundle("/js/bundle.js",
                "/lib/jquery/dist/jquery.js",
                "/lib/moment/js/moment.js",
                "/lib/moment/js/moment-duration-format.js",
                "/lib/infinite-scroll/infinite-scroll.pkgd.min.js",
                "/lib/hammer/hammer.min.js",
                "/lib/d3/d3.js",
                "/lib/d3/gantt-chart/gantt-chart-d3.js",
                "/lib/bootstrap/js/bootstrap.bundle.js",
                "/lib/bootstrap-toggle/js/bootstrap4-toggle.js",
                "/lib/tagify/tagify.js",
                "/lib/clipboard/clipboard.js",
                "/lib/datatables/datatables.js",
                "/lib/jquery-scrollTo/js/jquery.scrollTo.js",
                "/lib/bootstrap-select/js/bootstrap-select.js",
                "/lib/prism/prism.js",
                "/js/site.js");
        });

        // Forward along data from docker
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("::ffff:172.18.0.0"), 16));
        });

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = TgDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.Cookie.Name = "Scrubby";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                options.Cookie.IsEssential = true;
            })
            .AddTgstation(options =>
            {
                var tgAuthSection = Configuration.GetSection("Authentication:Tgstation");
                options.ClientId = tgAuthSection["ClientId"];
                options.ClientSecret = tgAuthSection["ClientSecret"];
                options.CallbackPath = "/auth";
                options.Scope.Add("user.groups");
            });

        services.AddTransient<IPlayerService, SqlPlayerService>();
        services.AddSingleton<IRoundService, SqlRoundService>();
        services.AddTransient<IConnectionService, SqlConnectionService>();
        services.AddSingleton<IAnnouncementService, SqlAnnouncementService>();
        services.AddTransient<ICKeyService, SqlCKeyService>();
        services.AddTransient<IUserService, SqlUserService>();
        services.AddTransient<INewscasterService, SqlNewscasterService>();
        services.AddTransient<IScrubbyService, SqlScrubbyService>();
        services.AddTransient<IFileService, SqlFileService>();
        services.AddTransient<IMaintenanceService, SqlMaintenanceService>();
        services.AddTransient<BYONDDataService>();
        services.AddSingleton<IClaimsTransformation, ScrubbyUserClaimsTransformation>();
        services.AddRestEaseClient<IStockpileApi>(Configuration.GetValue<string>("Stockpile:Url"), new()
        {
            InstanceConfigurer = instance =>
            {
                instance.ApiKey = Configuration.GetValue<string>("Stockpile:ApiKey");
            }, RestClientConfigurer = client =>
            {
                client.JsonSerializerSettings = new JsonSerializerSettings().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            }
        });
        services.AddTransient<IFileContentService, StockpileFileService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        app.UseForwardedHeaders();

        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions()
            {
                AllowStatusCode404Response = true,
                ExceptionHandlingPath = "/exception"
            });
            
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/error", "?statusCode={0}");
        app.UseHttpsRedirection();
        app.UseWebOptimizer();
        app.UseStaticFiles();
        app.UseSession();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        });
    }
}