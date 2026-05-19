namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public record PermissionIudDTO
    {
        #region Properties
        public int? PermissionID { get; init; }
        public int? PermissionParentID { get; init; }
        public string PermissionName { get; init; }
        public string PermissionPagePath { get; init; }                
        public bool? PermissionIsMenuItem { get; init; }
        public string PermissionMenuIcon { get; init; }
        public string PermissionMenuTitle { get; init; }
        public int? PermissionSortIndex { get; init; }
        public string PermissionGuid { get; init; }
        #endregion
    }   
}
