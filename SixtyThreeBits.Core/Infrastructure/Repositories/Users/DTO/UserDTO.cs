using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public record UserDTO
    {
        #region Properties
        public int? UserID { get; init; }
        public string UserFullname { get; init; }
        public string UserFirstname { get; init; }
        public string UserLastname { get; init; }
        public DateTime? UserBirthdate { get; init; }
        public string UserEmail { get; init; }
        public string UserPassword { get; init; }
        public string UserPhoneNumberMobile { get; init; }
        public bool UserIsActive { get; init; } 
        public string UserAvatarFilename { get; init; }
        public DateTime? UserDateCreated { get; init; }
        public int? RoleID { get; init; }
        public int? RoleCode { get; init; }
        public string RoleName { get; init; }
        public List<PermissionDTO> Permissions { get; init; }
        #endregion Properties

        #region Methods
        public List<PermissionDTO> GetChildPermissionsByParent(string parentPermission, bool? isMenuItem = null)
        {
            List<PermissionDTO> childPermissions = null;
            var parentPermissionItem = GetPermission(parentPermission);

            if (parentPermissionItem != null)
            {
                childPermissions = Permissions
                    .Where(item => item.PermissionParentID == parentPermissionItem.PermissionID && (isMenuItem == null || item.PermissionIsMenuItem == isMenuItem))
                    .OrderBy(item => item.PermissionSortIndex)
                    .ToList();
            }

            return childPermissions;
        }

        public PermissionDTO GetPermission(string permissionStringValue)
        {
            if (string.IsNullOrWhiteSpace(permissionStringValue))
            {
                return null;
            }
            else
            {
                if (Uri.TryCreate(permissionStringValue, UriKind.Absolute, out var uri))
                {
                    permissionStringValue = uri.PathAndQuery;
                }

                var permission = Permissions?
                    .Where(item =>
                        item.PermissionGuid == permissionStringValue
                        ||
                        (!string.IsNullOrWhiteSpace(item.PermissionPagePath) && Regex.IsMatch(permissionStringValue, $"^{item.PermissionPagePath}*$"))
                    ).LastOrDefault();

                return permission;
            }
        }

        public bool HasPermission(string permissionStringValue)
        {
            var p = GetPermission(permissionStringValue);
            return p != null;
        }
        #endregion Methods        
    }
}
