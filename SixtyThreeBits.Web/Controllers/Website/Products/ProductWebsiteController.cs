using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Libraries.Extensions;
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
                var model = WebUtilities.GetModelFromController<ProductWebsiteModel>(context.Controller);
                var productID = model.RouteData.Values[RouteKeys63.ProductID]?.ToString().ToInt();

                var repository = model.RepositoryFactory.CreateProductsRepository();
                model.Product = await repository.ProductsGetSingleByID(productID);

                if (model.Product == null)
                {
                    context.Result = model.GetNotFoundWebsiteViewResult();
                    return new ActionExecutedContext(context, context.Filters, context.Controller) { Result = context.Result, Canceled = true };
                }
                else
                {
                    if (!model.IsAjaxRequest)
                    {
                        var pageTitle = model.Product.ProductName;
                        model.PageTitle.Set(pageTitle);
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