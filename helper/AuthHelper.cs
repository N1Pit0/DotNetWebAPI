﻿using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using DotNetWebApp.Data;
using DotNetWebApp.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotNetWebApp.helper;

public class AuthHelper
{
    private readonly IConfiguration _config;
    private readonly DataContextDapper _dapper;
    public AuthHelper(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _config = config;
    }
    
    public byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
        string passwordSaltPlusString = _config.GetSection("AppsSettings:PasswordKey").Value
                                        + Convert.ToBase64String(passwordSalt);

        byte[] passwordHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8
        );

        return passwordHash;
    }

    public string CreateToken(int userId)
    {
        Claim[] claims = new Claim[] {
            new Claim("userId", userId.ToString())
        };

        string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

        var tokenKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(tokenKeyString ?? "")
        );

        SigningCredentials credentials = new SigningCredentials(
            tokenKey,
            SecurityAlgorithms.HmacSha512Signature
        );

        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.Now.AddDays(1)
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        SecurityToken token = tokenHandler.CreateToken(descriptor);

        return tokenHandler.WriteToken(token);

    }

    public bool SetPassword(UserForLoginDto userForSetPassword)
    {
        byte[] passwordSalt = new byte[128 / 8];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetNonZeroBytes(passwordSalt);
        }

        byte[] passwordHash = GetPasswordHash(userForSetPassword.Password, passwordSalt);

        string sqlAddAuth = @"EXEC
                                    TutorialAppSchema.spRegistration_Upsert
                                    @Email = @EmailParam, 
                                    @PasswordHash = @PasswordHashParam, 
                                    @PasswordSalt = @PasswordSaltParam";

        // List<SqlParameter> sqlParameters = new List<SqlParameter>();
        //
        // SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltParam", SqlDbType.Binary);
        // passwordSaltParameter.Value = passwordSalt;
        // sqlParameters.Add(passwordSaltParameter);
        //
        // SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashParam", SqlDbType.Binary);
        // passwordHashParameter.Value = passwordHash;
        // sqlParameters.Add(passwordHashParameter);
        //
        // SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
        // emailParameter.Value = userForSetPassword.Email;
        // sqlParameters.Add(emailParameter);

        DynamicParameters sqlParameters = new DynamicParameters();
        
        sqlParameters.Add("@EmailParam",userForSetPassword.Email, DbType.String);
        sqlParameters.Add("@PasswordSaltParam",passwordSalt, DbType.Binary);
        sqlParameters.Add("@PasswordHashParam",passwordHash, DbType.Binary);

        return _dapper.ExecuteSqlWithParameter(sqlAddAuth, sqlParameters);
    }
}