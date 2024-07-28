using DotNetWebApp.Data;
using DotNetWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public UserController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("TestConnection")]
        public IActionResult TestConnection()
        {
            try
            {
                DateTime currentTime = _dapper.LoadDataSingle<DateTime>("SELECT GETDATE();");
                return Ok(currentTime);
            }
            catch (Exception ex)
            {
                // Log the exception (for production code, use a logging framework)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private readonly string[] _summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("GetUsers")]
        public IEnumerable<User> GetUsers()
        {
            var sql = @"
                        SELECT [UserId],
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active]    
                        FROM TutorialAppSchema.Users";
            IEnumerable<User> users = _dapper.LoadData<User>(sql);
            return users;
        }

        [HttpGet("GetSingleUser/{userId}")]
        public User GetSingleUser(int userId)
        {
            var sql = $@"
                    SELECT [UserId],
                        [FirstName],
                        [LastName],
                        [Email],
                        [Gender],
                        [Active]    
                    FROM TutorialAppSchema.Users
                    WHERE UserId = {userId}";

            var user = _dapper.LoadDataSingle<User>(sql);
            return user;
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {
            var sql = $@"
                UPDATE TutorialAppSchema.Users
                    SET [FirstName] = '{user.FirstName}',
                        [LastName] = '{user.LastName}',
                        [Email] = '{user.Email}',
                        [Gender] = '{user.Gender}',
                        [Active] = '{user.Active}'
                    WHERE UserId = {user.UserId}";
            if (_dapper.Execute(sql)) return Ok();

            throw new Exception("Failed to Update User");
        }

        
        [HttpPost("AddUser")]
        public IActionResult AddUser(User user)
        {
            var sql = $@"
                INSERT INTO TutorialAppSchema.Users
                (
                    [FirstName],
                    [LastName],
                    [Email],
                    [Gender],
                    [Active]
                )
                VALUES 
                (
                    '{user.FirstName}',
                    '{user.LastName}',
                    '{user.Email}',
                    '{user.Gender}',
                    '{user.Active}'
                )";

            if (_dapper.Execute(sql)) return Ok();

            throw new Exception("Failed to Add User");
        }

        public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
        {
            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        }
    }
}