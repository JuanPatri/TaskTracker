using Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Enums;

namespace DomainTest;

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

        [TestMethod]
        public void SetProjectForProjectRoleTest()
        {
            Project project = new Project();
            _projectRole.Project = project;
            Assert.AreEqual(project, _projectRole.Project);
        }
        
        [TestMethod]
        public void SetProjectNullReturnsAnException()
        {
            Project project = null;
    
            ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _projectRole.Project = project);
            Assert.AreEqual("Project cannot be null", ex.Message);
        }

        [TestMethod]
        public void SetUserForProjectRoleTest()
        {
            User user = new User();
            _projectRole.User = user;
            Assert.AreEqual(user, _projectRole.User);
        }
        
        [TestMethod]
        public void SetUserNullReturnsAnException()
        {
            User user = null;
    
            ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _projectRole.User = user);
            Assert.AreEqual("User cannot be null", ex.Message);
        }
    }
