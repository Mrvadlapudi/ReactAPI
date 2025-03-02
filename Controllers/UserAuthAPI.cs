using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ReactAPI.Model;

namespace ReactAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly string _connectionString;

        public UserController(IConfiguration configuration)
        {
            // Use SQL Authentication
            var builder = new SqlConnectionStringBuilder(configuration.GetConnectionString("DefaultConnection"))
            {
                UserID = "sa",   // Change this to your SQL Server username
                Password = "sa@123" // Change this to your SQL Server password
            };

            _connectionString = builder.ConnectionString;
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest("Invalid user data.");
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
                        cmd.ExecuteNonQuery();
                    }
                }

                return Ok(new { message = "User registered successfully!" });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }
    }
}
