using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentRegister.Api.Authorization;
using StudentRegister.Api.Models;
using StudentRegister.Api.Services;

namespace StudentRegister.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Viewer}")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Student>>> GetStudents()
        {
            var students = await _studentService.GetStudentsAsync();
            return Ok(students);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Student>>> SearchStudents(string search)
        {
            var results = await _studentService.SearchStudentsAsync(search);
            if (results.Any())
            {
                return Ok(results);
            }

            return NotFound($"No records matching {search}");
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<ActionResult<Student>> CreateStudent(Student student)
        {
            var created = await _studentService.CreateStudentAsync(student);
            return CreatedAtAction(nameof(GetStudentById), new { id = created.Id }, created);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudentById(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound("Student details not found");
            }

            return Ok(student);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<ActionResult<Student>> UpdateStudent(int id, Student student)
        {
            var current = await _studentService.UpdateStudentAsync(id, student);
            if (current == null)
            {
                return NotFound("Student details not found");
            }

            return Ok(current);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            var deleted = await _studentService.DeleteStudentAsync(id);
            if (!deleted)
            {
                return NotFound("Student details not found");
            }

            return Accepted();
        }
    }
}
