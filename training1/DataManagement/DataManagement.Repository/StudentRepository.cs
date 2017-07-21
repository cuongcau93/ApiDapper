using System;
using System.Collections.Generic;
using System.Text;
using DataManagement.Entities.Models;
using DataManagement.Repository.Interfaces;
using Euroland.NetCore.ToolsFramework.Data;
using System.Linq;

namespace DataManagement.Repository
{
    public class StudentRepository : BaseRepository, IStudentRepository
    {
       
        public IEnumerable<Student> GetAllUser()
        {
            IEnumerable<Student> studentList = DatabaseConnection.Exec<Student>("spStudentGetAllStudent");
            return studentList;
        }

        public Student GetStudentById(int studentId)
        {
            using (var multipleResult = DatabaseConnection.QueryMultipleResult("spStudentSelectAll", new { @StudentID = 1 }))
            {
                Student students = multipleResult.GetSingle<Student>();
                List<Enrollment> enrollment = multipleResult.Get<Enrollment>().ToList();
                students.Enrollments = enrollment;
                return students;
            }
        }
    }
}
