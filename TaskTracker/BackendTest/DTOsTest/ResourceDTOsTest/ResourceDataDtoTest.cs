using Backend.DTOs.Resource;

namespace BackendTest.DTOsTest.ResourceDTOsTest;

[TestClass]
public class ResourceDataDtoTest
{
    [TestMethod]
    public void CreateNameForResource()
    {
        ResourceDataDto resourceDto = new ResourceDataDto();
        resourceDto.Name = "Pedro";
        Assert.AreEqual("Pedro", resourceDto.Name);
    }
}