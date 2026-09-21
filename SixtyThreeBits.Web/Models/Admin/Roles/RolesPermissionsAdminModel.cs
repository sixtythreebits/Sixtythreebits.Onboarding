using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Web.Controllers.Admin;
using SixtyThreeBits.Web.Domain.Libraries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class RolesPermissionsAdminModel : AdminModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();            
            viewModel.RolesGrid = new ViewModel.RolesGridModel();
            viewModel.RolesGrid.UrlLoad = UrlFactory.CreateUrl(controllerName: nameof(RolesPermissionsAdminController), actionName: nameof(RolesPermissionsAdminController.RolesGrid));

            viewModel.PermissionsTree = new ViewModel.PermissionsTreeModel();
            viewModel.PermissionsTree.UrlLoad = UrlFactory.CreateUrl(controllerName: nameof(RolesPermissionsAdminController), actionName: nameof(RolesPermissionsAdminController.PermissionsTree));

            viewModel.UrlPermissionsGetByRole = UrlFactory.CreateUrl(controllerName: nameof(RolesPermissionsAdminController), actionName: nameof(RolesPermissionsAdminController.PermissionsGetByRole));
            viewModel.UrlSave = UrlFactory.CreateUrl(controllerName: nameof(RolesPermissionsAdminController), actionName: nameof(RolesPermissionsAdminController.Save));
            viewModel.IsSaveButtonVisible = User.HasPermission(viewModel.UrlSave);

            return viewModel;
        }

        public async Task<AjaxResponse> GetRolePermissions(int? roleID)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoryFactory.CreatePermissionsRepository();
            var permissionsResult = await repository.PermissionsListByRoleID(roleID);

            viewModel.IsSuccess = !permissionsResult.IsError;
            viewModel.Data = permissionsResult.IsError ? permissionsResult.ErrorMessage : permissionsResult.Value.Select(item => item.PermissionID).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> RolesGrid()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoryFactory.CreateRolesRepository();

            var rolesResult = await repository.RolesList();

            viewModel.IsSuccess = !rolesResult.IsError;
            viewModel.Data = rolesResult.IsError ? rolesResult.ErrorMessage : rolesResult.Value.Select(Item => new ViewModel.RolesGridModel.GridItem
            {
                RoleID = Item.RoleID,
                RoleName = Item.RoleName
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> PermissionsTree()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoryFactory.CreatePermissionsRepository();

            var permissionsResult = await repository.PermissionsList();

            viewModel.IsSuccess = !permissionsResult.IsError;
            viewModel.Data = permissionsResult.IsError ? permissionsResult.ErrorMessage : permissionsResult.Value.Select(item => new ViewModel.PermissionsTreeModel.TreeItem
            {
                PermissionID = item.PermissionID,
                PermissionParentID = item.PermissionParentID,
                PermissionCaption = item.PermissionName
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> Save(SubmitModelRolePermissionSave submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoryFactory.CreateRolesRepository();

            var result = await repository.RolesPermissionsUpdate(
                roleID: submitModel.RoleID,
                permissionIDs: submitModel.PermissionIDs
            );
            viewModel.IsSuccess = !result.IsError;

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool IsSaveButtonVisible { get; set; }
            public RolesGridModel RolesGrid { get; set; }
            public PermissionsTreeModel PermissionsTree { get; set; }
            public string UrlPermissionsGetByRole { get; set; }
            public string UrlSave { get; set; }

            public readonly string TextPermissions = Resources.TextPermissions;
            public readonly string TextRoles = Resources.TextRoles;
            public readonly string TextSave = Resources.TextSave;
            #endregion

            #region Nested Classes
            public class RolesGridModel : DevExtremeGridModelBase63<RolesGridModel.GridItem>
            {
                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper Html)
                {
                    var Grid = CreateGridWithStartupValues(html: Html, keyFieldName: nameof(GridItem.RoleID));

                    Grid
                    .ID("RolesGrid")
                    .OnInitialized("model.onGridInit")
                    .OnFocusedRowChanged("model.onGridFocusedRowChanged")
                    .FilterRow(Options =>
                    {
                        Options.Visible(false);
                    })
                    .Paging(Options =>
                    {
                        Options.Enabled(false);
                    })
                    .Pager(Options =>
                    {
                        Options.ShowInfo(false);
                    })
                    .Columns(Columns =>
                    {
                        Columns.AddFor(m => m.RoleName).Caption(Resources.TextRole);
                    });


                    return Grid;
                }
                #endregion

                #region Nested Classes
                public record GridItem
                {
                    #region Properties
                    public int? RoleID { get; init; }
                    public string RoleName { get; init; }
                    #endregion
                }
                #endregion
            }

            public class PermissionsTreeModel : DevExtremeTreeModelBase63<PermissionsTreeModel.TreeItem>
            {
                #region Methods
                public override TreeListBuilder<TreeItem> Render(IHtmlHelper Html)
                {
                    var tree = CreateTreeWithStartupValues(html: Html, keyFieldName: nameof(TreeItem.PermissionID), parentFieldName: nameof(TreeItem.PermissionParentID));

                    tree
                    .ID("PermissionsTree")
                    .OnInitialized("model.onTreeInit")
                    .OnContentReady("model.onTreeContentReady")
                    .FilterRow(Options =>
                    {
                        Options.Visible(false);
                    })
                    .Paging(Options =>
                    {
                        Options.Enabled(false);
                    })
                    .Pager(Options =>
                    {
                        Options.ShowInfo(false);
                    })
                    .Selection(Options =>
                    {
                        Options.Mode(SelectionMode.Multiple);
                        Options.Recursive(false);
                    })
                    .Columns(Columns =>
                    {
                        Columns.AddFor(m => m.PermissionCaption).Caption(Resources.TextPermission);

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
                    public string PermissionCaption { get; init; }
                    #endregion
                }
                #endregion
            }            
            #endregion
        }

        public class SubmitModelRolePermissionSave
        {
            #region Properties
            public int? RoleID { get; set; }
            public List<int?> PermissionIDs { get; set; }
            #endregion
        }
        #endregion
    }
}