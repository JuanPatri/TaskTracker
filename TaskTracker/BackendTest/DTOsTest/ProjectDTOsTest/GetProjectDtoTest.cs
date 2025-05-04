using Backend.DTOs.ProjectDTOs;

namespace BackendTest.DTOsTest.ProjectDTOsTest;

[TestClass]
public class GetProjectDtoTest
{
    [TestMethod]
    public void AddNameToProjectTest()
    {
        GetProjectDTO projectDto = new GetProjectDTO();
        projectDto.Name = "Project A";
        Assert.AreEqual("Project A", projectDto.Name);
    }
    
    [TestMethod]
    public void ToEntityShouldReturnProjectNameTest()
    {
        var dto = new GetProjectDTO()
        {
            Name = "Project A"
        };
        var result = dto.ToEntity();
        Assert.AreEqual("Project A", result.Name);
    }
    
}