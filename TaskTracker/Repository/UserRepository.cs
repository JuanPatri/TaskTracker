using Domain;

namespace Repository;

public class UserRepository : IRepository<User>
{
    private readonly SqlContext _sqlContext;
    
    public UserRepository(SqlContext sqlContext)
    {
        _sqlContext = sqlContext;

        if (!_sqlContext.Users.Any(u => u.Email == "admin@admin.com"))
        {
            User newUser = new User()
            {
                Name = "Admin",
                LastName = "Admin",
                Email = "admin@admin.com",
                Password = "Admin123@",
                Admin = true,
                BirthDate = new DateTime(1990, 1, 1)
            };
        
            _sqlContext.Users.Add(newUser);
            _sqlContext.SaveChanges();
        }

    }
    
    public User Add(User user)
    {
        _sqlContext.Users.Add(user);
        _sqlContext.SaveChanges();
        return user;
    }

    public User? Find(Func<User, bool> predicate)
    {
        return _sqlContext.Users.FirstOrDefault(predicate);
    }

    public IList<User> FindAll()
    {
        return _sqlContext.Users.ToList();
    }

    public User? Update(User updatedUser)
    {
        _sqlContext.Users.Select(u => u.Email == updatedUser.Email ? updatedUser : u);
        // if (existingUser != null)
        // {
        //     existingUser.Name = updatedUser.Name;
        //     existingUser.LastName = updatedUser.LastName;
        //     existingUser.Password = updatedUser.Password;
        //     existingUser.Admin = updatedUser.Admin;
        //     existingUser.BirthDate = updatedUser.BirthDate;
        //
        //     return existingUser;
        // }
        
        _sqlContext.SaveChanges();
        return _sqlContext.Users.FirstOrDefault(u => u.Email == updatedUser.Email) ?? null;
    }

    public void Delete(String email)
    {
        User? user = _sqlContext.Users.FirstOrDefault(u => u.Email == email);
        _sqlContext.Users.Remove(user);
        _sqlContext.SaveChanges();
    }
}