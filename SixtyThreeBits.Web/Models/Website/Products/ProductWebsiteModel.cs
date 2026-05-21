using SixtyThreeBits.Core.Infrastructure.Repositories;

namespace SixtyThreeBits.Web.Models.Website
{
    public class ProductWebsiteModel : WebsiteModelBase
    {
        #region Properties
        public ProductDTO Product { get; set; }
        #endregion

        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.PageTitle = Product.ProductName;
            viewModel.ProductName = Product.ProductName;
            viewModel.ProductCoverImageHttpPath = FileStorage.GetUploadedFileHttpPath(Product.ProductCoverImageFilename);
            viewModel.ProductPrice = Utilities.FormatPrice(price: Product.ProductPrice, currencySign: "$");

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