using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/products/{productID:int}/properties")]
    [TypeFilter(typeof(ProductFilterAttribute), Order = 2)]
    public class ProductPropertiesController : AdminControllerBase<ProductPropertiesModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.ProductPropertiesController.Properties)]
        public async Task<IActionResult> Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableJQueryNumericInput(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = await Model.GetViewModel();
            return View(ViewNames.Admin.Products.ProductPropertiesView, viewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(ProductPropertiesModel.ViewModel submitModel)
        {
            var viewModel = await Model.Save(submitModel);
            if (viewModel.HasErrors)
            {
                Model.PluginsClient.Enable63BitsForms(true).EnableJQueryNumericInput(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);
                return View(ViewNames.Admin.Products.ProductPropertiesView, viewModel);
            }
            else
            {
                Model.ShowSuccessToastNotification();
                return Redirect(Model.UrlCurrentPageWithDomain);
            }
        }

        [HttpPost]
        [Route("delete-image", Name = ControllerActionRouteNames.Admin.ProductPropertiesController.DeleteImage)]
        public async Task<IActionResult> DeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }
        #endregion
    }
}