using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Base;
using SixtyThreeBits.Web.Models.Website;

namespace SixtyThreeBits.Web.Controllers.Website
{    
    public class LogoutController : ControllerBase63<LogoutModel>
    {
        #region Actions
        [Route("logout", Name = $"{nameof(LogoutController)}{nameof(Logout)}")]
        public IActionResult Logout()
        {
            var redirectUrl = Model.LogoutAndGetRedirectUrl();
            return Redirect(redirectUrl);
        }
        #endregion
    }    
}