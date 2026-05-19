using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Web.Controllers.Base;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Website;
using SixtyThreeBits.Web.Models.Website;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class WebsiteControllerBase<T> : ControllerBase<T> where T : WebsiteModelBase, new()
    {
        #region Properties
        WebsiteModelBase _model;
        WebsiteLayoutViewModel _viewModel;
        #endregion

        #region Methods
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await base.OnActionExecutionAsync(context, async () =>
            {
                _model = WebUtilities.GetModelFromController<WebsiteModelBase>(context.Controller);
                var c = context.Controller as Controller;

                if (_model.IsAjaxRequest)
                {
                    return await next();
                }
                else
                {
                    initViewModel();
                    initClientPlugins();
                    initPageTitle();
                    _model.ViewData[ViewDataKeys63.LayoutViewModel] = _viewModel;

                    return await next();
                }
            });
        }
        void initViewModel()
        {
            _model.LayoutViewModel = _viewModel = new WebsiteLayoutViewModel();
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
            _model.PageTitle = _viewModel.PageTitle = new PageTitle63(_model.SystemProperties.ProjectName);
        }
        #endregion
    }
}
