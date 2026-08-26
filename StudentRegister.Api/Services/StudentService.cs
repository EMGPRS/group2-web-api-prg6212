using StudentRegister.Api.Models;

namespace StudentRegister.Api.Services
{
    public class StudentService
    {
        public List<Student> GetStudents()
        {
            return new List<Student>
            {
                new Student
                {
                    Id = 1,
                    FirstName = "Thabo",
                    LastName ="Mokoena" ,
                    StudentNumber = "ST001",
                    Gender = "M"
                },
                new Student
                {
                    Id = 2,
                    FirstName = "Naledi",
                    LastName ="Dlamini" ,
                    StudentNumber = "ST002",
                    Gender = "F"
                },
                new Student
                {
                    Id = 3,
                    FirstName = "Sipho",
                    LastName ="Ndlovu" ,
                    StudentNumber = "ST003",
                    Gender = "M"
                },
                new Student
                {
                    Id = 4,
                    FirstName = "Tracy",
                    LastName ="Jones" ,
                    StudentNumber = "ST004",
                    Gender = "F"
                },
            };
        }
    }
}
