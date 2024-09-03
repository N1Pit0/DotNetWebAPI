//Can be deleted

namespace DotNetWebApp.Controllers;
using AutoMapper;
using DotNetWebApp.Data;
using DotNetWebApp.Models;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("[controller]")]
[Obsolete]

public class UserJobInfoEFController : ControllerBase
{
    private readonly DataContextEF _entityFramework;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UserJobInfoEFController(IConfiguration config, IUserRepository userRepository)
        {
            _entityFramework = new DataContextEF(config);
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserJobInfo, UserJobInfo>();
            }));
            _userRepository = userRepository;
        }

        private readonly string[] _summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("GetAllUserJobInfo")]
        public IEnumerable<UserJobInfo> GetAllUserJobInfo()
        {
            return _userRepository.GetAllUserJobInfo();
        }

        [HttpGet("GetSingleUserJobInfo/{userId}")]
        public UserJobInfo GetSingleUserJobInfo(int userId)
        {
            var userJobInfo = _userRepository.GetSingleUserJobInfo(userId);
            
            if(userJobInfo != null) return userJobInfo;
            throw new Exception("Failed to Get UserJobInfo" );
        }

        [HttpPut("EditUserJobInfo")]
        public IActionResult EditUserJobInfo(UserJobInfo userForUpdate)
        {
            var userToUpdate = _userRepository.GetSingleUserJobInfo(userForUpdate.UserId);

            if (userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                throw new Exception("Updating UserJobInfo failed on save");
            }
            throw new Exception("Failed to find UserJobInfo to Update");
        }

        
        [HttpPost("AddUserJobInfoDb")]
        public IActionResult AddUserJobInfoDb(UserJobInfo userJobInfo)
        {
            var userJobInfoDb = _mapper.Map<UserJobInfo>(userJobInfo);

            _userRepository.AddEntity<UserJobInfo>(userJobInfoDb);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
        
            throw new Exception("Failed to Add UserJobInfo" );
        }

        [HttpDelete("DeleteUserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            var userJobInfoDb = _userRepository.GetSingleUserJobInfo(userId);

            if (userJobInfoDb != null)
            {
                _userRepository.RemoveEntity(userJobInfoDb);

                if (_userRepository.SaveChanges()) return Ok();
            }
            throw new Exception("Failed to Delete UserJobInfo" );
            
        }
}