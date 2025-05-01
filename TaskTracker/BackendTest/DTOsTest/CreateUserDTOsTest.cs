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
}