using Backend.DTOs.Resource;

namespace BackendTest.DTOsTest.ResourceDTOsTest;

[TestClass]
public class ResourceDataDtoTest
{
    [TestMethod]
    public void CreateNameForResource()
    {
        ResourceDataDto resourceDto = new ResourceDataDto();
        resourceDto.Name = "resource";
        Assert.AreEqual("resource", resourceDto.Name);
    }
    
    [TestMethod]
    public void CreateDescriptionForResource()
    {
        ResourceDataDto resourceDto = new ResourceDataDto();
        resourceDto.Description = "description";
        Assert.AreEqual("description", resourceDto.Description);
    }
    
    [TestMethod]
    public void CreateTypeResourceForResource()
    {
        ResourceDataDto resourceDto = new ResourceDataDto();
        resourceDto.TypeResource = "type";
        Assert.AreEqual("type", resourceDto.TypeResource);
    }
}