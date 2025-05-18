namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static class ControllerActionRouteNames
    {
        public static class Admin
        {
            #region Nested Classes
            public static class AuthController
            {
                #region Properties
                public const string Login = "AdminAuthLogin";
                public const string Logout = "AdminAuthLogout";
                public const string Relogin = "AdminAuthRelogin";
                #endregion
            }                        

            public static class DictionariesController
            {
                #region Properties
                public const string Dictionaries = "AdminDictionariesControllerDictionaries";
                public const string Tree = "AdminDictionariesControllerTree";
                public const string TreeAdd = "AdminDictionariesControllerTreeAdd";
                public const string TreeUpdate = "AdminDictionariesControllerTreeUpdate";
                public const string TreeDelete = "AdminDictionariesControllerTreeDelete";
                #endregion
            }            

            public static class HomeController
            {
                #region Properties
                public const string Index = "AdminHomeControllerIndex";
                #endregion
            }
                        
            public static class PermissionsController
            {
                #region Properties
                public const string Permissions = "AdminPermissionsControllerPermissions";
                public const string Tree = "AdminPermissionsControllerTree";
                public const string TreeAdd = "AdminPermissionsControllerTreeAdd";
                public const string TreeUpdate = "AdminPermissionsControllerTreeUpdate";
                public const string TreeDelete = "AdminPermissionsControllerTreeDelete";
                #endregion
            }                        

            public static class ProductPropertiesController
            {
                #region Properties
                public const string Properties = "AdminProductsPropertiesControllerProperties";
                public const string DeleteImage = "AdminProductsPropertiesControllerDeleteImage";                
                #endregion
            }            

            public static class RolesControllers
            {
                #region Properties
                public const string Roles = "AdminRolesControllerRoles";
                public const string Grid = "AdminRolesControllerGrid";
                public const string GridAdd = "AdminRolesControllerGridAdd";
                public const string GridUpdate = "AdminRolesControllerGridUpdate";
                public const string GridDelete = "AdminRolesControllerGridDelete";
                #endregion
            }

            public static class RolePermissionsController
            {
                #region Properties
                public const string RolesPermissions = "AdminRolePermissionsControllerRolesPermissions";
                public const string RolesGrid = "AdminRolePermissionsControllerRolesGrid";
                public const string PermissionsTree = "AdminRolePermissionsControllerPermissionsTree";
                public const string GetPermissionsByRole = "AdminRolePermissionsControllerGetPermissionsByRole";
                public const string Save = "AdminRolePermissionsControllerSave";
                #endregion
            }
                        
            public static class UsersController
            {
                #region Properties
                public const string Users = "AdminUsersControllerUsers";
                public const string Grid = "AdminUsersControllerGrid";
                public const string GridAdd = "AdminUsersControllerGridAdd";
                public const string GridUpdate = "AdminUsersControllerGridUpdate";
                public const string GridDelete = "AdminUsersControllerGridDelete";
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
                public const string Index = "WebsiteHomeIndex";
                public const string IndexCulture = "WebsiteHomeIndexCulture";

                #endregion
            }                       

            public static class ProductsController
            {
                #region Proeprties
                public const string Products = "WebsiteProductsControllerProducts";
                public const string Product = "WebsiteProductsControllerProduct";
                #endregion
            }
            #endregion
        }
    }
}
