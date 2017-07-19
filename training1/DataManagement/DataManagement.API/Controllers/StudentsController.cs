using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataManagement.Business.Interfaces;
using DataManagement.Entities.Models;

namespace DataManagement.API.Controllers
{
    [Route("api/[controller]")]
    public class StudentsController: Controller
    {
        public IStudentManager _studentManager;

        public StudentsController(IStudentManager studentManager)
        {
            this._studentManager = studentManager;
        }

        [HttpGet]
        public IEnumerable<Student> Get()
        {
            return _studentManager.GetAllUser();
        }
    }
}
