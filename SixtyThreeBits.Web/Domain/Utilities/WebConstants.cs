namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static class WebConstants
    {
        public static class Cookies
        {
            #region Properties
            public const string IsAdminSideBarCollapsed = "IsAdminSideBarCollapsed";
            public const string User = "User";
            public const string AdminLanguageCultureCode = "AdminLanguage";
            #endregion
        }

        public static class RouteValues
        {
            #region Properties
            public const string Culture = "culture";
            public const string ProductID = "productID";
            #endregion
        }

        public static class Session
        {
            #region Properties
            public const string User = "User";
            #endregion

            #region Nested Classes
            public class SuccessErrorMessage
            {
                #region Properties
                public const string Error = "SuccessErrorMessageError";
                public const string Success = "SuccessErrorMessageSuccess";
                #endregion
            }
            #endregion
        }

        public static class ViewData
        {
            #region Properties
            public const string LayoutViewModel = "LayoutViewModel";
            public const string UserLayoutViewModel = "UserLayoutViewModel";
            #endregion
        }

        public static class ViewSections
        {
            #region Properties
            public const string HeaderSection = "HeaderSection";
            public const string FooterSection = "FooterSection";
            #endregion
        }
    }
}
