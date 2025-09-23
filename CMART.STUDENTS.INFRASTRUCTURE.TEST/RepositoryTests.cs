using Moq;
using MongoDB.Driver;
using CMART.STUDENTS.CORE.Models;
using CMART.STUDENTS.INFRASTRUCTURE.Respositories;

public class StudentRepositoryTests
{
    private readonly Mock<IMongoClient> _mockClient;
    private readonly Mock<IMongoDatabase> _mockDatabase;
    private readonly Mock<IMongoCollection<Student>> _mockCollection;
    private readonly Mock<IAsyncCursor<Student>> _mockCursor;
    private readonly StudentRepository _repo;
    private readonly IStudentStoreDatabaseSettings _dbSettings;

    public StudentRepositoryTests()
    {
        _mockClient = new Mock<IMongoClient>();
        _mockDatabase = new Mock<IMongoDatabase>();
        _mockCollection = new Mock<IMongoCollection<Student>>();
        _mockCursor = new Mock<IAsyncCursor<Student>>();

        _dbSettings = new StudentStoreDatabaseSettings
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "TestDb",
            StudentCoursesCollectionName = "Students"
        };

        _mockClient.Setup(c => c.GetDatabase(_dbSettings.DatabaseName, null))
                   .Returns(_mockDatabase.Object);

        _mockDatabase.Setup(d => d.GetCollection<Student>(_dbSettings.StudentCoursesCollectionName, null))
                     .Returns(_mockCollection.Object);

        _repo = new StudentRepository(_mockClient.Object, _dbSettings);
    }

    private void SetupCursor(List<Student> students)
    {
        _mockCursor.Setup(_ => _.Current).Returns(students);
        _mockCursor.SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        _mockCursor.SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllStudents()
    {
        var studentList = new List<Student> { new Student { Id = "id1", Name = "Student1" } };

        SetupCursor(studentList);

        _mockCollection.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Student>>(),
            It.IsAny<FindOptions<Student, Student>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(_mockCursor.Object);

        var result = await _repo.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("Student1", result[0].Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsStudent_WhenFound()
    {
        var student = new Student { Id = "myid", Name = "TestStudent" };
        var students = new List<Student> { student };

        SetupCursor(students);

        _mockCollection.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Student>>(),
            It.IsAny<FindOptions<Student, Student>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(_mockCursor.Object);

        var result = await _repo.GetByIdAsync("myid");

        Assert.NotNull(result);
        Assert.Equal("TestStudent", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        var emptyList = new List<Student>();

        SetupCursor(emptyList);

        _mockCollection.Setup(c => c.FindAsync(
            It.IsAny<FilterDefinition<Student>>(),
            It.IsAny<FindOptions<Student, Student>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(_mockCursor.Object);

        var result = await _repo.GetByIdAsync("nonexistentId");

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CallsInsertOneAsync()
    {
        var student = new Student { Id = "1", Name = "InsertMe" };

        _mockCollection.Setup(c => c.InsertOneAsync(
            student,
            null,
            It.IsAny<CancellationToken>()
        )).Returns(Task.CompletedTask).Verifiable();

        await _repo.CreateAsync(student);

        _mockCollection.Verify(c => c.InsertOneAsync(
            student,
            null,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_CallsReplaceOneAsync()
    {
        var student = new Student { Id = "2", Name = "UpdatedName" };

        _mockCollection.Setup(c => c.ReplaceOneAsync(
            It.IsAny<FilterDefinition<Student>>(),
            student,
            It.IsAny<ReplaceOptions>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(Mock.Of<ReplaceOneResult>()).Verifiable();

        await _repo.UpdateAsync(student.Id, student);

        _mockCollection.Verify(c => c.ReplaceOneAsync(
            It.IsAny<FilterDefinition<Student>>(),
            student,
            It.IsAny<ReplaceOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_CallsDeleteOneAsync()
    {
        _mockCollection.Setup(c => c.DeleteOneAsync(
            It.IsAny<FilterDefinition<Student>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(Mock.Of<DeleteResult>()).Verifiable();

        await _repo.DeleteAsync("anyid");

        _mockCollection.Verify(c => c.DeleteOneAsync(
            It.IsAny<FilterDefinition<Student>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private class StudentStoreDatabaseSettings : IStudentStoreDatabaseSettings
    {
        public string StudentCoursesCollectionName { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}
