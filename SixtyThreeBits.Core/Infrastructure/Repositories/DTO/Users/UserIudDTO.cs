namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record UserIudDTO
    {
        #region Properties
        public int? UserID { get; init; }
        public int? RoleID { get; init; }
        public string UserEmail { get; init; }
        public string UserPassword { get; init; }
        public string UserFirstname { get; init; }
        public string UserLastname { get; init; }
        #endregion
    }   
}
