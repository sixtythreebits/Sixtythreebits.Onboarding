namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static class ControllerActionRouteNames
    {
        public static class Admin
        {
            #region Nested Classes
            public static class Auth
            {
                #region Properties
                public const string Login = "AdminAuthLogin";
                public const string Logout = "AdminAuthLogout";
                public const string Relogin = "AdminAuthRelogin";
                #endregion
            }

            public static class ChangeLanguage
            {
                #region Properties
                public const string Page = "AdminChangeLanguage";
                #endregion
            }

            public static class Dictionaries
            {
                #region Properties
                public const string Page = "AdminDictionaries";
                public const string DictionariesTree = "AdminDictionariesTree";
                public const string DictionariesTreeAdd = "AdminDictionariesTreeAdd";
                public const string DictionariesTreeUpdate = "AdminDictionariesTreeUpdate";
                public const string DictionariesTreeDelete = "AdminDictionariesTreeDelete";
                #endregion
            }

            public static class Home
            {
                #region Properties
                public const string Page = "AdminHomeIndex";
                #endregion
            }            

            public static class Permissions
            {
                #region Properties
                public const string Page = "AdminPermissions";
                public const string Tree = "AdminPermissionsTree";
                public const string TreeAdd = "AdminPermissionsTreeAdd";
                public const string TreeUpdate = "AdminPermissionsTreeUpdate";
                public const string TreeDelete = "AdminPermissionsTreeDelete";
                public const string TreeUpdateParent = "AdminPermissionsTreeUpdateParent";
                #endregion
            }            

            public static class ProductsController
            {
                #region Properties
                public const string Products = "AdminProductsControllerProducts";
                public const string Grid = "AdminProductsControllerGrid";
                public const string GridAdd = "AdminProductsControllerGridAdd";
                public const string GridUpdate = "AdminProductsControllerGridUpdate";
                public const string GridDelete = "AdminProductsControllerGridDelete";                
                #endregion
            }

            public static class ProductPropertiesController
            {
                #region Properties                
                public const string Properties = "ProductPropertiesControllerProperties";
                public const string DeleteImage = "ProductPropertiesControllerDeleteImage";
                #endregion
            }

            public static class Roles
            {
                #region Properties
                public const string Page = "AdminRoles";
                public const string Grid = "AdminRolesGrid";
                public const string GridAdd = "AdminRolesGridAdd";
                public const string GridUpdate = "AdminRolesGridUpdate";
                public const string GridDelete = "AdminRolesGridDelete";
                #endregion
            }

            public static class RolesPermissions
            {
                #region Properties
                public const string Page = "AdminRolesPermissions";
                public const string RolesGrid = "AdminRolesPermissionsRolesGrid";
                public const string PermissionsTree = "AdminRolesPermissionsPermissionsTree";
                public const string PermissionsGetByRole = "AdminRolesPermissionsPermissionsGetByRole";
                public const string Save = "AdminRolesPermissionsSave";
                #endregion
            }

            public static class Users
            {
                #region Properties
                public const string Page = "AdminUsers";
                public const string Grid = "AdminUsersGrid";
                public const string GridAdd = "AdminUsersGridAdd";
                public const string GridUpdate = "AdminUsersGridUpdate";
                public const string GridDelete = "AdminUsersGridDelete";
                #endregion

                #region Nested Classes
                public static class User
                {
                    #region Properties
                    public const string Root = "AdminUsersUser";
                    public const string Properties = "AdminUsersUserProperties";
                    #endregion
                }
                #endregion
            }            
            #endregion
        }

        public static class Website
        {
            #region Nested Classes
            public static class HomeController
            {
                #region Properties
                public const string Index = "WebsiteHomeControllerIndex";
                #endregion
            }
            
            public static class ProductsController
            {
                #region Properties
                public const string Products = "WebsiteProductsControllerProducts";
                public const string Product = "WebsiteProductsControllerProduct"; 
                #endregion
            }
            #endregion
        }
    }
}
