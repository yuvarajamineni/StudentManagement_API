using CMART.STUDENTS.CORE.Models;
using CMART.STUDENTS.INFRASTRUCTURE.Respositories;

namespace CMART.STUDENTS.SERVICES.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<Student> CreateAsync(Student student)
        {
            await _repository.CreateAsync(student);
            return student;
        }

        public async Task<List<Student>> GetAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Student?> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task RemoveAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task UpdateAsync(string id, Student student)
        {
            await _repository.UpdateAsync(id, student);
        }
    }
}
