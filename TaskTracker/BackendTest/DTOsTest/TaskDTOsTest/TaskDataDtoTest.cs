namespace BackendTest.DTOsTest.TaskDTOsTest;
using Backend.DTOs.TaskDTOs;

[TestClass]
public class TaskDataDtoTest
{

    private TaskDataDTO _taskDataDto;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _taskDataDto = new TaskDataDTO();
    }
    
    [TestMethod]
    public void SetTitleForTask()
    {
        _taskDataDto.Title = "Task 1";
        Assert.AreEqual("Task 1", _taskDataDto.Title);
    }

    [TestMethod]
    public void SetDescriptionForTask()
    {
        _taskDataDto.Description = "Description of Task 1";
        Assert.AreEqual("Description of Task 1", _taskDataDto.Description);
    }
}