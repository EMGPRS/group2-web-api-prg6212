using StudentRegister.Api.Models;

namespace StudentRegister.Api.Services
{
    public class StudentService : IStudentService
    {
        private readonly object _syncRoot = new();
        private readonly List<Student> _students =
        [
            new() { Id = 1, FirstName = "Thabo", LastName = "Mokoena", StudentNumber = "ST001", Gender = "M" },
            new() { Id = 2, FirstName = "Naledi", LastName = "Dlamini", StudentNumber = "ST002", Gender = "F" },
            new() { Id = 3, FirstName = "Sipho", LastName = "Ndlovu", StudentNumber = "ST003", Gender = "M" },
            new() { Id = 4, FirstName = "Tracy", LastName = "Jones", StudentNumber = "ST004", Gender = "F" }
        ];
        private int _nextId = 5;

        public Task<List<Student>> GetStudentsAsync()
        {
            lock (_syncRoot)
            {
                return Task.FromResult(_students.OrderBy(student => student.Id).ToList());
            }
        }

        public Task<List<Student>> SearchStudentsAsync(string search)
        {
            lock (_syncRoot)
            {
                if (string.IsNullOrWhiteSpace(search))
                {
                    return Task.FromResult(_students.OrderBy(student => student.Id).ToList());
                }

                var students = _students
                    .Where(student => student.StudentNumber.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                      student.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                      student.LastName.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(student => student.Id)
                    .ToList();

                return Task.FromResult(students);
            }
        }

        public Task<Student?> GetStudentByIdAsync(int id)
        {
            lock (_syncRoot)
            {
                return Task.FromResult(_students.FirstOrDefault(student => student.Id == id));
            }
        }

        public Task<Student> CreateStudentAsync(Student student)
        {
            lock (_syncRoot)
            {
                student.Id = _nextId++;
                _students.Add(student);
                return Task.FromResult(student);
            }
        }

        public Task<Student?> UpdateStudentAsync(int id, Student student)
        {
            lock (_syncRoot)
            {
                var current = _students.FirstOrDefault(existingStudent => existingStudent.Id == id);
                if (current == null)
                {
                    return Task.FromResult<Student?>(null);
                }

                current.StudentNumber = student.StudentNumber;
                current.FirstName = student.FirstName;
                current.LastName = student.LastName;
                current.Gender = student.Gender;

                return Task.FromResult<Student?>(current);
            }
        }

        public Task<bool> DeleteStudentAsync(int id)
        {
            lock (_syncRoot)
            {
                var student = _students.FirstOrDefault(existingStudent => existingStudent.Id == id);
                if (student == null)
                {
                    return Task.FromResult(false);
                }

                _students.Remove(student);
                return Task.FromResult(true);
            }
        }
    }
}
