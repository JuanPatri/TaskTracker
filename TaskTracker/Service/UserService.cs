using Domain;
using DTOs.UserDTOs;
using Repository;

namespace Service;

public class UserService
{
    private readonly IRepository<User> _userRepository;
    public  UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }
    
    public User AddUser(UserDataDTO user)
    {
        ValidateUserEmail(user.Email);
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
    
    public List<UserDataDTO> GetAllUsersDto()
    {
        return _userRepository.FindAll()
            .Select(user => new UserDataDTO
            {
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                BirthDate = user.BirthDate,
                Admin = user.Admin
            })
            .ToList();
    }

    public List<User> GetUsersFromEmails(IEnumerable<string> userEmails)
    {
        return _userRepository
            .FindAll()
            .Where(user => userEmails.Contains(user.Email))
            .ToList();
    }
    
    public void ValidateUserEmail(string email)
    {
        if (_userRepository.Find(u => u.Email == email) != null)
        {
            throw new ArgumentException("A user with this email already exists");
        }
    }

}