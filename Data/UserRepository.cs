using DotNetWebApp.Models;

namespace DotNetWebApp.Data;
[Obsolete]
public class UserRepository : IUserRepository
{
    private readonly DataContextEF _entityFramework;

    public UserRepository(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
    }

    public bool SaveChanges()
    {
        return _entityFramework.SaveChanges() > 0;
    }

    public void AddEntity<T>(T entityToAdd)
    {
        if(entityToAdd != null) _entityFramework.Add(entityToAdd);
    }
    
    public void RemoveEntity<T>(T entityToAdd)
    {
        if(entityToAdd != null) _entityFramework.Remove(entityToAdd);
    }
    
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _entityFramework.Users.ToList<User>();
        return users;
    }
    
    public User GetSingleUser(int userId)
    {
        var user = _entityFramework.Users
            .FirstOrDefault<User>(u => u.UserId == userId);
            
        if(user != null) return user;
        throw new Exception("Failed to Get User" );
        
    }
    
    public IEnumerable<UserSalary> GetAllUserSalary()
    {
        IEnumerable<UserSalary> users = _entityFramework.UserSalary.ToList<UserSalary>();
        return users;
    }
    
    public UserSalary GetSingleUserSalary(int userId)
    {
        var userSalary = _entityFramework.UserSalary
            .FirstOrDefault<UserSalary>(u => u.UserId == userId);
            
        if(userSalary != null) return userSalary;
        throw new Exception("Failed to Get User" );
    }
    
    public IEnumerable<UserJobInfo> GetAllUserJobInfo()
    {
        IEnumerable<UserJobInfo> users = _entityFramework.UserJobInfo.ToList<UserJobInfo>();
        return users;
    }
    public UserJobInfo GetSingleUserJobInfo(int userId)
    {
        var userJobInfo = _entityFramework.UserJobInfo
            .FirstOrDefault<UserJobInfo>(u => u.UserId == userId);
            
        if(userJobInfo != null) return userJobInfo;
        throw new Exception("Failed to Get User" );
    }

    
}
