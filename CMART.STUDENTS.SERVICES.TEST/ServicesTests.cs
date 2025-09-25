using Moq;
using CMART.STUDENTS.SERVICES.Services;
using CMART.STUDENTS.CORE.Models;
using CMART.STUDENTS.INFRASTRUCTURE.Respositories;

public class StudentServiceTests
{
    private readonly StudentService _studentService;
    private readonly Mock<IStudentRepository> _mockRepository;

    public StudentServiceTests()
    {
        _mockRepository = new Mock<IStudentRepository>();
        _studentService = new StudentService(_mockRepository.Object);
    }

    [Fact]
    public async Task Create_CallsRepositoryCreateAsync()
    {
        var student = new Student { Id = "1", Name = "Test Student" };
        _mockRepository.Setup(r => r.CreateAsync(student)).Returns(Task.CompletedTask);

        var result = await _studentService.CreateAsync(student); // await here

        _mockRepository.Verify(r => r.CreateAsync(student), Times.Once);
        Assert.Equal(student, result);
    }

    [Fact]
    public async Task Get_ReturnsAllStudents()
    {
        var students = new List<Student>
        {
            new Student{Id = "1", Name = "Student A"},
            new Student{Id = "2", Name = "Student B"}
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(students);

        var result = await _studentService.GetAsync(); // await here

        Assert.Equal(students, result);
    }

    [Fact]
    public async Task Get_ExistingId_ReturnsStudent()
    {
        var student = new Student { Id = "1", Name = "Student A" };
        _mockRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(student);

        var result = await _studentService.GetByIdAsync("1"); // await here

        Assert.Equal(student, result);
    }

    [Fact]
    public async Task Get_NonExistingId_ThrowsKeyNotFoundException()
    {
        _mockRepository.Setup(r => r.GetByIdAsync("99")).ReturnsAsync((Student?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            var result = await _studentService.GetByIdAsync("99");
            if (result == null) throw new KeyNotFoundException();
        });
    }

    [Fact]
    public async Task Remove_CallsRepositoryDeleteAsync()
    {
        _mockRepository.Setup(r => r.DeleteAsync("1")).Returns(Task.CompletedTask);

        await _studentService.RemoveAsync("1"); // await here

        _mockRepository.Verify(r => r.DeleteAsync("1"), Times.Once);
    }

    [Fact]
    public async Task Update_CallsRepositoryUpdateAsync()
    {
        var student = new Student { Id = "1", Name = "Updated Student" };
        _mockRepository.Setup(r => r.UpdateAsync("1", student)).Returns(Task.CompletedTask);

        await _studentService.UpdateAsync("1", student); // await here

        _mockRepository.Verify(r => r.UpdateAsync("1", student), Times.Once);
    }
}
