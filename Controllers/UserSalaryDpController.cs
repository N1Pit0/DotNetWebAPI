//Can be deleted

using Microsoft.AspNetCore.Mvc;
using DotNetWebApp.Data;
using DotNetWebApp.DTOs;
using DotNetWebApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotNetWebApp.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Obsolete]

    public class UserSalaryDpController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public UserSalaryDpController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        private readonly string[] _summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("GetAllUserSalary")]
        public IEnumerable<UserSalary> GetAllUserSalary()
        {
            var sql = @"
                        SELECT 
                            [UserId],   
                            [Salary]
                        FROM TutorialAppSchema.UserSalary";
            IEnumerable<UserSalary> users = _dapper.LoadData<UserSalary>(sql);
            return users;
        }

        [HttpGet("GetSingleUserSalary/{userId}")]
        public UserSalary GetSingleUserSalary(int userId)
        {
            var sql = $@"
                    SELECT 
                        [UserId],
                        [Salary]
                    FROM TutorialAppSchema.UserSalary
                    WHERE UserId = {userId}";

            var userSalary = _dapper.LoadDataSingle<UserSalary>(sql);
            return userSalary;
        }

        [HttpPut("EditUserSalary")]
        public IActionResult EditUserSalary(UserSalary userJobInfo)
        {
            var sql = $@"
                UPDATE TutorialAppSchema.UserSalary
                    SET 
                        [UserId] = {userJobInfo.UserId},
                        [Salary] = {userJobInfo.Salary}
                    WHERE UserId = {userJobInfo.UserId}";
            if (_dapper.Execute(sql)) return Ok();

            throw new Exception("Failed to Update UserSalary");
        }


        [HttpPost("AddUserSalary")]
        public IActionResult AddUserSalary(UserSalary userSalary)
        {
            var sql = $@"
                INSERT INTO TutorialAppSchema.UserSalary
                (
                    [UserId],
                    [Salary]
                )
                VALUES 
                (
                    '{userSalary.UserId}',
                    '{userSalary.Salary}'
                )";

            if (_dapper.Execute(sql)) return Ok();

            throw new Exception("Failed to Add UserSalary");
        }

        [HttpDelete("DeleteUserSalary/{userId}")]
        public IActionResult DeleteUserSalary(int userId)
        {
            var sql = $@"
                    DELETE FROM TutorialAppSchema.UserSalary
                        WHERE UserId = {userId}";

            if (_dapper.Execute(sql)) return Ok();

            throw new Exception("Failed to Delete UserSalary");

        }
    }

}