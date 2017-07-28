using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Euroland.NetCore.ToolsFramework.Translation
{
    /// <summary>
    /// The database context class used to connect to translaton database
    /// </summary>
    public sealed class TranslationDataContext : ITranslationDataContext
    {
        private readonly string _connectionString;
        /// <summary>
        /// Create a <see cref="TranslationDataContext"/>
        /// </summary>
        /// <param name="options">The <see cref="TranslationOptions"/></param>
        public TranslationDataContext(Microsoft.Extensions.Options.IOptions<TranslationOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrWhiteSpace(options.Value.ConnectionString))
            {
                throw new ArgumentException("Must provide a valid connection string of Translation", "ConnectionString");
            }

            _connectionString = options.Value.ConnectionString;
        }

        public IDbConnection Connection => new SqlConnection(_connectionString);

        public IEnumerable<Translation> LoadTranslations(string storedProcedure, object parameters)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            using (var connection = Connection)
            {
                IDataReader reader = connection.ExecuteReader(storedProcedure, param: parameters, commandType: CommandType.StoredProcedure);
                int fieldCount = reader.FieldCount;
                IDictionary<string, int> fieldIndex = this.GetIgnoreCaseDictionary<int>(null);
                string colName = "";
                for (int i = 0; i < fieldCount; i++)
                {
                    colName = reader.GetName(i);
                    fieldIndex.Add(colName, i);
                }

                while (reader.Read())
                {
                    IDictionary<string, string> tranRow = this.GetIgnoreCaseDictionary<string>(null);
                    foreach (KeyValuePair<string, int> kv in fieldIndex)
                    {
                        if (kv.Key.ToLower() != "id" && kv.Key.Length == 2)
                        {
                            tranRow.Add(
                                kv.Key,     // "EN"
                                reader.GetString(kv.Value) // "0", "1" 
                            );

                        }
                    }
                    yield return this.CreateTranslation(reader.GetInt32(fieldIndex["id"]), tranRow);
                }
            }
        }

        public T ExecSingle<T>(string storedProcedure, IEnumerable<IDataParameter> parameters)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            dynamic paramObj = GeneratePrameters(parameters);
            return ExecSingle<T>(storedProcedure, paramObj);
        }

        public T ExecSingle<T>(string storedProcedure, dynamic parameterObject = null)
        {
            ThrowIfInvalidStoredProcedure(storedProcedure);
            var obj = (object)parameterObject;
            using (var connection = Connection)
            {
                connection.Open();
                return connection.QuerySingleOrDefault<T>(storedProcedure, param: obj, commandType: CommandType.StoredProcedure);
            }
        }

        private Translation CreateTranslation(int id, IDictionary<string, string> languages)
        {
            return new Translation()
            {
                Id = id,
                TranslationMap = this.GetIgnoreCaseDictionary<string>(languages)
            };
        }

        private IDictionary<string, TValue> GetIgnoreCaseDictionary<TValue>(IDictionary<string, TValue> dictionaries)
        {
            if (dictionaries != null)
            {
                return new Dictionary<string, TValue>(dictionaries, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                return new Dictionary<string, TValue>(StringComparer.OrdinalIgnoreCase);
            }
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
                throw new ArgumentNullException("Procedure name must be not empty", "storedProcedure");
        }
    }
}
