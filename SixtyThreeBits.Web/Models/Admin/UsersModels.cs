using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Properties;
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
    public class UsersModels : ModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();

            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.UsersController.GridAdd);
            viewModel.Grid = new ViewModel.GridModel();

            var repository = RepositoriesFactory.CreateRolesRepository();
            viewModel.Grid.Roles = await repository.RolesListAsKeyValueTuple();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.UsersController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.UsersController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.UsersController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.UsersController.GridDelete);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.UsersController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.UsersController.GridDelete);
            return viewModel;
        }

        public async Task<List<ViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var repository = RepositoriesFactory.CreateUsersRepository();
            var viewModel = (await repository.UsersList())
            ?.Select(Item => new ViewModel.GridModel.GridItem
            {
                UserID = Item.UserID,
                UserFirstname = Item.UserFirstname,
                UserLastname = Item.UserLastname,
                UserEmail = Item.UserEmail,
                RoleID = Item.RoleID,
                UserDateCreated = Item.UserDateCreated
            })
            .ToList();
            return viewModel;
        }

        public async Task ValidateUserEmail(string userEmail, int? userID)
        {
            var repository = RepositoriesFactory.CreateUsersRepository();
            var isUniq = await repository.UsersIsEmailUnique(userEmail, userID);
            if (!isUniq)
            {
                Form.AddError(Resources.ValidationUserEmailNotUniq);
            }
        }

        public async Task CRUD(Enums.DatabaseActions databaseAction, int? userID, ViewModel.GridModel.GridItem submitModel)
        {
            var repository = RepositoriesFactory.CreateUsersRepository();

            await repository.UsersIUD(
                databaseAction: databaseAction,
                userID: userID,
                user: new UserIudDTO
                {
                    RoleID = submitModel.RoleID,
                    UserEmail = submitModel.UserEmail,
                    UserPassword = submitModel.UserPassword,
                    UserFirstname = submitModel.UserFirstname,
                    UserLastname = submitModel.UserLastname
                }                
            );            

            if (repository.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridViewModelBase<GridModel.GridItem>
            {
                #region Properties
                public List<KeyValueTuple<int?, string>> Roles { get; set; }
                #endregion

                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.UserID));

                    grid
                    .ID("UsersGrid")
                    .OnInitialized("model.onGridInit")
                    .Columns(columns =>
                    {                        
                        columns.AddFor(m => m.UserFirstname).Caption(Resources.TextFirstname).Width(150).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.AddFor(m => m.UserLastname).Caption(Resources.TextLastname).Width(150);
                        columns.AddFor(m => m.UserEmail).Caption(Resources.TextEmail).Width(200).ValidationRules(options =>
                        {
                            options.AddRequired();
                            //Options.AddEmail();
                        });
                        columns.AddFor(m => m.UserPassword).Caption(Resources.TextPassword).Width(150);
                        columns.AddFor(m => m.RoleID).Caption(Resources.TextRole).Width(150).InitLookupColumn(data: Roles);
                        columns.AddFor(m => m.UserDateCreated).Caption(Resources.TextDateCreated).DataType(GridColumnDataType.DateTime).Width(140).InitDateColumn(true).AllowEditing(false);
                        columns.Add();
                    });


                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? UserID { get; set; }
                    public string UserFirstname { get; set; }
                    public string UserLastname { get; set; }
                    public string UserEmail { get; set; }
                    public string UserPassword { get; set; }
                    public int? RoleID { get; set; }
                    public DateTime? UserDateCreated { get; set; }                    
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
