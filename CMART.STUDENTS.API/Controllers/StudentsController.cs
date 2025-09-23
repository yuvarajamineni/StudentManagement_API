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

        // GET: api/Students
        [HttpGet]
        public ActionResult<List<Student>> Get()
        {
            _logger.LogInformation("Getting all students");
            return studentService.Get();
        }

        // GET: api/Students/{id}
        [HttpGet("{id}")]
        public ActionResult<Student> Get(string id)
        {
            var student = studentService.Get(id);
            if (student == null)
                return NotFound($"Student with Id = {id} not found");

            return student;
        }

        // POST: api/Students
        [HttpPost]
        public ActionResult<Student> Post([FromBody] Student student)
        {
            studentService.Create(student);
            return CreatedAtAction(nameof(Get), new { id = student.Id }, student);
        }

        // PUT: api/Students/{id}
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] Student student)
        {
            var existingStudent = studentService.Get(id);
            if (existingStudent == null)
                return NotFound($"Student with Id= {id} not found");

            studentService.Update(id, student);
            return NoContent();
        }

        // DELETE: api/Students/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var student = studentService.Get(id);
            if (student == null)
                return NotFound($"Student with Id = {id} not found");

            studentService.Remove(student.Id);
            return Ok($"Student with Id = {id} deleted");
        }
    }
}
