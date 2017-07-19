using System;
using System.Collections.Generic;
using System.Text;
using DataManagement.Entities;
using DataManagement.Repository.Interfaces;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using static System.Data.CommandType;
using Euroland.NetCore.ToolsFramework.Data;

namespace DataManagement.Repository
{
    public class UserRepository: IUserRepository
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

        public bool AddUser(User user)
        {
            try
            {
                return DatabaseConnection.ExecNonQuery("spUserAddUser", new { @UserName = user.UserName, @UserMobile = user.UserMobile, @UserEmail = user.UserEmail, @FaceBookUrl  = user.FaceBookUrl, @LinkedInUrl = user.LinkedInUrl, @TwitterUrl  = user.TwitterUrl, @PersonalWebUrl = user.PersonalWebUrl}) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public bool AddUser(User user)
        //{
        //    try
        //    {
        //        DynamicParameters parameters = new DynamicParameters();
        //        parameters.Add("UserName", user.UserName);
        //        parameters.Add("@UserMobile", user.UserMobile);
        //        parameters.Add("@UserEmail", user.UserEmail);
        //        parameters.Add("@FaceBookUrl", user.FaceBookUrl);
        //        parameters.Add("@LinkedInUrl", user.LinkedInUrl);
        //        parameters.Add("@TwitterUrl", user.TwitterUrl);
        //        parameters.Add("@PersonalWebUrl", user.PersonalWebUrl);
        //        return SqlMapper.Execute(con, "spUserAddUser", parameters, null, null, commandType: System.Data.CommandType.StoredProcedure) > 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public bool DeleteUser(int userId)
        {
            return DatabaseConnection.ExecNonQuery("spUserDeleteUser", new { @UserId = userId }) > 0;
        }

        public IEnumerable<User> GetAllUser()
        {
            IEnumerable<User> userList = DatabaseConnection.Exec<User>("spUserGetAllUser");
            return userList;
        }

        public User GetUserById(int userId)
        {
            try
            {
                User user = DatabaseConnection.ExecSingle<User>("spUserSelectById", new { @UserId = userId });
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateUser(User user)
        {
            try
            {
                return DatabaseConnection.ExecNonQuery("spUserUpdateUser", 
                    new {
                            @UserId = user.UserId,
                            @UserName = user.UserName,
                            @UserMobile = user.UserMobile,
                            @UserEmail = user.UserEmail,
                            @FaceBookUrl = user.FaceBookUrl,
                            @LinkedInUrl = user.LinkedInUrl,
                            @TwitterUrl = user.TwitterUrl,
                            @PersonalWebUrl = user.PersonalWebUrl
                        }) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
