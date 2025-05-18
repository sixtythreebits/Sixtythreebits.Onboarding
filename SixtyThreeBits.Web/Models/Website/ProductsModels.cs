using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Website
{
    public class ProductsModel : ModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.PageTitle = "Products";

            var repository = RepositoriesFactory.CreateProductsRepository();
            viewModel.Products = (await repository.ProductsList())?
            .Select(item => new ViewModel.Product
            {
                ProductName = item.ProductName,
                ProductCoverImageHttpPath = FileStorage.GetUploadedFileHttpPath(item.ProductCoverImageFilename),
                ProductPrice = Utilities.FormatPrice(item.ProductPrice),
                UrlProductDetails = "#"
            }).ToList();

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public string PageTitle { get; set; }
            public List<Product> Products { get; set; }
            public bool HasProducts => Products?.Any() == true;
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

    public class ProductModel : ModelBase
    {

    }
}