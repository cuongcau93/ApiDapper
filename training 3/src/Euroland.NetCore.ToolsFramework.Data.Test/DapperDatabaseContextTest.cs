using System.Collections.Generic;
using System.Linq;
using Xunit;
using Euroland.NetCore.ToolsFramework.Data.Test.Models;
using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections;

namespace Euroland.NetCore.ToolsFramework.Data.Test
{
    public class DapperDatabaseContextTest
    {
        string serverName = "(localdb)\\mssqllocaldb";
        string databaseName = "SchoolDB";
        /// <summary>
        /// option to login mssql server
        /// if you usage sql express then do not care about User and Password to login sql server
        /// </summary>
        string userID = "sa";
        string password = "123456sa";
        string connectionString => $"Server={serverName};Database={databaseName};User ID={userID};Password={password};Trusted_Connection=True;MultipleActiveResultSets=true";
        public SchoolContext context => new SchoolContext(connectionString);
        DapperDatabaseContext DatabaseConnection => new DapperDatabaseContext(connectionString);
        public DapperDatabaseContextTest()
        {
            DbInitializer.Initialize(context);

            //If Database had created then we begin create stores
            if (InitialStoreProcedure.CheckExistDB(connectionString))
            {
                InitialStoreProcedure initStore = new InitialStoreProcedure(connectionString);
                initStore.BuildStore("spStudentSelect", "SELECT ID, FirstMidName, LastName, EnrollmentDate FROM Student");
                initStore.BuildStore("spStudentSelectByID", "SELECT ID, FirstMidName, LastName, EnrollmentDate FROM Student WHERE ID = @ID", "@ID INT");
                initStore.BuildStore("spStudentSelectTotalStudent", "SELECT COUNT(ID) FROM Student");
                initStore.BuildStore("spStudentInsertOrUpdate", "DECLARE @StudentID INT \n IF @ID IS NULL OR @ID <= 0 \n BEGIN \n INSERT INTO Student(FirstMidName, LastName, EnrollmentDate) \n VALUES(@FirstName, @LastName, @EnrollmentDate) \n SET @StudentID = @@ROWCOUNT \n END \n ELSE \n BEGIN \n UPDATE Student SET FirstMidName = @FirstName, LastName = @LastName, EnrollmentDate = @EnrollmentDate \n WHERE ID = @ID \n SET @StudentID = @ID \n END \n SELECT @StudentID", "@ID INT = NULL, \n @FirstName NVARCHAR(50), \n  @LastName NVARCHAR(50), \n  @EnrollmentDate DATETIME");
                initStore.BuildStore("spStudentBulkInsert", "INSERT INTO Student (EnrollmentDate, FirstMidName, LastName) \n SELECT EnrollmentDate, FirstMidName, LastName FROM @Students", "@Students StudentUserDefinedTableType1 READONLY", "StudentUserDefinedTableType1", "ID INT NOT NULL, EnrollmentDate DATETIME, FirstMidName NVARCHAR(MAX),LastName NVARCHAR(MAX)");
                initStore.BuildStore("spStudentBulkSelect", "SELECT ID, EnrollmentDate, FirstMidName, LastName FROM @Students", "@Students StudentUserDefinedTableType2 READONLY", "StudentUserDefinedTableType2", "ID INT NOT NULL, EnrollmentDate DATETIME, FirstMidName NVARCHAR(MAX),LastName NVARCHAR(MAX)");
                initStore.BuildStore("spStudentSelectMultiResult", "SELECT * FROM Student SELECT * FROM Course SELECT * FROM Student WHERE ID = @id", "@id int");
                initStore.BuildStore("spStudentInserts", "INSERT INTO Student (FirstMidName, LastName, EnrollmentDate) VALUES (@FirstMidName, @LastName, @EnrollmentDate)", "@FirstMidName NVARCHAR(MAX),@LastName NVARCHAR(MAX),@EnrollmentDate DATETIME ");
                initStore.BuildStore("spStudentUpdate", "UPDATE Student SET FirstMidName = @FirstMidName WHERE ID = @ID", "@ID INT, @FirstMidName NVARCHAR(MAX)");
                initStore.BuildStore("spStudentDelete", "DELETE Student WHERE ID = @ID", "@ID INT");
                initStore.BuildStore("spStudentEnrollmentByStudentId", "SELECT Enrollment.StudentID, Student.LastName, Student.FirstMidName, Student.EnrollmentDate, Enrollment.EnrollmentID, Enrollment.CourseID, Enrollment.Grade FROM Student INNER JOIN Enrollment ON Student.ID = @ID AND Enrollment.StudentID = @ID;", "@ID INT");
                initStore.BuildStore("spCourseSelectMultipleResult", "SELECT Course.CourseID, Course.Title FROM Course \n JOIN Enrollment \n  on Enrollment.CourseID = Course.CourseID");
                initStore.BuildStore("spStudentSelectAllMultipleResult", "SELECT Student.ID, Student.FirstMidName From Student \n SELECT * FROM Enrollment, Student \n WHERE Enrollment.StudentID = Student.ID");
                initStore.BuildStore("spStCoSelectById", "SELECT * FROM Student WHERE ID = @StudentId \n SELECT * FROM Course WHERE Course.CourseID = @CourseId", "@StudentId INT, \n @CourseId INT");
                initStore.BuildStore("spStudentSelectByMultiId", "SELECT * FROM Student \n WHERE ID IN(@ID1, @ID2)", "@ID1 INT, \n @ID2 INT");
                initStore.BuildStore("spStudentSelectIdByID", "SELECT ID From Student WHERE ID = @ID", "@ID INT");
                initStore.BuildStore("spStudentSelectAll", "SELECT * From Student WHERE ID = @StudentID SELECT * From Enrollment WHERE StudentID = @StudentID", "@StudentID INT");

                
            }
            else
            {
                Console.WriteLine("Database must be created before create stored procedure.");
            }
        }

