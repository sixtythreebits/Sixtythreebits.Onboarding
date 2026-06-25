using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Base;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class AdminModelBase : ModelBase63
    {
        #region Methods
        public IActionResult GetNotFoundAdminViewResult()
        {
            var viewModel = new NotFoundViewModel();
            PluginsClient.EnableAdminTheme(true);
            viewModel.PluginsClient = PluginsClient;
            viewModel.UrlLogout = UrlFactory.Website.CreateUrlLogout();

            return new ViewResult
            {
                ViewName = ViewNamesShared63.Admin.NotFoundView,
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
