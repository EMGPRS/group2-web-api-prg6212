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
    }
}
