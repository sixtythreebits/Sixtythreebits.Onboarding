using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class ProductControllers : WebsiteControllerBase<ProductModel>
    {
        #region Actions
        [Route("products/{productID:int}", Name = ControllerActionRouteNames.Website.ProductController.Product)]
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
