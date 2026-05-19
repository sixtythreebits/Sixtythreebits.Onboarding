using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/roles-permissions")]
    public class RolesPermissionsAdminController : AdminControllerBase<RolesPermissionsAdminModel>
    {
        #region Properties
        const string _viewName= "~/Views/Admin/Roles/RolesPermissionsAdminView.cshtml";
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = $"{nameof(RolesPermissionsAdminController)}{nameof(RolesPermissions)}")]
        public IActionResult RolesPermissions()
        {
            Model.PluginsClient.EnableDevextreme(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetViewModel();
            return View(_viewName, viewModel);
        }
        
        [HttpGet]
        [Route("roles/grid", Name = $"{nameof(RolesPermissionsAdminController)}{nameof(RolesGrid)}")]
        public async Task<IActionResult> RolesGrid()
        {
            var viewModel = await Model.GetRolesGridItems();
            return DevExtremeGridResult(viewModel);
        }

        [HttpGet]
        [Route("permissions/tree", Name = $"{nameof(RolesPermissionsAdminController)}{nameof(PermissionsTree)}")]
        public async Task<IActionResult> PermissionsTree()
        {
            var viewModel = await Model.GetPermissionsTreeItems();
            return DevExtremeGridResult(viewModel);
        }

        [HttpGet]
        [Route("permissions/get-by-role", Name = $"{nameof(RolesPermissionsAdminController)}{nameof(PermissionsGetByRole)}")]
        public async Task<IActionResult> PermissionsGetByRole(int? RoleID)
        {
            var viewModel = await Model.GetRolePermissions(RoleID);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("save", Name = $"{nameof(RolesPermissionsAdminController)}{nameof(Save)}")]
        public async Task<IActionResult> Save(RolesPermissionsAdminModel.SubmitModelRolePermissionSave submitModel)
        {
            var viewModel = await Model.Save(submitModel);
            return Json(viewModel);
        }
        #endregion
    }
}