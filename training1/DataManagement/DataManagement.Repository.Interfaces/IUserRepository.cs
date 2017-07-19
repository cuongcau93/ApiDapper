using System;
using DataManagement.Repository;
using DataManagement.Entities;
using System.Collections.Generic;

namespace DataManagement.Repository.Interfaces
{
    public interface IUserRepository
    {
        bool AddUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(int userId);
        IEnumerable<User> GetAllUser();
        User GetUserById(int userId);
    }
}
