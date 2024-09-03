//Can be deleted

using AutoMapper;
using DotNetWebApp.Data;
using DotNetWebApp.DTOs;
using DotNetWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNetWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Obsolete]

    public class UserEFController : ControllerBase
    {
        private readonly DataContextEF _entityFramework;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UserEFController(IConfiguration config, IUserRepository userRepository)
        {
            _entityFramework = new DataContextEF(config);
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserToAddDto, User>();
            }));
            _userRepository = userRepository;
        }

        private readonly string[] _summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("GetUsers")]
        public IEnumerable<User> GetUsers()
        {
            return _userRepository.GetUsers();
        }

        [HttpGet("GetSingleUser/{userId}")]
        public User GetSingleUser(int userId)
        {
            return _userRepository.GetSingleUser(userId);
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {
            var userDb = _userRepository.GetSingleUser(user.UserId);
            
            if (userDb != null)
            {
                userDb.Active = user.Active;
                userDb.FirstName = user.FirstName;
                userDb.LastName = user.LastName;
                userDb.Email = user.Email;
                userDb.Gender = user.Gender;
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                } 

                throw new Exception("Failed to Update User");
            }
        
            throw new Exception("Failed to Get User");
        }

        
        [HttpPost("AddUser")]
        public IActionResult AddUser(UserToAddDto userToAdd)
        {
            var userDb = _mapper.Map<User>(userToAdd);

            _userRepository.AddEntity<User>(userDb);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
        
            throw new Exception("Failed to Add User" );
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            var userDb = _userRepository.GetSingleUser(userId);

            if (userDb != null)
            {
                _userRepository.RemoveEntity(userDb);

                if (_userRepository.SaveChanges()) return Ok();
            }
            throw new Exception("Failed to Delete User" );
            
        }
        
    }
}
    