namespace BackendTest.DTOsTest.TaskDTOsTest;
using Backend.DTOs.TaskDTOs;
using Backend.Domain.Enums;

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
    
    [TestMethod]
    public void SetDurationForTask()
    {
        _taskDataDto.Duration = TimeSpan.FromHours(1);
        Assert.AreEqual(TimeSpan.FromHours(1), _taskDataDto.Duration);
    }
    
    [TestMethod]
    public void SetStatusForTask()
    {
        _taskDataDto.Status = Status.Blocked;
        Assert.AreEqual(Status.Blocked, _taskDataDto.Status);
    }
}