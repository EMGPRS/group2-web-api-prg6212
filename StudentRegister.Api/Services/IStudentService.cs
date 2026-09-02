using StudentRegister.Api.Models;

namespace StudentRegister.Api.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetStudentsAsync();
        Task<List<Student>> SearchStudentsAsync(string search);
        Task<Student?> GetStudentByIdAsync(int id);
        Task<Student> CreateStudentAsync(Student student);
        Task<Student?> UpdateStudentAsync(int id, Student student);
        Task<bool> DeleteStudentAsync(int id);
    }
}
