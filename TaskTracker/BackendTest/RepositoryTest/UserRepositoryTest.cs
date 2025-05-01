using Backend.Repository;
using Backend.Domain;

namespace BackendTest.RepositoryTest;

[TestClass]
public class UserRepositoryTest
{
    private UserRepository _userRepository;
    //private User _user;
    //private User _updatedUser;
    
    [TestMethod]
    public void CreateUserRepository()
    {
        _userRepository = new UserRepository();
        Assert.IsNotNull(_userRepository);
    }
    
}