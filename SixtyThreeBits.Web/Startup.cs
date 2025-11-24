using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Libraries.Loggers;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using System;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web
{
    public class Startup
    {
        #region Properties
        readonly AppSettingsCollection _appSettings;
        readonly UtilityCollection _utilities;
        readonly RepositoryFactory _repositoryFactory;
        readonly bool _isDevelopmentEnvironment;
        #endregion

        #region Constructors
        public Startup(IWebHostEnvironment env)
        {
            IConfiguration appSettingsConfiguration;
            _isDevelopmentEnvironment = env.IsDevelopment();
            if (_isDevelopmentEnvironment)
            {
                appSettingsConfiguration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.json").Build();
            }
            else
            {
#if DEBUG
                appSettingsConfiguration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.debug.json").Build();
#else
                appSettingsConfiguration = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.release.json").Build();                
#endif
            }
            _appSettings = new AppSettingsCollection(
                contentRootPath: env.ContentRootPath,
                webRootPath: env.WebRootPath,
                configuration: appSettingsConfiguration,
                isDevelopmentEnvironment: _isDevelopmentEnvironment
            );
            _utilities = new UtilityCollection(
                contentRootPath: env.ContentRootPath,
                webRootPath: env.WebRootPath
            );
            _repositoryFactory = new RepositoryFactory(
                dbConnectionString: _appSettings.ConnectionStrings.DbConnectionString,
                logger: new ErrorLogTxtFileLogger()
            );
        }
        #endregion

        #region Methods
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_appSettings);
            services.AddSingleton(_utilities);
            services.AddSingleton(_repositoryFactory);

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.Configure<CookiePolicyOptions>(Options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                Options.CheckConsentNeeded = context => false;
                Options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddControllersWithViews(Options =>
            {
                Options.RespectBrowserAcceptHeader = true;
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.Configure<RouteOptions>(routeOptions =>
            {
                routeOptions.AppendTrailingSlash = false;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var urlRewriteOptions = new RewriteOptions().AddRedirect(@"(.*)/$", "$1", 301).AddRewrite(@"^$", "/", true).AddRewrite(@"(.*)/$", "$1", true);

            if (_isDevelopmentEnvironment)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {

            }

            app.UseRewriter(urlRewriteOptions);

            app.UseFileServer();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(_appSettings.UploadFolderPhysicalPath),
                RequestPath = _appSettings.UploadFolderHttpPath.TrimEnd('/')
            });

            app.UseRouting();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        #endregion
    }
}