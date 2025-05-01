using Backend.DTOs;

namespace BackendTest.DTOsTest;

[TestClass]
public class RemoveUserDtoTest
{
    [TestMethod]
    public void AddEmailToUser()
    {
        RemoveUserDto userDto = new RemoveUserDto();
        userDto.Email = "prodriguez@gmail.com";
        Assert.AreEqual("prodriguez@gmail.com", userDto.Email);
    }
}