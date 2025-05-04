using Backend.DTOs.ResourceDTOs;

namespace BackendTest.DTOsTest.ResourceDTOsTest;

[TestClass]
public class GetResourceDtoTest
{
    [TestMethod]
    public void CreateNameForResource()
    {
        GetResourceDto resourceDto = new GetResourceDto();
        resourceDto.Name = "resource";
        Assert.AreEqual("resource", resourceDto.Name);
    }
}