using DotNetWebApp.Data;
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

        [HttpGet("GetUsers/{testValue}")]
        public string[] GetUsers(string testValue)
        {
            string[] responseArray = new[]
            {
                "test1",
                "test2",
                testValue
            };
            return responseArray;
        }

        public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
        {
            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        }
    }
}