using System;
using System.Collections.Generic;
using System.Text;
using DataManagement.Entities.Models;


namespace DataManagement.Repository.Interfaces
{
    public interface IStudentRepository
    {
        IEnumerable<Student> GetAllUser();
        Student GetStudentById(int studentId);

    }
}
