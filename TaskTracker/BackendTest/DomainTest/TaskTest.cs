namespace BackendTest.DomainTest;
using Backend.Domain;

[TestClass]
public class TaskTest
{
    private Task _task;

    [TestInitialize]
    public void OnInitialize()
    {
        _task = new Task();
    }
    
    [TestMethod]
    public void CreateTaskTest()
    {
        Assert.IsNotNull(_task);
    }

    [TestMethod]
    public void CreateTittleForTask()
    {
        _task.Tittle = "Tittle For Task";
        Assert.AreEqual("Tittle For Task", _task.Tittle);
    }
}