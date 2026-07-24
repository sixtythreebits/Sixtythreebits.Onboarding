using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Models.Website;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class ProductsWebsiteController : WebsiteControllerBase<ProductsWebsiteModel>
    {
        #region Properties
        const string _viewName = "~/Views/Website/Products/ProductsWebsiteView.cshtml";
        #endregion

        #region Actions
        [Route("products", Name = $"{nameof(ProductsWebsiteController)}{nameof(Products)}")]
        public async Task<IActionResult> Products()
        {
            var viewModel = await Model.GetViewModel();
            return View(_viewName, viewModel);
        }
        #endregion
    }
}