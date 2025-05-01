using Backend.Domain;
using Backend.DTOs;

namespace BackendTest.DTOsTest;

[TestClass]
public class CreateUserDTOsTest
{
    [TestMethod]
    public void CreateNameForUser()
    {
        CreateUserDTOs userDto = new CreateUserDTOs();
        userDto.Name = "Pedro";
        Assert.AreEqual("Pedro", userDto.Name);
    }
    
    [TestMethod]
    public void CreateLastNameForUser()
    {
        CreateUserDTOs userDto = new CreateUserDTOs();
        userDto.LastName = "Rodriguez";
        Assert.AreEqual("Rodriguez", userDto.LastName);
    }
    
    [TestMethod]
    public void AddEmailToUser()
    {
        CreateUserDTOs userDto = new CreateUserDTOs();
        userDto.Email = "prodriguez@gmail.com";
        Assert.AreEqual("prodriguez@gmail.com", userDto.Email);
    }

    
}