        [Fact]
        public void CanReturnListOfStudent()
        {
            IEnumerable<Student> students = DatabaseConnection.Exec<Student>("spStudentSelect");
            Assert.True(students.Count() > 0);
        }

        [Fact]
        public async Task CanReturnListOfStudentAsync()
        {
            string message = "Init";
            await Task.Run(async () =>
            {
                IEnumerable<Student> students = await DatabaseConnection.ExecAsync<Student>("spStudentSelect");
                await Task.Delay(2000);
                message += " number of student are: " + students.Count();
            });
            message += " Work";
            string messageResult = message;
            Assert.True(message.Contains("number of student are:"));
        }

        [Fact]
        public async void CanReturnListOfStudentAwait()
        {
            IEnumerable<Student> students = await DatabaseConnection.ExecAsync<Student>("spStudentSelect");
            Assert.True(students.Count() > 0);
        }

        [Fact]
        public void ThrowExceptionWithWrongStoreName()
        {
            Assert.Throws<System.Data.SqlClient.SqlException>(() => DatabaseConnection.Exec<Student>("spStudentSelect1"));
        }

        [Fact]
        public void ThrowExceptionWithoutStorename()
        {
            Assert.Throws<ArgumentException>(() => DatabaseConnection.Exec<Student>(""));
        }

        [Fact]
        public void CanReturnObject()
        {
            Student student = DatabaseConnection.ExecSingle<Student>("spStudentSelectByID", new { ID = 1 });
            Assert.True(student != null);
        }

        [Fact]
        public void ThrowExceptionTooManyArgument()
        {
            Assert.Throws<System.Data.SqlClient.SqlException>(() => DatabaseConnection.ExecSingle<Student>("spStudentSelectByID", new { ID = 1, LastName = "Nam" }));
        }

