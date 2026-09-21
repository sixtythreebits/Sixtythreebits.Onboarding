using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Libraries.Common;
using SixtyThreeBits.Core.Libraries.Database;
using SixtyThreeBits.Core.Libraries.Extensions;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class PermissionsRepository : RepositoryBase
    {
        #region Contructors
        public PermissionsRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {            
        }
        #endregion

        #region Methods
        public async Task<Result63> PermissionsDeleteRecursive(int? permissionID)
        {
            var result = await TryAsync(
                logString: $"{nameof(PermissionsDeleteRecursive)}({nameof(permissionID)} = {permissionID})",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PermissionsDeleteRecursive),
                            sqlParameters:
                            [
                                permissionID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );
                        await sqb.ExecuteStoredProcedure();
                    }
                }
            );
            return result;
        }

        public async Task<Result63<int?>> PermissionsIUD(DatabaseActions databaseAction, int? permissionID, PermissionIudDTO permission)
        {
            var permissionJson = permission.ToJson();

            var result = await TryAsync(
                logString: $"{nameof(PermissionsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(permissionID)} = {permissionID}, {nameof(permission)} = {permissionJson})",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PermissionsIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(SqlDbType.TinyInt),
                                permissionID.ToSqlParameterOutput(SqlDbType.Int),
                                permissionJson.ToSqlParameter(SqlDbType.NVarChar)                                
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        permissionID = sqb.GetNextOutputParameterValue<int?>();
                        return permissionID;                        
                    }
                }
            );
            return result;
        }

        public async Task<Result63<ReadOnlyCollection<PermissionsListDTO>>> PermissionsList()
        {
            var result = await TryAsync(
                logString: $"{nameof(PermissionsList)}()",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PermissionsList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<PermissionsListDTO>();
                        resultQueryable = resultQueryable.OrderBy(P => P.PermissionSortIndex);
                        var result = await resultQueryable.ToReadOnlyListAsync();
                        
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<Result63<ReadOnlyCollection<PermissionsListByRoleIDDTO>>> PermissionsListByRoleID(int? roleID)
        {
            var result = await TryAsync(
                logString: $"{nameof(PermissionsListByRoleID)}({nameof(roleID)} = {roleID}",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PermissionsListByRoleID),
                            sqlParameters:
                            [
                                roleID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<PermissionsListByRoleIDDTO>();
                        var result = await resultQueryable.ToReadOnlyListAsync();

                        return result;
                    }
                }
            );
            return result;
        }
        #endregion
    }        
}