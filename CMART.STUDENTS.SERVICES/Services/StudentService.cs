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

        public Student Create(Student student)
        {
            _repository.CreateAsync(student).Wait();
            return student;
        }

        public List<Student> Get()
        {
            return _repository.GetAllAsync().Result;
        }

        public Student Get(string id)
        {
            var student = _repository.GetByIdAsync(id).Result;
            if (student == null)
                throw new KeyNotFoundException($"Student with id '{id}' not found.");
            return student;
        }

        public void Remove(string id)
        {
            _repository.DeleteAsync(id).Wait();
        }

        public void Update(string id, Student student)
        {
            _repository.UpdateAsync(id, student).Wait();
        }
    }
}
