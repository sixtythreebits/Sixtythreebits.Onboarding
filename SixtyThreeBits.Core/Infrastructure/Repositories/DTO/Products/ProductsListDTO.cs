using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record ProductsListDTO
    {
        #region Properties
        public int? ProductID { get; init; }
        public string ProductName { get; init; }
        public decimal? ProductPrice { get; init; }
        public string ProductCoverImageFilename { get; init; }
        public int? CategoryID { get; init; }
        public bool ProductIsPublished { get; init; }
        public DateTime? ProductDateCreated { get; init; }
        #endregion
    }
}