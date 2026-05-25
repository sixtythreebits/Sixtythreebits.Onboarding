using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route($"admin/products/{{{RouteKeys63.ProductID}:int}}/properties")]
    public class ProductPropertiesAdminController : AdminControllerBase<ProductPropertiesAdminModel>
    {
        #region Properties
        const string _viewName = "~/Views/Admin/Products/ProductPropertiesAdminView.cshtml";
        #endregion

        #region Filter
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await base.OnActionExecutionAsync(context, async () =>
            {
                var model = WebUtilities.GetModelFromController<ProductPropertiesAdminModel>(context.Controller);
                var productID = model.RouteData.Values[RouteKeys63.ProductID]?.ToString().ToInt();

                var repository = model.RepositoryFactory.CreateProductsRepository();
                model.Product = await repository.ProductsGetSingleByID(productID);

                if (model.Product == null)
                {
                    context.Result = model.GetNotFoundAdminViewResult();
                    return new ActionExecutedContext(context, context.Filters, context.Controller) { Result = context.Result, Canceled = true };
                }
                else
                {
                    if (!model.IsAjaxRequest)
                    {
                        Model.PluginsClient.Enable63BitsForms(true).EnableJQueryNumericInput(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);

                        var pageTitle = model.Product.ProductName;
                        model.PageTitle.Set(pageTitle);
                        model.Breadcrumbs.RenameLastItem(pageTitle);                        
                    }

                    return await next();
                }
            });
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = $"{nameof(ProductPropertiesAdminController)}{nameof(Properties)}")]
        public async Task<IActionResult> Properties()
        {            
            var viewModel = await Model.GetViewModel();
            return View(_viewName, viewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(ProductPropertiesAdminModel.ViewModel submitModel)
        {
            var viewModel = await Model.Save(submitModel);
            if (viewModel.HasErrors)
            {
                return View(_viewName, viewModel);
            }
            else
            {
                Model.ShowSuccessToastNotification();
                return Redirect(Model.UrlCurrentPageWithDomain);
            }
        }

        [HttpPost]
        [Route("delete-image", Name = $"{nameof(ProductPropertiesAdminController)}{nameof(DeleteImage)}")]
        public async Task<IActionResult> DeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }
        #endregion
    }
}