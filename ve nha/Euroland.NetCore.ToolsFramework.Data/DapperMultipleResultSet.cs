using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace Euroland.NetCore.ToolsFramework.Data
{
    /// <summary>
    /// Abstract class represents the set of multiple result by using Dapper library 
    /// at https://github.com/StackExchange/Dapper
    /// </summary>
    public class DapperMultipleResultSet : IMultipleResultSet
    {
        private readonly dynamic _parameterObject = null;
        private readonly string _storedProcedure;
        private readonly IDbConnection _connection;
        private readonly SqlMapper.GridReader _gridReader;

        /// <summary>
        /// Creates a new <see cref="DapperMultipleResultSetChain"/>
        /// </summary>
        /// <param name="storedProcedure">Name of stored procedure</param>
        /// <param name="parameters">List of <see cref="System.Data.IDataParameter"/></param>
        public DapperMultipleResultSet(IDatabaseContext context, string storedProcedure, IEnumerable<IDataParameter> parameters)
            : this(context, storedProcedure)
        {
            _parameterObject = this.ConvertParameterToDynamic(parameters);
        }

        /// <summary>
        /// Creates a new <see cref="DapperMultipleResultSetChain"/>
        /// </summary>
        /// <param name="context">The object of <see cref="IDatabaseContext"/></param>
        /// <param name="parameterObject">The parameter object</param>
        /// <param name="storedProcedure">Name of stored procedure</param>
        public DapperMultipleResultSet(IDatabaseContext context, string storedProcedure, dynamic parameterObject = null)
        {
            if (storedProcedure == null)
                throw new ArgumentNullException(nameof(storedProcedure));

            if (context == null)
                throw new ArgumentNullException(nameof(context));            

            _storedProcedure = storedProcedure;
            _parameterObject = parameterObject;
            _connection = context.Connection;
            this._gridReader = _connection.QueryMultiple(_storedProcedure, param: (object)_parameterObject, commandType: CommandType.StoredProcedure);
        }
        
        /// <inheritdoc />
        public IEnumerable<TResult> Get<TResult>() where TResult: class
        {
            return this._gridReader.Read<TResult>();
        }

        /// <inheritdoc />
        public TResult GetSingle<TResult>() where TResult : class
        {
            return this._gridReader.ReadSingleOrDefault<TResult>();
        }

        private dynamic ConvertParameterToDynamic(IEnumerable<IDataParameter> parameters)
        {
            if (parameters == null || parameters.Count() == 0)
                return null;
            dynamic obj = null;
            foreach (var pa in parameters)
            {
                string prop = pa.ParameterName.TrimStart(new char[] { '@' });
                obj[prop] = pa.Value;
            }

            return obj;
        }

        public void Dispose()
        {
            if (_gridReader != null)
                _gridReader.Dispose();

            if (_connection.State == ConnectionState.Open || _connection.State == ConnectionState.Connecting)
                _connection.Close();
        }
    }
}
