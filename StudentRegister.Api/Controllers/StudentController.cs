using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentRegister.Api.Models;
using StudentRegister.Api.Services;

namespace StudentRegister.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentService _studentService;
        public StudentController(StudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public ActionResult<List<Student>> GetStudents()
        {
            var students = _studentService.GetStudents();
            return Ok(students);
        }

        [HttpGet("search")]
        public ActionResult<List<Student>> SearchStudents(string search)
        {
            var students = _studentService.GetStudents();
            var results = students.Where(x => x.StudentNumber.Contains(search) ||
                                    x.FirstName.Contains(search) ||
                                    x.LastName.Contains(search)).ToList();
            if (results.Any())
            {
                return Ok(results);
            }
            else
            {
                return NotFound($"No records matching {search}");
            }
        }

        [HttpPost]
        public ActionResult<Student> CreateStudent(Student student)
        {
            var students = _studentService.GetStudents();
            student.Id = students.Max(x => x.Id) + 1;
            students.Add(student);
            return Ok(student);
        }

        [HttpGet("{id}")]
        public ActionResult<Student> GetStudentById(int id)
        {
            var students = _studentService.GetStudents();
            var student = students.FirstOrDefault(x => x.Id == id);
            if (student == null)
                return NotFound("Student details not found");
            return Ok(student);
        }

        [HttpPut("{id}")]
        public ActionResult<Student> UpdateStudent(int id, Student student)
        {
            var students = _studentService.GetStudents();
            var current = students.FirstOrDefault(x => x.Id == id);
            if (current == null)
                return NotFound("Student details not found");
            current.StudentNumber = student.StudentNumber;
            current.FirstName = student.FirstName;
            current.LastName = student.LastName;
            current.Gender = student.Gender;
            return Ok(current);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteStudent(int id)
        {
            var students = _studentService.GetStudents();
            var student = students.FirstOrDefault(x => x.Id == id);
            if (student == null)
                return NotFound("Student details not found");
            else
                students.Remove(student);
            return Accepted();
        }
    }
}
