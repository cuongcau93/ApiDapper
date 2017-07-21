using Euroland.NetCore.ToolsFramework.Data;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataManagement.Repository
{
    public class BaseRepository : IDisposable
    {
            string serverName = "123-HP";
            string databaseName = "DataManagement";
            /// <summary>
            /// option to login mssql server
            /// if you usage sql express then do not care about User and Password to login sql server
            /// </summary>
            string userID = "sa";
            string password = "123456";
            string connectionString => $"Server={serverName};Database={databaseName};User ID={userID};Password={password};Trusted_Connection=True;MultipleActiveResultSets=true";
            protected DapperDatabaseContext DatabaseConnection => new DapperDatabaseContext(connectionString);

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

    }
}
