using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMART.STUDENTS.SERVICES.Services;
using CMART.STUDENTS.CORE.Models;

namespace CMART.STUDENTS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(ILogger<StudentsController> logger, IStudentService studentService)
        {
            _logger = logger;
            _studentService = studentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Moderator,ReadOnly")]
        public async Task<ActionResult<List<Student>>> Get()
        {
            _logger.LogInformation("Get all students requested");
            try
            {
                var students = await _studentService.GetAsync();
                _logger.LogInformation("Returned {StudentCount} students", students.Count);
                return Ok(students);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching students");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Moderator,ReadOnly")]
        public async Task<ActionResult<Student>> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Get student called with null/empty ID");
                return BadRequest("ID cannot be null or empty.");
            }

            try
            {
                var student = await _studentService.GetByIdAsync(id);
                if (student == null)
                {
                    _logger.LogWarning("Student with Id {StudentId} not found", id);
                    return NotFound($"Student with Id = {id} not found");
                }

                return Ok(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching student with Id {StudentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<Student>> Post([FromBody] Student? student)
        {
            if (student == null)
            {
                _logger.LogWarning("Student object from request body is null");
                return BadRequest("Student cannot be null");
            }

            _logger.LogInformation("Create student requested: {StudentName}", student.Name);

            // 🔹 Validations
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
                _logger.LogWarning("Invalid model state for student creation");
                return BadRequest(ModelState);
            }

            try
            {
                var createdStudent = await _studentService.CreateAsync(student);
                _logger.LogInformation("Student created with Id {StudentId}", createdStudent.Id);
                return CreatedAtAction(nameof(GetById), new { id = createdStudent.Id }, createdStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult> Put(string id, [FromBody] Student student)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Update student called with null/empty ID");
                return BadRequest("ID cannot be null or empty.");
            }

            _logger.LogInformation("Update student with Id {StudentId} requested", id);

            // 🔹 Validations
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
                var existingStudent = await _studentService.GetByIdAsync(id);
                if (existingStudent == null)
                {
                    _logger.LogWarning("Student with Id {StudentId} not found for update", id);
                    return NotFound($"Student with Id = {id} not found");
                }

                await _studentService.UpdateAsync(id, student);
                _logger.LogInformation("Student updated with Id {StudentId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student with Id {StudentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogWarning("Delete student called with null/empty ID");
                return BadRequest("ID cannot be null or empty.");
            }

            _logger.LogInformation("Delete student with Id {StudentId} requested", id);

            try
            {
                var student = await _studentService.GetByIdAsync(id);
                if (student == null)
                {
                    _logger.LogWarning("Student with Id {StudentId} not found for deletion", id);
                    return NotFound($"Student with Id = {id} not found");
                }

                await _studentService.RemoveAsync(student.Id);
                _logger.LogInformation("Student deleted with Id {StudentId}", id);
                return Ok($"Student with Id = {id} deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student with Id {StudentId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
