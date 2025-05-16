using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Libraries.Loggers;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using System;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web
{
    public class Startup
    {
        readonly AppSettingsCollection _appSettings;
        readonly UtilityCollection _utilities;
        readonly RepositoryFactory _repositoryFactory;

        public Startup(IWebHostEnvironment env)
        {
            IConfiguration appSettingsConfiguration;
            if (env.IsDevelopment())
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
                configuration: appSettingsConfiguration
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
            

            services.AddControllersWithViews(Options=> { 
                Options.RespectBrowserAcceptHeader = true;                
            }).AddJsonOptions(options => { 
                options.JsonSerializerOptions.PropertyNamingPolicy = null;  
            });
            
            services.Configure<RouteOptions>(routeOptions => {
                routeOptions.AppendTrailingSlash = false;
            });                        
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

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

        public async Task RenderNotFoundView(HttpContext context)
        {
            // Set the response content type to HTML
            context.Response.ContentType = "text/html";

            // Get the MVC services from the request scope
            var services = context.RequestServices;
            var viewEngine = services.GetRequiredService<Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine>();
            var tempDataFactory = services.GetRequiredService<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionaryFactory>();
            var tempData = tempDataFactory.GetTempData(context);

            // Create an ActionContext using the current HttpContext
            var actionContext = new Microsoft.AspNetCore.Mvc.ActionContext(
                context,
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            );

            // Define the error view you want to return (e.g., Error.cshtml)
            var viewName = ViewNames.Website.Errors.NotFoundView; // You can define a specific view here

            // Use a ViewDataDictionary to pass data to the view (without ModelState)
            var viewModel = new NotFoundViewModel
            {
                PluginsClient = new PluginsClientViewModel(),
                UrlLogout = "/"
            };
            viewModel.PluginsClient.EnableBootstrap(true);
            var viewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary()
            )
            {
                Model = viewModel
            };

            // Find the view
            var viewEngineResult = viewEngine.GetView(viewName, viewName, false);

            if (viewEngineResult.Success)
            {
                // Render the view and write it to the response stream
                var view = viewEngineResult.View;
                using (var writer = new System.IO.StringWriter())
                {
                    var viewContext = new Microsoft.AspNetCore.Mvc.Rendering.ViewContext(
                        actionContext,
                        view,
                        viewData,
                        tempData,
                        writer,
                        new Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelperOptions()
                    );

                    await view.RenderAsync(viewContext);
                    await context.Response.WriteAsync(writer.ToString());
                }
            }
            else
            {
                // If the view is not found, you can display a default message
                await context.Response.WriteAsync("<h1>An error occurred</h1>");
            }
        }        
    }
}