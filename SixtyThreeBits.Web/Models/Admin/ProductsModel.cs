using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Shared;
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
            viewModel.Grid = new ViewModel.GridViewModel();

            var repository = RepositoriesFactory.GetProductsRepository();            
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductsController.GridDelete);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.ProductsController.GridDelete);

            viewModel.Grid.Categories = (await repository.CategoriesList())
                ?.Select(item => new KeyValueTuple<int?, string>
                {
                    Key = item.CategoryID,
                    Value = item.CategoryName
                }).ToList();

            return viewModel;
        }

        public async Task<List<ViewModel.GridViewModel.GridItem>> ListGridItems()
        {
            var repository = RepositoriesFactory.GetProductsRepository();
            var viewModel = (await repository.ProductsList())
            ?.Select(Item => new ViewModel.GridViewModel.GridItem
            {
                ProductID = Item.ProductID,
                ProductName = Item.ProductName,
                CategoryID = Item.CategoryID,
                ProductPrice = Item.ProductPrice,                
                ProductIsPublished = Item.ProductIsPublished,
                ProductDateCreated = Item.ProductDateCreated,
                UrlProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductPropertiesController.Properties, new { productID = Item.ProductID })
            }).ToList();
            return viewModel;
        }

        public async Task IUD(Enums.DatabaseActions databaseAction, int? productID, ViewModel.GridViewModel.GridItem submitModel)
        {
            var repository = RepositoriesFactory.GetProductsRepository();

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
                Form.AddError(repository.ErrorMessage);
            }
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridViewModel Grid { get; set; }
            #endregion

            #region Nested Classes
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
                        columns.AddFor(m => m.ProductName).Caption("Product").Width(300).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.AddFor(m => m.CategoryID).Caption("Category").Width(150).InitLookupColumn(data: Categories);
                        columns.AddFor(m => m.ProductPrice).Caption("Price").Width(100).InitNumberColumn(format: DevExtremeBuilderCustomExtensions.NumberColumnFormatType.Money);
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
                    public int? ProductID { get; init; }
                    public string ProductName { get; init; }
                    public int? CategoryID { get; init; }
                    public decimal? ProductPrice { get; init; }
                    public bool? ProductIsPublished { get; init; }
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

    }

    public class ProductPropertiesModel : ProductModelBase
    {

    }
}