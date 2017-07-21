using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Data.Test.Models
{
    public class Student
    {
        public int ID { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public string FirstMidName { get; set; }
        public string LastName { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
