using Backend.Domain;
using Backend.DTOs.UserDTOs;
using Backend.Repository;

namespace Backend.Service;

public class UserService
{
    private readonly IRepository<User> _userRepository;
    public  UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public User AddUser(UserDataDTO user)
    {
        User newUser = User.FromDto(user);
        return _userRepository.Add(newUser);
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
        User updatedUser = User.FromDto(userDto);
        return _userRepository.Update(updatedUser);
    }

    public List<User> GetUserWithEmail(List<string> emails)
    {
        List<User> users = new List<User>();
        
        emails.ForEach(email =>
        {
            User? user = _userRepository.Find(u => u.Email == email);
            if (user != null)
            {
                users.Add(user);
            }
        });
        
        return users;
    }
}