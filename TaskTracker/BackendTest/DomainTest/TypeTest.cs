namespace BackendTest.DomainTest;

[TestClass]
public class TypeTest
{
    [TestMethod]
    public void CreateType()
    {
        Type _type = new Type("Human");
        Assert.IsNotNull(_type);
    }
}