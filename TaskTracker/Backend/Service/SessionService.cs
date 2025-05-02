using Backend.Domain;
using Backend.DTOs.SessionDTOs;
using Backend.DTOs.UserDTOs;

namespace Backend.Service;

public class SessionService
{
    private UserService _userService;
    public User? CurrentUser { get; private set; }
    
    public SessionService(UserService userService)
    {
        _userService = userService;
    }
    public void Login(LoginDto loginDto)
    {
        GetUserDTO userDto = new GetUserDTO()
        {
            Email = loginDto.Email
        };
        User? user = _userService.GetUser(userDto);
        
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }
        
        CurrentUser = user;
    }
}