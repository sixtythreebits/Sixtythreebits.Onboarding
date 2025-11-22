using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
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
            var viewModel = await Model.GetGridModel();
            return DevExtremeGridResult(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.ProductsController.GridAdd)]
        public async Task<IActionResult> GridAdd(string values)
        {
            var submitModel = values.DeserializeJsonTo<ProductsModel.ViewModel.GridModel.GridItem>() ?? new ProductsModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.ProductAdd(submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.ProductsController.GridUpdate)]
        public async Task<IActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<ProductsModel.ViewModel.GridModel.GridItem>() ?? new ProductsModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.ProductUpdate(
                productID: key,
                submitModel: submitModel
            );
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.ProductsController.GridDelete)]
        public async Task<IActionResult> GridDelete(int? key)
        {
            var viewModel = await Model.ProductDelete(productID: key);
            return DevExtremeGridActionResult(viewModel);
        }
        #endregion
    }    
}