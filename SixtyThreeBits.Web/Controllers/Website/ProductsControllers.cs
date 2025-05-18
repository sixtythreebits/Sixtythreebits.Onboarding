using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class ProductsControllers : WebsiteControllerBase<ProductsModel>
    {
        #region Actions
        [Route("products", Name = ControllerActionRouteNames.Website.ProductsController.Products)]
        public async Task<IActionResult> Product()
        {
            var viewModel = await Model.GetViewModel();            
            return View(ViewNames.Website.Products.ProductsView, viewModel);
        }
        #endregion
    }

    public class ProductControllers : WebsiteControllerBase<ProductModel>
    {
        #region Actions
        [Route("products/{productID:int}", Name = ControllerActionRouteNames.Website.ProductsController.Product)]
        public async Task<IActionResult> Product(int? productID)
        {
            var viewModel = await Model.GetViewModel(productID);
            if (viewModel == null)
            {
                return Model.GetNotFoundWebsiteViewResult();
            }
            else
            {
                return View(ViewNames.Website.Products.ProductView, viewModel);
            }
        }
        #endregion
    }
}
