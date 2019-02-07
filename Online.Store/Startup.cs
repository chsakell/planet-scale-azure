using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Online.Store.Azure.Services;
using Online.Store.DocumentDB;
using Online.Store.Mappings;
using Online.Store.RedisCache;
using Microsoft.EntityFrameworkCore;
using Online.Store.Services;
using Online.Store.Extensions;
using Online.Store.SqlServer;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Online.Store.ServiceBus;
using Online.Store.Storage;
using Online.Store.AzureSearch;
using Online.Store.Models;
using Microsoft.AspNetCore.Identity;
using Online.Store.Identity;

namespace Online_Store
{
    public class Startup
    {
        private const string RedisCacheConnStringFormat = "{0},abortConnect=false,ssl=true,password={1}";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDirectoryBrowser();

            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
            
            // Make Configuration injectable
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Online.Store.SqlServer")));

            services.AddDbContext<ApplicationIdentityDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection"),
                b => b.MigrationsAssembly("Online.Store")));

            if (!bool.Parse(Configuration["UseIdentity"]))
            {
                if (!string.IsNullOrEmpty(Configuration["AzureAd:ClientId"]))
                {
                    services.AddAuthentication(sharedOptions =>
                    {
                        sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    })
                    .AddAzureAd(options => Configuration.Bind("AzureAd", options))
                    .AddCookie();
                }
            }
            else
            {
                services.AddIdentity<ApplicationUser, IdentityRole>(config =>
                {
                    config.SignIn.RequireConfirmedEmail = false;
                })
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();
            }

            // Configure Cache
            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = string.Format(RedisCacheConnStringFormat,
                            Configuration["RedisCache:Endpoint"] + ".redis.cache.windows.net:6380",
                            Configuration["RedisCache:Key"]);

                // If you want to use local redis, first install locally
                //option.Configuration = "localhost";

                //option.InstanceName = "master-";
            });

            services.AddScoped<IDocumentDBRepository, DocumentDBStoreRepository>();
            services.AddScoped<IRedisCacheRepository, RedisCacheReposistory>();
            // services.AddScoped<IShardingRepository, ShardingRepository>();
            services.AddScoped<IServiceBusRepository, ServiceBusRepository>();
            services.AddScoped<IStorageRepository, StorageRepository>();
            services.AddScoped<ISearchRepository, SearchStoreRepository>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IServiceBusService, ServiceBusService>();
            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<IAzureSearchService, AzureSearchService>();
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                //app.Use(async (context, next) =>
                //{
                //    if (context.Request.IsHttps)
                //    {
                //        await next();
                //    }
                //    //else //if(bool.Parse(Configuration["Production:UseAlwaysHTTPS"]))
                //    //{
                //    //    var toHttps = "https://" + context.Request.Host + context.Request.Path;
                //    //    context.Response.Redirect(toHttps);
                //    //}
                //});

                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "images")),
                RequestPath = new PathString("/images")
            });

            app.UseAuthentication();

            AutoMapperConfiguration.Configure(Configuration);

            app.UseAntiforgeryToken();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

            StorageInitializer.Initialize(Configuration);
            DocumentDBInitializer.Initialize(Configuration);
            AzureSearchInitializer.Initialize(Configuration);
        }
    }
}
