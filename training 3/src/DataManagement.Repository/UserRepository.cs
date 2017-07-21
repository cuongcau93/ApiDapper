using DataManagement.Entities;
using DataManagement.Repository.Interfaces;
using System;
using System.Collections.Generic;

namespace DataManagement.Repository
{
    public class UserRepository: BaseRepository, IUserRepository
    {
        //private readonly Microsoft.Extensions.Configuration.IConfigurationRoot _configuration;
        //private readonly IDatabaseContext DbContext;

        public bool AddUser(User user)
        {
            try
            {
                return DatabaseConnection.ExecNonQuery("spUserAddUser", new { @UserName = user.UserName, @UserMobile = user.UserMobile, @UserEmail = user.UserEmail, @FaceBookUrl = user.FaceBookUrl, @LinkedInUrl = user.LinkedInUrl, @TwitterUrl = user.TwitterUrl, @PersonalWebUrl = user.PersonalWebUrl }) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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
                    new
                    {
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
