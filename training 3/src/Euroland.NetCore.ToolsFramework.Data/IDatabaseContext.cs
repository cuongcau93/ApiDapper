using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Euroland.NetCore.ToolsFramework.Data
{
    /// <summary>
    /// Abstract DataContext class
    /// </summary>
    public interface IDatabaseContext
    {
        /// <summary>
        /// Gets always a new instance of <see cref="System.Data.IDbConnection"/>
        /// </summary>
        System.Data.IDbConnection Connection { get; }
        /// <summary>
        /// Executes parameterized stored procedure 
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameters">List of <see cref="System.Data.IDataParameter"/></param>
        /// <returns>Returns the number of affected rows</returns>
        int ExecNonQuery(string storedProcedure, IEnumerable<System.Data.IDataParameter> parameters);

        /// <summary>
        /// Executes parameterized stored procedure 
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameterObject">The object contains the properties and values for parameters. The property's name must match the parameter name</param>
        /// <returns>Returns the number of affected rows</returns>
        int ExecNonQuery(string storedProcedure, dynamic parameterObject = null);

        /// <summary>
        /// Executes asynchronously parameterized stored procedure 
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameters">List of <see cref="System.Data.IDataParameter"/></param>
        /// <returns>Returns the number of affected rows</returns>
        Task<int> ExecNonQueryAsync(string storedProcedure, IEnumerable<System.Data.IDataParameter> parameters);

        /// <summary>
        /// Executes asynchronously parameterized stored procedure 
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameterObject">The object contains the properties and values for parameters. The property's name must match the parameter name</param>
        /// <returns>Returns the number of affected rows</returns>
        Task<int> ExecNonQueryAsync(string storedProcedure, dynamic parameterObject = null);

        /// <summary>
        /// Executes parameterized stored procedure 
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameters">List of <see cref="System.Data.IDataParameter"/></param>
        /// <returns>Returns an object with properties matching columns</returns>
        T ExecSingle<T>(string storedProcedure, IEnumerable<System.Data.IDataParameter> parameters);

        /// <summary>
        /// Executes parameterized stored procedure 
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameterObject">The object contains the properties and values for parameters. The property's name must match the parameter name</param>
        /// <returns>Returns an object with properties matching columns</returns>
        T ExecSingle<T>(string storedProcedure, dynamic parameterObject = null);

        /// <summary>
        /// Executes asynchronously parameterized stored procedure 
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameters">List of <see cref="System.Data.IDataParameter"/></param>
        /// <returns>Returns an object with properties matching columns</returns>
        Task<T> ExecSingleAsync<T>(string storedProcedure, IEnumerable<System.Data.IDataParameter> parameters);

        /// <summary>
        /// Executes asynchronously parameterized stored procedure and return the first record
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameterObject">The object contains the properties and values for parameters. The property's name must match the parameter name</param>
        /// <returns>Returns an object with properties matching columns</returns>
        Task<T> ExecSingleAsync<T>(string storedProcedure, dynamic parameterObject = null);

        /// <summary>
        /// Executes parameterized stored procedure 
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameters">List of <see cref="System.Data.IDataParameter"/></param>
        /// <returns>Returns set of record of object with properties matching columns</returns>
        IEnumerable<T> Exec<T>(string storedProcedure, IEnumerable<System.Data.IDataParameter> parameters);

        /// <summary>
        /// Executes parameterized stored procedure 
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameterObject">The object contains the properties and values for parameters. The property's name must match the parameter name</param>
        /// <returns>Returns set of record of object with properties matching columns</returns>
        IEnumerable<T> Exec<T>(string storedProcedure, dynamic parameterObject = null);

        /// <summary>
        /// Executes asynchronously parameterized stored procedure 
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameters">List of <see cref="System.Data.IDataParameter"/></param>
        /// <returns>Returns set of record of object with properties matching columns</returns>
        Task<IEnumerable<T>> ExecAsync<T>(string storedProcedure, IEnumerable<System.Data.IDataParameter> parameters);

        /// <summary>
        /// Executes asynchronously parameterized stored procedure 
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameterObject">The object contains the properties and values for parameters. The property's name must match the parameter name</param>
        /// <returns>Returns set of record of object with properties matching columns</returns>
        Task<IEnumerable<T>> ExecAsync<T>(string storedProcedure, dynamic parameterObject = null);

        /// <summary>
        /// Executes parameterized stored procedure with a provided transaction object. 
        /// A tranaction object can be <see cref="System.Data.IDbTransaction"/>
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameters">List of <see cref="System.Data.IDataParameter"/></param>
        /// <returns>Nothing</returns>
        void ExecWithTransaction(string storedProcedure, IEnumerable<System.Data.IDataParameter> parameters);

        /// <summary>
        /// Executes parameterized stored procedure with a provided transaction object. 
        /// A tranaction object can be <see cref="System.Data.IDbTransaction"/>
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameterObject">The object contains the properties and values for parameters. The property's name must match the parameter name</param>
        /// <returns>Nothing</returns>
        void ExecWithTransaction(string storedProcedure, dynamic parameterObject = null);

        /// <summary>
        /// Executes asynchronously parameterized stored procedure with a provided transaction object. 
        /// A tranaction object can be <see cref="System.Data.IDbTransaction"/>
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameters">List of <see cref="System.Data.IDataParameter"/></param>
        /// <returns>Returns <see cref="Task"/> object</returns>
        Task ExecWithTransactionAsync(string storedProcedure, IEnumerable<System.Data.IDataParameter> parameters);

        /// <summary>
        /// Executes asynchronously parameterized stored procedure with a provided transaction object. 
        /// A tranaction object can be <see cref="System.Data.IDbTransaction"/>
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameterObject">The object contains the properties and values for parameters. The property's name must match the parameter name</param>
        /// <returns>Returns <see cref="Task"/> object</returns>
        Task ExecWithTransactionAsync(string storedProcedure, dynamic parameterObject = null);

        /// <summary>
        /// Executes parameterized stored procedure with a provided transaction object. 
        /// A tranaction object can be <see cref="System.Data.IDbTransaction"/>
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameters">List of <see cref="System.Data.IDataParameter"/></param>
        /// <returns>Returns an object with properties matching columns</returns>
        T ExecSingleWithTransaction<T>(string storedProcedure, IEnumerable<System.Data.IDataParameter> parameters);

        /// <summary>
        /// Executes parameterized stored procedure with a provided transaction object. 
        /// A tranaction object can be <see cref="System.Data.IDbTransaction"/>
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameterObject">The object contains the properties and values for parameters. The property's name must match the parameter name</param>
        /// <returns>Returns an object with properties matching columns</returns>
        T ExecSingleWithTransaction<T>(string storedProcedure, dynamic parameterObject = null);

        /// <summary>
        /// Executes asynchronously parameterized stored procedure with a provided transaction object. 
        /// A tranaction object can be <see cref="System.Data.IDbTransaction"/>
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameters">List of <see cref="System.Data.IDataParameter"/></param>
        /// <returns>Returns an object with properties matching columns</returns>
        Task<T> ExecSingleWithTransactionAsync<T>(string storedProcedure, IEnumerable<System.Data.IDataParameter> parameters);

        /// <summary>
        /// Executes asynchronously parameterized stored procedure with a provided transaction object. 
        /// A tranaction object can be <see cref="System.Data.IDbTransaction"/>
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameterObject">The object contains the properties and values for parameters. The property's name must match the parameter name</param>
        /// <returns>Returns an object with properties matching columns</returns>
        Task<T> ExecSingleWithTransactionAsync<T>(string storedProcedure, dynamic parameterObject = null);

        /// <summary>
        /// Executes parameterized stored procedure with a provided transaction object. 
        /// A tranaction object can be <see cref="System.Data.IDbTransaction"/>
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameters">List of <see cref="System.Data.IDataParameter"/></param>
        /// <returns>Returns set of record of object with properties matching columns</returns>
        IEnumerator<T> ExecWithTransaction<T>(string storedProcedure, IEnumerable<System.Data.IDataParameter> parameters);

        /// <summary>
        /// Executes parameterized stored procedure with a provided transaction object. 
        /// A tranaction object can be <see cref="System.Data.IDbTransaction"/>
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameterObject">The object contains the properties and values for parameters. The property's name must match the parameter name</param>
        /// <returns>Returns set of record of object with properties matching columns</returns>
        IEnumerator<T> ExecWithTransaction<T>(string storedProcedure, dynamic parameterObject = null);

        /// <summary>
        /// Executes asynchronously parameterized stored procedure with a provided transaction object. 
        /// A tranaction object can be <see cref="System.Data.IDbTransaction"/>
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameters">List of <see cref="System.Data.IDataParameter"/></param>
        /// <returns>Returns set of record of object with properties matching columns</returns>
        Task<IEnumerator<T>> ExecWithTransactionAsync<T>(string storedProcedure, IEnumerable<System.Data.IDataParameter> parameters);

        /// <summary>
        /// Executes asynchronously parameterized stored procedure with a provided transaction object. 
        /// A tranaction object can be <see cref="System.Data.IDbTransaction"/>
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameterObject">The object contains the properties and values for parameters. The property's name must match the parameter name</param>
        /// <returns>Returns set of record of object with properties matching columns</returns>
        Task<IEnumerator<T>> ExecWithTransactionAsync<T>(string storedProcedure, dynamic parameterObject = null);

        /// <summary>
        /// Executes parameterized stored procedure
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameters">List of <see cref="System.Data.IDataParameter"/></param>
        /// <returns>Return an object of <see cref="IMultipleResultSet"/> which can be used to chain and query multiple type of result set</returns>
        IMultipleResultSet QueryMultipleResult(string storedProcedure, IEnumerable<System.Data.IDataParameter> parameters);

        /// <summary>
        /// Executes parameterized stored procedure
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameterObject">The object contains the properties and values for parameters. The property's name must match the parameter name</param>
        /// <returns>Return an object of <see cref="IMultipleResultSet"/> which can be used to chain and query multiple type of result set</returns>
        IMultipleResultSet QueryMultipleResult(string storedProcedure, dynamic parameterObject = null);

        /// <summary>
        /// Executes asynchronously parameterized stored procedure 
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameters">List of <see cref="System.Data.IDataParameter"/></param>
        /// <returns>Return an object of <see cref="IMultipleResultSet"/> which can be used to chain and query multiple type of result set</returns>
        Task<IMultipleResultSet> QueryMultipleResultAsync(string storedProcedure, IEnumerable<System.Data.IDataParameter> parameters);

        /// <summary>
        /// Executes asynchronously parameterized stored procedure 
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameterObject">The object contains the properties and values for parameters. The property's name must match the parameter name</param>
        /// <returns>Return an object of <see cref="IMultipleResultSet"/> which can be used to chain and query multiple type of result set</returns>
        Task<IMultipleResultSet> QueryMultipleResultAsync(string storedProcedure, dynamic parameterObject = null);
    }

}
