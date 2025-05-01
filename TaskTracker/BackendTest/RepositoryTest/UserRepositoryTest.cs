using Backend.Repository;
using Backend.Domain;

namespace BackendTest.RepositoryTest;

[TestClass]
public class UserRepositoryTest
{
    private UserRepository _userRepository;
    //private User _user;
    //private User _updatedUser;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _userRepository = new UserRepository();
    }
    
    [TestMethod]
    public void CreateUserRepository()
    { 
        Assert.IsNotNull(_userRepository);
    }

    [TestMethod]
    public void AddUserToList()
    {
        User user = new User();
        user.Email = "usuario@nuevo.com";
        _userRepository.Add(user);
        Assert.AreEqual(_userRepository.Find(u => u.Email == "usuario@nuevo.com"), user);
    }
    
}