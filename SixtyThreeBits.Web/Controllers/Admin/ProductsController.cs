using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/products")]
    public class ProductsController : AdminControllerBase<ProductsModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.ProductsController.Products)]
        public async Task<IActionResult> Products()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = await Model.GetViewModel();
            return View(ViewNames.Admin.Products.ProductsView, viewModel);
        }

        [HttpGet]
        [Route("grid", Name = ControllerActionRouteNames.Admin.ProductsController.Grid)]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.GetGridItems();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.ProductsController.GridAdd)]
        public async Task<IActionResult> GridAdd(string values)
        {
            var result = default(IActionResult);

            var submitModel = values.DeserializeJsonTo<ProductsModel.ViewModel.GridViewModel.GridItem>() ?? new ProductsModel.ViewModel.GridViewModel.GridItem();

            var viewModel = await Model.IUD(
                databaseAction: Enums.DatabaseActions.CREATE,
                productID: null,
                submitModel: submitModel
            );

            if (viewModel.IsSuccess)
            {
                result = GetDevexpressSuccessResult();
            }
            else
            {
                result = GetDevexpressErrorResult(viewModel.Data.ToString());
            }

            return result;
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.ProductsController.GridUpdate)]
        public async Task<IActionResult> GridUpdate(int? key, string values)
        {
            var result = default(IActionResult);

            var submitModel = values.DeserializeJsonTo<ProductsModel.ViewModel.GridViewModel.GridItem>() ?? new ProductsModel.ViewModel.GridViewModel.GridItem();

            var viewModel = await Model.IUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                productID: key,
                submitModel: submitModel
            );

            if (viewModel.IsSuccess)
            {
                result = GetDevexpressSuccessResult();
            }
            else
            {
                result = GetDevexpressErrorResult(viewModel.Data.ToString());
            }

            return result;
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.ProductsController.GridDelete)]
        public async Task<IActionResult> GridDelete(int? key)
        {
            var result = default(IActionResult);

            var viewModel = await Model.IUD(
                databaseAction: Enums.DatabaseActions.DELETE,
                productID: key,
                submitModel: new ProductsModel.ViewModel.GridViewModel.GridItem()
            );

            if (viewModel.IsSuccess)
            {
                result = GetDevexpressSuccessResult();
            }
            else
            {
                result = GetDevexpressErrorResult(viewModel.Data.ToString());
            }

            return result;
        }
        #endregion
    }

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
        public async Task<IActionResult> Properties(ProductPropertiesModel.ViewModel viewModel)
        {
            var result = default(IActionResult);

            Model.PluginsClient.Enable63BitsForms(true).EnableJQueryNumericInput(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);

            viewModel = await Model.GetViewModel(viewModel);

            Model.Validate(viewModel);

            if (viewModel.IsValid)
            {
                await Model.Save(viewModel);
                if (viewModel.IsValid)
                {
                    Model.ShowSuccessToastNotification();
                    var redirectUrl = Model.Url.RouteUrl(ControllerActionRouteNames.Admin.ProductPropertiesController.Properties, new { productID = Model.Product.ProductID });
                    result = Redirect(redirectUrl);
                }
                else
                {
                    Model.ShowErrorToastNotification(viewModel.ErrorMessage);
                    result = View(ViewNames.Admin.Products.ProductPropertiesView, viewModel);
                }
            }
            else
            {
                result = View(ViewNames.Admin.Products.ProductPropertiesView, viewModel);
            }

            return result;
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