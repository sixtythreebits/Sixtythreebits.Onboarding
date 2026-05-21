using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class ProductsAdminModel : AdminModelBase
    {
        #region Methods
        public async Task<AjaxResponse> Grid()
        {
            var viewModel = new AjaxResponse();

            var repository = RepositoryFactory.CreateProductsRepository();
            var products = await repository.ProductsList();

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = 
                repository.IsError ? repository.ErrorMessage :
                products.Select(item => new ViewModel.GridModel.GridItem
                {
                    ProductID = item.ProductID,
                    ProductName = item.ProductName,
                    CategoryID = item.CategoryID,
                    ProductPrice = item.ProductPrice,
                    ProductIsPublished = item.ProductIsPublished,
                    ProductDateCreated = item.ProductDateCreated,
                    UrlProperties = "#"
                }).ToList();            

            return viewModel;
        }

        public async Task<AjaxResponse> GridAdd(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = new AjaxResponse();
            var submitModelValues = submitModel.Values.DeserializeJsonTo<ViewModel.GridModel.GridItem>();

            var repository = RepositoryFactory.CreateProductsRepository();
            await repository.ProductsIUD(
                databaseAction: Enums.DatabaseActions.INSERT,
                productID: null,
                product: new ProductIudDTO
                {
                    ProductName = submitModelValues.ProductName,
                    CategoryID = submitModelValues.CategoryID,
                    ProductPrice = submitModelValues.ProductPrice,
                    ProductIsPublished = submitModelValues.ProductIsPublished,
                }
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }

        public async Task<AjaxResponse> GridUpdate(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = new AjaxResponse();
            var productID = submitModel.Key.ToInt();
            var submitModelValues = submitModel.Values.DeserializeJsonTo<ViewModel.GridModel.GridItem>();

            var repository = RepositoryFactory.CreateProductsRepository();
            await repository.ProductsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                productID: productID,
                product: new ProductIudDTO
                {
                    ProductName = submitModelValues.ProductName,
                    CategoryID = submitModelValues.CategoryID,
                    ProductPrice = submitModelValues.ProductPrice,
                    ProductIsPublished = submitModelValues.ProductIsPublished,
                }
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }

        public async Task<AjaxResponse> GridDelete(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = new AjaxResponse();
            var productID = submitModel.Key.ToInt();

            var repository = RepositoryFactory.CreateProductsRepository();
            await repository.ProductsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                productID: productID,
                product: null
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            
            public bool IsAddNewButtonVisible { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridModelBase63<GridModel.GridItem>
            {
                #region Properties
                readonly IReadOnlyList<KeyValueTuple<int?, string>> _categories;
                #endregion

                #region Constructors
                public GridModel(IReadOnlyList<KeyValueTuple<int?, string>> categories)
                {
                    _categories = categories;
                }
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
                        columns.AddFor(m => m.CategoryID).Caption("Category").Width(150).InitLookupColumn(data: _categories);
                        columns.AddFor(m => m.ProductPrice).Caption("Price").InitNumberColumn(format: DevExtremeExtensions63.NumberColumnFormat.Money);
                        columns.AddFor(m => m.ProductIsPublished).Caption("Published").Width(120).InitCheckboxColumn();
                        columns.AddFor(m => m.ProductDateCreated).Caption("Date Created").Width(140).InitDateColumn(format: DevExtremeExtensions63.DateColumnFormat.DateTime).AllowEditing(false);
                        columns.Add();
                    });

                    return grid;
                }
                #endregion

                #region Nested Classes
                public record GridItem
                {
                    #region Properties
                    public int? ProductID { get; init; }
                    public string ProductName { get; init; }
                    public int? CategoryID { get; init; }
                    public decimal? ProductPrice { get; init; }
                    public bool? ProductIsPublished { get; init; }
                    public DateTime? ProductDateCreated { get; init; }
                    public string UrlProperties { get; init; } 
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}