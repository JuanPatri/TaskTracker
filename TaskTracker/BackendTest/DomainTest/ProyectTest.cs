using Backend.Domain;
namespace BackendTest.DomainTest;

[TestClass]
public class ProyectTest
{
    private Proyect _proyect;
    [TestInitialize]
    public void OnInitialize()
    {
        _proyect = new Proyect("Proyect1");
    }
    
    [TestMethod]
    public void CreateProyect()
    {
        Assert.IsNotNull(_proyect);
    }
    
    [TestMethod]
    public void CreateNameForProyect()
    {
        Proyect proyect2 = new Proyect("Proyect1");
        Assert.AreEqual("Proyect1", proyect2.getName());
    }
}