        [Fact]
        public void ThrowExceptionWithStoreHasWhiteSpace()
        {
            Assert.Throws<ArgumentException>(() => DatabaseConnection.ExecSingle<Student>("spStudentSelectByID ", new { ID = 1 }));
        }

        [Fact]
        public void CanExecuteTableValueParameters()
        {
            List<Student> students = new List<Student>();
            students.Add(new Student()
            {
                ID = 0,
                EnrollmentDate = DateTime.Now,
                FirstMidName = "Test11",
                LastName = "Test11"
            });
            students.Add(new Student()
            {
                ID = 0,
                EnrollmentDate = DateTime.Now,
                FirstMidName = "Test22",
                LastName = "Test22"
            });
            students.Add(new Student()
            {
                ID = 0,
                EnrollmentDate = DateTime.Now,
                FirstMidName = "Test33",
                LastName = "Test33"
            });
            students.Add(new Student()
            {
                ID = 0,
                EnrollmentDate = DateTime.Now,
                FirstMidName = "Test44",
                LastName = "Test44"
            });
            students.Add(new Student()
            {
                ID = 0,
                EnrollmentDate = DateTime.Now,
                FirstMidName = "Test55",
                LastName = "Test55"
            });

            var studentUserDefineTable = students.ToTVP("Students", "dbo.StudentUserDefinedTableType2");
            var rowEffected = DatabaseConnection.Exec<Student>("spStudentBulkSelect", studentUserDefineTable);
            Assert.True(rowEffected.Count() > 0);
        }

        [Fact]
        public void CanBulkInsertWithTableValueParameters()
        {
            List<Student> students = new List<Student>();
            students.Add(new Student()
            {
                ID = 0,
                EnrollmentDate = DateTime.Now,
                FirstMidName = "Test11",
                LastName = "Test11"
            });
            students.Add(new Student()
            {
                ID = 0,
                EnrollmentDate = DateTime.Now,
                FirstMidName = "Test22",
                LastName = "Test22"
            });
            students.Add(new Student()
            {
                ID = 0,
                EnrollmentDate = DateTime.Now,
                FirstMidName = "Test33",
                LastName = "Test33"
            });
            students.Add(new Student()
            {
                ID = 0,
                EnrollmentDate = DateTime.Now,
                FirstMidName = "Test44",
                LastName = "Test44"
            });
            students.Add(new Student()
            {
                ID = 0,
                EnrollmentDate = DateTime.Now,
                FirstMidName = "Test55",
                LastName = "Test55"
            });

            var studentUserDefineTable = students.ToTVP("Students", "dbo.StudentUserDefinedTableType1");
            var rowEffected = DatabaseConnection.ExecNonQuery("spStudentBulkInsert", studentUserDefineTable);
            Assert.True(rowEffected > 0);
        }

        [Fact]
        public void CanReturnSingleValue()
        {
            int total = DatabaseConnection.ExecSingle<int>("spStudentSelectTotalStudent");
            Assert.True(total > 0);
        }

        [Fact]
        public async void CanReturnSingleValueAsync()
        {
            var total = await Task.Run(() =>
                DatabaseConnection.ExecSingleAsync<int>("spStudentSelectTotalStudent"));
            Assert.True(total > 0);
        }

        [Fact]
        public async void CanReturnSingleObjectWithAsync()
        {
            Student student = await DatabaseConnection.ExecSingleAsync<Student>("spStudentSelectByID", new { ID = 1 });
            Assert.True(student != null);
        }

        [Fact]
        public async void CanReturnSingleObjectWithExecNonAsync()
        {
            int total = await DatabaseConnection.ExecNonQueryAsync("spStudentInsertOrUpdate", new { ID = 9, FirstName = "Amaw", LastName = "Francis", EnrollmentDate = DateTime.Now });
            Assert.True(total > 0);
        }

