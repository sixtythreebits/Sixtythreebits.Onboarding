using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Libraries.Common;
using SixtyThreeBits.Core.Libraries.Database;
using SixtyThreeBits.Core.Libraries.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class RolesRepository : RepositoryBase
    {
        #region Contructors
        public RolesRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {
        }
        #endregion

        #region Methods
        public async Task<Result63<int?>> RolesIUD(DatabaseActions databaseAction, int? roleID, RoleIudDTO role)
        {
            var roleJson = role.ToJson();

            var result = await TryAsync(
                logString: $"{nameof(RolesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(roleID)} = {roleID}, {nameof(role)} = {roleJson})",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(RolesIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(SqlDbType.TinyInt),
                                roleID.ToSqlParameterOutput(SqlDbType.Int),
                                roleJson.ToSqlParameter(SqlDbType.NVarChar)
                            ]
                         );

                        await sqb.ExecuteStoredProcedure();
                        roleID = sqb.GetNextOutputParameterValue<int?>();
                        return roleID;
                    }
                }
            );
            return result;
        }

        public async Task<Result63<ReadOnlyCollection<RoleDTO>>> RolesList()
        {
            var result = await TryAsync(
                logString: $"{nameof(RolesList)}()",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(RolesList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<RoleDTO>();
                        resultQueryable = resultQueryable.OrderBy(item => item.RoleCode);
                        var result = await resultQueryable.ToReadOnlyListAsync();

                        return result;
                    }
                }
            );
            return result;
        }        

        public async Task<Result63> RolesPermissionsUpdate(int? roleID, ReadOnlyCollection<int?> permissionIDs)
        {
            var permissionIDsJson = permissionIDs.ToJson();
            var result = await TryAsync(
                logString: $"{nameof(RolesPermissionsUpdate)}({nameof(roleID)} = {roleID}, {nameof(permissionIDs)} = {permissionIDsJson})",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(RolesPermissionsUpdate),
                            sqlParameters:
                            [
                                roleID.ToSqlParameter(SqlDbType.Int),
                                permissionIDsJson.ToSqlParameter(SqlDbType.NVarChar)
                            ]
                        );
                        await sqb.ExecuteStoredProcedure();
                    }
                }
            );
            return result;
        }
        #endregion
    }
}