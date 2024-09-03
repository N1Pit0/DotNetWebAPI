﻿using System.Data;
using Dapper;
using DotNetWebApp.Data;
using DotNetWebApp.DTOs;
using Microsoft.AspNetCore.Mvc;
using DotNetWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace DotNetWebApp.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    public PostController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
    public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None")
    {
        string sql = @"EXEC TutorialAppSchema.spPosts_Get";
        string parameters = "";
        DynamicParameters sqlParameters = new DynamicParameters();

        if (postId != 0)
        {
            parameters += ", @PostId=@PostIdParameter";
            sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);
        }

        if (userId != 0)
        {
            parameters += ", @UserId=@UserIdParameter";
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
        }

        if (searchParam.ToLower() != "none")
        {
            parameters += ", @SearchValue=@SearchValueParameter";
            sqlParameters.Add("@SearchValueParameter", searchParam, DbType.String);
        }

        if (parameters.Length > 0)
        {
            sql += parameters.Substring(1);
        }

        return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
    }

    [HttpGet("GetMyPosts")]
    public IEnumerable<Post> GetMyPosts()
    {
        string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId=@UserIdParameter";
        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);

        return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
    }

    [HttpPut("UpsertPost")]
    public IActionResult UpsertPost(Post postToUpsert)
    {
        DynamicParameters sqlParameters = new DynamicParameters();
        
        string sql = @"EXEC TutorialAppSchema.spPosts_Upsert 
                    @UserId=@UserIdParameter,
                    @PostTitle=@PostTitleParameter,
                    @PostContent=@PostContentParameter";

        sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
        sqlParameters.Add("@PostTitleParameter", postToUpsert.PostTitle, DbType.String);
        sqlParameters.Add("@PostContentParameter", postToUpsert.PostContent, DbType.String);
        
        if (postToUpsert.PostId > 0)
        {
            sql += ", @PostId=@PostIdParameter";
            sqlParameters.Add("@PostIdParameter", postToUpsert.PostId, DbType.Int32);
        }

        if (_dapper.ExecuteSqlWithParameter(sql, sqlParameters))
        {
            return Ok();
        }

        throw new Exception("Failed to upsert post!");
    }

    [HttpDelete("DeletePost/{postId}")]
    public IActionResult DeletePost(int postId)
    {
        string sql = @"EXEC TutorialAppSchema.spPost_Delete 
                @UserId=@UserIdParameter, 
                @PostId=@PostIdParameter";

        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
        sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);

        if (_dapper.ExecuteSqlWithParameter(sql, sqlParameters))
        {
            return Ok();
        }

        throw new Exception("Failed to delete post!");
    }
}