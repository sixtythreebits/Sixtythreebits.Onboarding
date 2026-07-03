using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories;
using SixtyThreeBits.Core.Libraries.Common;
using SixtyThreeBits.Core.Libraries.Extensions;
using SixtyThreeBits.Core.Libraries.Validation;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Controllers.Admin;
using SixtyThreeBits.Web.Domain.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class UsersAdminModel : AdminModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();

            viewModel.Grid = new ViewModel.GridModel();
            var repository = RepositoryFactory.CreateRolesRepository();
            viewModel.Grid.Roles = await repository.RolesListAsKeyValueTuple();
            viewModel.Grid.UrlLoad = UrlFactory.CreateUrl(controllerName: nameof(UsersAdminController), actionName: nameof(UsersAdminController.Grid));
            viewModel.Grid.UrlAddNew = UrlFactory.CreateUrl(controllerName: nameof(UsersAdminController), actionName: nameof(UsersAdminController.GridAdd));
            viewModel.Grid.UrlUpdate = UrlFactory.CreateUrl(controllerName: nameof(UsersAdminController), actionName: nameof(UsersAdminController.GridUpdate));
            viewModel.Grid.UrlDelete = UrlFactory.CreateUrl(controllerName: nameof(UsersAdminController), actionName: nameof(UsersAdminController.GridDelete));
            viewModel.Grid.IsEditButtonVisible = User.HasPermission(viewModel.Grid.UrlUpdate);
            viewModel.Grid.IsDeleteButtonVisible = User.HasPermission(viewModel.Grid.UrlDelete);
            viewModel.IsAddNewButtonVisible = User.HasPermission(viewModel.Grid.UrlAddNew);

            return viewModel;
        }

        public async Task<AjaxResponse> Grid()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoryFactory.CreateUsersRepository();

            var users = await repository.UsersList();

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : users.Select(item => new ViewModel.GridModel.GridItem
            {
                UserID = item.UserID,
                UserFirstname = item.UserFirstname,
                UserLastname = item.UserLastname,
                UserEmail = item.UserEmail,
                RoleID = item.RoleID,
                UserDateCreated = item.UserDateCreated
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> GridAdd(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = new AjaxResponse();
            var submitModelValues = submitModel.Values.DeserializeJsonTo<ViewModel.GridModel.GridItem>();

            var validationResult = await validate(
                userID: null,
                submitModelValues: submitModelValues
            );

            if (validationResult.HasErrors)
            {
                viewModel.Data = validationResult.ErrorMessage;
            }
            else
            {
                var repository = RepositoryFactory.CreateUsersRepository();
                await repository.UsersIUD(
                    databaseAction: DatabaseActions.INSERT,
                    userID: null,
                    user: new UserIudDTO
                    {
                        RoleID = submitModelValues.RoleID ?? Constants.NullValueFor.Numeric,
                        UserEmail = submitModelValues.UserEmail,
                        UserPassword = submitModelValues.UserPassword,
                        UserFirstname = submitModelValues.UserFirstname,
                        UserLastname = submitModelValues.UserLastname
                    }
                );
                viewModel.IsSuccess = !repository.IsError;
                viewModel.Data = repository.ErrorMessage;                
            }

            return viewModel;
        }

        public async Task<AjaxResponse> GridUpdate(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = new AjaxResponse();
            var userID = submitModel.Key.ToInt();
            var submitModelValues = submitModel.Values.DeserializeJsonTo<ViewModel.GridModel.GridItem>();

            var validationResult = await validate(
                userID: userID,
                submitModelValues: submitModelValues
            );

            if (validationResult.HasErrors)
            {
                viewModel.Data = validationResult.ErrorMessage;
            }
            else
            {
                var repository = RepositoryFactory.CreateUsersRepository();
                await repository.UsersIUD(
                    databaseAction: DatabaseActions.UPDATE,
                    userID: userID,
                    user: new UserIudDTO
                    {
                        RoleID = submitModelValues.RoleID ?? Constants.NullValueFor.Numeric,
                        UserEmail = submitModelValues.UserEmail,
                        UserPassword = submitModelValues.UserPassword,
                        UserFirstname = submitModelValues.UserFirstname,
                        UserLastname = submitModelValues.UserLastname
                    }
                );
                viewModel.IsSuccess = !repository.IsError;
                viewModel.Data = repository.ErrorMessage;
            }

            return viewModel;
        }

        public async Task<AjaxResponse> GridDelete(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = new AjaxResponse();
            var userID = submitModel.Key.ToInt();

            var repository = RepositoryFactory.CreateUsersRepository();
            var user = await repository.UsersGetSingleByID(userID);
            await FileStorage.DeleteFile(user.UserAvatarFilename);

            await repository.UsersIUD(
                databaseAction: DatabaseActions.DELETE,
                userID: userID,
                user: null
            );
            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }

        async Task<ValidationResult63> validate(int? userID, ViewModel.GridModel.GridItem submitModelValues)
        {
            var result = new ValidationResult63();
            var error = default(Error63);
           
            error = await Validation63.ValidateEmail(
                errorKey: null,
                userEmail: submitModelValues.UserEmail,
                validateRequired: true,
                validateUnique: true,
                validationPredicateReturnTrueWhenError: async () =>
                {
                    var repository = RepositoryFactory.CreateUsersRepository();
                    var isEmailUnique = await repository.UsersIsEmailUnique(submitModelValues.UserEmail, userID);
                    return !isEmailUnique;
                }
            );
            result.AddError(error);
            
            return result;
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
                    .OnRowUpdating("model.onGridRowUpdating")
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
                        });
                        columns.AddFor(m => m.UserPassword).Caption(Resources.TextPassword).Width(150);
                        columns.AddFor(m => m.RoleID).Caption(Resources.TextRole).Width(150).InitLookupColumn(data: Roles, allowNull: true);
                        columns.AddFor(m => m.UserDateCreated).Caption(Resources.TextDateCreated).Width(140).InitDateColumn(format: DevExtremeExtensions63.DateColumnFormat.DateTime).AllowEditing(false);
                        columns.Add();
                    });


                    return grid;
                }
                #endregion

                #region Nested Classes
                public record GridItem
                {
                    #region Properties
                    public int? UserID { get; init; }
                    public string UserFirstname { get; init; }
                    public string UserLastname { get; init; }
                    public string UserEmail { get; init; }
                    public string UserPassword { get; init; }
                    public int? RoleID { get; init; }
                    public DateTime? UserDateCreated { get; init; }                    
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }   
}