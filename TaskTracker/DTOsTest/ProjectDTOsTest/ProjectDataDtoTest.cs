using Domain;
using DTOs.ProjectDTOs;
using DTOs.UserDTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    public void AddAdministratorToProjectTest()
    {
        ProjectDataDTO projectDto = new ProjectDataDTO();
        projectDto.Administrator = new UserDataDTO
        {
            Name = "John",
            LastName = "Doe",
            Email = "john@example.com",
            BirthDate = new DateTime(1990, 01, 01),
            Password = "Pass123@",
            Admin = true
        };
        Assert.IsNotNull(projectDto.Administrator);
        Assert.AreEqual("John", projectDto.Administrator.Name);
        Assert.AreEqual("Doe", projectDto.Administrator.LastName);
        Assert.AreEqual("john@example.com", projectDto.Administrator.Email);
        Assert.AreEqual(new DateTime(1990, 01, 01), projectDto.Administrator.BirthDate);
        Assert.AreEqual("Pass123@", projectDto.Administrator.Password);
        Assert.IsTrue(projectDto.Administrator.Admin);
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
            Administrator = new UserDataDTO
            {
                Name = "John",
                LastName = "Doe",
                Email = "john@example.com",
                BirthDate = new DateTime(1990, 01, 01),
                Password = "Pass123@",
                Admin = true
            },
            Users = new List<string>(){
                "test@gmail.com"
            }
        };

        List<User> users = new List<User>()
        {
            new User()
            {
                Email = "test@gmail.com"
            }
        };
        
        Project project = Project.FromDto(dto, users);
        Assert.IsNotNull(project);
    }

}