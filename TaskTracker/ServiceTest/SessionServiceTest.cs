using Repository;
using Domain;
using DTOs.SessionDTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepositoryTest.Context;
using Service;

namespace BackendTest.ServiceTest;

[TestClass]
public class SessionServiceTest
{
    private SessionService _sessionService;
    private UserService _userService;
    private UserRepository _userRepository;
    private User _user;
    private SqlContext _sqlContext;

    [TestInitialize]
    public void OnInitialize()
    {
        _sqlContext = SqlContextFactory.CreateMemoryContext();
        _userRepository = new UserRepository(_sqlContext);
        _userService = new UserService(_userRepository);
        _sessionService = new SessionService(_userService);
        _user = new User
        {
            Email = "pedro@gmail.com",
            Name = "Pedro",                    
            LastName = "García",               
            Password = "Pedro123!",
            BirthDate = new DateTime(1990, 1, 1), 
            Admin = false
        };
        _userRepository.Add(_user);
        
    }

    [TestMethod]
    public void ValidLoginShouldSetCurrentUser()
    {

        LoginDto loginDto = new LoginDto()
        {
            Email = _user.Email,
            Password = _user.Password
        };
        
        _sessionService.Login(loginDto);
        
        Assert.AreEqual(_sessionService.CurrentUser, _user);
    }

    [TestMethod]
    public void NullLoginReturnException()
    {

        LoginDto loginDto = new LoginDto()
        {
            Email = "null@gmail.com",
            Password = "Null123!"
        };
        
        Assert.ThrowsException<ArgumentException>(() => _sessionService.Login(loginDto));
    }
    
    [TestMethod]
    public void LogoutShouldSetCurrentUserToNull()
    {
        LoginDto loginDto = new LoginDto()
        {
            Email = _user.Email,
            Password = _user.Password
        };
        
        _sessionService.Login(loginDto);
        
        Assert.AreEqual(_sessionService.CurrentUser, _user);
        
        _sessionService.Logout();
        
        Assert.IsNull(_sessionService.CurrentUser);
    }
    
    [TestMethod]
    public void LoginWithInvalidPasswordShouldThrowException()
    {
        LoginDto loginDto = new LoginDto()
        {
            Email = _user.Email,
            Password = "InvalidPassword"
        };
        
        Assert.ThrowsException<ArgumentException>(() => _sessionService.Login(loginDto));
    }

    [TestMethod]
    public void SomeoneIsLoggedInReturnBoolean()
    {
        Assert.IsFalse(_sessionService.IsLoggedIn());
        
        LoginDto loginDto = new LoginDto()
        {
            Email = _user.Email,
            Password = _user.Password
        };
        
        _sessionService.Login(loginDto);
        Assert.IsTrue(_sessionService.IsLoggedIn());
    }
 
}