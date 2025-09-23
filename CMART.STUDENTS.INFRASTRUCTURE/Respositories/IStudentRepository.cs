using CMART.STUDENTS.CORE.Models;

namespace CMART.STUDENTS.INFRASTRUCTURE.Respositories
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(string id);
        Task CreateAsync(Student student);
        Task UpdateAsync(string id, Student student);
        Task DeleteAsync(string id);
    }
}

