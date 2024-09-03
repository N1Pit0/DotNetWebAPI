//Can be deleted

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

    public class UserJobInfoDpController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public UserJobInfoDpController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        private readonly string[] _summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("GetAllUsersJobInfo")]
        public IEnumerable<UserJobInfo> GetAllUsersJobInfo()
        {
            var sql = @"
                        SELECT 
                            [UserId],   
                            [JobTitle],  
                            [Department]
                        FROM TutorialAppSchema.UserJobInfo";
            IEnumerable<UserJobInfo> users = _dapper.LoadData<UserJobInfo>(sql);
            return users;
        }

        [HttpGet("GetSingleUserJobInfo/{userId}")]
        public UserJobInfo GetSingleUserJobInfo(int userId)
        {
            var sql = $@"
                    SELECT 
                        [UserId],
                        [JobTitle],
                        [Department]
                    FROM TutorialAppSchema.UserJobInfo
                    WHERE UserId = {userId}";

            var userJobInfo = _dapper.LoadDataSingle<UserJobInfo>(sql);
            return userJobInfo;
        }

        [HttpPut("EditUserJobInfo")]
        public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
        {
            var sql = $@"
                UPDATE TutorialAppSchema.UserJobInfo
                    SET 
                        [UserId] = {userJobInfo.UserId},
                        [JobTitle] = {userJobInfo.JobTitle},
                        [Department] = {userJobInfo.Department}
                    WHERE UserId = {userJobInfo.UserId}";
            if (_dapper.Execute(sql)) return Ok();

            throw new Exception("Failed to Update UserJobInfo");
        }

        
        [HttpPost("AddUserJobInfo")]
        public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
        {
            var sql = $@"
                INSERT INTO TutorialAppSchema.UserJobInfo
                (
                    [UserId],
                    [JobTitle],
                    [Department]
                )
                VALUES 
                (
                    '{userJobInfo.UserId}',
                    '{userJobInfo.JobTitle}',
                    '{userJobInfo.Department}'
                )";

            if (_dapper.Execute(sql)) return Ok();

            throw new Exception("Failed to Add UserJobInfo");
        }

        [HttpDelete("DeleteUserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            var sql = $@"
                    DELETE FROM TutorialAppSchema.UserJobInfo
                        WHERE UserId = {userId}";
            
            if (_dapper.Execute(sql)) return Ok();

            throw new Exception("Failed to Delete UserJobInfo");
            
        }
    }
}