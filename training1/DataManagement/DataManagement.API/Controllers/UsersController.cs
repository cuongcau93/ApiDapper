using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataManagement.Business.Interfaces;
using DataManagement.Entities;

namespace DataManagement.API.Controllers
{

    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        public IUserManager _userManager;

        public UsersController(IUserManager userManager)
        {
            this._userManager = userManager;
        }

        [HttpGet]
        public IList<User> Get()
        {
            return _userManager.GetAllUser();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public User Get(int id)
        {
            return _userManager.GetUserById(id);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] User user)
        {
            _userManager.AddUser(user);
        }

        //// PUT api/values/5
        [HttpPut("{id}")]
        public bool Put(int id, [FromBody]User user)
        {
            return _userManager.UpdateUser(user);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public bool Delete(int id)
        {
            return _userManager.DeleteUser(id);
        }
    }
}
