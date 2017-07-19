using System;
using System.Collections.Generic;
using System.Text;
using DataManagement.Business.Interfaces;
using DataManagement.Entities.Models;
using DataManagement.Repository.Interfaces;

namespace DataManagement.Business
{
    public class StudentManager : IStudentManager
    {
        IStudentRepository _studentRepository;

        public StudentManager(IStudentRepository studentRepository) 
        {
            _studentRepository = studentRepository;
        }

        public IEnumerable<Student> GetAllUser()
        {
            return _studentRepository.GetAllUser();
        }
    }
}
