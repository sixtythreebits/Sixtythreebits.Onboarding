using SixtyThreeBits.Core.Utilities;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextCommands
    {
        #region Methods
        public async Task<int?> UsersIUD(Enums.DatabaseActions databaseAction, int? userID, int? roleID, string userEmail, string userPassword, string userFirstname, string userLastname)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(UsersIUD),
                sqlParameters:
                [
                    databaseAction.ToSqlParameter(nameof(databaseAction), SqlDbType.TinyInt),
                    userID.ToSqlOutputParameter(nameof(userID), SqlDbType.Int),
                    roleID.ToSqlParameter(nameof(roleID), SqlDbType.Int),
                    userEmail.ToSqlParameter(nameof(userEmail), SqlDbType.VarChar),
                    userPassword.ToSqlParameter(nameof(userPassword), SqlDbType.NVarChar),
                    userFirstname.ToSqlParameter(nameof(userFirstname), SqlDbType.NVarChar),
                    userLastname.ToSqlParameter(nameof(userLastname), SqlDbType.NVarChar)
                ]
            );

            await sqb.ExecuteCommand();
            userID = sqb.GetNextOutputParameterValue<int?>();
            return userID;
        }
        #endregion
    }
}