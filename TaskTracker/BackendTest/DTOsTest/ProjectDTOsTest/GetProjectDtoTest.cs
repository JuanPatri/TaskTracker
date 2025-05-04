namespace BackendTest.DTOsTest.ProjectDTOsTest;

[TestClass]
public class GetProjectDtoTest
{
    [TestMethod]
    public void AddNameToProject()
    {
        GetProjectDTO projectDto = new GetProjectDTO();
        projectDto.Name = "Project A";
        Assert.AreEqual("Project A", projectDto.Name);
    }
    
}