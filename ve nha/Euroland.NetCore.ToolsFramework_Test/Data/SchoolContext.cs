using Euroland.NetCore.ToolsFramework.Data.Test.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace Euroland.NetCore.ToolsFramework.Data.Test
{
    public class SchoolContext : DbContext
    {
        private string connectionString;
        public SchoolContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");
        }
    }
}
