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
    
    [TestMethod]
    public void CreateResourceIdForTaskResource()
    {
        _taskResource.ResourceId = 2;
        Assert.AreEqual(2, _taskResource.ResourceId);
    }
    
    [TestMethod]
    public void CreateQuantityForTaskResource()
    {
        _taskResource.Quantity = 5;
        Assert.AreEqual(5, _taskResource.Quantity);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PutQuantityZeroReturnsAnException()
    {
        _taskResource.Quantity = 0;
    }
    
    [TestMethod]
    public void AddResourceToTaskResource()
    {
        Resource resource = new Resource();
        _taskResource.Resource = resource;
        Assert.IsNotNull(_taskResource.Resource);
        Assert.AreEqual(resource, _taskResource.Resource);
    }
}