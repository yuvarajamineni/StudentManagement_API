using CMART.STUDENTS.CORE.Models;

public class StudentModelTests
{
    [Fact]
    public void Student_DefaultValues_CorrectlySet()
    {
        var student = new Student();

        Assert.Equal(string.Empty, student.Id);
        Assert.Equal(string.Empty, student.Name);
        Assert.False(student.IsGraduated);
        Assert.Null(student.Courses);
        Assert.Equal(string.Empty, student.Gender);
        Assert.Equal(0, student.Age);
    }

    [Fact]
    public void Student_CanSetProperties()
    {
        var courses = new[] { "Math", "Science" };
        var student = new Student
        {
            Id = "abc123",
            Name = "John Doe",
            IsGraduated = true,
            Courses = courses,
            Gender = "Male",
            Age = 20
        };

        Assert.Equal("abc123", student.Id);
        Assert.Equal("John Doe", student.Name);
        Assert.True(student.IsGraduated);
        Assert.Equal(courses, student.Courses);
        Assert.Equal("Male", student.Gender);
        Assert.Equal(20, student.Age);
    }
}
