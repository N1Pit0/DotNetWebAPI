using System.Data;
using System.Security.Cryptography;
using Dapper;
using DotNetWebApp.Data;
using Microsoft.AspNetCore.Mvc;
using DotNetWebApp.DTOs;
using DotNetWebApp.helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;

namespace DotNetWebApp.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly AuthHelper _authHelper;

    public AuthController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _authHelper = new AuthHelper(config);
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistrationDto)
    {
        if (userForRegistrationDto.Password == userForRegistrationDto.PasswordConfirm)
        {
            var sqlCheckUserExists = "SELECT [Email] FROM TutorialAppSchema.Auth WHERE Email ='"
                                     + userForRegistrationDto.Email + "'";

            IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

            if (!existingUsers.Any())
            {
                UserForLoginDto userForSetPassword = new UserForLoginDto()
                {
                    Email = userForRegistrationDto.Email,
                    Password = userForRegistrationDto.Password
                };
                
                if (_authHelper.SetPassword(userForSetPassword))
                {
                    string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert
                            @FirstName = '" + userForRegistrationDto.FirstName + 
                                        "', @LastName = '" + userForRegistrationDto.LastName +
                                        "', @Email = '" + userForRegistrationDto.Email + 
                                        "', @Gender = '" + userForRegistrationDto.Gender + 
                                        "', @Active = 1" + 
                                        ", @JobTitle = '" + userForRegistrationDto.JobTitle + 
                                        "', @Department = '" + userForRegistrationDto.Department + 
                                        "', @Salary = '" + userForRegistrationDto.Salary + "'";
                    if (_dapper.Execute(sqlAddUser))
                    {
                        return Ok();
                    }

                    throw new Exception("Failed to add user.");
                }
            }

            throw new Exception("User already exists");
        }

        throw new Exception("Passwords do not match");
    }

    [HttpPut("ResetPassword")]
    public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
    {
        if (_authHelper.SetPassword(userForSetPassword))
        {
            return Ok();
        }

        throw new Exception("Failed to update the password");
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLoginDto)
    {
        string sqlForHashAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get
                                   @Email = @EmailParam";

        DynamicParameters sqlParameters = new DynamicParameters();

        // SqlParameter passwordSaltParameter = new SqlParameter("@EmailParam", SqlDbType.Binary);
        // passwordSaltParameter.Value = userForLoginDto.Email;
        // sqlParameters.Add(passwordSaltParameter);
        
        sqlParameters.Add("@EmailParam", userForLoginDto.Email, DbType.String);
        
        UserForLoginConfirmationDto userForLoginConfirmation =
            _dapper.LoadDataSingleWithParameters<UserForLoginConfirmationDto>(sqlForHashAndSalt,sqlParameters);

        byte[] passwordHash =
            _authHelper.GetPasswordHash(userForLoginDto.Password, userForLoginConfirmation.PasswordSalt);

        if (!passwordHash.SequenceEqual(userForLoginConfirmation.PasswordHash))
        {
            return StatusCode(401, "Incorrect password");
        }

        string userIdSql = @"
                SELECT 
                        UserId FROM TutorialAppSchema.Users WHERE Email = '" +
                           userForLoginDto.Email + "'";

        int userId = _dapper.LoadDataSingle<int>(userIdSql);

        return Ok(new Dictionary<string, string>
        {
            { "token", _authHelper.CreateToken(userId) }
        });
    }

    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken()
    {
        string userId = User.FindFirst("userId")?.Value + "";

        string useridSql = "SELECT userId FROM TutorialAppSchema.Users WHERE userId = "
                           + userId;

        int useridFromDb = _dapper.LoadDataSingle<int>(useridSql);

        return Ok(new Dictionary<string, string>
        {
            { "token", _authHelper.CreateToken(useridFromDb) }
        });
    }
}