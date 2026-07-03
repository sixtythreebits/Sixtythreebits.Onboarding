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
    public class RolesAdminModel : AdminModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();            
            viewModel.Grid = new ViewModel.GridModel();
            viewModel.Grid.UrlLoad = UrlFactory.CreateUrl(controllerName: nameof(RolesAdminController), actionName: nameof(RolesAdminController.Grid));
            viewModel.Grid.UrlAddNew = UrlFactory.CreateUrl(controllerName: nameof(RolesAdminController), actionName: nameof(RolesAdminController.GridAdd));
            viewModel.Grid.UrlUpdate = UrlFactory.CreateUrl(controllerName: nameof(RolesAdminController), actionName: nameof(RolesAdminController.GridUpdate));
            viewModel.Grid.UrlDelete = UrlFactory.CreateUrl(controllerName: nameof(RolesAdminController), actionName: nameof(RolesAdminController.GridDelete));
            
            viewModel.Grid.IsAddNewButtonVisible = User.HasPermission(viewModel.Grid.UrlAddNew);
            viewModel.Grid.IsEditButtonVisible = User.HasPermission(viewModel.Grid.UrlUpdate);
            viewModel.Grid.IsDeleteButtonVisible = User.HasPermission(viewModel.Grid.UrlDelete);
            viewModel.IsAddNewButtonVisible = User.HasPermission(viewModel.Grid.UrlAddNew);

            return viewModel;
        }

        public async Task<AjaxResponse> Grid()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoryFactory.CreateRolesRepository();

            var roles = await repository.RolesList();

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : roles.Select(Item => new ViewModel.GridModel.GridItem
            {
                RoleID = Item.RoleID,
                RoleName = Item.RoleName,
                RoleCode = Item.RoleCode
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> GridAdd(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = new AjaxResponse();
            var submitModelValues = submitModel.Values.DeserializeJsonTo<ViewModel.GridModel.GridItem>();

            var repository = RepositoryFactory.CreateRolesRepository();
            await repository.RolesIUD(
                databaseAction: DatabaseActions.INSERT,
                roleID: null,
                role: new RoleIudDTO
                {
                    RoleName = submitModelValues.RoleName,
                    RoleCode = submitModelValues.RoleCode
                }
            );
            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }

        public async Task<AjaxResponse> GridUpdate(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = new AjaxResponse();
            var roleID = submitModel.Key.ToInt();
            var submitModelValues = submitModel.Values.DeserializeJsonTo<ViewModel.GridModel.GridItem>();

            var repository = RepositoryFactory.CreateRolesRepository();
            await repository.RolesIUD(
                databaseAction: DatabaseActions.UPDATE,
                roleID: roleID,
                role: new RoleIudDTO
                {
                    RoleName = submitModelValues.RoleName,
                    RoleCode = submitModelValues.RoleCode
                }                
            );
            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }
   
        public async Task<AjaxResponse> GridDelete(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = new AjaxResponse();
            var roleID = submitModel.Key.ToInt();

            var repository = RepositoryFactory.CreateRolesRepository();
            await repository.RolesIUD(
                databaseAction: DatabaseActions.DELETE,
                roleID: roleID,
                role: null
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

            public readonly string TextAdd = Resources.TextAdd;
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridModelBase63<GridModel.GridItem>
            {
                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.RoleID));

                    grid
                    .ID("RolesGrid")
                    .OnInitialized("model.onGridInit")
                    .Columns(columns =>
                    {
                        columns.AddFor(m => m.RoleName).Caption(Resources.TextName).Width(300).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.Add();
                    });


                    return grid;
                }
                #endregion

                #region Nested Classes
                public record GridItem
                {
                    #region Properties
                    public int? RoleID { get; init; }
                    public string RoleName { get; init; }
                    public int? RoleCode { get; init; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}