        [Fact]
        public async void CanReturnIntWithExecNonQueryAsync()
        {
            int students = 0;
            await Task.Run(async () =>
            {
                students = await DatabaseConnection.ExecNonQueryAsync("spStudentExecNonQueryInsert", new { FirstMidName = "Cuong", LastName = "Nguyen", EnrollmentDate = DateTime.Now });
                await Task.Delay(2000);
            });
            students = students + 1;
            Assert.True(students > 0);
        }

        [Fact]
        public void CanReturnIntWithExecNonQueryInsert()
        {
            int student = DatabaseConnection.ExecNonQuery("spStudentInserts", new { FirstMidName = "Cuong", LastName = "Nguyen", EnrollmentDate = DateTime.Now });
            Assert.True(student > 0);
        }

        [Fact]
        public void CanReturnIntWithExecNonQueryUpdate()
        {
            int student = DatabaseConnection.ExecNonQuery("spStudentUpdate", new { @ID = 1, @FirstMidName = "Nguyen Manh Cuong" });
            Assert.True(student > 0);
        }

        [Fact]
        public void CanReturnIntWithExecNonQueryDelete()
        {
            int student = DatabaseConnection.ExecNonQuery("spStudentDelete", new { @ID = 12 });
            Assert.True(student >= 0);
        }

        [Fact]
        public void CanReturnIntWithExecNonQueryDeletes()
        {
            int student = DatabaseConnection.ExecNonQuery("spStudentDelete", new[]
            {
                new { @ID = 20 },
                new { @ID = 21 },
                new { @ID = 22 }
            });
            Assert.True(student >= 0);
        }

        [Fact]
        public void CanReturnIntWithExecNonQueryDeleteViaWrongId()
        {
            int student = DatabaseConnection.ExecNonQuery("spStudentDelete", new { @ID = 1008 });
            Assert.True(student == 0);
        }

        [Fact]
        public void ThrowExceptionExecNonQueryDeleteViaWrongType()
        {
            Assert.Throws<System.Data.SqlClient.SqlException>(() => DatabaseConnection.ExecNonQuery("spStudentDelete", new { @ID = "Error" }));
        }

        [Fact]
        public void ThrowExceptionExecNonQueryWrongStoreName()
        {
            Assert.Throws<System.Data.SqlClient.SqlException>(() => DatabaseConnection.ExecNonQuery("spStudentInsertsl", new { LastName = "Nguyen", FirstMidName = "Cuong", EnrollmentDate = DateTime.Now }));
        }

        [Fact]
        public void ThrowExceptionExecNonQueryParamNull()
        {
            Assert.Throws<System.Data.SqlClient.SqlException>(() => DatabaseConnection.ExecNonQuery("spStudentInserts", new { }));
        }

        [Fact]
        public void ThrowExceptionTooManyArgumentWithExecNonQuery()
        {
            Assert.Throws<System.Data.SqlClient.SqlException>(() => DatabaseConnection.ExecNonQuery("spStudentDelete", new { @ID = 100, FirstMidName = "sss" }));
        }

        [Fact]
        public void ThrowExceptionWithExecNonQueryStoreHasWhiteSpace()
        {
            Assert.Throws<System.ArgumentException>(() => DatabaseConnection.ExecNonQuery("spStudentInserts ", new { ID = 1 }));
        }

        [Fact]
        public async void CanReturnIntWithExecNonQueryAsyncInsert()
        {
            int student = await DatabaseConnection.ExecNonQueryAsync("spStudentInserts", new { FirstMidName = "CuongExecNonQueryAsync", LastName = "Nguyen", EnrollmentDate = DateTime.Now });
            Assert.True(student > 0);
        }

        [Fact]
        public async void CanReturnIntWithExecNonQueryAsyncUpdate()
        {
            int student = await DatabaseConnection.ExecNonQueryAsync("spStudentUpdate", new { @ID = 1, @FirstMidName = "Nguyen Manh Cuong" });
            Assert.True(student > 0);
        }

