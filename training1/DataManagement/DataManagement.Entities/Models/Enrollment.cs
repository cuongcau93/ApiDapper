using System;
using System.Collections.Generic;
using System.Text;

namespace DataManagement.Entities.Models
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public int Grade { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        public Student Student { get; set; }
        public Course Course { get; set; }
    }
}
