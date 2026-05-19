using Microsoft.AspNetCore.Routing;
using SixtyThreeBits.Web.Controllers.Admin;
using SixtyThreeBits.Web.Domain.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace SixtyThreeBits.Web.Domain.Libraries
{
    public partial class UrlFactory63
    {
        #region Nested Classes
        public class UrlFactory63Admin
        {
            #region Properties
            UrlFactory63 _urlFactory63;
            #endregion

            #region Constructors
            public UrlFactory63Admin(UrlFactory63 urlFactory63)
            {
                _urlFactory63 = urlFactory63;
            }
            #endregion
            
            #region Methods
            public string CreateUrlHomeDashboard()
            {
                var url = _urlFactory63.createUrl(
                    controllerName: nameof(HomeAdminController),
                    actionName: nameof(HomeAdminController.Dashboard),
                    values: null
                );
                return url;
            }

            public string CreateUrlLogin()
            {
                var url = _urlFactory63.createUrl(
                    controllerName: nameof(LoginAdminController),
                    actionName: nameof(LoginAdminController.Login),
                    values: null
                );
                return url;
            }
            #endregion
        }
        #endregion
    }
}
