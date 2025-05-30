using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainTest;

[TestClass]
public class TaskResourceTest
{
    private TaskResource _taskResource;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _taskResource = new TaskResource();
    }
    
    [TestMethod]
    public void TaskResourceIsNotNull()
    {
        Assert.IsNotNull(_taskResource);
    }

    [TestMethod]
    public void CreateTaskIdForTaskResource()
    {
        _taskResource.TaskId = 1;
        Assert.AreEqual(1, _taskResource.TaskId);
    }
}