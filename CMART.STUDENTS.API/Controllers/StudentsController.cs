using Microsoft.AspNetCore.Mvc;
using CMART.STUDENTS.SERVICES.Services;
using CMART.STUDENTS.CORE.Models;

namespace CMART.STUDENTS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService studentService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(ILogger<StudentsController> logger, IStudentService studentService)
        {
            _logger = logger;
            this.studentService = studentService;
        }

        [HttpGet]
        public ActionResult<List<Student>> Get()
        {
            _logger.LogInformation("Received request to get all students");
            try
            {
                var students = studentService.Get();
                _logger.LogInformation("Returned {StudentCount} students", students.Count);
                return students;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all students");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Student> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Student ID is null or empty");
                return BadRequest("ID cannot be null or empty.");
            }

            _logger.LogInformation("Received request to get student with Id {StudentId}", id);
            try
            {
                var student = studentService.Get(id);
                if (student == null)
                {
                    _logger.LogWarning("Student with Id {StudentId} not found", id);
                    return NotFound($"Student with Id = {id} not found");
                }
                _logger.LogInformation("Returning student with Id {StudentId}", id);
                return student;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching student with Id {StudentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public ActionResult<Student> Post([FromBody] Student student)
        {
            _logger.LogInformation("Received request to create a new student with Name {StudentName}", student.Name);

            // Validations
            if (string.IsNullOrWhiteSpace(student.Id))
                ModelState.AddModelError(nameof(student.Id), "Id cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(student.Name))
                ModelState.AddModelError(nameof(student.Name), "Name is required.");

            if (string.IsNullOrWhiteSpace(student.Gender))
                ModelState.AddModelError(nameof(student.Gender), "Gender is required.");
            else if (student.Gender != "Male" && student.Gender != "Female")
                ModelState.AddModelError(nameof(student.Gender), "Gender must be 'Male' or 'Female'.");

            if (student.Age < 1 || student.Age > 100)
                ModelState.AddModelError(nameof(student.Age), "Age must be between 1 and 100.");

            // Enforce courses can be null only if graduated
            if (!student.IsGraduated && (student.Courses == null || student.Courses.Length == 0))
                ModelState.AddModelError(nameof(student.Courses), "Courses cannot be null or empty if the student has not graduated.");

            // Enforce courses be zero length if graduated
            if (student.IsGraduated && student.Courses != null && student.Courses.Length > 0)
                ModelState.AddModelError(nameof(student.Courses), "Courses must be empty if the student has graduated.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for student creation");
                return BadRequest(ModelState);
            }

            try
            {
                studentService.Create(student);
                _logger.LogInformation("Successfully created student with Id {StudentId}", student.Id);
                return CreatedAtAction(nameof(Get), new { id = student.Id }, student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating student with Name {StudentName}", student.Name);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] Student student)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Student ID is null or empty for update");
                return BadRequest("ID cannot be null or empty.");
            }

            _logger.LogInformation("Received request to update student with Id {StudentId}", id);

            if (string.IsNullOrWhiteSpace(student.Id))
                ModelState.AddModelError(nameof(student.Id), "Id cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(student.Name))
                ModelState.AddModelError(nameof(student.Name), "Name is required.");

            if (string.IsNullOrWhiteSpace(student.Gender))
                ModelState.AddModelError(nameof(student.Gender), "Gender is required.");
            else if (student.Gender != "Male" && student.Gender != "Female")
                ModelState.AddModelError(nameof(student.Gender), "Gender must be 'Male' or 'Female'.");

            if (student.Age < 1 || student.Age > 100)
                ModelState.AddModelError(nameof(student.Age), "Age must be between 1 and 100.");

            if (!student.IsGraduated && (student.Courses == null || student.Courses.Length == 0))
                ModelState.AddModelError(nameof(student.Courses), "Courses cannot be null or empty if the student has not graduated.");

            if (student.IsGraduated && student.Courses != null && student.Courses.Length > 0)
                ModelState.AddModelError(nameof(student.Courses), "Courses must be empty if the student has graduated.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for student update with Id {StudentId}", id);
                return BadRequest(ModelState);
            }

            try
            {
                var existingStudent = studentService.Get(id);
                if (existingStudent == null)
                {
                    _logger.LogWarning("Student with Id {StudentId} not found for update", id);
                    return NotFound($"Student with Id= {id} not found");
                }

                studentService.Update(id, student);
                _logger.LogInformation("Successfully updated student with Id {StudentId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating student with Id {StudentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Student ID is null or empty for deletion");
                return BadRequest("ID cannot be null or empty.");
            }

            _logger.LogInformation("Received request to delete student with Id {StudentId}", id);
            try
            {
                var student = studentService.Get(id);
                if (student == null)
                {
                    _logger.LogWarning("Student with Id {StudentId} not found for deletion", id);
                    return NotFound($"Student with Id = {id} not found");
                }

                studentService.Remove(student.Id);
                _logger.LogInformation("Successfully deleted student with Id {StudentId}", id);
                return Ok($"Student with Id = {id} deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting student with Id {StudentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
