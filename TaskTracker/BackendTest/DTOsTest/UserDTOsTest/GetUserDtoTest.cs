using Backend.DTOs.UserDTOs;

namespace BackendTest.DTOsTest.UserDTOsTest;

[TestClass]
public class GetUserDtoTest
{
    [TestMethod]
    public void AddEmailToUser()
    {
        GetUserDTO userDto = new GetUserDTO();
        userDto.Email = "prodriguez@gmail.com";
        Assert.AreEqual("prodriguez@gmail.com", userDto.Email);
    }

    [TestMethod]
    public void ToEntityShouldReturnEmail()
    {
        var dto = new GetUserDTO()
        {
            Email = "user@example.com"
        };
        
        var result = dto.ToEntity();
        
        Assert.AreEqual("user@example.com", result.Email);
    }
}