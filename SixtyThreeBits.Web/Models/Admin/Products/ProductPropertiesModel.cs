using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.Validation;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
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

        public async Task<ViewModel> Save(ViewModel submitModel)
        {
            var viewModel = await GetViewModel(submitModel);

            var validationResult = validateSubmitModel(submitModel);
            if (validationResult.HasErrors)
            {
                viewModel.AddFormErrors(validationResult.Errors);
            }
            else
            {
                var productCoverImageFilename = default(string);
                var hasProductCoverImage = submitModel.ProductCoverImage?.Length > 0;

                if (hasProductCoverImage)
                {
                    await FileStorage.DeleteFile(Product.ProductCoverImageFilename);
                    productCoverImageFilename = GetFilenameFromUploadedFile(submitModel.ProductCoverImage);
                }

                var repository = RepositoriesFactory.CreateProductsRepository();
                await repository.ProductsIUD(
                    databaseAction: Enums.DatabaseActions.UPDATE,
                    productID: Product.ProductID,
                    product: new ProductIudDTO
                    {
                        ProductName = submitModel.ProductName,
                        ProductPrice = submitModel.ProductPrice ?? Constants.NullValueFor.Numeric,
                        ProductCoverImageFilename = productCoverImageFilename,
                        CategoryID = submitModel.CategoryID ?? Constants.NullValueFor.Numeric,
                        ProductIsPublished = submitModel.ProductIsPublished
                    }
                );

                if (repository.IsError)
                {
                    viewModel.AddToastError(repository.ErrorMessage);
                }
                else
                {
                    if (hasProductCoverImage)
                    {
                        await SaveUploadedFile(submitModel.ProductCoverImage, productCoverImageFilename);
                    }
                }
            }

            return viewModel;
        }
        ValidationResult63 validateSubmitModel(ViewModel submitModel)
        {
            var result = new ValidationResult63();
            var error = default(Error63);

            error = Validation63.ValidateRequired(
                errorKey: Validation63.GetJQueryNameSelectorFor(nameof(submitModel.ProductName)),
                valueToValidate: submitModel.ProductName
            );
            result.AddError(error);

            error = Validation63.ValidateRequired(
                errorKey: Validation63.GetJQueryNameSelectorFor(nameof(submitModel.ProductPrice)),
                valueToValidate: submitModel.ProductPrice
            );
            result.AddError(error);

            error = Validation63.Validate(
                errorAction: () =>
                {
                    var isError = false;
                    var isPriceGreaterThanZero = submitModel.ProductPrice > 0;
                    if (!isPriceGreaterThanZero)
                    {
                        isError = true;
                    }
                    return isError;
                },
                errorKey: Validation63.GetJQueryNameSelectorFor(nameof(submitModel.ProductPrice)),
                errorMessage: "Price must be greater than 0"
            );
            result.AddError(error);

            error = Validation63.ValidateRequired(
                errorKey: Validation63.GetJQueryNameSelectorFor(nameof(submitModel.CategoryID)),
                valueToValidate: submitModel.CategoryID
            );
            result.AddError(error);

            return result;
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