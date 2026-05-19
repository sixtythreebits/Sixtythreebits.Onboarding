using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Libraries;
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
            var permissionIDs = (await repository.PermissionsListByRoleID(roleID))?.Select(item => item.PermissionID).ToList();

            viewModel.IsSuccess = true;
            viewModel.Data = permissionIDs;

            return viewModel;
        }

        public async Task<AjaxResponse> GetRolesGridItems()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoryFactory.CreateRolesRepository();

            var roles = await repository.RolesList();

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : roles.Select(Item => new ViewModel.RolesGridModel.GridItem
            {
                RoleID = Item.RoleID,
                RoleName = Item.RoleName
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> GetPermissionsTreeItems()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoryFactory.CreatePermissionsRepository();

            var permissions = await repository.PermissionsList();

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : permissions.Select(item => new ViewModel.PermissionsTreeModel.TreeItem
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

            await repository.RolesPermissionsUpdate(
                roleID: submitModel.RoleID,
                permissionIDs: submitModel.PermissionIDs
            );
            viewModel.IsSuccess = !repository.IsError;

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