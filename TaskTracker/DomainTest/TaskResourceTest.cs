using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Task = Domain.Task;

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
        Task task = new Task();
        
        _taskResource.Task = task;
        Assert.AreEqual(task, _taskResource.Task);
    }
    
    [TestMethod]
    public void CreateResourceIdForTaskResource()
    {
        Resource resource = new Resource();
        _taskResource.Resource = resource;
        Assert.AreEqual(resource, _taskResource.Resource);
    }
    
    [TestMethod]
    public void CreateQuantityForTaskResource()
    {
        _taskResource.Quantity = 5;
        Assert.AreEqual(5, _taskResource.Quantity);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PutQuantityNegativeReturnsAnException()
    {
        _taskResource.Quantity = -1;
    }
    
    [TestMethod]
    public void AddResourceToTaskResource()
    {
        Resource resource = new Resource();
        _taskResource.Resource = resource;
        Assert.IsNotNull(_taskResource.Resource);
        Assert.AreEqual(resource, _taskResource.Resource);
    }
    
    [TestMethod]
    public void SetValidIdForTaskResource()
    {
        _taskResource.Id = 10;
        Assert.AreEqual(10, _taskResource.Id);
    }
    
    [TestMethod]
    public void SetZeroIdForTaskResource()
    {
        _taskResource.Id = 0;
        Assert.AreEqual(0, _taskResource.Id);
    }
}