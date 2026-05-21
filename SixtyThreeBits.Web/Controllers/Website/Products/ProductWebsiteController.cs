using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class ProductWebsiteController : WebsiteControllerBase<ProductWebsiteModel>
    {
        #region Properties
        const string _viewName = "~/Views/Website/Products/ProductWebsiteView.cshtml";
        #endregion

        #region Actions
        [Route($"products/{{{RouteKeys63.ProductID}:int}}", Name = $"{nameof(ProductWebsiteController)}{nameof(Product)}")]
        public IActionResult Product()
        {

        }
        #endregion
    }
}