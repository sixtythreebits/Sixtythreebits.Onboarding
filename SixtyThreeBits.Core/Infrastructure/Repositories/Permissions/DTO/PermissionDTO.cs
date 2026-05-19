namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public record PermissionDTO
    {
        #region Properties
        public int? PermissionID { get; init; }
        public int? PermissionParentID { get; init; }
        public string PermissionName { get; init; }
        public string PermissionPagePath { get; init; }                
        public bool PermissionIsMenuItem { get; init; }
        public string PermissionMenuIcon { get; init; }
        public int? PermissionSortIndex { get; init; }

        public string PermissionMenuTitle { get; init; }
        public bool HasPermissionMenuTitle => !string.IsNullOrWhiteSpace(PermissionMenuTitle);
        public string PermissionMenuTitleOrCaption => HasPermissionMenuTitle ? PermissionMenuTitle : PermissionName;

        public string PermissionGuid { get; init; }
        #endregion        
    }   
}
