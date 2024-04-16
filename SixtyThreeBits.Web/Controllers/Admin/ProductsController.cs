using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
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
        #region Constructors
        public ProductsController()
        {
            Model = new ProductsModel();
        }
        #endregion

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
            var viewModel = await Model.ListGridItems();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.ProductsController.GridAdd)]
        public async Task<IActionResult> GridAdd(int? key, string values)
        {
            var result = default(IActionResult);
            var submitModel = values.DeserializeJsonTo<ProductsModel.ViewModel.GridViewModel.GridItem>() ?? new ProductsModel.ViewModel.GridViewModel.GridItem();
            
            await Model.IUD(
                databaseAction: Enums.DatabaseActions.CREATE, 
                productID: key, 
                submitModel: submitModel
            );
            if(Model.Form.HasErrors)
            {
                result = GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                result = GetDevexpressSuccessResult();
            }

            return result;
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.ProductsController.GridUpdate)]
        public async Task<IActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<ProductsModel.ViewModel.GridViewModel.GridItem>() ?? new ProductsModel.ViewModel.GridViewModel.GridItem();

            await Model.IUD(
                databaseAction: Enums.DatabaseActions.UPDATE, 
                productID: key, 
                submitModel: submitModel
            );

            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }            
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.ProductsController.GridDelete)]
        public async Task<IActionResult> GridDelete(int? key)
        {
            await Model.IUD(
                databaseAction: Enums.DatabaseActions.DELETE, 
                productID: key, 
                submitModel: new ProductsModel.ViewModel.GridViewModel.GridItem()
            );

            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }
        #endregion
    }    
}