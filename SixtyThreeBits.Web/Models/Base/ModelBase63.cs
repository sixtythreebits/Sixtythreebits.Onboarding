using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using System.Collections.Generic;

namespace SixtyThreeBits.Web.Models.Base
{
    public class ModelBase63
    {
        #region Properties        
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string UrlCurrentPageWithDomain { get; set; }
        public string UrlCurrentPageWithoutDomain { get; set; }
        public string UrlPreviousPage { get; set; }
        public string WebsiteDomain { get; set; }
        public string WebsiteHttpPath => $"{WebsiteDomain}/";
        public string IP { get; set; }
        public bool IsHttps { get; set; }
        public string Protocol => IsHttps ? "https" : "http";
        public bool IsAjaxRequest { get; set; }
        public AppSettingsCollection AppSettings { get; set; }
        public RepositoryFactory RepositoryFactory { get; set; }        
        public UtilityCollection Utilities { get; set; }        
        public Controller Controller { get; set; }
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        public RouteData RouteData { get; set; }
        public UrlFactory63 UrlFactory { get; set; }        
        public PageTitle63 PageTitle { get; set; }
        public SessionAssistance63 SessionAssistance { get; set; }
        public CookieAssistance63 CookieAssistance { get; set; }
        public ViewDataDictionary ViewData { get; set; }
        public IFileStorage FileStorage { get; set; }

        public Breadcrumbs63 Breadcrumbs { get; set; }
        public List<Menu63ViewItem> Tabs { get; set; } = [];

        public PluginsClientViewModel PluginsClient { get; set; }
        public UserDTO User { get; set; }
        public bool IsUserLoggedIn => User != null;
        
        public SystemPropertiesDTO SystemProperties { get; set; }

        public ToastNotificationManager63 ToastNotificationManager { get; set; }        
        #endregion
    }
}