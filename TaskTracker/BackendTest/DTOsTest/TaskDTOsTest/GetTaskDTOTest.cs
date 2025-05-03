namespace BackendTest.DTOsTest.TaskDTOsTest;

[TestClass]
public class GetTaskDTOTest
{
    [TestMethod]
    public void AddTtileToTask()
    {
        GetTaskDTOTest taskDto = new GetTaskDTOTest();
        taskDto.Title = "Test";
        Assert.AreEqual("Test", taskDto.Title);
    }
}