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
    public void Create_CallsRepositoryCreateAsync()
    {
        var student = new Student { Id = "1", Name = "Test Student" };
        _mockRepository.Setup(r => r.CreateAsync(student)).Returns(Task.CompletedTask);

        var result = _studentService.Create(student);

        _mockRepository.Verify(r => r.CreateAsync(student), Times.Once);
        Assert.Equal(student, result);
    }

    [Fact]
    public void Get_ReturnsAllStudents()
    {
        var students = new List<Student>
        {
            new Student{Id = "1", Name = "Student A"},
            new Student{Id = "2", Name = "Student B"}
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(students);

        var result = _studentService.Get();

        Assert.Equal(students, result);
    }

    [Fact]
    public void Get_ExistingId_ReturnsStudent()
    {
        var student = new Student { Id = "1", Name = "Student A" };
        _mockRepository.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(student);

        var result = _studentService.Get("1");

        Assert.Equal(student, result);
    }

    [Fact]
    public void Get_NonExistingId_ThrowsKeyNotFoundException()
    {
        _mockRepository.Setup(r => r.GetByIdAsync("99")).ReturnsAsync((Student?)null);

        Assert.Throws<KeyNotFoundException>(() => _studentService.Get("99"));
    }

    [Fact]
    public void Remove_CallsRepositoryDeleteAsync()
    {
        _mockRepository.Setup(r => r.DeleteAsync("1")).Returns(Task.CompletedTask);

        _studentService.Remove("1");

        _mockRepository.Verify(r => r.DeleteAsync("1"), Times.Once);
    }

    [Fact]
    public void Update_CallsRepositoryUpdateAsync()
    {
        var student = new Student { Id = "1", Name = "Updated Student" };
        _mockRepository.Setup(r => r.UpdateAsync("1", student)).Returns(Task.CompletedTask);

        _studentService.Update("1", student);

        _mockRepository.Verify(r => r.UpdateAsync("1", student), Times.Once);
    }
}
