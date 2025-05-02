using System.Runtime.InteropServices.JavaScript;
using Backend.Domain;
using Backend.DTOs;
using Backend.Repository;

namespace BackendTest.ServiceTest;
using Backend.Service;

[TestClass]
public class UserServiceTest
{
    private UserService _userService;
    private UserRepository _userRepository;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _userRepository = new UserRepository();
        _userService = new UserService(_userRepository);
    }
    
    [TestMethod]
    public void CreateUserService()
    {
        Assert.IsNotNull(_userService);
    }

    [TestMethod]
    public void AddUserShouldReturnUser()
    {
        CreateUserDTOs user = new CreateUserDTOs()
        {
            Name = "Pedro",
            LastName = "Rodriguez",
            BirthDate = new DateTime(2003, 03, 14),
            Email = "user@user.com",
            Password = "Pedro1234@"
        };
        User? createdUser = _userService.AddUser(user);
        Assert.IsNotNull(createdUser);
        Assert.AreEqual(_userRepository.FindAll().Last(), createdUser);
    }

    [TestMethod]
    public void RemoveUserShouldRemoveUser()
    {
        User user = new User()
        {
            Email = "pedro@gmail.com"
        };
        _userRepository.Add(user);

        GetUserDTOs userToDelete = new GetUserDTOs()
        {
            Email = "pedro@gmail.com"
        };
        _userService.RemoveUser(userToDelete);
    }
    
    
}