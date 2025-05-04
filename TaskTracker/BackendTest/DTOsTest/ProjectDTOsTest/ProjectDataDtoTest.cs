using Backend.DTOs.ProjectDTOs;

namespace BackendTest.DTOsTest.ProjectDTOsTest;

[TestClass]
public class ProjectDataDtoTest
{
    [TestMethod]
    public void CreateNameForProjectTest()
    {
        ProjectDataDTO projectDto = new ProjectDataDTO();
        projectDto.Name = "Project A";
        Assert.AreEqual("Project A", projectDto.Name);
    }
    
    [TestMethod]
    public void CreateDescriptionForProjectTest()
    {
        ProjectDataDTO projectDto = new ProjectDataDTO();
        projectDto.Description = "This is a test project.";
        Assert.AreEqual("This is a test project.", projectDto.Description);
    }
    
}