using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class ProductsModel : ModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.GridAdd);
            viewModel.Grid = new ViewModel.GridModel();

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

        public async Task<AjaxResponse> GetGridModel()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateProductsRepository();

            var data = (await repository.ProductsList())?
            .Select(item => new ViewModel.GridModel.GridItem
            {
                ProductID = item.ProductID,
                ProductName = item.ProductName,
                CategoryID = item.CategoryID,
                ProductPrice = item.ProductPrice,
                ProductIsPublished = item.ProductIsPublished,
                ProductDateCreated = item.ProductDateCreated,
                UrlProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductPropertiesController.Properties, new { productID = item.ProductID })
            }).ToList();

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : data;            

            return viewModel;
        }

        public async Task<AjaxResponse> ProductAdd(ViewModel.GridModel.GridItem submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateProductsRepository();

            await repository.ProductsIUD(
                databaseAction: Enums.DatabaseActions.INSERT,
                productID: null,
                product: new ProductIudDTO
                {
                    ProductName = submitModel.ProductName,
                    CategoryID = submitModel.CategoryID,
                    ProductPrice = submitModel.ProductPrice,
                    ProductIsPublished = submitModel.ProductIsPublished,
                }
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }

        public async Task<AjaxResponse> ProductUpdate(int? productID, ViewModel.GridModel.GridItem submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateProductsRepository();

            await repository.ProductsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                productID: productID,
                product: new ProductIudDTO
                {
                    ProductName = submitModel.ProductName,
                    CategoryID = submitModel.CategoryID,
                    ProductPrice = submitModel.ProductPrice,
                    ProductIsPublished = submitModel.ProductIsPublished,
                }
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;
                
            return viewModel;
        }

        public async Task<AjaxResponse> ProductDelete(int? productID)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateProductsRepository();

            var product = await repository.ProductsGetSingleByID(productID);
            if (product == null)
            {
                viewModel.Data = repository.ErrorMessage;
            }
            else
            {
                if(!string.IsNullOrWhiteSpace(product.ProductCoverImageFilename))
                {
                    await FileStorage.DeleteFile(product.ProductCoverImageFilename);
                }

                await repository.ProductsIUD(
                    databaseAction: Enums.DatabaseActions.DELETE,
                    productID: productID,
                    product: null
                );

                viewModel.IsSuccess = !repository.IsError;
                viewModel.Data = repository.ErrorMessage;
            }

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }

            public class GridModel : DevExtremeGridViewModelBase<GridModel.GridItem>
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
}