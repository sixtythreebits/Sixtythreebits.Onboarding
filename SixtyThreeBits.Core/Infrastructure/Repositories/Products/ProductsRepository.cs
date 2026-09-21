using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Libraries.Common;
using SixtyThreeBits.Core.Libraries.Database;
using SixtyThreeBits.Core.Libraries.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class ProductsRepository : RepositoryBase
    {
        #region Contructors
        public ProductsRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {            
        }
        #endregion

        #region Methods
        public async Task<Result63<List<CategoriesListDTO>>> CategoriesList()
        {
            var result = await TryAsync(
                logString: $"{nameof(CategoriesList)}()",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(CategoriesList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<CategoriesListDTO>();
                        resultQueryable = resultQueryable.OrderBy(item => item.CategoryName);
                        var result = await resultQueryable.ToListAsync();

                        return result;
                    }
                }
            );

            return result;
        }

        public async Task<Result63<ProductDTO>> ProductsGetSingleByID(int? productID)
        {
            var result = await TryAsync(
                logString: $"{nameof(ProductsGetSingleByID)}({nameof(productID)} = {productID})",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsGetSingleByID),
                            sqlParameters:
                            [
                                productID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<ProductDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<Result63<int?>> ProductsIUD(DatabaseActions databaseAction, int? productID, ProductsIudDTO product)
        {
            var productJson = product.ToJson();

            var result = await TryAsync(
                logString: $"{nameof(ProductsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(productID)} = {product}, {nameof(productJson)} = {productJson})",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(SqlDbType.TinyInt),
                                productID.ToSqlParameter(SqlDbType.Int),
                                productJson.ToSqlParameter(SqlDbType.NVarChar),
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        productID = sqb.GetNextOutputParameterValue<int?>();
                        return productID;
                    }
                }
            );
            return result;
        }

        public async Task<Result63<List<ProductsListDTO>>> ProductsList()
        {
            var result = await TryAsync(
                logString: $"{nameof(ProductsList)}()",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<ProductsListDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.ProductDateCreated);
                        var result = await resultQueryable.ToListAsync();

                        return result;
                    }
                }
            );
            return result;
        }
        #endregion
    }
}