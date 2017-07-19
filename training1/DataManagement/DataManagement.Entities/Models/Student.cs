using System;
using System.Collections.Generic;
using System.Text;

namespace DataManagement.Entities.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
