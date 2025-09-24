using Moq;
using Microsoft.Extensions.Logging;
using CMART.STUDENTS.API.Controllers;
using CMART.STUDENTS.CORE.Models;
using CMART.STUDENTS.SERVICES.Services;
using Microsoft.AspNetCore.Mvc;

public class StudentsControllerTests
{
    private readonly StudentsController _controller;
    private readonly Mock<IStudentService> _mockService;
    private readonly Mock<ILogger<StudentsController>> _mockLogger;

    public StudentsControllerTests()
    {
        _mockService = new Mock<IStudentService>();
        _mockLogger = new Mock<ILogger<StudentsController>>();
        _controller = new StudentsController(_mockLogger.Object, _mockService.Object);
    }

    [Fact]
    public void Get_ReturnsListOfStudents()
    {
        var students = new List<Student>
        {
            new Student { Id = "1", Name = "Alice", Age = 22, Gender = "Female", IsGraduated = false, Courses = new[] { "Math" } }
        };
        _mockService.Setup(s => s.Get()).Returns(students);

        var result = _controller.Get();

        var actionResult = Assert.IsType<ActionResult<List<Student>>>(result);
        var returnValue = Assert.IsType<List<Student>>(actionResult.Value);
        Assert.Single(returnValue);
        Assert.Equal("Alice", returnValue[0].Name);
    }

    [Fact]
    public void Get_WithValidId_ReturnsStudent()
    {
        var student = new Student { Id = "1", Name = "Bob", Age = 24, Gender = "Male", IsGraduated = false, Courses = new[] { "Science" } };
        _mockService.Setup(s => s.Get("1")).Returns(student);

        var result = _controller.Get("1");

        var actionResult = Assert.IsType<ActionResult<Student>>(result);
        var returnValue = Assert.IsType<Student>(actionResult.Value);
        Assert.Equal("Bob", returnValue.Name);
    }

    [Fact]
    public void Get_WithInvalidId_ReturnsNotFound()
    {
        _mockService.Setup(s => s.Get("99")).Returns((Student?)null);

        var result = _controller.Get("99");

        var actionResult = Assert.IsType<ActionResult<Student>>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public void Post_CreatesStudent_ReturnsCreatedAtAction()
    {
        var student = new Student
        {
            Id = "2",
            Name = "Charlie",
            Age = 21,
            Gender = "Male",
            IsGraduated = false,
            Courses = new[] { "Math" }
        };

        _mockService.Setup(s => s.Create(It.IsAny<Student>())).Returns(student).Verifiable();

        var actionResult = _controller.Post(student);
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);

        Assert.Equal("Get", createdAtResult.ActionName);
        Assert.Equal(student, createdAtResult.Value);
        _mockService.Verify(s => s.Create(student), Times.Once);
    }

    [Fact]
    public void Post_WithInvalidModel_ReturnsBadRequest()
    {
        var student = new Student
        {
            Id = "",
            Name = "",
            Age = 0,
            Gender = "Other",
            IsGraduated = false,
            Courses = null
        };

        var actionResult = _controller.Post(student);
        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);

        Assert.NotNull(badRequest.Value);
    }

    [Fact]
    public void Put_ExistingStudent_UpdatesAndReturnsNoContent()
    {
        var student = new Student
        {
            Id = "1",
            Name = "Dana",
            Age = 25,
            Gender = "Female",
            IsGraduated = true,
            Courses = new string[] { } // empty because graduated
        };
        _mockService.Setup(s => s.Get("1")).Returns(student);
        _mockService.Setup(s => s.Update("1", student)).Verifiable();

        var result = _controller.Put("1", student);

        _mockService.Verify(s => s.Update("1", student), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void Put_NonExistingStudent_ReturnsNotFound()
    {
        _mockService.Setup(s => s.Get("99")).Returns((Student?)null);
        var student = new Student
        {
            Id = "99",
            Name = "Eve",
            Age = 28,
            Gender = "Female",
            IsGraduated = true,
            Courses = new string[] { }
        };

        var result = _controller.Put("99", student);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public void Delete_ExistingStudent_DeletesAndReturnsOk()
    {
        var student = new Student { Id = "1", Name = "Frank", Age = 23, Gender = "Male", IsGraduated = false, Courses = new[] { "Math" } };
        _mockService.Setup(s => s.Get("1")).Returns(student);
        _mockService.Setup(s => s.Remove("1")).Verifiable();

        var result = _controller.Delete("1");

        _mockService.Verify(s => s.Remove("1"), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public void Delete_NonExistingStudent_ReturnsNotFound()
    {
        _mockService.Setup(s => s.Get("99")).Returns((Student?)null);

        var result = _controller.Delete("99");

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }
}
