using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;

namespace BackendTest.RepositoryTest;

[TestClass]
public class UserRepositoryTest
{
    private UserRepository _userRepository;
    private User _user;
    //private User _updatedUser;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _userRepository = new UserRepository();
        _user = new User();
        _user.Email = "usuario@nuevo.com";
    }
    
    [TestMethod]
    public void CreateUserRepository()
    { 
        Assert.IsNotNull(_userRepository);
    }

    [TestMethod]
    public void AddUserToList()
    {
        _userRepository.Add(_user);
        Assert.AreEqual(_userRepository.Find(u => u.Email == "usuario@nuevo.com"), _user);
    }

    [TestMethod]
    public void SearchForAllUserInTheList()
    {
        Assert.AreEqual(_userRepository.FindAll().Count, 1);
        _userRepository.Add(_user);
        Assert.AreEqual(_userRepository.FindAll().Count, 2);
    }

    [TestMethod]
    public void UpdateExistingUserUpdatesFieldsCorrectly()
    {
        _user.Name = "Name";
        _userRepository.Add(_user);
        User updateUser = new User()
        {
            Name = "UpdatedName",
            LastName = "UpdatedLastName",
            Password = "UpdatedPassword123!",
            Admin = false,
            BirthDate = new DateTime(1995, 5, 5),
            Email = "usuario@nuevo.com"
        };
        _userRepository.Update(updateUser);
        Assert.AreEqual(_user.Name, "UpdatedName");
    }

    [TestMethod]
    public void UpdatingAUserThatIsNotInTheListReturnsNull()
    {
        _user.Name = "Name";
        _userRepository.Add(_user);
        User updateUser = new User()
        {
            Name = "UpdatedName",
            LastName = "UpdatedLastName",
            Password = "UpdatedPassword123!",
            Admin = false,
            BirthDate = new DateTime(1995, 5, 5),
            Email = "Email@incorrecto.com"
        };
        _userRepository.Update(updateUser);
        Assert.AreEqual(_userRepository.Update(updateUser), null);
    }
    
    [TestMethod]
    public void DeleteUserFromList()
    {
        _userRepository.Add(_user);
        Assert.AreEqual(_userRepository.FindAll().Count, 2);
        _userRepository.Delete(_user.Email);
        Assert.AreEqual(_userRepository.FindAll().Count, 1);
    }
}