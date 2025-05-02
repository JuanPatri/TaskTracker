namespace BackendTest.ServiceTest;

[TestClass]
public class ProjectServiceTest
{
    [TestMethod]
    public void CreateProjectService()
    {
        ProjectService projectService = new ProjectService();
        Assert.IsNotNull(projectService);
    }
}