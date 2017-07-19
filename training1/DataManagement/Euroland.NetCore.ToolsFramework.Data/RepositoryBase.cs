using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Data
{
    /// <summary>
    /// Base class for repository classes.
    /// </summary>
    public abstract class RepositoryBase
    {
        private readonly IDatabaseContext _dbContext;
        ///// <summary>
        ///// Gets DB context to execute query against database
        ///// </summary>
        public IDatabaseContext DbContext
        {
            get
            {
                return _dbContext;
            }
        }

        /// <summary>
        /// Instantiate an object of <see cref="RepositoryBase"/> with default DbContext is <see cref="DapperDatabaseContext"/>
        /// </summary>
        /// <param name="connectionString">The connection string to the database</param>
        public RepositoryBase(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("connectionString");

            _dbContext = new DapperDatabaseContext(connectionString);
        }

        /// <summary>
        ///  Instantiate an object of <see cref="RepositoryBase"/>
        /// </summary>
        /// <param name="dbContext">The database context</param>
        public RepositoryBase(IDatabaseContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("dbContext");

            _dbContext = dbContext;
        }
    }
}
