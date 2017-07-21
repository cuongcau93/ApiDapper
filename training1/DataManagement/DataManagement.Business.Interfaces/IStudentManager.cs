using DataManagement.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataManagement.Business.Interfaces
{
    public interface IStudentManager
    {
        IEnumerable<Student> GetAllUser();
        Student GetStudentById(int studentId);
    }
}
