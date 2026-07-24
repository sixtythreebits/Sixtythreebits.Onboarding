using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class ProductWebsiteController : WebsiteControllerBase<ProductWebsiteModel>
    {
        #region Properties
        const string _viewName = "~/Views/Website/Products/ProductWebsiteView.cshtml";
        #endregion

        #region Filter
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await base.OnActionExecutionAsync(context, async () =>
            {
                var productID = Model.RouteData.Values[RouteKeys63.ProductID]?.ToString().ToInt();

                var repository = Model.RepositoryFactory.CreateProductsRepository();
                Model.Product = await repository.ProductsGetSingleByID(productID);

                if (Model.Product == null)
                {
                    context.Result = Model.GetNotFoundWebsiteViewResult();
                    return new ActionExecutedContext(context, context.Filters, context.Controller) { Result = context.Result, Canceled = true };
                }
                else
                {
                    if (!Model.IsAjaxRequest)
                    {
                        var pageTitle = Model.Product.ProductName;
                        Model.PageTitle.Set(pageTitle);
                    }

                    return await next();
                }
            });
        }
        #endregion

        #region Actions
        [Route($"products/{{{RouteKeys63.ProductID}:int}}", Name = $"{nameof(ProductWebsiteController)}{nameof(Product)}")]
        public IActionResult Product()
        {
            var viewModel = Model.GetViewModel();
            return View(_viewName, viewModel);
        }
        #endregion
    }
}