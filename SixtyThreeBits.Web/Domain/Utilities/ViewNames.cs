namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static class ViewNames
    {
        #region Nested Classes
        public static class Admin
        {
            #region Nested Classes
            public static class Auth
            {
                #region Properties
                public const string Login = "~/Views/Admin/Auth/Login.cshtml";
                #endregion
            }

            public static class Dictionaries
            {
                #region Properties
                public const string Page = "~/Views/Admin/Dictionaries/Dictionaries.cshtml";
                #endregion
            }

            public static class Home
            {
                #region Properties
                public const string Index = "~/Views/Admin/Home/Index.cshtml";
                #endregion
            }

            public static class Errors
            {
                #region Properties
                public const string NotFoundView = "~/Views/Admin/Errors/NotFound.cshtml";
                #endregion
            }

            public static class Permissions
            {
                #region Properties
                public const string Page = "~/Views/Admin/Permissions/Permissions.cshtml";
                #endregion
            }

            public static class Products
            {
                #region Properties
                public const string ProductPropertiesView = "~/Views/Admin/Products/ProductProperties.cshtml";
                public const string ProductsView = "~/Views/Admin/Products/Products.cshtml";                
                #endregion
            }

            public static class Roles
            {
                #region Properties
                public const string Page = "~/Views/Admin/Roles/Roles.cshtml";
                #endregion
            }

            public static class RolesPermissions
            {
                #region Properties
                public const string Page = "~/Views/Admin/RolesPermissions/RolesPermissions.cshtml";
                #endregion
            }

            public static class Shared
            {
                #region Properties
                public const string Layout = "~/Views/Admin/Shared/Layout.cshtml";
                public const string PopupLayout = "~/Views/Admin/Shared/PopupLayout.cshtml";
                public const string Tabs = "~/Views/Admin/Shared/Tabs.cshtml";
                public const string SuccessErrorToastPartialView = "~/Views/Admin/Shared/SuccessErrorToastPartialView.cshtml";

                public const string ButtonAddNew = "~/Views/Admin/Shared/ButtonAddNew.cshtml";
                public const string ButtonSave = "~/Views/Admin/Shared/ButtonSave.cshtml";
                #endregion

                #region Nested Classes
                public static class FileTreeEditor
                {
                    #region Properties
                    public const string Editor = "~/Views/Admin/Shared/FileTreeEditor/FileTreeEditor.cshtml";
                    public const string File = "~/Views/Admin/Shared/FileTreeEditor/FileTreeEditorFile.cshtml";
                    #endregion
                }
                #endregion
            }

            public static class Users
            {
                #region Properties
                public const string Page = "~/Views/Admin/Users/Users.cshtml";
                #endregion

                #region Nested Classes
                public static class User
                {
                    #region Properties
                    public const string Layout = "~/Views/Admin/Users/User/UserLayout.cshtml";
                    public const string Properties = "~/Views/Admin/Users/User/UserProperties.cshtml";
                    #endregion
                }
                #endregion
            }            
            #endregion
        }

        public static class Website
        {
            #region Nested Classes
            public static class Home
            {
                #region Properties
                public const string Page = "~/Views/Website/Home/Index.cshtml";
                #endregion
            }

            public static class Errors
            {
                #region Properties
                public const string NotFoundView = "~/Views/Website/Errors/NotFound.cshtml";
                #endregion
            }

            public static class Products
            {
                #region Properties
                public const string ProductView = "~/Views/Website/Products/Product.cshtml";
                public const string ProductsView = "~/Views/Website/Products/Products.cshtml";
                #endregion
            }

            public static class Shared
            {
                #region Properties
                public const string Layout = "~/Views/Website/Shared/Layout.cshtml";
                public const string Pager = "~/Views/Website/Shared/Pager.cshtml";
                #endregion
            }
            #endregion
        }

        public static class Shared
        {
            #region Nested Classes            
            public static class FileTree
            {
                #region Properties
                public const string Tree = "~/Views/Shared/FileTree/FileTree.cshtml";
                public const string Folder = "~/Views/Shared/FileTree/FileTreeFolder.cshtml";
                public const string File = "~/Views/Shared/FileTree/FileTreeFile.cshtml";
                #endregion
            }

            public static class PluginsClient
            {
                #region Properties
                public const string Footer = "~/Views/Shared/PluginsClient/PluginsClientFooter.cshtml";
                public const string Header = "~/Views/Shared/PluginsClient/PluginsClientHeader.cshtml";
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
