using System;

namespace SixtyThreeBits.Core.DTO
{
    public record ProductDTO
    {
        #region Properties
        public int? ProductID { get; init; }
        public string ProductName { get; init; }
        public decimal? ProductPrice { get; init; }
        public string ProductCoverImageFilename { get; init; }        
        public bool ProductIsPublished { get; init; }
        public DateTime? ProductDateCreated { get; set; }
        public int? CategoryID { get; init; }
        public string CategoryName { get; init; }
        #endregion
    }

    public record ProductIudDTO
    {
        #region Properties
        public string ProductName { get; init; }
        public decimal? ProductPrice { get; init; }
        public string ProductCoverImageFilename { get; init; }
        public int? CategoryID { get; init; }
        public bool? ProductIsPublished { get; init; }
        #endregion
    }

    public record ProductsListDTO
    {
        #region Properties
        public int? ProductID { get; init; }
        public string ProductName { get; init; }
        public decimal? ProductPrice { get; init; }        
        public int? CategoryID { get; init; }
        public bool ProductIsPublished { get; init; }
        public DateTime? ProductDateCreated { get; init; }
        #endregion
    }
}