using System;
using System.Collections.Generic;
using System.Text;

namespace Euroland.NetCore.ToolsFramework.Data.Test.Models
{
    class EnrollmentStudent
    {
        public int StudentID { get; set; }

        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }
        public Grade? Grade { get; set; }
    }
}
