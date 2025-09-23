using MongoDB.Driver;
using CMART.STUDENTS.CORE.Models;

namespace CMART.STUDENTS.INFRASTRUCTURE.Respositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IMongoCollection<Student> _students;

        public StudentRepository(IMongoClient client, IStudentStoreDatabaseSettings settings)
        {
            var database = client.GetDatabase(settings.DatabaseName);
            _students = database.GetCollection<Student>(settings.StudentCoursesCollectionName);
        }

        public async Task<List<Student>> GetAllAsync() =>
            await _students.Find(_ => true).ToListAsync();

        public async Task<Student?> GetByIdAsync(string id) =>
            await _students.Find(s => s.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Student student) =>
            await _students.InsertOneAsync(student);

        public async Task UpdateAsync(string id, Student student) =>
            await _students.ReplaceOneAsync(s => s.Id == id, student);

        public async Task DeleteAsync(string id) =>
            await _students.DeleteOneAsync(s => s.Id == id);
    }
}
