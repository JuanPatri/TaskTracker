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
    
    [TestMethod]
    public void AddStartDateToProjectTest()
    {
        ProjectDataDTO projectDto = new ProjectDataDTO();
        DateTime startDate = new DateTime(2025, 10, 01);
        projectDto.StartDate = startDate;
        Assert.AreEqual(startDate, projectDto.StartDate);
    }
    
    [TestMethod]
    public void AddFinishDateToProjectTest()
    {
        ProjectDataDTO projectDto = new ProjectDataDTO();
        DateTime finishDate = new DateTime(2025, 12, 31);
        projectDto.FinishDate = finishDate;
        Assert.AreEqual(finishDate, projectDto.FinishDate);
    }
}