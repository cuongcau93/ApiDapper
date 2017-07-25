using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Translation
{
    public class TranslationDataContext
    {
        private string connectionString;

        public TranslationDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection DbContext => new SqlConnection(connectionString);

        public IEnumerable<Translation> LoadTranslations(string storedProcedure, object parameters)
        {
            using (var dbContext = DbContext)
            {
                IDataReader reader = dbContext.ExecuteReader(storedProcedure, param: parameters, commandType: CommandType.StoredProcedure);
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
                return new Dictionary<string, TValue>(dictionaries, new IgnoredCaseDicComparer());
            }
            else
            {
                return new Dictionary<string, TValue>(new IgnoredCaseDicComparer());
            }
        }

        private class IgnoredCaseDicComparer : IEqualityComparer<string>
        {
            #region IEqualityComparer<string> Members

            public bool Equals(string x, string y)
            {
                return string.Compare(x, y, true) == 0;
            }

            public int GetHashCode(string obj)
            {
                return 0;
            }

            #endregion
        }
    }
}
