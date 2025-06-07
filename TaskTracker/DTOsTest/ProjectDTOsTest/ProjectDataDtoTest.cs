using Domain;
using DTOs.ProjectDTOs;
using DTOs.UserDTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enums;

namespace DTOsTest.ProjectDTOsTest;

[TestClass]
public class ProjectDataDtoTest
{
    [TestMethod]
    public void CreateIdForProjectTest()
    {
        ProjectDataDTO projectDto = new ProjectDataDTO();
        projectDto.Id = 1;
        Assert.AreEqual(1, projectDto.Id);
    }
    
    [TestMethod]
    public void CreateNameForProjectTest()
    {
        ProjectDataDTO projectDto = new ProjectDataDTO();
        projectDto.Name = "Project A";
        Assert.AreEqual("Project A", projectDto.Name);
    }
    
    [TestMethod]
    public void CreateDescriptionForProjectTest()
    {
        ProjectDataDTO projectDto = new ProjectDataDTO();
        projectDto.Description = "This is a test project.";
        Assert.AreEqual("This is a test project.", projectDto.Description);
    }
    
    [TestMethod]
    public void AddStartDateToProjectTest()
    {
        ProjectDataDTO projectDto = new ProjectDataDTO();
        DateOnly startDate = new DateOnly(2025, 10, 01);
        projectDto.StartDate = startDate;
        Assert.AreEqual(startDate, projectDto.StartDate);
    }
    
    [TestMethod]
    public void AddUsersToProjectTest()
    {
        ProjectDataDTO projectDto = new ProjectDataDTO();
        projectDto.Users = new List<string>
        {
            "john@example.com",
            "jane@example.com"
        };
        Assert.IsNotNull(projectDto.Users);
        Assert.AreEqual(2, projectDto.Users.Count);
        Assert.AreEqual("john@example.com", projectDto.Users[0]);
        Assert.AreEqual("jane@example.com", projectDto.Users[1]);
    }

    [TestMethod]
    public void ToEntityShouldMapAllPropertiesCorrectlyTest()
    {
        ProjectDataDTO dto = new ProjectDataDTO
        {
            Id = 1,
            Name = "Project A",
            Description = "This is a test project.",
            StartDate = new DateOnly(2025, 10, 01),
            Users = new List<string>
            {
                "admin@example.com",
                "user@example.com"
            }
        };

        User adminUser = new User()
        {
            Name = "John",
            LastName = "Doe",
            Email = "admin@example.com",
            BirthDate = new DateTime(1990, 01, 01),
            Password = "Pass123@",
            Admin = true
        };

        User regularUser = new User()
        {
            Name = "Jane",
            LastName = "Smith",
            Email = "user@example.com",
            BirthDate = new DateTime(1985, 05, 15),
            Password = "Pass456@",
            Admin = false
        };

        ProjectRole adminRole = new ProjectRole()
        {
            RoleType = RoleType.ProjectAdmin,
            User = adminUser
        };

        ProjectRole memberRole = new ProjectRole()
        {
            RoleType = RoleType.ProjectMember,
            User = regularUser
        };

        List<ProjectRole> projectRoles = new List<ProjectRole> { adminRole, memberRole };
        
        Project project = Project.FromDto(dto, projectRoles);
        
        Assert.IsNotNull(project);
        Assert.AreEqual(dto.Id, project.Id);
        Assert.AreEqual(dto.Name, project.Name);
        Assert.AreEqual(dto.Description, project.Description);
        Assert.AreEqual(dto.StartDate, project.StartDate);
        Assert.AreEqual(2, project.ProjectRoles.Count);
        
        ProjectRole adminRoleInProject = project.ProjectRoles.FirstOrDefault(pr => pr.RoleType == RoleType.ProjectAdmin);
        Assert.IsNotNull(adminRoleInProject);
        Assert.AreEqual("admin@example.com", adminRoleInProject.User.Email);
        
        ProjectRole memberRoleInProject = project.ProjectRoles.FirstOrDefault(pr => pr.RoleType == RoleType.ProjectMember);
        Assert.IsNotNull(memberRoleInProject);
        Assert.AreEqual("user@example.com", memberRoleInProject.User.Email);
    }
}