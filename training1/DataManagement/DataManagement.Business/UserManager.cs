using DataManagement.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using DataManagement.Entities;
using DataManagement.Repository;
using DataManagement.Repository.Interfaces;
namespace DataManagement.Business
{
    public class UserManager : IUserManager
    {
        IUserRepository _userRepository;

        public UserManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool AddUser(User user)
        {
            return _userRepository.AddUser(user);
        }

        public bool DeleteUser(int userId)
        {
            return _userRepository.DeleteUser(userId);
        }

        public IEnumerable<User> GetAllUser()
        {
            IEnumerable<User> userList = _userRepository.GetAllUser();
            userList = null;
            if(userList == null)
            {
                return null;
            }
            else
            {
                return userList;
            }
        }

        public User GetUserById(int userId)
        {
            return _userRepository.GetUserById(userId);
        }

        public bool UpdateUser(User user)
        {
            return _userRepository.UpdateUser(user);
        }
    }
}
