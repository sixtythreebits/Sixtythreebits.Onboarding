using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Website;
using SixtyThreeBits.Web.Models.Base;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Filters.Website
{
    public class WebsiteFilterAttribut : IAsyncActionFilter
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

            WebUtilities.SetLayoutViewModel(viewData: c.ViewData, viewModel: _viewModel, key: WebConstants.ViewData.LayoutViewModel);
            await next();
        }

        async Task initStartUp(ActionExecutingContext filterContext)
        {
            var repository = _model.RepositoriesFactory.CreateSystemPropertiesRepository();            
            _model.SystemProperties = await repository.SystemPropertiesGet();
            _viewModel.ProjectName = _model.SystemProperties.ProjectName;            
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