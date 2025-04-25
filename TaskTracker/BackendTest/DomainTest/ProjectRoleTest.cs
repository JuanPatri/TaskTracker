namespace BackendTest.DomainTest;
using Backend.Domain;
using Backend.Domain.Enums;


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

        [TestMethod]
        public void SetRoleTypeForProjectRoleTest()
        {
            _projectRole.RoleType = RoleType.ProjectAdmin;
            Assert.AreEqual(RoleType.ProjectAdmin, _projectRole.RoleType);
        }
    }
