using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories;
using SixtyThreeBits.Core.Libraries.Extensions;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Controllers.Admin;
using SixtyThreeBits.Web.Domain.Libraries;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class DictionariesAdminModel : AdminModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            
            viewModel.Tree = new ViewModel.TreeModel();            
            viewModel.Tree.UrlLoad = UrlFactory.CreateUrl(controllerName: nameof(DictionariesAdminController), actionName: nameof(DictionariesAdminController.Tree));
            viewModel.Tree.UrlAddNew = UrlFactory.CreateUrl(controllerName: nameof(DictionariesAdminController), actionName: nameof(DictionariesAdminController.TreeAdd));
            viewModel.Tree.UrlUpdate = UrlFactory.CreateUrl(controllerName: nameof(DictionariesAdminController), actionName: nameof(DictionariesAdminController.TreeUpdate));
            viewModel.Tree.UrlDelete = UrlFactory.CreateUrl(controllerName: nameof(DictionariesAdminController), actionName: nameof(DictionariesAdminController.TreeDelete));

            viewModel.IsAddNewButtonVisible = User.HasPermission(viewModel.Tree.UrlAddNew);
            viewModel.Tree.IsAddNewButtonVisible = User.HasPermission(viewModel.Tree.UrlAddNew);
            viewModel.Tree.IsEditButtonVisible = User.HasPermission(viewModel.Tree.UrlUpdate);
            viewModel.Tree.IsDeleteButtonVisible = User.HasPermission(viewModel.Tree.UrlDelete);

            return viewModel;
        }

        public async Task<AjaxResponse> Tree()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoryFactory.CreateDictionariesRepository();

            var dictionaries = (await repository.DictionariesList());

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : dictionaries.Select(item => new ViewModel.TreeModel.TreeItem
            {
                DictionaryID = item.DictionaryID,
                DictionaryParentID = item.DictionaryParentID,
                DictionaryCaption = item.DictionaryCaption,
                DictionaryStringCode = item.DictionaryStringCode,
                DictionaryIntCode = item.DictionaryIntCode,
                DictionaryDecimalValue = item.DictionaryDecimalValue,
                DictionaryCode = item.DictionaryCode,
                DictionarySortIndex = item.DictionarySortIndex,
                DictionaryIsVisible = item.DictionaryIsVisible
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> TreeAdd(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = new AjaxResponse();
            var submitModelValues = submitModel.Values.DeserializeJsonTo<ViewModel.TreeModel.TreeItem>();

            var repository = RepositoryFactory.CreateDictionariesRepository();
            await repository.DictionariesIUD(
                databaseAction: DatabaseActions.INSERT,
                dictionaryID: null,
                dictionary: new DictionariesIudDTO
                {
                    DictionaryParentID = submitModelValues.DictionaryParentID,
                    DictionaryCaption = submitModelValues.DictionaryCaption,
                    DictionaryStringCode = submitModelValues.DictionaryStringCode ?? Constants.NullValueFor.String,
                    DictionaryIntCode = submitModelValues.DictionaryIntCode ?? Constants.NullValueFor.Numeric,
                    DictionaryDecimalValue = submitModelValues.DictionaryDecimalValue ?? Constants.NullValueFor.Numeric,
                    DictionaryCode = submitModelValues.DictionaryCode,
                    DictionarySortIndex = submitModelValues.DictionarySortIndex ?? Constants.NullValueFor.Numeric,
                    DictionaryIsVisible = submitModelValues.DictionaryIsVisible
                }
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }

        public async Task<AjaxResponse> TreeUpdate(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = new AjaxResponse();
            var dictionaryID = submitModel.Key.ToInt();
            var submitModelValues = submitModel.Values.DeserializeJsonTo<ViewModel.TreeModel.TreeItem>();

            var repository = RepositoryFactory.CreateDictionariesRepository();
            await repository.DictionariesIUD(
                databaseAction: DatabaseActions.UPDATE,
                dictionaryID: dictionaryID,
                dictionary: new DictionariesIudDTO
                {
                    DictionaryParentID = submitModelValues.DictionaryParentID,
                    DictionaryCaption = submitModelValues.DictionaryCaption,
                    DictionaryStringCode = submitModelValues.DictionaryStringCode ?? Constants.NullValueFor.String,
                    DictionaryIntCode = submitModelValues.DictionaryIntCode ?? Constants.NullValueFor.Numeric,
                    DictionaryDecimalValue = submitModelValues.DictionaryDecimalValue ?? Constants.NullValueFor.Numeric,
                    DictionaryCode = submitModelValues.DictionaryCode,
                    DictionarySortIndex = submitModelValues.DictionarySortIndex ?? Constants.NullValueFor.Numeric,
                    DictionaryIsVisible = submitModelValues.DictionaryIsVisible
                }
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }

        public async Task<AjaxResponse> TreeDelete(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = new AjaxResponse();
            var dictionaryID = submitModel.Key.ToInt();

            var repository = RepositoryFactory.CreateDictionariesRepository();
            await repository.DictionariesDeleteRecursive(dictionaryID);

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
            public TreeModel Tree { get; set; }

            public readonly string TextAdd = Resources.TextAdd;
            #endregion

            #region Nested Classes
            public class TreeModel : DevExtremeTreeModelBase63<TreeModel.TreeItem>
            {
                #region Methods
                public override TreeListBuilder<TreeItem> Render(IHtmlHelper html)
                {
                    var tree = CreateTreeWithStartupValues(html: html, keyFieldName: nameof(TreeItem.DictionaryID), parentFieldName: nameof(TreeItem.DictionaryParentID));

                    tree
                    .ID("DictionariesTree")
                    .OnInitialized("model.onTreeInit")
                    .OnRowUpdating("model.onTreeRowUpdating")
                    .AutoExpandAll(false)
                    .Pager(options =>
                    {
                        options.ShowInfo(false);
                    })
                    .Paging(options =>
                    {
                        options.Enabled(false);
                    })
                    .Columns(columns =>
                    {
                        columns.AddFor(m => m.DictionaryCaption).Caption(Resources.TextCaption).Width(300).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.AddFor(m => m.DictionaryStringCode).Caption(Resources.TextStringCode).Width(150);
                        columns.AddFor(m => m.DictionaryIntCode).Caption(Resources.TextIntCode).DataType(GridColumnDataType.Number).Width(150);
                        columns.AddFor(m => m.DictionaryCode).Caption(Resources.TextDictionaryCode).DataType(GridColumnDataType.Number).Width(150);
                        columns.AddFor(m => m.DictionarySortIndex).Caption(Resources.TextSortIndex).Width(150);
                        columns.AddFor(m => m.DictionaryIsVisible).Caption(Resources.TextPublished).Width(150);

                        columns.AddFor(m => m.DictionaryID).Caption("ID").EditCellTemplate($"<%= data.{nameof(TreeItem.DictionaryID)} %>").Width(100);

                        columns.Add();
                    });


                    return tree;
                }
                #endregion

                #region Nested Classes
                public record TreeItem
                {
                    #region Properties
                    public int? DictionaryID { get; init; }
                    public int? DictionaryParentID { get; init; }
                    public string DictionaryCaption { get; init; }
                    public string DictionaryStringCode { get; init; }
                    public int? DictionaryIntCode { get; init; }
                    public decimal? DictionaryDecimalValue { get; init; }
                    public int? DictionaryCode { get; init; }
                    public bool DictionaryIsVisible { get; init; }
                    public int? DictionarySortIndex { get; init; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}