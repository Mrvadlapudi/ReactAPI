using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ReactAPI.Model;
using Microsoft.Extensions.Logging; // Add Logging

namespace ReactAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<UserController> _logger;

        public UserController(IConfiguration configuration, ILogger<UserController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // POST: api/user
        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest(new { message = "Invalid user data." });
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", user.Username);
                        cmd.Parameters.AddWithValue("@Email", user.Email);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "User registered successfully!" });
                        }
                        else
                        {
                            return StatusCode(500, new { message = "User registration failed." });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError($"SQL Error: {ex.Message}");
                return StatusCode(500, new { message = "Database error occurred." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
