using System;
using System.Collections.Generic;
using System.Text;
using DataManagement.Entities.Models;
using DataManagement.Repository.Interfaces;
using Euroland.NetCore.ToolsFramework.Data;

namespace DataManagement.Repository
{
    public class StudentRepository : IStudentRepository
    {
        string serverName = "123-HP";
        string databaseName = "DataManagement";
        /// <summary>
        /// option to login mssql server
        /// if you usage sql express then do not care about User and Password to login sql server
        /// </summary>
        string userID = "sa";
        string password = "123456sa";
        string connectionString => $"Server={serverName};Database={databaseName};User ID={userID};Password={password};Trusted_Connection=True;MultipleActiveResultSets=true";

        DapperDatabaseContext DatabaseConnection => new DapperDatabaseContext(connectionString);

        public IEnumerable<Student> GetAllUser()
        {
            IEnumerable<Student> studentList = DatabaseConnection.Exec<Student>("spStudentGetAllStudent");
            return studentList;
        }
    }
}
