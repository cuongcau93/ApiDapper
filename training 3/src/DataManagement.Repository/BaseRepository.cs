using Euroland.NetCore.ToolsFramework.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataManagement.Repository
{
    public class BaseRepository
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


        //private readonly Microsoft.Extensions.Configuration.IConfigurationRoot _configuration;
        //private readonly IDatabaseContext DbContext;
        //public BaseRepository(Microsoft.Extensions.Configuration.IConfigurationRoot configuration)
        //{
        //    _configuration = configuration;
        //}
    }
}