        [Fact]
        public async void CanReturnIntWithExecNonQueryAsyncUpdateViaWrongId()
        {
            int student = await DatabaseConnection.ExecNonQueryAsync("spStudentUpdate", new { @ID = 1111, @FirstMidName = "Nguyen Manh Cuong" });
            Assert.True(student == 0);
        }

        [Fact]
        public async void CanReturnIntWithExecNonQueryAsyncDelete()
        {
            int student = await DatabaseConnection.ExecNonQueryAsync("spStudentDelete", new { @ID = 14 });
            Assert.True(student >= 0);
        }

        [Fact]
        public async void CanReturnIntWithExecNonQueryAsyncDeleteViaWrongId()
        {
            int student = await DatabaseConnection.ExecNonQueryAsync("spStudentDelete", new { @ID = 10000 });
            Assert.True(student == 0);
        }
        
        [Fact]
        public async void CanReturnWithObjectExecSingleAsyncById()
        {
            Student student = await DatabaseConnection.ExecSingleAsync<Student>("spStudentSelectByID", new { ID = 1 });
            Assert.True(student != null);
        }

        [Fact]
        public async void CanReturnWithPrimitiveExecSingleAsyncById()
        {
            int student = await DatabaseConnection.ExecSingleAsync<int>("spStudentSelectIdByID", new { ID = 10 });
            Assert.True(student >= 0);
        }

        [Fact]
        public async void CanReturnObjectWithExecSingleAsyncByWrongId()
        {
            Student student = await DatabaseConnection.ExecSingleAsync<Student>("spStudentSelectByID", new { ID = 10000 });
            Assert.True(student == null);
        }

        [Fact]
        public void CanReturnObjectWithExecSingle()
        {
            Student student = DatabaseConnection.ExecSingle<Student>("spStudentSelectByID", new { ID = 1 });
            Assert.True(student != null);
        }

        [Fact]
        public void CanReturnWithObjectMultipleResultStudentAll()
        {
            using (var multipleResult = DatabaseConnection.QueryMultipleResult("spStudentSelectAll", new { @StudentID = 1 }))
            {
                Student students = multipleResult.GetSingle<Student>();
                List<Enrollment> enrollment = multipleResult.Get<Enrollment>().ToList();
                students.Enrollments = enrollment;
                Assert.NotNull(students);
            }
        }

        [Fact]
        public void CanReturnWithObjectExecSingleEnrollmentStudent()
        {
            IEnumerable<EnrollmentStudent> enrollmentStudent = DatabaseConnection.Exec<EnrollmentStudent>("spStudentEnrollmentByStudentId", new { ID = 1 });
            Assert.True(enrollmentStudent != null);
        }


