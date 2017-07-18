using System;
using System.Collections.Generic;
using System.Text;
using DataManagement.Entities;
using DataManagement.Repository.Interfaces;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using static System.Data.CommandType;

namespace DataManagement.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public bool AddUser(User user)
        {
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("UserName", user.UserName);
                parameters.Add("@UserMobile", user.UserMobile);
                parameters.Add("@UserEmail", user.UserEmail);
                parameters.Add("@FaceBookUrl", user.FaceBookUrl);
                parameters.Add("@LinkedInUrl", user.LinkedInUrl);
                parameters.Add("@TwitterUrl", user.TwitterUrl);
                parameters.Add("@PersonalWebUrl", user.PersonalWebUrl);
                return SqlMapper.Execute(con, "spUserAddUser", parameters, null, null, commandType: System.Data.CommandType.StoredProcedure) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteUser(int userId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            return SqlMapper.Execute(con, "spUserDeleteUser", parameters, null, null, commandType: StoredProcedure) > 0;
            
        }

        public IList<User> GetAllUser()
        {
            IList<User> userList = SqlMapper.Query<User>(con, "spUserGetAllUser", StoredProcedure).ToList();
            return userList;
        }

        public User GetUserById(int userId)
        {
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);
                User user = SqlMapper.Query<User>(con, "spUserSelectById", parameters, null, true, null, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
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
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserId", user.UserId);
                parameters.Add("@UserName", user.UserName);
                parameters.Add("@UserMobile", user.UserMobile);
                parameters.Add("@UserEmail", user.UserEmail);
                parameters.Add("@FaceBookUrl", user.FaceBookUrl);
                parameters.Add("@LinkedInUrl", user.LinkedInUrl);
                parameters.Add("@TwitterUrl", user.TwitterUrl);
                parameters.Add("@PersonalWebUrl", user.PersonalWebUrl);
                return SqlMapper.Execute(con, "spUserUpdateUser", parameters, null, null, commandType: StoredProcedure) > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
