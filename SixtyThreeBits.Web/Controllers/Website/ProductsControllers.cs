using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class ProductsController : WebsiteControllerBase<ProductsModel>
    {
        #region Constructors
        public ProductsController()
        {
            Model = new ProductsModel();
        }
        #endregion

        #region Actions
        [Route("products", Name = ControllerActionRouteNames.Website.ProductsController.Products)]
        public async Task<IActionResult> Products()
        {
            var viewModel = await Model.GetViewModel();
            Model.PageTitle.Set(viewModel.PageTitle);
            return View(ViewNames.Website.Products.ProductsView, viewModel);
        }         
        #endregion
    }

    public class ProductController : WebsiteControllerBase<ProductModel>
    {
        #region Constructors
        public ProductController()
        {
            Model = new ProductModel();
        }
        #endregion

        #region Actions
        [Route("products/{productID:int}", Name = ControllerActionRouteNames.Website.ProductsController.Product)]
        public async Task<IActionResult> Product(int? productID)
        {
            var viewModel = await Model.GetViewModel(productID);
            if (viewModel == null)
            {
                return Model.GetNotFoundAdminViewResult();
            }
            else
            {
                Model.PageTitle.Set(viewModel.PageTitle);
                return View(ViewNames.Website.Products.ProductView, viewModel);
            }
        }
        #endregion
    }
}