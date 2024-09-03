using DotNetWebApp.Models;

namespace DotNetWebApp.Data;
[Obsolete]
public interface IUserRepository
{

    public bool SaveChanges();

    public void AddEntity<T>(T entityToAdd);

    public void RemoveEntity<T>(T entityToAdd);

    public IEnumerable<User> GetUsers();

    public User GetSingleUser(int userId);

    public IEnumerable<UserSalary> GetAllUserSalary();

    public UserSalary GetSingleUserSalary(int userId);

    public IEnumerable<UserJobInfo> GetAllUserJobInfo();

    public UserJobInfo GetSingleUserJobInfo(int userId);

}