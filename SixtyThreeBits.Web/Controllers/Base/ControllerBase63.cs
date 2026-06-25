using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories;
using SixtyThreeBits.Core.Libraries.FileStorages;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Base;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Base
{
    public class ControllerBase63<T> : Controller where T : ModelBase63, new()
    {
        #region Properties
        public T Model { get; private set; }
        #endregion        

        #region Constructors
        public ControllerBase63()
        {
            Model = new T();            
        }
        #endregion

        #region Methods
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            setInitialProperties(context);
            await initSystemProperties();
            initFileStorage();
            initUrlFactory();
            initPluginsClient();
            await initUser();
            await next();
        }
        void setInitialProperties(ActionExecutingContext context)
        {
            var c = context.Controller as Controller;
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            Model.AppSettings = HttpContext.RequestServices.GetRequiredService<AppSettingsCollection>();
            Model.Utilities = HttpContext.RequestServices.GetRequiredService<UtilityCollection>();
            Model.RepositoryFactory = HttpContext.RequestServices.GetRequiredService<RepositoryFactory>();

            Model.Controller = c;
            Model.RouteData = c.RouteData;
            Model.ViewData = c.ViewData;
            Model.Request = c.Request;
            Model.Response = c.Response;

            Model.ControllerName = actionDescriptor.ControllerTypeInfo.Name;
            Model.ActionName = actionDescriptor.ActionName;
            
            Model.WebsiteDomain = WebUtilities.GetWebsiteDomain(c.Request);
            Model.UrlCurrentPageWithoutDomain = c.Request.Path;
            Model.UrlCurrentPageWithDomain = $"{Model.WebsiteDomain}{c.Request.Path}";

            Model.IsHttps = c.Request.IsHttps;
            Model.IsAjaxRequest = c.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            Model.IP = WebUtilities.GetClientIP(c.Request);

            Model.SessionAssistance = new SessionAssistance63(c.HttpContext.Session);
            Model.CookieAssistance = new CookieAssistance63(c.Request, c.Response);
            
            Model.UrlPreviousPage = Model.Request.Headers["Referer"].ToString();            
        }
        async Task initUser()
        {
            Model.User = Model.SessionAssistance.GetUser<UserDTO>();

            if (Model.User == null)
            {
                var userIDEncrypted = Model.CookieAssistance.Get(CookieKeys63.User);
                var userID = userIDEncrypted.AesDecryptString().ToInt();
                if (userID != null)
                {
                    var repository = Model.RepositoryFactory.CreateUsersRepository();
                    Model.User = await repository.UsersGetSingleByID(userID);
                    if (Model.User != null)
                    {
                        Model.SessionAssistance.SetUser(Model.User);
                    }
                }
            }
        }
        async Task initSystemProperties()
        {
            var repository = Model.RepositoryFactory.CreateSystemPropertiesRepository();
            Model.SystemProperties = await repository.SystemPropertiesGet();
        }
        void initFileStorage()
        {
            Model.FileStorage = new LocalFileStorage(
                uploadFolderPhysicalPath: Model.AppSettings.UploadFolderPhysicalPath,
                uploadFolderHttpPath: Model.AppSettings.UploadFolderHttpPath,
                noImageHttpPath: "/images/no_photo.jpg",
                websiteDomain: Model.WebsiteDomain
           );            
        }
        void initUrlFactory()
        {
            Model.UrlFactory = new UrlFactory63(
                url: Model.Controller.Url,
                languageCultureCodeSystem: "en",
                languageCultureCodeDefault: "en",
                protocol: Model.IsHttps ? "https" : "http"
            );
        }
        void initPluginsClient()
        {
            Model.PluginsClient = new PluginsClientViewModel(languageCultureCode: "en");
        }        
        #endregion
    }
}