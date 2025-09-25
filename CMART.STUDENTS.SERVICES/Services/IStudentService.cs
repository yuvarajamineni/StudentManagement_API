using CMART.STUDENTS.CORE.Models;

namespace CMART.STUDENTS.SERVICES.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetAsync();
        Task<Student?> GetByIdAsync(string id);
        Task<Student> CreateAsync(Student student);
        Task UpdateAsync(string id, Student student);
        Task RemoveAsync(string id);
    }
}
