using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SixtyThreeBits.Core.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SixtyThreeBits.Web.Domain.Libraries
{
    public partial class UrlFactory63
    {
        #region Properties     
        readonly string _protocol;
        readonly string _languageCultureCodeSystem;
        readonly string _languageCultureCodeDefault;
        readonly IUrlHelper _url;

        public readonly UrlFactory63Admin Admin;
        public readonly UrlFactory63Website Website;
        #endregion

        #region Constructor
        public UrlFactory63(IUrlHelper url, string languageCultureCodeSystem, string languageCultureCodeDefault, string protocol)
        {
            _url = url;            
            _languageCultureCodeSystem = languageCultureCodeSystem;
            _languageCultureCodeDefault = languageCultureCodeDefault;
            _protocol = protocol;

            Admin = new UrlFactory63Admin(this);
            Website = new UrlFactory63Website(this);
        }
        #endregion

        #region Methods
        string createUrl(string routeName, RouteValueDictionary values)
        {
            var url = _url.RouteUrl(
                routeName: routeName,
                values: values,
                protocol: _protocol
            );
            return url;
        }

        string createUrl(string controllerName, string actionName, RouteValueDictionary values)
        {
            var url = createUrl(
                routeName: $"{controllerName}{actionName}",
                values: values
            );
            return url;
        }

        string createUrlWithLanguage(string controllerName, string actionName, string languageCultureCode, RouteValueDictionary values)
        {
            var url = default(string);
            if (string.IsNullOrWhiteSpace(languageCultureCode))
            {
                languageCultureCode = _languageCultureCodeSystem;
            }

            var isLanguageDefault = languageCultureCode == _languageCultureCodeDefault;

            if (isLanguageDefault)
            {
                url = createUrl(
                    routeName: $"{controllerName}{actionName}",
                    values: values
                );
            }
            else
            {
                if (values == null)
                {
                    values = new RouteValueDictionary();
                }
                values.Add(key: RouteKeys63.LanguageCultureCode, _languageCultureCodeSystem);

                url = createUrl(
                    routeName: $"{controllerName}{actionName}{nameof(RouteKeys63.LanguageCultureCode)}",
                    values: values
                );
            }

            return url;
        }


        public string CreateUrl(string controllerName, string actionName, Dictionary<string,object> routeValues = null)
        {
            var routeValueDict = routeValues.HasElements() ? new RouteValueDictionary(routeValues) : null;

            var url = createUrl(
                controllerName: controllerName,
                actionName: actionName,
                values: routeValueDict
            );
            return url;
        }

        public string CreateUrlWithLanguage(string controllerName, string actionName, string languageCultureCode = null)
        {
            var url = createUrlWithLanguage(
                controllerName: controllerName,
                actionName: actionName,
                languageCultureCode: languageCultureCode,
                values: null
            );
            return url;
        }

        public string CreateUrlReplaceRegexValues(string stringWithRegex, params object[] values)
        {
            int index = 0;

            return Regex.Replace(
                stringWithRegex,
                @"\([^)]+\)",
                match =>
                {
                    if (index >= values.Length)
                    {
                        throw new ArgumentException("Not enough values provided");
                    }

                    return values[index++].ToString();
                });
        }
        #endregion
    }
}