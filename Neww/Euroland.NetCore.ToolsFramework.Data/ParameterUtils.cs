using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using static Dapper.SqlMapper;

namespace Euroland.NetCore.ToolsFramework.Data
{
    public static class ParameterUtils
    {
        /// <summary>
        /// Need to improve the order of paramter fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="tableName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static DynamicParameters ToTVP<T>(this IEnumerable<T> enumerable, string tableName, string typeName)
        {
            List<SqlDataRecord> records = new List<SqlDataRecord>();
            var properties = typeof(T).GetProperties().Where(p => Mapper.TypeToSQLMap.ContainsKey(p.PropertyType));            
            var definitions = properties.Select(p => Mapper.TypeToMetaData(p.Name, p.PropertyType)).ToArray();

            foreach (var item in enumerable)
            {
                var values = properties.Select(p => p.GetValue(item, null)).ToArray();                
                var schema = new SqlDataRecord(definitions);                
                schema.SetValues(values);
                records.Add(schema);
            }

            SqlParameter result = new SqlParameter(tableName, SqlDbType.Structured);
            result.Direction = ParameterDirection.Input;
            result.TypeName = typeName;
            result.Value = records;
            return new DynamicParameters(result);
        }
    }

    public class DynamicParameters : IDynamicParameters
    {
        private readonly SqlParameter _param;
        public DynamicParameters(SqlParameter param)
        {
            _param = param;
        }

        public void AddParameters(IDbCommand command, Identity identity)
        {
            command.Parameters.Add(_param);
        }

        public SqlDbType GetSqlDbTypeName()
        {
            return this._param.SqlDbType;
        }

        public object GeParametertValues()
        {
            return this._param.Value;
        }
    }

    public class Mapper
    {
        public static Dictionary<Type, SqlDbType> TypeToSQLMap = new Dictionary<Type, SqlDbType>()
        {
              {typeof (long),SqlDbType.BigInt},
              {typeof (long?),SqlDbType.BigInt},
              {typeof (byte[]),SqlDbType.Image},
              {typeof (bool),SqlDbType.Bit},
              {typeof (bool?),SqlDbType.Bit},
              {typeof (string),SqlDbType.NVarChar},
              {typeof (DateTime),SqlDbType.DateTime},
              {typeof (DateTime?),SqlDbType.DateTime},
              {typeof (decimal),SqlDbType.Money},
              {typeof (decimal?),SqlDbType.Money},
              {typeof (double),SqlDbType.Float},
              {typeof (double?),SqlDbType.Float},
              {typeof (int),SqlDbType.Int},
              {typeof (int?),SqlDbType.Int},
              {typeof (float),SqlDbType.Real},
              {typeof (float?),SqlDbType.Real},
              {typeof (Guid),SqlDbType.UniqueIdentifier},
              {typeof (Guid?),SqlDbType.UniqueIdentifier},
              {typeof (short),SqlDbType.SmallInt},
              {typeof (short?),SqlDbType.SmallInt},
              {typeof (byte),SqlDbType.TinyInt},
              {typeof (byte?),SqlDbType.TinyInt},
              {typeof (object),SqlDbType.Variant},
              {typeof (DataTable),SqlDbType.Structured},
              {typeof (DateTimeOffset),SqlDbType.DateTimeOffset}
        };

        public static SqlMetaData TypeToMetaData(string name, Type type)
        {
            SqlMetaData data = null;

            if (type == typeof(string))
            {
                data = new SqlMetaData(name, SqlDbType.NVarChar, -1);
            }
            else if (type == typeof(DateTime))
            {
                data = new SqlMetaData(name, SqlDbType.DateTime);
            }
            else
            {
                data = new SqlMetaData(name, TypeToSQLMap[type]);
            }

            return data;
        }
    }
}
