using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Base;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin")]
    public class LoginAdminController : ControllerBase63<LoginAdminModel>
    {
        #region Properties
        const string _viewName = "~/Views/Admin/Auth/LoginAdminView.cshtml";
        #endregion

        #region Actions
        [HttpGet]
        [Route("login", Name = $"{nameof(LoginAdminController)}{nameof(Login)}")]
        public IActionResult Login()
        {
            if (Model.IsUserLoggedIn)
            {
                var UrlAdminDashboard = Model.UrlFactory.Admin.CreateUrlHomeDashboard();
                return Redirect(UrlAdminDashboard);
            }
            else
            {
                Model.PluginsClient.EnableAdminTheme(true).EnableFontAwesome(true).Enable63BitsFonts(true);
                var viewModel = Model.GetViewModel();
                return View(_viewName, viewModel);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginAdminModel.ViewModel submitModel)
        {
            Model.PluginsClient.EnableAdminTheme(true).EnableFontAwesome(true).Enable63BitsFonts(true);
            var viewModel = Model.GetViewModel(submitModel);
            var isAuthenticated = await Model.AuthenticateUser(viewModel: viewModel);
            if (isAuthenticated)
            {
                var UrlAdminDashboard = Model.UrlFactory.Admin.CreateUrlHomeDashboard();
                return Redirect(UrlAdminDashboard);
            }
            else
            {
                return View(_viewName, viewModel);
            }
        }
        #endregion
    }    
}