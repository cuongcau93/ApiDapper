using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Euroland.NetCore.ToolsFramework.Data.Test
{
    public class InitialStoreProcedure
    {
        private string connectionString;
        public InitialStoreProcedure(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void BuildStore(string storeName, string storeBody, string storePara = null, string userDefineType = null, string userDefineTypeBody = null)
        {
            if (string.IsNullOrEmpty(storePara))
            {
                storePara = string.Empty;
            }
            string headerStoreTemplate = $"IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{storeName}]') AND type in (N'P', N'PC')) \n DROP PROCEDURE [dbo].[{storeName}]";
            string bodyStoreTemplate = $"CREATE PROCEDURE [dbo].[{storeName}] \n {storePara} \n AS \n BEGIN \n {storeBody} \n END";
            string userDefineTypeTemplate = string.Empty;
            string headerUserDefineTypeTemplate = string.Empty;
            if (!string.IsNullOrEmpty(userDefineType) && !string.IsNullOrEmpty(userDefineTypeBody))
            {
                headerUserDefineTypeTemplate = $"IF TYPE_ID(N'{userDefineType}') IS NOT NULL \n DROP TYPE {userDefineType}";
                userDefineTypeTemplate = $"CREATE TYPE [dbo].[{userDefineType}] AS TABLE({userDefineTypeBody})";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                Exec(headerStoreTemplate, bodyStoreTemplate, connection, headerUserDefineTypeTemplate, userDefineTypeTemplate); 
            }
        }

        private void Exec(string headerStoreTemplate, string bodyStoreTemplate, SqlConnection connection, string headerUserDefineTypeTemplate = null, string userDefineTypeTemplate = null)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                connection.Open();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrEmpty(userDefineTypeTemplate) && !string.IsNullOrEmpty(headerUserDefineTypeTemplate))
                {
                    cmd.CommandText = headerStoreTemplate;
                    cmd.ExecuteNonQuery();                   

                    cmd.CommandText = headerUserDefineTypeTemplate;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = userDefineTypeTemplate;
                    cmd.ExecuteNonQuery();
                }
                cmd.CommandText = headerStoreTemplate;
                cmd.ExecuteNonQuery();
                cmd.CommandText = bodyStoreTemplate;
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public static bool CheckExistDB(string connectionString)
        {
            bool isExist = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    isExist = true;
                }
                catch (SqlException)
                {
                    isExist = false;
                }
            }
            return isExist;
        }
    }
}
