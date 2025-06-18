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
}