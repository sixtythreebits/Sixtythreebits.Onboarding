using SixtyThreeBits.Core.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Website;
using SixtyThreeBits.Web.Domain.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Website
{
    public class ProductsWebsiteModel : WebsiteModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();

            var repository = RepositoryFactory.CreateProductsRepository();
            var productsResult = (await repository.ProductsList());
            viewModel.Products = productsResult.Value?
            .Select(item => new ViewModel.Product
            {
                ProductName = item.ProductName,
                ProductCoverImageHttpPath = FileStorage.GetUploadedFileHttpPath(item.ProductCoverImageFilename),
                ProductPrice = Utilities.FormatPrice(price: item.ProductPrice, currencySign: "$"),
                UrlProductDetails = UrlFactory.CreateUrl(
                    controllerName: nameof(ProductWebsiteController), 
                    actionName: nameof(ProductWebsiteController.Product),
                    routeValues: new Dictionary<string, object> { { RouteKeys63.ProductID, item.ProductID }  }
                )
            }).ToList().AsReadOnly();

            PageTitle.Set("Products");

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public ReadOnlyCollection<Product> Products { get; set; }
            public bool HasProducts => Products.HasElements();
            #endregion

            #region Nested Classes
            public class Product
            {
                #region Properties
                public string ProductName { get; set; }
                public string ProductCoverImageHttpPath { get; set; }
                public string ProductPrice { get; set; }
                public string UrlProductDetails { get; set; }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}