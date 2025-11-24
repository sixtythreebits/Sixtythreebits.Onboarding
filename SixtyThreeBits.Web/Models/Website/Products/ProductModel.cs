using SixtyThreeBits.Web.Models.Base;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Website
{ 
    public class ProductModel : ModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel(int? productID)
        {
            var viewModel = default(ViewModel);

            var repository = RepositoriesFactory.CreateProductsRepository();
            var product = await repository.ProductsGetSingleByID(productID);
            if(product != null)
            {
                viewModel = new ViewModel();
                viewModel.PageTitle = product.ProductName;
                viewModel.ProductName = product.ProductName;
                viewModel.ProductCoverImageHttpPath = FileStorage.GetUploadedFileHttpPath(product.ProductCoverImageFilename);
                viewModel.ProductPrice = Utilities.FormatPrice(price: product.ProductPrice, currencySign: "$");

                PageTitle.Set(viewModel.PageTitle);
            }

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public string PageTitle { get; set; }
            public string ProductName { get; set; }
            public string ProductCoverImageHttpPath { get; set; }
            public string ProductPrice { get; set; }
            #endregion            
        }
        #endregion
    }
}