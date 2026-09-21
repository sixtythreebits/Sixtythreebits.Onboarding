using SixtyThreeBits.Web.Controllers.Website;

namespace SixtyThreeBits.Web.Domain.Libraries
{
    public partial class UrlFactory63
    {
        #region Nested Classes
        public class UrlFactory63Website
        {
            #region Properties
            UrlFactory63 _urlFactory63;
            #endregion

            #region Constructors
            public UrlFactory63Website(UrlFactory63 urlFactory63)
            {
                _urlFactory63 = urlFactory63;
            }
            #endregion

            #region Methods
            public string CreateUrlHome()
            {
                var url = _urlFactory63.CreateUrl(
                    controllerName: nameof(HomeWebsiteController),
                    actionName: nameof(HomeWebsiteController.Index)
                );
                return url;
            }

            public string CreateUrlLogout()
            {
                var url = _urlFactory63.CreateUrl(
                    controllerName: nameof(LogoutController),
                    actionName: nameof(LogoutController.Logout)
                );
                return url;
            }

            public string CreateUrlRelogin()
            {
                var url = _urlFactory63.CreateUrl(
                    controllerName: nameof(ReloginController),
                    actionName: nameof(ReloginController.Relogin)
                );
                return url;
            }           
            #endregion
        }
        #endregion
    }
}
