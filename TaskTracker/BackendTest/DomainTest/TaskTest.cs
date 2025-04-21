namespace BackendTest.DomainTest;
using Backend.Domain;

[TestClass]
public class TaskTest
{
    private Task _task;

    [TestInitialize]
    public void OnInitialize()
    {
        _task = new Task("Tittle For Task");
    }
    
    [TestMethod]
    public void CreateTaskTest()
    {
        Assert.IsNotNull(_task);
    }

    [TestMethod]
    public void CreateTitleForTask()
    {

        Task task2 = new Task("Tittle For Task2");
        Assert.AreEqual("Tittle For Task2", task2.GetTitle());
    }
}