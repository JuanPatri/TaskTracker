using Backend.Domain;
using Backend.DTOs.ResourceDTOs;

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
        resourceDto.TypeResource = 1;
        Assert.AreEqual(1, resourceDto.TypeResource);
    }
    
    [TestMethod]
    public void ToEntityShouldMapAllPropertiesCorrectly()
    {
        ResourceDataDto resourceDto = new ResourceDataDto
        {
            Name = "ResourceName",
            Description = "ResourceDescription",
            TypeResource = 1
        };
        
        ResourceType resourceType = new ResourceType
        {
            Id = resourceDto.TypeResource,
            Name = "Human"
        };
        
        Resource resource = resourceDto.ToEntity(resourceType);

        Assert.IsNotNull(resource);
    }
}