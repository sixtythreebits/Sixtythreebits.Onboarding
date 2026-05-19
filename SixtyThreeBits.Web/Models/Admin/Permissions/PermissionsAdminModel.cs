using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin;
using SixtyThreeBits.Web.Domain.Libraries;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class PermissionsAdminModel : AdminModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            
            viewModel.Tree = new ViewModel.TreeModel();            
            viewModel.Tree.UrlLoad = UrlFactory.CreateUrl(controllerName: nameof(PermissionsAdminController), actionName: nameof(PermissionsAdminController.Tree));
            viewModel.Tree.UrlAddNew = UrlFactory.CreateUrl(controllerName: nameof(PermissionsAdminController), actionName: nameof(PermissionsAdminController.TreeAdd));
            viewModel.Tree.UrlUpdate = viewModel.UrlUpdate = UrlFactory.CreateUrl(controllerName: nameof(PermissionsAdminController), actionName: nameof(PermissionsAdminController.TreeUpdate));
            viewModel.Tree.UrlDelete = UrlFactory.CreateUrl(controllerName: nameof(PermissionsAdminController), actionName: nameof(PermissionsAdminController.TreeDelete));

            viewModel.IsAddNewButtonVisible = User.HasPermission(viewModel.Tree.UrlAddNew);
            viewModel.Tree.IsAddNewButtonVisible = User.HasPermission(viewModel.Tree.UrlAddNew);
            viewModel.Tree.IsEditButtonVisible = User.HasPermission(viewModel.Tree.UrlUpdate);
            viewModel.Tree.IsDeleteButtonVisible = User.HasPermission(viewModel.Tree.UrlDelete);

            return viewModel;
        }

        public async Task<AjaxResponse> GetTreeItems()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoryFactory.CreatePermissionsRepository();

            var permissions = await repository.PermissionsList();

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : permissions.Select(item => new ViewModel.TreeModel.TreeItem
            {
                PermissionID = item.PermissionID,
                PermissionParentID = item.PermissionParentID,
                PermissionName = item.PermissionName,
                PermissionPagePath = item.PermissionPagePath,                
                PermissionIsMenuItem = item.PermissionIsMenuItem,
                PermissionMenuIcon = item.PermissionMenuIcon,
                PermissionMenuTitle = item.PermissionMenuTitle,
                PermissionSortIndex = item.PermissionSortIndex,
                PermissionGuid = item.PermissionGuid
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> Add(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = new AjaxResponse();
            var submitModelValues = submitModel.Values.DeserializeJsonTo<ViewModel.TreeModel.TreeItem>();

            var repository = RepositoryFactory.CreatePermissionsRepository();
            await repository.PermissionsIUD(
                databaseAction: Enums.DatabaseActions.INSERT,
                permissionID: null,
                permission: new PermissionIudDTO
                {
                    PermissionParentID = submitModelValues.PermissionParentID,
                    PermissionName = submitModelValues.PermissionName,
                    PermissionPagePath = submitModelValues.PermissionPagePath,                    
                    PermissionIsMenuItem = submitModelValues.PermissionIsMenuItem,
                    PermissionMenuIcon = submitModelValues.PermissionMenuIcon,
                    PermissionMenuTitle = submitModelValues.PermissionMenuTitle,
                    PermissionSortIndex = submitModelValues.PermissionSortIndex,
                    PermissionGuid = submitModelValues.PermissionGuid
                }
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }

        public async Task<AjaxResponse> Update(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = new AjaxResponse();
            var permissionID = submitModel.Key.ToInt();
            var submitModelValues = submitModel.Values.DeserializeJsonTo<ViewModel.TreeModel.TreeItem>();

            var repository = RepositoryFactory.CreatePermissionsRepository();
            await repository.PermissionsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                permissionID: permissionID,
                permission: new PermissionIudDTO
                {
                    PermissionParentID = submitModelValues.PermissionParentID,
                    PermissionName = submitModelValues.PermissionName,
                    PermissionPagePath = submitModelValues.PermissionPagePath,                    
                    PermissionIsMenuItem = submitModelValues.PermissionIsMenuItem,
                    PermissionMenuIcon = submitModelValues.PermissionMenuIcon,
                    PermissionMenuTitle = submitModelValues.PermissionMenuTitle,
                    PermissionSortIndex = submitModelValues.PermissionSortIndex,
                    PermissionGuid = submitModelValues.PermissionGuid
                }   
            );

            viewModel.IsSuccess=!repository.IsError;
            viewModel.Data = repository.ErrorMessage;                 

            return viewModel;
        }

        public async Task<AjaxResponse> Delete(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = new AjaxResponse();
            var permissionID = submitModel.Key.ToInt();
            var repository = RepositoryFactory.CreatePermissionsRepository();
            await repository.PermissionsDeleteRecursive(permissionID);
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
            public string UrlUpdate { get; set; }

            public readonly string TextAdd = Resources.TextAdd;
            #endregion

            #region Nested Classes
            public class TreeModel : DevExtremeTreeModelBase63<TreeModel.TreeItem>
            {
                #region Methods
                public override TreeListBuilder<TreeItem> Render(IHtmlHelper Html)
                {
                    var tree = CreateTreeWithStartupValues(html: Html, keyFieldName: nameof(TreeItem.PermissionID), parentFieldName: nameof(TreeItem.PermissionParentID));

                    tree
                    .ID("PermissionsTree")
                    .OnInitialized("model.onTreeInit")
                    .OnInitNewRow("model.onTreeInitNewRow")
                    .RowDragging(Options =>
                    {
                        if (IsEditButtonVisible)
                        {
                            Options.AllowDropInsideItem(true);
                            Options.AllowReordering(false);
                            Options.ShowDragIcons(true);
                            Options.OnReorder("model.onTreeReorder");
                        }
                    })
                    .AutoExpandAll(false)
                    .Pager(Options =>
                    {
                        Options.ShowInfo(false);
                    })
                    .Paging(Options =>
                    {
                        Options.Enabled(false);
                    })
                    .Columns(Columns =>
                    {
                        Columns.AddFor(m => m.PermissionName).Caption(Resources.TextCaption).Width(400).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                        });                        
                        Columns.AddFor(m => m.PermissionPagePath).Caption(Resources.TextPagePath).Width(300);
                        Columns.AddFor(m => m.PermissionSortIndex).Caption(Resources.TextSortIndex).DataType(GridColumnDataType.Number).Width(100);
                        Columns.AddFor(m => m.PermissionIsMenuItem).Caption(Resources.TextMenu).DataType(GridColumnDataType.Boolean).Width(80);
                        Columns.AddFor(m => m.PermissionMenuTitle).Caption(Resources.TextMenuTitle).Width(200);
                        Columns.AddFor(m => m.PermissionMenuIcon).Caption(Resources.TextMenuIcon).Width(100);
                        Columns.AddFor(m => m.PermissionGuid).Caption(Resources.TextCode).Width(250);
                        Columns.Add();

                    });

                    return tree;
                }
                #endregion

                #region Nested Classes
                public record TreeItem
                {
                    #region Properties
                    public int? PermissionID { get; init; }
                    public int? PermissionParentID { get; init; }
                    public string PermissionName { get; init; }
                    public string PermissionPagePath { get; init; }                    
                    public bool? PermissionIsMenuItem { get; init; }
                    public string PermissionMenuIcon { get; init; }
                    public string PermissionMenuTitle { get; init; }
                    public int? PermissionSortIndex { get; init; }
                    public string PermissionGuid { get; init; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
