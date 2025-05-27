using Backend.Domain;
using DTOs.SessionDTOs;
using Backend.DTOs.UserDTOs;
using Domain;
using DTOs.SessionDTOs;


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
        if (user.Password != loginDto.Password)
        {
            throw new ArgumentException("Invalid password");
        }
        
        CurrentUser = user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public bool IsLoggedIn()
    {
        return CurrentUser != null;
    }
}