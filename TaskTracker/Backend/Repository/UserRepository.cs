using Backend.Domain;

namespace Backend.Repository;

public class UserRepository : IRepository<User>
{
    private readonly List<User> _users;
    
    public UserRepository()
    {
        _users = new List<User>()
        {
            new User()
            {
                Name = "Admin",
                LastName = "Admin",
                Email = "admin@admin.com",
                Password = "Admin123@",
                Admin = true,
                BirthDate = new DateTime(1990, 1, 1)
            }
        };
    }
    
    public User Add(User user)
    {
        _users.Add(user);
        return user;
    }

    public User? Find(Func<User, bool> predicate)
    {
        return _users.FirstOrDefault(predicate);
    }

    public IList<User> FindAll()
    {
        return _users;
    }

    public User? Update(User updatedUser)
    {
        User? existingUser = _users.FirstOrDefault(u => u.Email == updatedUser.Email);
        if (existingUser != null)
        {
            existingUser.Name = updatedUser.Name;
            existingUser.LastName = updatedUser.LastName;
            existingUser.Password = updatedUser.Password;
            existingUser.Admin = updatedUser.Admin;
            existingUser.BirthDate = updatedUser.BirthDate;

            return existingUser;
        }
        return null;
    }

    public void Delete(String email)
    {
        User? user = _users.FirstOrDefault(u => u.Email == email);
        if (user != null)
        {
            _users.Remove(user);
        }
    }
}