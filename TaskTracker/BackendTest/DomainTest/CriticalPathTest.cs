namespace BackendTest.DomainTest;
using Backend.Domain;

    [TestClass]
    public class CriticalPathTest
    {
        private CriticalPath _criticalPath;

        [TestInitialize]
        public void OnInitialize()
        {
            _criticalPath = new CriticalPath();
        }
        
        [TestMethod]
        public void CreateCriticalPathTest()
        {
            Assert.IsNotNull(_criticalPath);
        }
    }