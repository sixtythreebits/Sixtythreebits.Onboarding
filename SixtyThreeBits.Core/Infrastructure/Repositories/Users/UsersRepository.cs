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
    public class UsersRepository : RepositoryBase
    {
        #region Contructors
        public UsersRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {            
        }
        #endregion

        #region Methods
        public async Task<Result63<UserDTO>> UsersGetSingleByID(int? userID)
        {
            var result = await TryAsync(
                logString: $"{nameof(UsersGetSingleByID)}({nameof(userID)} = {userID})", 
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(UsersGetSingleByID),
                            sqlParameters:
                            [
                                userID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );
                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<UserDTO>();                                                
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<Result63<UserDTO>> UsersGetSingleByEmailAndPassword(string userEmail, string userPassword)
        {
            var result = await TryAsync(
                logString: $"{nameof(UsersGetSingleByEmailAndPassword)}({nameof(userEmail)} = {userEmail}, {nameof(userPassword)} = {userPassword})",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(UsersGetSingleByEmailAndPassword),
                            sqlParameters:
                            [
                                userEmail.ToSqlParameter(SqlDbType.VarChar),
                                userPassword.ToSqlParameter(SqlDbType.NVarChar)
                            ]
                        );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();                        
                        var result = resultJson.DeserializeJsonTo<UserDTO>();

                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<Result63<bool>> UsersIsEmailUnique(string userEmail, int? userID = null)
        {
            var result = await TryAsync(
                logString: $"{nameof(UsersIsEmailUnique)}({nameof(userEmail)} = {userEmail}, {nameof(userID)} = {userID})",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(UsersIsEmailUnique),
                            sqlParameters:
                            [
                                userEmail.ToSqlParameter(SqlDbType.NVarChar),
                                userID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );
                        var result = await sqb.ExecuteScalarValuedFunction<bool>();                        
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<Result63<int?>> UsersIUD(DatabaseActions databaseAction, int? userID, UserIudDTO user)
        {
            var userJson = user.ToJson();

            var result = await TryAsync(
                logString: $"{nameof(UsersIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(userID)} = {user}, {nameof(userJson)} = {userJson})",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(UsersIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(SqlDbType.TinyInt),
                                userID.ToSqlParameterOutput(SqlDbType.Int),
                                userJson.ToSqlParameter(SqlDbType.NVarChar),
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        userID = sqb.GetNextOutputParameterValue<int?>();
                        return userID;
                    }
                }
            );
            return result;
        }

        public async Task<Result63<List<UsersListDTO>>> UsersList()
        {
            var result = await TryAsync(
                logString: $"{nameof(UsersList)}()",
                tryFunc: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(UsersList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<UsersListDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.UserDateCreated);
                        var result = await resultQueryable.ToListAsync();
                        
                        return result;
                    }
                }
            );
            return result;
        }
        #endregion Methods
    }    
}