using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SixtyThreeBits.Web.Domain.Utilities
{
    public class WebUtilities
    {
        #region Methods
        public static string GetClientIP(HttpRequest request)
        {
            return request.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public static T GetModelFromController<T>(object controller) where T : class
        {
            dynamic C = controller;
            var Model = C.Model as T;
            return Model;
        }

        public static string GetWebsiteDomain(HttpRequest request)
        {
            var port = request.Host.Port;
            var hostString = request.Host.Host.TrimEnd(':');
            var portString = port == 80 || port == 443 || port == null ? "" : $":{port}";

            var websiteDomain = $"{request.Scheme}://{hostString}{portString}";
            return websiteDomain;
        }
        #endregion
    }
}