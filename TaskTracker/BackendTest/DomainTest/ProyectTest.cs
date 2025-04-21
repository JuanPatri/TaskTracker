using Backend.Domain;
namespace BackendTest.DomainTest;

[TestClass]
public class ProyectTest
{
    private Proyect proyect;
    [TestInitialize]
    public void OnInitialize()
    {
        proyect = new Proyect();
    }
    
    [TestMethod]
    public void CreateProyect()
    {
        Assert.IsNotNull(proyect);
    }
}