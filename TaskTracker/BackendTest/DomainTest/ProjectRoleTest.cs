namespace BackendTest.DomainTest;
using Backend.Domain;


    [TestClass]
    public class ProjectRoleTest
    {
        private ProjectRole _projectRole;

        [TestInitialize]
        public void OnInitialize()
        {
            _projectRole = new ProjectRole();
        }
        
        [TestMethod]

        public void CreateProjectRoleTest()
        {
            Assert.IsNotNull(_projectRole);
        }
    }
