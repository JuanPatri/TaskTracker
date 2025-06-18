using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Task = Domain.Task;

namespace DomainTest;

[TestClass]
public class TaskDependencyTest
{
    private TaskDependency _taskDependency;

    [TestInitialize]
    public void OnInitialize()
    {
        _taskDependency = new TaskDependency();
    }
    
    [TestMethod]
    public void TaskDependencyIsNotNull()
    {
        Assert.IsNotNull(_taskDependency);
    }
    
    [TestMethod]
    public void SetValidIdForTaskDependency()
    {
        _taskDependency.Id = 1;
        Assert.AreEqual(1, _taskDependency.Id);
    }
    
    [TestMethod]
    public void SetValidTaskForTaskDependency()
    {
        Task task = new Task { Title = "Task 1", Description = "Description 1" };
        _taskDependency.Task = task;
        Assert.AreEqual(task, _taskDependency.Task);
    }
}