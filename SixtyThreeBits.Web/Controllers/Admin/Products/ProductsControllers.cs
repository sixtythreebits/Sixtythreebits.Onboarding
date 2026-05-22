using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/products")]
    public class ProductsAdminController : AdminControllerBase<ProductsAdminModel>
    {
        #region Properties
        const string _viewName = "~/Views/Admin/Products/ProductsAdminView.cshtml";
        #endregion

        #region Actions
        [Route("", Name = $"{nameof(ProductsAdminController)}{nameof(Products)}")]
        public async Task<IActionResult> Products()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = await Model.GetViewModel();
            return View(_viewName, viewModel);
        }

        [Route("grid", Name = $"{nameof(ProductsAdminController)}{nameof(Grid)}")]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.Grid();
            return DevExtremeGridResult(viewModel);
        }

        [Route("grid/add", Name = $"{nameof(ProductsAdminController)}{nameof(GridAdd)}")]
        public async Task<IActionResult> GridAdd(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = await Model.GridAdd(submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [Route("grid/update", Name = $"{nameof(ProductsAdminController)}{nameof(GridUpdate)}")]
        public async Task<IActionResult> GridUpdate(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = await Model.GridUpdate(submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [Route("grid/delete", Name = $"{nameof(ProductsAdminController)}{nameof(GridDelete)}")]
        public async Task<IActionResult> GridDelete(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = await Model.GridDelete(submitModel);
            return DevExtremeGridActionResult(viewModel);
        }
        #endregion
    }    
}