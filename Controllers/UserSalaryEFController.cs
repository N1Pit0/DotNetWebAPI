//Can be deleted

namespace DotNetWebApp.Controllers;

using AutoMapper;
using DotNetWebApp.Data;
using DotNetWebApp.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
[Obsolete]

public class UserSalaryEFController: ControllerBase
{
    private readonly DataContextEF _entityFramework;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public UserSalaryEFController(IConfiguration config, IUserRepository userRepository)
    {
        _entityFramework = new DataContextEF(config);
        _mapper = new Mapper(new MapperConfiguration(cfg => { cfg.CreateMap<UserSalary, UserSalary>(); }));
        _userRepository = userRepository;
    }
    
    [HttpGet("GetAllUserSalary")]
    public IEnumerable<UserSalary> GetAllUserSalary()
    {
        return _userRepository.GetAllUserSalary();
    }

    [HttpGet("GetSingleUserSalary/{userId}")]
    public UserSalary GetSingleUserSalary(int userId)
    {
        return _userRepository.GetSingleUserSalary(userId);
    }
    
    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary userSalary)
    {
        var userToUpdate = _userRepository.GetSingleUserSalary(userSalary.UserId);

        if (userToUpdate != null)
        {
            _mapper.Map(userSalary, userToUpdate);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Updating UserSalary failed on save");
        }
        throw new Exception("Failed to find UserSalary to Update");
    }

    [HttpPost("AddUserSalary")]
    public IActionResult AddUserSalary(UserSalary userSalary)
    {
        
        var userUserSalaryDb = _mapper.Map<UserSalary>(userSalary);

        _userRepository.AddEntity(userUserSalaryDb);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        throw new Exception("Adding UserSalary failed on save");
    }

    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        var userToDelete = _userRepository.GetSingleUserSalary(userId);

        if (userToDelete != null)
        {
            _userRepository.RemoveEntity(userToDelete);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Deleting UserSalary failed on save");
        }
        throw new Exception("Failed to find UserSalary to delete");
    }
}