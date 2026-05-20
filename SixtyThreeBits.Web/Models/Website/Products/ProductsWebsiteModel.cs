using SixtyThreeBits.Core.Libraries.Extensions;
using System.Collections.Generic;
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
            viewModel.Products = (await repository.ProductsList())?
            .Select(item => new ViewModel.Product
            {
                ProductName = item.ProductName,
                ProductCoverImageHttpPath = FileStorage.GetUploadedFileHttpPath(item.ProductCoverImageFilename),
                ProductPrice = Utilities.FormatPrice(price: item.ProductPrice, currencySign: "$"),
                UrlProductDetails = "#"
            }).ToList();

            PageTitle.Set("Products");

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public List<Product> Products { get; set; }
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