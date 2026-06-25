using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Domain.ViewModels.Website;
using SixtyThreeBits.Web.Models.Base;

namespace SixtyThreeBits.Web.Models.Website
{
    public class WebsiteModelBase : ModelBase63
    {
        #region Properties
        public WebsiteLayoutViewModel LayoutViewModel { get; set; }
        #endregion

        #region Methods
        public IActionResult GetNotFoundWebsiteViewResult()
        {
            var viewModel = new NotFoundViewModel();
            viewModel.PluginsClient = PluginsClient;
            viewModel.UrlLogout = UrlFactory.Website.CreateUrlLogout();

            return new ViewResult
            {
                ViewName = ViewNamesShared63.Website.NotFoundView,
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = viewModel
                },
                StatusCode = Enums.HttpStatusCodes.Status404NotFound
            };
        }
        #endregion
    }
}
