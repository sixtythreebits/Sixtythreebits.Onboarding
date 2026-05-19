using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Models.Admin;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin")]
    public class HomeAdminController : AdminControllerBase<HomeAdminModel>
    {
        #region Properties
        const string _viewName = "~/Views/Admin/Home/DashboardAdminView.cshtml";
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = $"{nameof(HomeAdminController)}{nameof(Dashboard)}")]
        public IActionResult Dashboard()
        {
            var viewModel = Model.GetViewModel();
            return View(_viewName, viewModel);
        } 
        #endregion
    }
}