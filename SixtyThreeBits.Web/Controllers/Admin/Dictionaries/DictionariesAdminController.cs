using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/dictionaries")]
    public class DictionariesAdminController : AdminControllerBase<DictionariesAdminModel>
    {
        #region Properties
        const string _viewName = "~/Views/Admin/Dictionaries/DictionariesAdminView.cshtml";
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = $"{nameof(DictionariesAdminController)}{nameof(Dictionaries)}")]
        public IActionResult Dictionaries()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(_viewName, viewModel);
        }

        [Route("tree", Name = $"{nameof(DictionariesAdminController)}{nameof(Tree)}")]
        public async Task<IActionResult> Tree()
        {
            var viewModel = await Model.Tree();
            return DevExtremeGridResult(viewModel);
        }

        [HttpPost]
        [Route("tree/add", Name = $"{nameof(DictionariesAdminController)}{nameof(TreeAdd)}")]
        public async Task<IActionResult> TreeAdd(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = await Model.TreeAdd(submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpPut]
        [Route("tree/update", Name = $"{nameof(DictionariesAdminController)}{nameof(TreeUpdate)}")]
        public async Task<IActionResult> TreeUpdate(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = await Model.TreeUpdate(submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpDelete]
        [Route("tree/delete", Name = $"{nameof(DictionariesAdminController)}{nameof(TreeDelete)}")]
        public async Task<IActionResult> TreeDelete(DevExtremeSubmitModelKeyValues63 submitModel)
        {
            var viewModel = await Model.TreeDelete(submitModel);
            return DevExtremeGridActionResult(viewModel);
        }
        #endregion
    }
}