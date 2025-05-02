using Backend.Domain;
using Backend.DTOs;
using Backend.Repository;

namespace Backend.Service;

public class UserService
{
    private readonly IRepository<User> _userRepository;
    public  UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public User AddUser(CreateUserDTOs user)
    {
        User createdUser = user.ToEntity();
        return _userRepository.Add(createdUser);
    }
    
    public void RemoveUser(GetUserDTOs user)
    {
        _userRepository.Delete(user.Email);
    }
}