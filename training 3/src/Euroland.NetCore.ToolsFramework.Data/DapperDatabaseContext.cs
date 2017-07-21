using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace Euroland.NetCore.ToolsFramework.Data
{
    public class DapperDatabaseContext : IDatabaseContext
    {
        private string _connectionString;

        /// <summary>
        /// Instantiate a <see cref="DapperDatabaseContext"/> object
        /// </summary>
        /// <param name="connectionString">The connection string to the database of the current context</param>
        public DapperDatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc/>
        public IDbConnection Connection => new System.Data.SqlClient.SqlConnection(_connectionString);

        /// <inheritdoc/>
        public IEnumerable<T> Exec<T>(string storedProcedure, IEnumerable<IDataParameter> parameters)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            dynamic paraObj = GeneratePrameters(parameters);
            return Exec<T>(storedProcedure, paraObj);
        }

        /// <inheritdoc/>
        public IEnumerable<T> Exec<T>(string storedProcedure, dynamic parameterObject = null)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            var paraObj = (object)parameterObject;
            using (var connection = Connection)
            {
                connection.Open();
                return connection.Query<T>(storedProcedure, param: paraObj, commandType: CommandType.StoredProcedure);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> ExecAsync<T>(string storedProcedure, IEnumerable<IDataParameter> parameters)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            dynamic paraObj = GeneratePrameters(parameters);
            return await ExecAsync<T>(storedProcedure, paraObj);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> ExecAsync<T>(string storedProcedure, dynamic parameterObject = null)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            var paraObj = (object)parameterObject;
            using (var connection = Connection)
            {
                connection.Open();
                return await connection.QueryAsync<T>(storedProcedure, param: paraObj, commandType: CommandType.StoredProcedure);
            }
        }

        /// <inheritdoc/>
        public int ExecNonQuery(string storedProcedure, IEnumerable<IDataParameter> parameters)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            dynamic paraObjQuery = GeneratePrameters(parameters);
            return ExecNonQuery(storedProcedure, paraObjQuery);
        }

        /// <inheritdoc/>
        public int ExecNonQuery(string storedProcedure, dynamic parameterObject = null)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            var objPara = (object)parameterObject;
            using (var connection = Connection)
            {
                connection.Open();
                return connection.Execute(storedProcedure, param: objPara, commandType: CommandType.StoredProcedure);
            }
        }

        /// <inheritdoc/>
        public async Task<int> ExecNonQueryAsync(string storedProcedure, IEnumerable<IDataParameter> parameters)
        {
            dynamic paraObjQuery = GeneratePrameters(parameters);
            return await ExecNonQueryAsync(storedProcedure, paraObjQuery);
        }

        /// <inheritdoc/>
        public async Task<int> ExecNonQueryAsync(string storedProcedure, dynamic parameterObject = null)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            var objPara = (object)parameterObject;
            using (var connection = Connection)
            {
                connection.Open();
                int result = await connection.ExecuteAsync(storedProcedure, param: objPara, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        /// <inheritdoc/>
        public T ExecSingle<T>(string storedProcedure, IEnumerable<IDataParameter> parameters)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            dynamic paramObj = GeneratePrameters(parameters);
            return ExecSingle<T>(storedProcedure, paramObj);
        }

        /// <inheritdoc/>
        public T ExecSingle<T>(string storedProcedure, dynamic parameterObject = null)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            var obj = (object)parameterObject;
            using (var connection = Connection)
            {
                connection.Open();
                if (TypeUtils.IsPrimitive(typeof(T)))
                {
                    return connection.ExecuteScalar<T>(storedProcedure, param: obj, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    return connection.QuerySingleOrDefault<T>(storedProcedure, param: obj, commandType: CommandType.StoredProcedure);
                }
            }
        }

        /// <inheritdoc/>
        public async Task<T> ExecSingleAsync<T>(string storedProcedure, IEnumerable<IDataParameter> parameters)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            dynamic paramObj = GeneratePrameters(parameters);
            return await ExecSingleAsync<T>(storedProcedure, paramObj);
        }

        /// <inheritdoc/>
        public async Task<T> ExecSingleAsync<T>(string storedProcedure, dynamic parameterObject = null)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            var objPara = (object)parameterObject;
            using (var connection = Connection)
            {
                connection.Open();
                if (TypeUtils.IsPrimitive(typeof(T)))
                {
                    return await connection.ExecuteScalarAsync<T>(storedProcedure, param: objPara, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    return await connection.QuerySingleOrDefaultAsync<T>(storedProcedure, param: objPara, commandType: CommandType.StoredProcedure);
                }
            }
        }

        public T ExecSingleWithTransaction<T>(string storedProcedure, IEnumerable<IDataParameter> parameters)
        {
            throw new NotImplementedException();
        }

        public T ExecSingleWithTransaction<T>(string storedProcedure, dynamic parameterObject = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> ExecSingleWithTransactionAsync<T>(string storedProcedure, IEnumerable<IDataParameter> parameters)
        {
            throw new NotImplementedException();
        }

        public Task<T> ExecSingleWithTransactionAsync<T>(string storedProcedure, dynamic parameterObject = null)
        {
            throw new NotImplementedException();
        }

        public void ExecWithTransaction(string storedProcedure, IEnumerable<IDataParameter> parameters)
        {
            throw new NotImplementedException();
        }

        public void ExecWithTransaction(string storedProcedure, dynamic parameterObject = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> ExecWithTransaction<T>(string storedProcedure, IEnumerable<IDataParameter> parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> ExecWithTransaction<T>(string storedProcedure, dynamic parameterObject = null)
        {
            throw new NotImplementedException();
        }

        public Task ExecWithTransactionAsync(string storedProcedure, IEnumerable<IDataParameter> parameters)
        {
            throw new NotImplementedException();
        }

        public Task ExecWithTransactionAsync(string storedProcedure, dynamic parameterObject = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerator<T>> ExecWithTransactionAsync<T>(string storedProcedure, IEnumerable<IDataParameter> parameters)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerator<T>> ExecWithTransactionAsync<T>(string storedProcedure, dynamic parameterObject = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IMultipleResultSet QueryMultipleResult(string storedProcedure, IEnumerable<IDataParameter> parameters)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            return new DapperMultipleResultSet(this, storedProcedure, parameters);
        }

        /// <inheritdoc/>
        public IMultipleResultSet QueryMultipleResult(string storedProcedure, dynamic parameterObject = null)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            return new DapperMultipleResultSet(this, storedProcedure, parameterObject);
        }

        /// <inheritdoc/>
        public async Task<IMultipleResultSet> QueryMultipleResultAsync(string storedProcedure, IEnumerable<IDataParameter> parameters)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            return await Task.Run<IMultipleResultSet>(
                () => new DapperMultipleResultSet(this, storedProcedure, parameters)
            );
        }

        /// <inheritdoc/>
        public async Task<IMultipleResultSet> QueryMultipleResultAsync(string storedProcedure, dynamic parameterObject = null)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            return await Task.Run<IMultipleResultSet>(
                () => new DapperMultipleResultSet(this, storedProcedure, parameterObject)
            );
        }

        /// <summary>
        /// Processing parameters prepare execute command
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private dynamic GeneratePrameters(IEnumerable<IDataParameter> parameters)
        {
            if (parameters == null || parameters.Count() == 0)
                return null;

            dynamic execParamObj = null;
            if (parameters != null && parameters.Count() > 0)
            {
                foreach (var param in parameters)
                {
                    string prop = param.ParameterName.TrimStart(new char[] { '@' });
                    execParamObj[prop] = param.Value;
                }
            }
            return execParamObj;
        }

        /// <summary>
        /// Throw exception when store name is invalid
        /// </summary>
        /// <param name="name"></param>
        private void ThrowIfInvalidStoredProcedure(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Procedure name must be not empty", "storedProcedure");
            if (name.Contains(" "))
                throw new ArgumentException("Illegal of store procedure", "storedProcedure");
        }
    }
}
