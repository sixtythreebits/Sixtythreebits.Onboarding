using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/users")]
    public class UsersAdminController : AdminControllerBase<UsersAdminModel>
    {
        #region Properties
        const string _viewName = "~/Views/Admin/Users/UsersAdminView.cshtml";
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = $"{nameof(UsersAdminController)}{nameof(Users)}")]
        public async Task<IActionResult> Users()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = await Model.GetViewModel();
            return View(_viewName, viewModel);
        }

        [Route("grid", Name = $"{nameof(UsersAdminController)}{nameof(Grid)}")]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.GetGridItems();
            return DevExtremeGridResult(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = $"{nameof(UsersAdminController)}{nameof(GridAdd)}")]
        public async Task<IActionResult> GridAdd(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = await Model.Add(submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpPut]
        [Route("grid/update", Name = $"{nameof(UsersAdminController)}{nameof(GridUpdate)}")]
        public async Task<IActionResult> GridUpdate(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = await Model.Update(submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpDelete]
        [Route("grid/delete", Name = $"{nameof(UsersAdminController)}{nameof(GridDelete)}")]
        public async Task<IActionResult> GridDelete(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = await Model.Delete(submitModel);
            return DevExtremeGridActionResult(viewModel);
        }
        #endregion
    }    
}