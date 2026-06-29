using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/roles")]
    public class RolesAdminController : AdminControllerBase<RolesAdminModel>
    {
        #region Properties
        const string _viewName = "~/Views/Admin/Roles/RolesAdminView.cshtml";
        #endregion

        #region Methods
        [HttpGet]
        [Route("", Name = $"{nameof(RolesAdminController)}{nameof(Roles)}")]
        public IActionResult Roles()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(_viewName, viewModel);
        }

        [Route("grid", Name = $"{nameof(RolesAdminController)}{nameof(Grid)}")]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.Grid();
            return DevExtremeGridResult(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = $"{nameof(RolesAdminController)}{nameof(GridAdd)}")]
        public async Task<IActionResult> GridAdd(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = await Model.GridAdd(submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpPut]
        [Route("grid/update", Name = $"{nameof(RolesAdminController)}{nameof(GridUpdate)}")]
        public async Task<IActionResult> GridUpdate(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = await Model.GridUpdate(submitModel);
            return DevExtremeGridActionResult(viewModel);            
        }

        [HttpDelete]
        [Route("grid/delete", Name = $"{nameof(RolesAdminController)}{nameof(GridDelete)}")]
        public async Task<IActionResult> GridDelete(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = await Model.GridDelete(submitModel);
            return DevExtremeGridActionResult(viewModel);
        }
        #endregion
    }    
}