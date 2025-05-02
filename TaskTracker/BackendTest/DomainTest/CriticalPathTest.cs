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

        [TestMethod]
        public void SetProjectForCriticalPathTest()
        {
            Project project = new Project();
            _criticalPath.Project = project;
            Assert.AreEqual(project, _criticalPath.Project);
        }
        
        [TestMethod]
        public void SetProjectNullReturnsAnException()
        {
            Project project = null;
    
            ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _criticalPath.Project = project);
            Assert.AreEqual("Project cannot be null", ex.Message);
        }
        
        [TestMethod]
        public void SetCriticalPathTasksTest()
        {
            List<ProjectTask> listTask = new List<ProjectTask>();
    
            _criticalPath.CriticalPathTasks = listTask;
    
            Assert.AreEqual(listTask, _criticalPath.CriticalPathTasks);
        }
        
        
    }