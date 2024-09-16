using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Filters.Admin
{
    public class BeforeProductPageLoad : IAsyncActionFilter
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
            if (_model.DBItem == null)
            {
                filterContext.Result = _model.GetNotFoundAdminViewResult();                
            }
            else
            {
                initPageTitle();
                reinitBreadcrumbs();
                await next();
            }
        }

        async Task initProduct()
        {
            var repository = _model.RepositoriesFactory.GetProductsRepository();
            var productID = _filterContext.RouteData.Values["productID"]?.ToString().ToInt();
            _model.DBItem = await repository.ProductsGetSingleByID(productID);
        }

        void initPageTitle()
        {
            _model.PageTitle.Set(_model.DBItem.ProductName);
        }
      
        void reinitBreadcrumbs()
        {
            _model.Breadcrumbs.DeleteLastItem();
            _model.Breadcrumbs.RenameLastItem(_model.DBItem.ProductName);
        }
        #endregion
    }
}