        //[Fact]
        //public void CanReturnQueryMultipleResultByIdOK()
        //{
        //    try
        //    {
        //        using (var multipleResult = DatabaseConnection.QueryMultipleResult("spStudentSelectMultiResult", new { id = 1 }))
        //        {
        //            IEnumerable<Student> students = multipleResult.Get<Student>();
        //            List<Course> courses = multipleResult.Get<Course>().ToList();
        //            Student student = multipleResult.GetSingle<Student>();
        //            Assert.NotNull(students);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ArgumentException(ex.Message);
        //    }
        //}
        [Fact]
        public void QueryMultipleResult_PassMoreParamThanIntoStore()
        {
            Assert.Throws<SqlException>(() => DatabaseConnection.QueryMultipleResult("spStudentSelectMultiResult", new { id = 1, id2 = 2 }));
        }
        [Fact]
        public void QueryMultipleResult_PassWrongNameOfParam()
        {
            Assert.Throws<SqlException>(() => DatabaseConnection.QueryMultipleResult("spStudentSelectMultiResult", new { Idd = 1 }));
        }
        [Fact]
        public void QueryMultipleResult_PassNavcharToNumber()
        {
            Assert.Throws<SqlException>(() => DatabaseConnection.QueryMultipleResult("spStudentSelectMultiResult", new { id = "abc" }));
        }
        [Fact]
        public void QueryMultipleResult_PassNameStoreNotMatch()
        {
            Assert.Throws<SqlException>(() => DatabaseConnection.QueryMultipleResult("ErrorNameStore"));
        }
        //[Fact]
        //public void ReturnQueryMultipleResultJoinOK()
        //{
        //    bool flag = false;
        //    try
        //    {
        //        using (var multiTest = DatabaseConnection.QueryMultipleResult("spCourseSelectMultipleResult"))
        //        {
        //            IEnumerable<Course> CourseEnrollment = multiTest.Get<Course>();
        //            Assert.NotNull(CourseEnrollment);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.True(flag);
        //        throw new ArgumentException(ex.Message);
        //    }
        //}
        //[Fact]
        //public void ReturnQueryMultipleResultAllOK()
        //{
        //    try
        //    {
        //        using (var multiTest2 = DatabaseConnection.QueryMultipleResult("spStudentSelectAllMultipleResult"))
        //        {
        //            List<Student> st = multiTest2.Get<Student>().ToList();
        //            IEnumerable<Enrollment> stEnrollments = multiTest2.Get<Enrollment>().ToList();
        //            st.ForEach(x => x.Enrollments = stEnrollments.Where(n => n.StudentID == x.ID).ToList());
        //            Assert.NotNull(st);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ArgumentException(ex.Message);
        //    }
        //}
        //[Fact]
        //public void ReturnQueryMultipleResult2ObjectOK()
        //{
        //    try
        //    {
        //        using (var multiTest2 = DatabaseConnection.QueryMultipleResult("spStCoSelectById", new { StudentId = 1, CourseId = 1045 }))
        //        {
        //            Student st = multiTest2.GetSingle<Student>();
        //            Course co = multiTest2.GetSingle<Course>();
        //            Assert.NotNull(st);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ArgumentException(ex.Message);
        //    }
        //}
        //[Fact]
        //public void ReturnQueryMultipleResultByMultiIdOK()
        //{
        //    try
        //    {
        //        using (var multiTest2 = DatabaseConnection.QueryMultipleResult("spStudentSelectByMultiId", new { ID1 = 1, ID2 = 2 }))
        //        {
        //            IEnumerable<Student> sts = multiTest2.Get<Student>();
        //            Assert.NotNull(sts);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ArgumentException(ex.Message);
        //    }
        //}
        [Fact]
        public async void QueryMultipleResultAsync_PassOutDataRange()
        {
            await Task.Run(async () =>
            {
                await Assert.ThrowsAsync<SqlException>(async () => await DatabaseConnection.QueryMultipleResultAsync("spStudentSelectMultiResult", new { id = 19999999999 }));
            });
        }
        [Fact]
        public async void QueryMultipleResultAsync_PassMoreParamThanIntoStore()
        {
            await Task.Run(async () =>
            {
                await Assert.ThrowsAsync<SqlException>(async () => await DatabaseConnection.QueryMultipleResultAsync("spStudentSelectMultiResult", new { id = 1.5, id2 = 2 }));
            });
        }
        [Fact]
        public async void ReturnQueryMultipleResultAsync_PassWrongNameOfParam()
        {

            await Task.Run(async () =>
            {
                await Assert.ThrowsAsync<SqlException>(async () => await DatabaseConnection.QueryMultipleResultAsync("spStudentSelectMultiResult", new { Idd = 1 }));
            });
        }
        [Fact]
        public async void QueryMultipleResultAsync_PassNavcharToNumber()
        {
            await Task.Run(async () =>
            {
                await Assert.ThrowsAsync<SqlException>(async () => await DatabaseConnection.QueryMultipleResultAsync("spStudentSelectMultiResult", new { Id = "abc" }));
            });
        }
        [Fact]
        public async void QueryMultipleResultAsync_PassNameStoreNotMatch()
        {
            await Task.Run(async () =>
            {
                await Assert.ThrowsAsync<SqlException>(async () => await DatabaseConnection.QueryMultipleResultAsync("ErrorNameStore"));
            });
        }
        //[Fact]
        //public async void ReturnQueryMultipleResultAsynctJoinOK()
        //{
        //    //try
        //    //{
        //    //    await Task.Run(async () =>
        //    //    {
        //    //        using (var multipleResult = await DatabaseConnection.QueryMultipleResultAsync("spCourseSelectMultipleResult"))
        //    //        {
        //    //            IEnumerable<Course> CourseEnrollment = multipleResult.Get<Course>();
        //    //            Assert.Null(CourseEnrollment);
        //    //        }
        //    //    });
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw new ArgumentException(ex.Message);
        //    //}
        //    await Task.Run(async () =>
        //    {
        //        await Assert.ThrowsAsync<System.ArgumentException>(async () => await DatabaseConnection.QueryMultipleResultAsync("spCourseSelectMultipleResult"));
        //    });
        //}
        //[Fact]
        //public async void ReturnQueryMultipleResultAsynctAllOK()
        //{
        //    //try
        //    //{
        //    //    await Task.Run(async () =>
        //    //    {
        //    //        using (var multipleResult = await DatabaseConnection.QueryMultipleResultAsync("spStudentSelectAllMultipleResult"))
        //    //        {
        //    //            List<Student> st = multipleResult.Get<Student>().ToList();
        //    //            IEnumerable<Enrollment> stEnrollments = multipleResult.Get<Enrollment>().ToList();
        //    //            st.ForEach(x => x.Enrollments = stEnrollments.Where(n => n.StudentID == x.ID).ToList());
        //    //            Assert.NotNull(st);
        //    //        }
        //    //    });
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw new ArgumentException(ex.Message);
        //    //}
        //    await Task.Run(async () =>
        //    {
        //        await Assert.ThrowsAsync<System.ArgumentException>(async () => await DatabaseConnection.QueryMultipleResultAsync("spStudentSelectAllMultipleResult"));
        //    });
        //}
        //[Fact]
        //public async void ReturnQueryMultipleResultAsync2Object2ModelOK()
        //{
        //    //try
        //    //{
        //    //    await Task.Run(async () =>
        //    //    {
        //    //        using (var multiTest = await DatabaseConnection.QueryMultipleResultAsync("spStCoSelectById", new { StudentId = 1, CourseId = 1045 }))
        //    //        {
        //    //            Student st = multiTest.GetSingle<Student>();
        //    //            Course co = multiTest.GetSingle<Course>();
        //    //            Assert.NotNull(st);
        //    //        }
        //    //    });
        //    //}
        //    //catch (Exception ex)
        //    //{

        //    //    throw new ArgumentException(ex.Message);
        //    //}
        //    await Task.Run(async () =>
        //    {
        //        await Assert.ThrowsAsync<System.ArgumentException>(async () => await DatabaseConnection.QueryMultipleResultAsync("spStCoSelectById", new { StudentId = 1, CourseId = 1045 }));
        //    });
        //}
        //[Fact]
        //public async void ReturnQueryMultipleResultAsyncByMultiIdOK()
        //{
        //    //try
        //    //{
        //    //    await Task.Run(async () =>
        //    //    {
        //    //        using (var multiTest2 = await DatabaseConnection.QueryMultipleResultAsync("spStudentSelectByMultiId", new { ID1 = 1, ID2 = 2 }))
        //    //        {
        //    //            IEnumerable<Student> sts = multiTest2.Get<Student>();
        //    //            Assert.NotNull(sts);
        //    //        }
        //    //    });
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw new ArgumentException(ex.Message);
        //    //}
        //    await Task.Run(async () =>
        //    {
        //        await Assert.ThrowsAsync<System.ArgumentException>(async () => await DatabaseConnection.QueryMultipleResultAsync("spStudentSelectByMultiId", new { ID1 = 1, ID2 = 2 }));
        //    });
        //}
    }
}

