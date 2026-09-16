using Microsoft.EntityFrameworkCore;
using StudentRegister.Api.Data;
using StudentRegister.Api.Models;

namespace StudentRegister.Api.Services
{
    public class StudentService : IStudentService
    {
        private readonly object _syncRoot = new();

        private readonly StudentRegisterContext _context;

        public StudentService(StudentRegisterContext context)
        {
            _context = context;
        }

        public Task<List<Student>> GetStudentsAsync()
        {
            return _context.Students.OrderBy(student => student.Id).ToListAsync();
        }

        public Task<List<Student>> SearchStudentsAsync(string search)
        {
            return _context.Students.AsTracking()
                   .Where(student => student.StudentNumber.Contains(search) ||
                                     student.FirstName.Contains(search) ||
                                     student.LastName.Contains(search))
                   .OrderBy(student => student.Id)
                   .ToListAsync();
        }

        public Task<Student?> GetStudentByIdAsync(int id)
        {

            return _context.Students
                 .AsNoTracking()
                 .FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            student.Id = 0;
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Student?> UpdateStudentAsync(int id, Student student)
        {

            var current = await _context.Students.FirstOrDefaultAsync(existingStudent => existingStudent.Id == id);
            if (current == null)
            {
                return null;
            }
            current.StudentNumber = student.StudentNumber;
            current.FirstName = student.FirstName;
            current.LastName = student.LastName;
            current.Gender = student.Gender;
            await _context.SaveChangesAsync();
            return student;

        }

        public async Task<bool> DeleteStudentAsync(int id)
        {

            var current = await _context.Students.FirstOrDefaultAsync(existingStudent => existingStudent.Id == id);
            if (current == null)
            {
                return false;
            }

            _context.Remove(current);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

