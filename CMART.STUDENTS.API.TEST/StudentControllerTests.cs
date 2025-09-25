using Moq;
using Microsoft.Extensions.Logging;
using CMART.STUDENTS.API.Controllers;
using CMART.STUDENTS.CORE.Models;
using CMART.STUDENTS.SERVICES.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

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
    public async Task Get_ReturnsListOfStudents()
    {
        var students = new List<Student>
        {
            new Student { Id = "1", Name = "Alice", Age = 22, Gender = "Female", IsGraduated = false, Courses = new[] { "Math" } }
        };
        _mockService.Setup(s => s.GetAsync()).ReturnsAsync(students);

        var result = await _controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<Student>>(okResult.Value);

        Assert.Single(returnValue);
        Assert.Equal("Alice", returnValue[0].Name);
    }

    [Fact]
    public async Task Get_WithValidId_ReturnsStudent()
    {
        var student = new Student { Id = "1", Name = "Bob", Age = 24, Gender = "Male", IsGraduated = false, Courses = new[] { "Science" } };
        _mockService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(student);

        var result = await _controller.GetById("1");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Student>(okResult.Value);

        Assert.Equal("Bob", returnValue.Name);
    }

    [Fact]
    public async Task Get_WithInvalidId_ReturnsNotFound()
    {
        _mockService.Setup(s => s.GetByIdAsync("99")).ReturnsAsync((Student?)null);

        var result = await _controller.GetById("99");

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task Post_CreatesStudent_ReturnsCreatedAtAction()
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

        _mockService.Setup(s => s.CreateAsync(It.IsAny<Student>())).ReturnsAsync(student);

        var actionResult = await _controller.Post(student);
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);

        Assert.Equal("GetById", createdAtResult.ActionName);
        Assert.Equal(student, createdAtResult.Value);
        _mockService.Verify(s => s.CreateAsync(student), Times.Once);
    }

    [Fact]
    public async Task Post_WithInvalidModel_ReturnsBadRequest()
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

        var actionResult = await _controller.Post(student);
        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);

        Assert.NotNull(badRequest.Value);
    }

    [Fact]
    public async Task Put_ExistingStudent_UpdatesAndReturnsNoContent()
    {
        var student = new Student
        {
            Id = "1",
            Name = "Dana",
            Age = 25,
            Gender = "Female",
            IsGraduated = true,
            Courses = new string[] { }
        };
        _mockService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(student);
        _mockService.Setup(s => s.UpdateAsync("1", student)).Returns(Task.CompletedTask);

        var result = await _controller.Put("1", student);

        _mockService.Verify(s => s.UpdateAsync("1", student), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Put_NonExistingStudent_ReturnsNotFound()
    {
        _mockService.Setup(s => s.GetByIdAsync("99")).ReturnsAsync((Student?)null);
        var student = new Student
        {
            Id = "99",
            Name = "Eve",
            Age = 28,
            Gender = "Female",
            IsGraduated = true,
            Courses = new string[] { }
        };

        var result = await _controller.Put("99", student);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingStudent_DeletesAndReturnsOk()
    {
        var student = new Student { Id = "1", Name = "Frank", Age = 23, Gender = "Male", IsGraduated = false, Courses = new[] { "Math" } };
        _mockService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(student);
        _mockService.Setup(s => s.RemoveAsync("1")).Returns(Task.CompletedTask);

        var result = await _controller.Delete("1");

        _mockService.Verify(s => s.RemoveAsync("1"), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task Delete_NonExistingStudent_ReturnsNotFound()
    {
        _mockService.Setup(s => s.GetByIdAsync("99")).ReturnsAsync((Student?)null);

        var result = await _controller.Delete("99");

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }
}
