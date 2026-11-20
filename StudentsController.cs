using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IConfiguration configuration, ILogger<StudentsController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
            _logger = logger;
        }

        // GET: api/students
        [HttpGet]
        public ActionResult<IEnumerable<Student>> GetAll()
        {
            var students = new List<Student>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT Id, Name, Email, Course FROM Students ORDER BY Id", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                students.Add(Map(reader));

            return Ok(students);
        }

        // GET: api/students/1
        [HttpGet("{id:int}")]
        public ActionResult<Student> GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT Id, Name, Email, Course FROM Students WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return NotFound(new { message = $"Student with ID {id} not found." });

            return Ok(Map(reader));
        }

        // POST: api/students
        [HttpPost]
        public ActionResult<Student> Create([FromBody] Student student)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = new SqlCommand(
                    @"INSERT INTO Students (Name, Email, Course)
                      OUTPUT INSERTED.Id
                      VALUES (@Name, @Email, @Course)", conn);
                cmd.Parameters.AddWithValue("@Name", student.Name);
                cmd.Parameters.AddWithValue("@Email", student.Email);
                cmd.Parameters.AddWithValue("@Course", student.Course);

                student.Id = (int)cmd.ExecuteScalar()!;
                _logger.LogInformation("Created student {Id}", student.Id);
                return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return Conflict(new { message = "A student with this email already exists." });
            }
        }

        // PUT: api/students/1
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Student student)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                using var cmd = new SqlCommand(
                    @"UPDATE Students
                      SET Name = @Name, Email = @Email, Course = @Course, UpdatedDate = GETUTCDATE()
                      WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Name", student.Name);
                cmd.Parameters.AddWithValue("@Email", student.Email);
                cmd.Parameters.AddWithValue("@Course", student.Course);

                if (cmd.ExecuteNonQuery() == 0)
                    return NotFound(new { message = $"Student with ID {id} not found." });

                student.Id = id;
                return Ok(student);
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return Conflict(new { message = "A student with this email already exists." });
            }
        }

        // DELETE: api/students/1
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM Students WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            if (cmd.ExecuteNonQuery() == 0)
                return NotFound(new { message = $"Student with ID {id} not found." });

            return NoContent();
        }

        private static Student Map(SqlDataReader r) => new()
        {
            Id = r.GetInt32(0),
            Name = r.GetString(1),
            Email = r.GetString(2),
            Course = r.GetString(3)
        };
    }
}
