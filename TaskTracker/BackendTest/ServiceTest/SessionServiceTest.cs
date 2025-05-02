using Backend.Domain;
using Backend.DTOs.SessionDTOs;
using Backend.Repository;
using Backend.Service;

namespace BackendTest.ServiceTest;

[TestClass]
public class SessionServiceTest
{
    private SessionService _sessionService;
    private UserService _userService;
    private UserRepository _userRepository;
    private User _user;

    [TestMethod]
    public void ValidLoginShouldSetCurrentUser()
    {
        _userRepository = new UserRepository();
        _userService = new UserService(_userRepository);
        _sessionService = new SessionService(_userService);
        _user = new User()
        {
            Email = "pedro@gmail.com",
            Password = "Pedro123!"
        };
        
        _userRepository.Add(_user);
        
        LoginDto loginDto = new LoginDto()
        {
            Email = _user.Email,
            Password = _user.Password
        };
        
        _sessionService.Login(loginDto);
        
        Assert.AreEqual(_sessionService.CurrentUser, _user);
    }
}