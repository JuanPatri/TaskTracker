using Backend.DTOs;

namespace BackendTest.DTOsTest;

[TestClass]
public class GetUserDtOsTest
{
    [TestMethod]
    public void AddEmailToUser()
    {
        GetUserDTOs userDtOs = new GetUserDTOs();
        userDtOs.Email = "prodriguez@gmail.com";
        Assert.AreEqual("prodriguez@gmail.com", userDtOs.Email);
    }

    [TestMethod]
    public void ToEntityShouldReturnEmail()
    {
        var dto = new GetUserDTOs()
        {
            Email = "user@example.com"
        };
        
        var result = dto.ToEntity();
        
        Assert.AreEqual("user@example.com", result);
    }
}