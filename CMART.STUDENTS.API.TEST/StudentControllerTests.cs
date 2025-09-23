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
        var students = new List<Student> { new Student { Id = "1", Name = "Alice", Age = 22, Gender = "Female" } };
        _mockService.Setup(s => s.Get()).Returns(students);

        var result = _controller.Get();

        var actionResult = Assert.IsType<ActionResult<List<Student>>>(result);
        var returnValue = actionResult.Value;
        Assert.NotNull(returnValue);
        Assert.Single(returnValue!);
        Assert.Equal("Alice", returnValue![0].Name);
    }

    [Fact]
    public void Get_WithValidId_ReturnsStudent()
    {
        var student = new Student { Id = "1", Name = "Bob", Age = 24, Gender = "Male" };
        _mockService.Setup(s => s.Get("1")).Returns(student);

        var result = _controller.Get("1");

        var actionResult = Assert.IsType<ActionResult<Student>>(result);
        var returnValue = actionResult.Value;
        Assert.NotNull(returnValue);
        Assert.Equal("Bob", returnValue!.Name);
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
        var student = new Student { Id = "2", Name = "Charlie", Age = 21, Gender = "Male" };

        var result = _controller.Post(student);

        _mockService.Verify(s => s.Create(student), Times.Once);
        var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal("Get", createdAtResult.ActionName);
        Assert.Equal(student, createdAtResult.Value);
    }

    [Fact]
    public void Put_ExistingStudent_UpdatesAndReturnsNoContent()
    {
        var student = new Student { Id = "1", Name = "Dana" };
        _mockService.Setup(s => s.Get("1")).Returns(student);

        var result = _controller.Put("1", student);

        _mockService.Verify(s => s.Update("1", student), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void Put_NonExistingStudent_ReturnsNotFound()
    {
        _mockService.Setup(s => s.Get("99")).Returns((Student?)null);
        var student = new Student { Id = "99", Name = "Eve" };

        var result = _controller.Put("99", student);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public void Delete_ExistingStudent_DeletesAndReturnsOk()
    {
        var student = new Student { Id = "1", Name = "Frank" };
        _mockService.Setup(s => s.Get("1")).Returns(student);

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
