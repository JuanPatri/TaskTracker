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
    public void CreateTitleForTask()
    {

        _task.ValidateTitle = "Tittle2";
        Assert.AreEqual("Tittle2", _task.ValidateTitle);
    }
}