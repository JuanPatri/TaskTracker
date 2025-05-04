using Backend.Domain;
using Backend.DTOs.UserDTOs;
using Backend.Repository;

namespace Backend.Service;

public class 
    UserService
{
    private readonly IRepository<User> _userRepository;
    public  UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public User AddUser(UserDataDTO user)
    {
        User createdUser = user.ToEntity();
        return _userRepository.Add(createdUser);
    }
    
    public void RemoveUser(GetUserDTO user)
    {
        _userRepository.Delete(user.Email);
    }
    
    public User? GetUser(GetUserDTO user)
    {
        return _userRepository.Find(u => u.Email == user.Email);
    }

    public List<User> GetAllUsers()
    {
        return _userRepository.FindAll().ToList();
    }

    public User? UpdateUser(UserDataDTO userDto)
    {
        User updatedUser = userDto.ToEntity();
        return _userRepository.Update(updatedUser);
    }
}