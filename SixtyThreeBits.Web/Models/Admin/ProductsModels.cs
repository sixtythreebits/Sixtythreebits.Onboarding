using DevExtreme.AspNet.Mvc.Builders;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Models.Base;
using System;
using System.Collections.Generic;
using SixtyThreeBits.Web.Domain.Utilities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Utilities;
using Microsoft.AspNetCore.Http;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Web.Domain.ViewModels.Base;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class ProductsModel : ModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.GridAdd);
            viewModel.Grid = new ViewModel.GridViewModel();

            var repository = RepositoriesFactory.CreateProductsRepository();

            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.GridDelete);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.GridDelete);

            viewModel.Grid.Categories = (await repository.CategoriesList())?
            .Select(item => new KeyValueTuple<int?, string>
            {
                Key = item.CategoryID,
                Value = item.CategoryName
            }).ToList();

            return viewModel;
        }

        public async Task<List<ViewModel.GridViewModel.GridItem>> GetGridItems()
        {
            var repository = RepositoriesFactory.CreateProductsRepository();

            var viewModel = (await repository.ProductsList())?
            .Select(item => new ViewModel.GridViewModel.GridItem
            {
                ProductID = item.ProductID,
                ProductName = item.ProductName,
                CategoryID = item.CategoryID,
                ProductPrice = item.ProductPrice,
                ProductIsPublished = item.ProductIsPublished,
                ProductDateCreated = item.ProductDateCreated,
                UrlProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductPropertiesController.Properties, new { productID = item.ProductID })
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> IUD(Enums.DatabaseActions databaseAction, int? productID, ViewModel.GridViewModel.GridItem submitModel)
        {
            var viewModel = new AjaxResponse();

            var repository = RepositoriesFactory.CreateProductsRepository();

            await repository.ProductsIUD(
                databaseAction: databaseAction,
                productID: productID,
                product: new ProductIudDTO
                {
                    ProductName = submitModel.ProductName,
                    CategoryID = submitModel.CategoryID,
                    ProductPrice = submitModel.ProductPrice,
                    ProductIsPublished = submitModel.ProductIsPublished,
                }
            );

            if (repository.IsError)
            {
                viewModel.Data = repository.ErrorMessage;
            }
            else
            {
                viewModel.IsSuccess = true;
            }

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridViewModel Grid { get; set; }

            public class GridViewModel : DevExtremeGridViewModelBase<GridViewModel.GridItem>
            {
                #region Properties
                public List<KeyValueTuple<int?, string>> Categories { get; set; }
                #endregion

                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.ProductID));

                    grid
                    .ID("ProductsGrid")
                    .OnInitialized("model.onGridInit")
                    .Columns(columns =>
                    {
                        columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlProperties));
                        columns.AddFor(m => m.ProductName).Caption("Product").Width(300)
                        .ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.AddFor(m => m.CategoryID).Caption("Category").Width(150).InitLookupColumn(data: Categories);
                        columns.AddFor(m => m.ProductPrice).Caption("Price").InitNumberColumn(format: DevExtremeBuilderCustomExtensions.NumberColumnFormatType.Money);
                        columns.AddFor(m => m.ProductIsPublished).Caption("Published").Width(120).InitCheckboxColumn();
                        columns.AddFor(m => m.ProductDateCreated).Caption("Date Created").Width(140).InitDateColumn(true).AllowEditing(false);
                        columns.Add();
                    });

                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? ProductID { get; set; }
                    public string ProductName { get; set; }
                    public int? CategoryID { get; set; }
                    public decimal? ProductPrice { get; set; }
                    public bool? ProductIsPublished { get; set; }
                    public DateTime? ProductDateCreated { get; set; }
                    public string UrlProperties { get; set; }
                    #endregion
                }
                #endregion
            } 
            #endregion
        }
        #endregion
    }

    public class ProductModelBase : ModelBase
    {
        #region Properties
        public ProductDTO Product { get; set; }
        #endregion
    }

    public class ProductPropertiesModel : ProductModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel(ViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
                viewModel.ProductName = Product.ProductName;
                viewModel.ProductPrice = Product.ProductPrice;
                viewModel.CategoryID = Product.CategoryID;
                viewModel.ProductIsPublished = Product.ProductIsPublished;
            }

            viewModel.ProductPriceString = Utilities.FormatPrice(viewModel.ProductPrice);
            viewModel.ProductCoverImageFileName = Product.ProductCoverImageFilename;
            viewModel.ProductCoverImageHttpPath = FileStorage.GetUploadedFileHttpPath(Product.ProductCoverImageFilename);
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductPropertiesController.DeleteImage, new { ProductID = Product.ProductID });

            var reopsitory = RepositoriesFactory.CreateProductsRepository();
            viewModel.Categories = (await reopsitory.CategoriesList())?
            .Select(item => new KeyValueSelectedTuple<int?, string>
            {
                Key = item.CategoryID,
                Value = item.CategoryName,
                IsSelected = item.CategoryID == Product.CategoryID,
            }).ToList();

            return viewModel;
        }

        public void Validate(ViewModel viewModel)
        {
            var error = default(ErrorItem);

            error = Validation.ValidateRequired(
                errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.ProductName)),
                valueToValidate: viewModel.ProductName
            );
            viewModel.AddError(error);

            error = Validation.ValidateRequired(
                errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.ProductPrice)),
                valueToValidate: viewModel.ProductPrice
            );
            viewModel.AddError(error);

            error = Validation.Validate(
                errorAction: () =>
                {
                    var isError = false;
                    if (!(viewModel.ProductPrice > 0))
                    {
                        isError = true;
                    }
                    return isError;
                },
                errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.ProductPrice)),
                errorMessage: "Price must be greater than 0"
            );
            viewModel.AddError(error);

            error = Validation.ValidateRequired(
                errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.CategoryID)),
                valueToValidate: viewModel.CategoryID
            );
            viewModel.AddError(error);
        }

        public async Task Save(ViewModel viewModel)
        {
            var productCoverImageFilename = default(string);
            var hasProductCoverImage = viewModel.ProductCoverImage?.Length > 0;

            if (hasProductCoverImage)
            {
                await FileStorage.DeleteFile(Product.ProductCoverImageFilename);

                productCoverImageFilename = GetFilenameFromUploadedFile(viewModel.ProductCoverImage);
            }

            var repository = RepositoriesFactory.CreateProductsRepository();

            await repository.ProductsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                productID: Product.ProductID,
                product: new ProductIudDTO
                {
                    ProductName = viewModel.ProductName,
                    ProductPrice = viewModel.ProductPrice ?? Constants.NullValueFor.Numeric,
                    ProductCoverImageFilename = productCoverImageFilename,
                    CategoryID = viewModel.CategoryID ?? Constants.NullValueFor.Numeric,
                    ProductIsPublished = viewModel.ProductIsPublished
                }
            );

            if (repository.IsError)
            {
                viewModel.AddError(repository.ErrorMessage);
            }
            else
            {
                if (hasProductCoverImage)
                {
                    await SaveUploadedFile(viewModel.ProductCoverImage, productCoverImageFilename);
                }
            }
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();

            await FileStorage.DeleteFile(Product.ProductCoverImageFilename);

            var repository = RepositoriesFactory.CreateProductsRepository();

            await repository.ProductsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                productID: Product.ProductID,
                product: new ProductIudDTO
                {
                    ProductCoverImageFilename = Constants.NullValueFor.String
                }
            );

            if (repository.IsError)
            {
                viewModel.Data = repository.ErrorMessage;
            }
            else
            {
                viewModel.IsSuccess = true;
            }

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel : FormViewModelBase
        {
            #region Properties
            public string ProductName { get; set; }
            public decimal? ProductPrice { get; set; }
            public string ProductPriceString { get; set; }
            public string ProductCoverImageFileName { get; set; }
            public string ProductCoverImageHttpPath { get; set; }
            public bool HasProductCoverImageFileName => !string.IsNullOrWhiteSpace(ProductCoverImageFileName);
            public IFormFile ProductCoverImage { get; set; }
            public bool ProductIsPublished { get; set; }
            public int? CategoryID { get; set; }
            public List<KeyValueSelectedTuple<int?, string>> Categories { get; set; }
            public bool HasCategories => Categories?.Any() == true;
            public string UrlDeleteImage { get; set; }
            #endregion
        }
        #endregion
    }
}