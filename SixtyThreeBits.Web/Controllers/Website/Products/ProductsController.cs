using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class ProductsController : WebsiteControllerBase<ProductsModel>
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
}
