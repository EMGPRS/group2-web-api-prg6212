namespace StudentRegister.Web.Models
{
    public class StudentRegisterViewModel
    {
        public List<Student> Students { get; set; } = [];
        public Student Student { get; set; } = new();
    }
}