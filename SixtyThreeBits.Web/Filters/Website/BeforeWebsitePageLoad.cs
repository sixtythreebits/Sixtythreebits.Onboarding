using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Models.Shared;
using SixtyThreeBits.Web.Models.Website;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Filters.Website
{
    public class BeforeWebsitePageLoad : IAsyncActionFilter
    {
        #region Properties
        ModelBase _model;
        WebsiteLayoutViewModel _viewModel;
        #endregion

        #region Methods
        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _viewModel = new WebsiteLayoutViewModel();
            _model = WebUtilities.GetModelFromController<ModelBase>(filterContext.Controller);
            var c = filterContext.Controller as Controller;

            await initStartUp(filterContext);
            initClientPlugins();
            initPageTitle();
            initLanguageSwitch();

            WebUtilities.SetLayoutViewModel(viewData: c.ViewData, viewModel: _viewModel, key: WebConstants.ViewData.LayoutViewModel);
            await next();
        }

        async Task initStartUp(ActionExecutingContext filterContext)
        {
            var repository = _model.RepositoriesFactory.GetSystemPropertiesRepository();
            _model.LanguageCultureCode = filterContext.RouteData.Values[WebConstants.RouteValues.Culture]?.ToString() ?? Enums.Languages.GEORGIAN;
            _model.SystemProperties = await repository.SystemPropertiesGet();
            _viewModel.ScriptsHeader = _model.SystemProperties.ScriptsHeader;
            _viewModel.ScriptsBodyStart = _model.SystemProperties.ScriptsBodyStart;
            _viewModel.ScriptsBodyEnd = _model.SystemProperties.ScriptsBodyEnd;
        }

        void initClientPlugins()
        {
            _model.PluginsClient
            .EnableGoogleFonts(true)
            .EnableFontAwesome(true)
            .Enable63BitsFonts(true)
            .EnableBootstrap(true)
            .EnableJQuery(true)
            .EnablePreloader(true);

            _viewModel.PluginsClient = _model.PluginsClient;
        }

        void initPageTitle()
        {
            _model.PageTitle = _viewModel.PageTitle = new PageTitle(_model.SystemProperties.ProjectName);
        }


        void initLanguageSwitch()
        {
            _viewModel.ShowUrlKa = _model.LanguageCultureCode != Enums.Languages.GEORGIAN;
            _viewModel.ShowUrlEn = !_viewModel.ShowUrlKa;

            var Index = _model.UrlCurrentPageWithDomain.IndexOf('/', 8) + 1;

            _viewModel.UrlEn = _model.UrlCurrentPageWithDomain.Replace($"/{_model.LanguageCultureCode}", null);
            _viewModel.UrlKa = _model.UrlCurrentPageWithDomain.Insert(Index, $"{Enums.Languages.ENGLISH}/").Replace($"/{_model.LanguageCultureCode}", null);
        }
        #endregion

        #region Nested Classes
        class customRedirectResult
        {
            #region Properties
            public bool IsRedirect { get; set; }
            public string RedirectUrl { get; set; }
            #endregion
        }
        #endregion
    }
}