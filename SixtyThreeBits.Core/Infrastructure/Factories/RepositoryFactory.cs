using SixtyThreeBits.Core.Infrastructure.Repositories;

namespace SixtyThreeBits.Core.Infrastructure.Factories
{
    public class RepositoryFactory
    {
        #region Properties                
        readonly ConnectionFactory _connectionFactory;
        #endregion

        #region Constructors
        public RepositoryFactory(string commandsConnectionString, string ConnectionString)
        {
            _connectionFactory = new ConnectionFactory(commandsConnectionString, ConnectionString);
        }
        #endregion

        #region Methods
        public DictionariesRepository GetDictionariesRepository()
        {
            return new DictionariesRepository(_connectionFactory);
        }
       
        public PermissionsRepository GetPermissionsRepository()
        {
            return new PermissionsRepository(_connectionFactory);
        }       
        public RolesRepository GetRolesRepository()
        {
            return new RolesRepository(_connectionFactory);
        }

        public SystemPropertiesRepository GetSystemPropertiesRepository()
        {
            return new SystemPropertiesRepository(_connectionFactory);
        }      

        public UsersRepository GetUsersRepository()
        {
            return new UsersRepository(_connectionFactory);
        }
        #endregion        
    }
}
