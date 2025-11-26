using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Filters.Admin
{
    public class ProductFilterAttribute : IAsyncActionFilter
    {
        #region Properties
        ActionExecutingContext _filterContext;
        ProductModelBase _model;
        #endregion

        #region Methods
        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _filterContext = filterContext;
            _model = WebUtilities.GetModelFromController<ProductModelBase>(_filterContext.Controller);

            await initProduct();

            if(_model.Product == null)
            {
                _filterContext.Result = _model.GetNotFoundAdminViewResult();
            }
            else
            {
                initPageTitle();
                reinitBreadCrumbs();
                await next();
            }            
        }

        async Task initProduct()
        {
            var repository = _model.RepositoriesFactory.CreateProductsRepository();
            var productID = _filterContext.RouteData.Values["productID"].ToString().ToInt();
            _model.Product = await repository.ProductsGetSingleByID(productID);
        }

        void initPageTitle()
        {
            _model.PageTitle.Set(_model.Product.ProductName);
        }

        void reinitBreadCrumbs()
        {
            _model.Breadcrumbs.DeleteLastItem();
            _model.Breadcrumbs.RenameLastItem(_model.Product.ProductName);
        }
        #endregion
    }
}
