using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Shared;
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
            var repository = RepositoriesFactory.GetProductsRepository();

            viewModel.PageTitle = "Products";
            viewModel.Products = (await repository.ProductsList())
                ?.Select(item => new ViewModel.Product
                {
                    ProductName = item.ProductName,
                    ProductCoverImageHttpPath = FileStorage.GetUploadedFileHttpPath(item.ProductCoverImageFilename),
                    ProductPrice = Utilities.FormatPrice(item.ProductPrice),
                    UrlProductDetails = Url.RouteUrl(ControllerActionRouteNames.Website.ProductsController.Product, new { productID = item.ProductID })
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
        #region Methods
        public async Task<ViewModel> GetViewModel(int? productID)
        {
            var viewModel = default(ViewModel);
            var repository = RepositoriesFactory.GetProductsRepository();

            var dbItem = await repository.ProductsGetSingleByID(productID);

            if (dbItem != null)
            {
                viewModel = new ViewModel();
                viewModel.PageTitle = dbItem.ProductName;
                viewModel.ProductName = dbItem.ProductName;
                viewModel.ProductCoverImageHttpPath = FileStorage.GetUploadedFileHttpPath(dbItem.ProductCoverImageFilename);
                viewModel.ProductPrice = Utilities.FormatPrice(dbItem.ProductPrice);
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
            public string ProductPrice { get; set; }
            public string ProductCoverImageHttpPath { get; set; }
            #endregion            
        }
        #endregion
    }
}