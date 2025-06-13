using BusinessLogicTest.Context;
using Domain;
using DTOs.ProjectDTOs;
using DTOs.ResourceDTOs;
using DTOs.TaskDTOs;
using DTOs.TaskResourceDTOs;
using DTOs.UserDTOs;
using Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Service;
using Task = Domain.Task;

namespace ServiceTest;

[TestClass]
public class ProjectServiceTest
{
    private ProjectService _projectService;
    private ProjectRepository _projectRepository;
    private Project _project;
    private TaskRepository _taskRepository;
    private ResourceRepository _resourceRepository;
    private ResourceTypeRepository _resourceTypeRepository;
    private UserRepository _userRepository;
    private Task _task;
    private TaskService _taskService;
    private Resource _resource;
    private UserService _userService;
    private CriticalPathService _criticalPathService;
    private SqlContext _sqlContext;

    [TestInitialize]
    public void OnInitialize()
    {
        _sqlContext = SqlContextFactory.CreateMemoryContext();
        _projectRepository = new ProjectRepository(_sqlContext);
        _taskRepository = new TaskRepository(_sqlContext);
        _resourceRepository = new ResourceRepository(_sqlContext);
        _resourceTypeRepository = new ResourceTypeRepository();
        _userRepository = new UserRepository(_sqlContext);

        _userService = new UserService(_userRepository);
        _criticalPathService = new CriticalPathService(_projectRepository, _taskRepository);

        _projectService = new ProjectService(_taskRepository, _projectRepository,
            _resourceTypeRepository, _userRepository, _userService, _criticalPathService);

        _taskService = new TaskService(_taskRepository, _resourceRepository, _projectRepository, _projectService,
            _criticalPathService);

        User adminUser = new User()
        {
            Name = "Admin",
            LastName = "User", 
            Email = "admin@test.com",
            Password = "Admin123!",
            BirthDate = DateTime.Now.AddYears(-30),
            Admin = true
        };

        _project = new Project()
        {
            Id = 35,
            Name = "Test Project",
            Description = "Description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        ProjectRole adminRole = new ProjectRole
        {
            RoleType = RoleType.ProjectAdmin,
            User = adminUser,
            Project = _project
        };

        _project.ProjectRoles = new List<ProjectRole> { adminRole };
        _projectRepository.Add(_project);

        _task = new Task() { Title = "Test Task", };
        _taskRepository.Add(_task);

        _resource = new Resource()
        {
            Name = "Resource",
            Description = "Description",
            Type = new ResourceType()
            {
                Id = 4,
                Name = "Type"
            }
        };
        _resourceRepository.Add(_resource);
        _resourceTypeRepository.Add(_resource.Type);
    }


    [TestMethod]
    public void CreateProjectService()
    {
        Assert.IsNotNull(_projectService);
    }

    [TestMethod]
    public void AddProjectShouldReturnProject()
    {
        User adminUser = new User()
        {
            Name = "John",
            LastName = "Doe",
            Email = "john@example.com",
            BirthDate = new DateTime(1990, 01, 01),
            Password = "Pass123@",
            Admin = true
        };
        _userRepository.Add(adminUser);

        ProjectDataDTO project = new ProjectDataDTO()
        {
            Id = 2,
            Name = "Project 2",
            Description = "Description of project 2",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Users = new List<string> { "john@example.com", "john@example.com" } 
        };

        Project? createdProject = _projectService.AddProject(project);

        Assert.IsNotNull(createdProject);
        Assert.AreEqual(_projectRepository.FindAll().Last(), createdProject);

        User? admin = _projectService.GetAdministratorByProjectId(createdProject.Id);
        Assert.IsNotNull(admin);
        Assert.AreEqual("john@example.com", admin.Email);
    }

    [TestMethod]
    public void RemoveProjectShouldRemoveProject()
    {
        Assert.AreEqual(_projectRepository.FindAll().Count, 1);

        GetProjectDTO projectToDelete = new GetProjectDTO()
        {
            Id = 35,
            Name = "Project 1",
        };

        _projectService.RemoveProject(projectToDelete);
        Assert.AreEqual(_projectRepository.FindAll().Count, 0);
    }

    [TestMethod]
    public void GetProjectReturnProject()
    {
        Project project = new Project()
        {
            Id = 1,
            Name = "Project1",
            Description = "Description1",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            ProjectRoles = new List<ProjectRole>()
        };

        _projectRepository.Add(project);

        GetProjectDTO projectToFind = new GetProjectDTO()
        {
            Id = 1,
            Name = "Project1"
        };
    
        Project? foundProject = _projectService.GetProject(projectToFind);
    
        Assert.IsNotNull(foundProject);
        Assert.AreEqual(projectToFind.Id, foundProject.Id);
        Assert.AreEqual(projectToFind.Name, foundProject.Name);
    }

    [TestMethod]
    public void GetAllProjectsReturnAllProjects()
    {
        List<Project> projects = _projectService.GetAllProjects();
        Assert.IsNotNull(projects);
        Assert.AreEqual(_projectRepository.FindAll().Count(), projects.Count);
    }

    [TestMethod]
    public void UpdateProjectShouldReturnUpdatedProject()
    {
        Project project = new Project()
        {
            Id = 1,
            Name = "Project 1",
            Description = "Description of project 1",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            ProjectRoles = new List<ProjectRole>()
        };

        _projectRepository.Add(project);

        User adminUser = new User()
        {
            Name = "John",
            LastName = "Doe",
            Email = "john@example.com",
            BirthDate = new DateTime(1990, 01, 01),
            Password = "Pass123@",
            Admin = true
        };
        _userRepository.Add(adminUser);

        ProjectDataDTO projectUpdate = new ProjectDataDTO()
        {
            Id = 1,
            Name = "Project 1",
            Description = "Updated description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            Users = new List<string> { "john@example.com" } 
        };
    
        Project? updatedProject = _projectService.UpdateProject(projectUpdate);
    
        Assert.IsNotNull(updatedProject);
        Assert.AreEqual(projectUpdate.Id, updatedProject.Id);
        Assert.AreEqual(projectUpdate.Name, updatedProject.Name);
        Assert.AreEqual("Updated description", updatedProject.Description);

        User? admin = _projectService.GetAdministratorByProjectId(updatedProject.Id);
        Assert.IsNotNull(admin);
        Assert.AreEqual("john@example.com", admin.Email);
    }
    
    [TestMethod]
    public void AddProjectShouldThrowExceptionWhenNameAlreadyExists()
    {
        Project existingProject = new Project
        {
            Id = 10,
            Name = "DuplicateName",
            Description = "Desc",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            ProjectRoles = new List<ProjectRole>()
        };
        _projectRepository.Add(existingProject);

        User adminUser = new User
        {
            Name = "Admin",
            LastName = "Admin",
            Email = "admin@example.com",
            Password = "Admin123@",
            BirthDate = new DateTime(1990, 1, 1),
            Admin = true
        };
        _userRepository.Add(adminUser);

        ProjectDataDTO newProjectDto = new ProjectDataDTO
        {
            Name = "DuplicateName",
            Description = "New Desc",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            Users = new List<string> { "admin@example.com" }
        };

        Assert.ThrowsException<ArgumentException>(() => _projectService.AddProject(newProjectDto));
    }

    [TestMethod]
    public void GetProjectsByUserEmailTest()
    {
        User adminUser = new User()
        {
            Name = "Admin",
            LastName = "Admin",
            Email = "admin@example.com",
            Password = "Admin123@",
            BirthDate = new DateTime(1990, 1, 1),
            Admin = false
        };

        User user = new User()
        {
            Name = "User",
            LastName = "User",
            Email = "user@user.com",
            Password = "User123@",
            BirthDate = new DateTime(1990, 1, 1),
            Admin = true
        };

        _userRepository.Add(adminUser);
        _userRepository.Add(user);

        Project project1 = new Project()
        {
            Id = 1,
            Name = "Project 1",
            Description = "Description of project 1",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        ProjectRole adminRole = new ProjectRole
        {
            RoleType = RoleType.ProjectAdmin,
            User = adminUser,
            Project = project1
        };

        project1.ProjectRoles = new List<ProjectRole> { adminRole };
        _projectRepository.Add(project1);

        List<GetProjectDTO> projectDto = _projectService.GetProjectsByUserEmail("admin@example.com");

        Assert.IsNotNull(projectDto);
        Assert.AreEqual(1, projectDto.Count);
    }

    [TestMethod]
    public void GetAllProjectsDTOs_ShouldReturnProjectsWithCorrectMapping()
    {
        List<ProjectDataDTO> projects = _projectService.GetAllProjectsDTOs();
        Assert.IsNotNull(projects);
        Assert.AreEqual(1, projects.Count);
    }

    [TestMethod]
    public void GetAllProjectsByUserEmailShouldReturnAllProjectsWhereUserIsMember()
    {
        string userEmail = "user@example.com";

        User admin = new User
        {
            Name = "Admin",
            LastName = "Admin",
            Email = "admin@example.com",
            Password = "Admin123@",
            BirthDate = new DateTime(1980, 1, 1),
            Admin = true
        };

        User member = new User
        {
            Name = "User",
            LastName = "User",
            Email = userEmail,
            Password = "User123@",
            BirthDate = new DateTime(1990, 1, 1),
            Admin = false
        };

        _userRepository.Add(admin);
        _userRepository.Add(member);

        Project project1 = new Project
        {
            Id = 1,
            Name = "Project One",
            Description = "First Project",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        ProjectRole adminRole1 = new ProjectRole
        {
            RoleType = RoleType.ProjectAdmin,
            User = admin,
            Project = project1
        };

        ProjectRole memberRole1 = new ProjectRole
        {
            RoleType = RoleType.ProjectMember,
            User = member,
            Project = project1
        };

        project1.ProjectRoles = new List<ProjectRole> { adminRole1, memberRole1 };

        Project project2 = new Project
        {
            Id = 2,
            Name = "Project Two",
            Description = "Second Project",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
        };

        ProjectRole adminRole2 = new ProjectRole
        {
            RoleType = RoleType.ProjectAdmin,
            User = member,
            Project = project2
        };

        project2.ProjectRoles = new List<ProjectRole> { adminRole2 };

        _projectRepository.Add(project1);
        _projectRepository.Add(project2);

        List<GetProjectDTO> result = _projectService.GetAllProjectsByUserEmail(userEmail);

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(p => p.Name == "Project One"));
        Assert.IsTrue(result.Any(p => p.Name == "Project Two"));
    }
    
    [TestMethod]
    public void GetTasksForProjectWithIdShouldReturnEmptyWhenProjectNotFound()
    {
        List<GetTaskDTO> result = _taskService.GetTasksForProjectWithId(999);
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void AddTaskToProjectShouldThrowWhenProjectNotFound()
    {
        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Test",
            Description = "Test",
            Duration = 1,
            Status = Status.Pending,
            Dependencies = new List<string>(),
            Resources = new List<TaskResourceDataDTO>()
        };

        Assert.ThrowsException<ArgumentException>(() =>
            _projectService.AddTaskToProject(taskDto, 999));
    }

    [TestMethod]
    public void ProjectsDataByUserEmailShouldReturnAssociatedProjects()
    {
        User user = new User { Name = "Ana", LastName = "Lopez", Email = "ana@example.com" };
        User admin = new User { Name = "Admin", LastName = "Root", Email = "admin@example.com", Admin = true };

        _userRepository.Add(user);
        _userRepository.Add(admin);

        Project project = new Project
        {
            Id = 1,
            Name = "Test Project",
            Description = "Project description",
            StartDate = DateOnly.FromDateTime(DateTime.Now).AddDays(1)
        };

        ProjectRole adminRole = new ProjectRole
        {
            RoleType = RoleType.ProjectAdmin,
            User = admin,
            Project = project
        };

        ProjectRole userRole = new ProjectRole
        {
            RoleType = RoleType.ProjectMember,
            User = user,
            Project = project
        };

        project.ProjectRoles = new List<ProjectRole> { adminRole, userRole };
        _projectRepository.Add(project);

        List<ProjectDataDTO> result = _projectService.ProjectsDataByUserEmail("ana@example.com");

        Assert.AreEqual(project.Id, result[0].Id);
        Assert.AreEqual(project.Name, result[0].Name);
        Assert.IsTrue(result[0].Users?.Contains("ana@example.com"));
    }
    
    [TestMethod]
    public void GetAllUsers_ReturnsAllUsersAsUserDataDTOs()
    {
        int initialUserCount = _userRepository.FindAll().Count();

        User testUser1 = new User
        {
            Name = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "JohnDoe123@",
            BirthDate = new DateTime(1980, 1, 1),
            Admin = true
        };

        User testUser2 = new User
        {
            Name = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Password = "JaneSmith456#",
            BirthDate = new DateTime(1985, 5, 5),
            Admin = false
        };

        _userRepository.Add(testUser1);
        _userRepository.Add(testUser2);

        List<UserDataDTO> result = _userService.GetAllUsersDto();

        Assert.IsNotNull(result);
        Assert.AreEqual(initialUserCount + 2, result.Count);

        Assert.IsTrue(result.Any(u => u.Email == "john.doe@example.com"));
        Assert.IsTrue(result.Any(u => u.Email == "jane.smith@example.com"));

        UserDataDTO johnDto = result.FirstOrDefault(u => u.Email == "john.doe@example.com");
        Assert.IsNotNull(johnDto);
        Assert.AreEqual("John", johnDto.Name);
        Assert.AreEqual("Doe", johnDto.LastName);
        Assert.AreEqual("JohnDoe123@", johnDto.Password);
        Assert.AreEqual(new DateTime(1980, 1, 1), johnDto.BirthDate);
        Assert.IsTrue(johnDto.Admin);

        UserDataDTO janeDto = result.FirstOrDefault(u => u.Email == "jane.smith@example.com");
        Assert.IsNotNull(janeDto);
        Assert.AreEqual("Jane", janeDto.Name);
        Assert.AreEqual("Smith", janeDto.LastName);
        Assert.AreEqual("JaneSmith456#", janeDto.Password);
        Assert.AreEqual(new DateTime(1985, 5, 5), janeDto.BirthDate);
        Assert.IsFalse(janeDto.Admin);
    }

    [TestMethod]
    public void GetAllUsers_MapsPropertiesCorrectly()
    {
        User testUser = new User
        {
            Name = "Test",
            LastName = "User",
            Email = "test.user@example.com",
            Password = "TestUser789$",
            BirthDate = new DateTime(1990, 10, 10),
            Admin = true
        };

        _userRepository.Add(testUser);

        List<UserDataDTO> result = _userService.GetAllUsersDto();

        UserDataDTO testUserDto = result.FirstOrDefault(u => u.Email == "test.user@example.com");
        Assert.IsNotNull(testUserDto);

        Assert.AreEqual(testUser.Name, testUserDto.Name);
        Assert.AreEqual(testUser.LastName, testUserDto.LastName);
        Assert.AreEqual(testUser.Email, testUserDto.Email);
        Assert.AreEqual(testUser.Password, testUserDto.Password);
        Assert.AreEqual(testUser.BirthDate, testUserDto.BirthDate);
        Assert.AreEqual(testUser.Admin, testUserDto.Admin);
    }

    [TestMethod]
    public void GetAdminEmailByTaskTitleTest()
    {
        User adminUser = new User 
        { 
            Name = "Admin",
            LastName = "User",
            Email = "admin@example.com",
            Password = "Admin123!",
            BirthDate = DateTime.Now.AddYears(-30)
        };

        ProjectRole adminRole = new ProjectRole
        {
            RoleType = RoleType.ProjectAdmin,
            User = adminUser,
            Project = _project
        };

        _project.ProjectRoles = new List<ProjectRole> { adminRole };

        Task task = new Task { Title = "Test Task" };
        _project.Tasks.Add(task);

        string? email = _projectService.GetAdminEmailByTaskTitle("Test Task");

        Assert.IsNotNull(email);
        Assert.AreEqual("admin@example.com", email);
    }

    [TestMethod]
    public void GetLeadEmailByTaskTitleTest()
    {
        User leadUser = new User 
        { 
            Name = "Lead",
            LastName = "User",
            Email = "lead@example.com",
            Password = "Lead123!",
            BirthDate = DateTime.Now.AddYears(-30)
        };

        ProjectRole leadRole = new ProjectRole
        {
            RoleType = RoleType.ProjectLead,
            User = leadUser,
            Project = _project
        };

        _project.ProjectRoles = new List<ProjectRole> { leadRole };

        Task task = new Task { Title = "Test Task" };
        _project.Tasks.Add(task);

        string? email = _projectService.GetLeadEmailByTaskTitle("Test Task");

        Assert.IsNotNull(email);
        Assert.AreEqual("lead@example.com", email);
    }
    
[TestMethod]
public void UpdateProject_ShouldUseExistingAdminPassword_WhenPasswordIsEmpty()
{
    User adminUser = new User
    {
        Name = "Admin",
        LastName = "User",
        Email = "admin.user@example.com",
        Password = "AdminPass123$",
        BirthDate = new DateTime(1985, 5, 5),
        Admin = true
    };
    _userRepository.Add(adminUser);

    Project existingProject = new Project
    {
        Id = 500,
        Name = "Existing Project",
        Description = "Description",
        StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
    };

    ProjectRole adminRole = new ProjectRole
    {
        RoleType = RoleType.ProjectAdmin,
        User = adminUser,
        Project = existingProject
    };

    existingProject.ProjectRoles = new List<ProjectRole> { adminRole };
    _projectRepository.Add(existingProject);

    ProjectDataDTO projectUpdateDto = new ProjectDataDTO
    {
        Id = 500,
        Name = "Updated Project Name",
        Description = "Updated description",
        StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
        Users = new List<string> { adminUser.Email }
    };

    Project? updatedProject = _projectService.UpdateProject(projectUpdateDto);

    Assert.IsNotNull(updatedProject);
    Assert.AreEqual(projectUpdateDto.Name, updatedProject.Name);
    Assert.AreEqual(projectUpdateDto.Description, updatedProject.Description);
}

[TestMethod]
public void UpdateProject_ShouldUpdateUsersBasedOnEmails()
{
    User adminUser = new User
    {
        Name = "Admin",
        LastName = "User",
        Email = "admin@test.com",
        Password = "AdminTest123$",
        BirthDate = new DateTime(1980, 1, 1),
        Admin = true
    };

    User user1 = new User
    {
        Name = "User",
        LastName = "One",
        Email = "user1@test.com",
        Password = "UserOne123$",
        BirthDate = new DateTime(1990, 2, 2),
        Admin = false
    };

    User user2 = new User
    {
        Name = "User",
        LastName = "Two",
        Email = "user2@test.com",
        Password = "UserTwo123$",
        BirthDate = new DateTime(1995, 3, 3),
        Admin = false
    };

    _userRepository.Add(adminUser);
    _userRepository.Add(user1);
    _userRepository.Add(user2);

    Project existingProject = new Project
    {
        Id = 600,
        Name = "Project With Users",
        Description = "Description",
        StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
    };

    ProjectRole adminRole = new ProjectRole
    {
        RoleType = RoleType.ProjectAdmin,
        User = adminUser,
        Project = existingProject
    };

    existingProject.ProjectRoles = new List<ProjectRole> { adminRole };
    _projectRepository.Add(existingProject);

    ProjectDataDTO projectUpdateDto = new ProjectDataDTO
    {
        Id = 600,
        Name = "Updated Project With Users",
        Description = "Updated with more users",
        StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
        Users = new List<string> { adminUser.Email, user1.Email, user2.Email }
    };

    Project? updatedProject = _projectService.UpdateProject(projectUpdateDto);

    Assert.IsNotNull(updatedProject);
    Assert.AreEqual(projectUpdateDto.Name, updatedProject.Name);
    Assert.AreEqual(3, updatedProject.ProjectRoles.Count);
    Assert.IsTrue(updatedProject.ProjectRoles.Any(pr => pr.User.Email == adminUser.Email));
    Assert.IsTrue(updatedProject.ProjectRoles.Any(pr => pr.User.Email == user1.Email));
    Assert.IsTrue(updatedProject.ProjectRoles.Any(pr => pr.User.Email == user2.Email));
}

[TestMethod]
public void AddTaskToProjectShouldAddTaskToProjectTasksList()
{
    TaskDataDTO taskDto = new TaskDataDTO
    {
        Title = "Project Task",
        Description = "Description of the project task",
        Duration = 1,
        Status = Status.Pending,
        Dependencies = new List<string>(),
        Resources = new List<TaskResourceDataDTO>()
    };

    Project project = new Project
    {
        Id = 99,
        Name = "Task Project",
        Description = "Description of the project",
        StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
        Tasks = new List<Task>(),
        ProjectRoles = new List<ProjectRole>()
    };

    _projectRepository.Add(project);

    Project initialProject = _projectRepository.Find(p => p.Id == 99);
    Assert.AreEqual(0, initialProject.Tasks.Count);

    _taskRepository.Add(_taskService.FromDto(taskDto, new List<TaskResource>(), new List<Task>()));
    _projectService.AddTaskToProject(taskDto, project.Id);

    Project updatedProject = _projectRepository.Find(p => p.Id == 99);

    Assert.AreEqual(1, updatedProject.Tasks.Count);

    Task addedTask = updatedProject.Tasks[0];
    Assert.IsNotNull(addedTask);
    Assert.AreEqual(taskDto.Title, addedTask.Title);
    Assert.AreEqual(taskDto.Description, addedTask.Description);
    Assert.AreEqual(taskDto.Duration, addedTask.Duration);
    Assert.AreEqual(taskDto.Status, addedTask.Status);
}

[TestMethod]
public void GetNextExclusiveResourceId_NoExclusiveResources_ShouldReturnId1000()
{
    Project project = new Project
    {
        Id = 100,
        Name = "Project Without Exclusive Resources",
        Description = "Valid description",
        StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
        ExclusiveResources = new List<Resource>(),
        ProjectRoles = new List<ProjectRole>()
    };
    _projectRepository.Add(project);

    ResourceDataDto resourceDto = new ResourceDataDto
    {
        Name = "First Exclusive Resource",
        Description = "Description",
        Quantity = 1,
        TypeResource = 4
    };

    _projectService.AddExclusiveResourceToProject(100, resourceDto);

    Project updatedProject = _projectRepository.Find(p => p.Id == 100);
    Assert.AreEqual(1, updatedProject.ExclusiveResources.Count);
    Assert.AreEqual(1000, updatedProject.ExclusiveResources[0].Id);
}

[TestMethod]
public void GetNextExclusiveResourceId_WithExistingResources_ShouldReturnMaxIdPlusOne()
{
    Resource existingResource1 = new Resource { Id = 1003, Name = "Resource1" };
    Resource existingResource2 = new Resource { Id = 1001, Name = "Resource2" };

    Project project = new Project
    {
        Id = 101,
        Name = "Project With Exclusive Resources",
        Description = "Valid description",
        StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
        ExclusiveResources = new List<Resource> { existingResource1, existingResource2 },
        ProjectRoles = new List<ProjectRole>()
    };
    _projectRepository.Add(project);

    ResourceDataDto resourceDto = new ResourceDataDto
    {
        Name = "New Exclusive Resource",
        Description = "Description",
        Quantity = 1,
        TypeResource = 4
    };

    _projectService.AddExclusiveResourceToProject(101, resourceDto);

    Project updatedProject = _projectRepository.Find(p => p.Id == 101);
    Assert.AreEqual(3, updatedProject.ExclusiveResources.Count);
    Resource newResource = updatedProject.ExclusiveResources.FirstOrDefault(r => r.Name == "New Exclusive Resource");
    Assert.IsNotNull(newResource);
    Assert.AreEqual(1004, newResource.Id);
}

[TestMethod]
public void CalculateTaskDates_ShouldCalculateCorrectDates()
{
    User admin = new User
    {
        Name = "Admin",
        LastName = "User",
        Email = "admin@test.com",
        Password = "Test123!",
        BirthDate = DateTime.Now.AddYears(-30)
    };

    Project project = new Project
    {
        Id = 150,
        Name = "Date Test Project",
        Description = "Test description",
        StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
        Tasks = new List<Task>(),
        ProjectRoles = new List<ProjectRole>()
    };
    _projectRepository.Add(project);

    Task dependencyTask = new Task
    {
        Title = "Dependency Task",
        Description = "Dependency",
        Duration = 3,
        EarlyStart = project.StartDate.ToDateTime(new TimeOnly(0, 0)),
        EarlyFinish = project.StartDate.ToDateTime(new TimeOnly(0, 0)).AddDays(3)
    };

    Task newTask = new Task
    {
        Title = "New Task",
        Description = "Test task",
        Duration = 2,
        Dependencies = new List<Task> { dependencyTask }
    };

    project.Tasks.Add(dependencyTask);
    _taskRepository.Add(dependencyTask);
    _taskRepository.Add(newTask);

    TaskDataDTO taskDto = new TaskDataDTO
    {
        Title = "New Task",
        Description = "Test task",
        Duration = 2,
        Dependencies = new List<string> { "Dependency Task" },
        Resources = new List<TaskResourceDataDTO>()
    };

    _projectService.AddTaskToProject(taskDto, 150);

    Task addedTask = project.Tasks.FirstOrDefault(t => t.Title == "New Task");
    Assert.IsNotNull(addedTask);
    Assert.AreEqual(dependencyTask.EarlyFinish.AddDays(1), addedTask.EarlyStart);
    Assert.AreEqual(dependencyTask.EarlyFinish.AddDays(3), addedTask.EarlyFinish);
}

[TestMethod]
public void GetProjectWithCriticalPath_ShouldReturnProjectWithCriticalPathData()
{
    Task taskA = new Task
    {
        Title = "Task A",
        Duration = 2,
        EarlyStart = DateTime.Now.AddDays(1),
        EarlyFinish = DateTime.Now.AddDays(3)
    };

    Task taskB = new Task
    {
        Title = "Task B",
        Duration = 3,
        Dependencies = new List<Task> { taskA },
        EarlyStart = DateTime.Now.AddDays(4),
        EarlyFinish = DateTime.Now.AddDays(7)
    };

    Project project = new Project
    {
        Id = 200,
        Name = "Critical Path Project",
        Description = "Test critical path",
        StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
        Tasks = new List<Task> { taskA, taskB },
        ProjectRoles = new List<ProjectRole>()
    };
    _projectRepository.Add(project);

    GetProjectDTO result = _projectService.GetProjectWithCriticalPath(200);

    Assert.IsNotNull(result);
    Assert.AreEqual("Critical Path Project", result.Name);
    Assert.IsNotNull(result.CriticalPathTitles);
    Assert.IsNotNull(result.Tasks);
    Assert.AreEqual(2, result.Tasks.Count);
}

[TestMethod]
public void GetEstimatedProjectFinishDate_ShouldReturnMaxTaskFinishDate()
{
    Task task1 = new Task
    {
        Title = "Task 1",
        Duration = 2,
        EarlyStart = DateTime.Now.AddDays(1),
        EarlyFinish = DateTime.Now.AddDays(3)
    };

    Task task2 = new Task
    {
        Title = "Task 2",
        Duration = 5,
        EarlyStart = DateTime.Now.AddDays(1),
        EarlyFinish = DateTime.Now.AddDays(6)
    };

    Project project = new Project
    {
        Id = 250,
        Name = "Finish Date Project",
        Description = "Test finish date",
        StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
        Tasks = new List<Task> { task1, task2 },
        ProjectRoles = new List<ProjectRole>()
    };
    _projectRepository.Add(project);

    DateTime finishDate = _projectService.GetEstimatedProjectFinishDate(project);
    Assert.AreEqual(task2.EarlyFinish, finishDate);
}

[TestMethod]
public void GetUsersFromProject_ShouldReturnProjectUsers()
{
    User user1 = new User { Name = "UserOne", Email = "user1@test.com" };
    User user2 = new User { Name = "UserTwo", Email = "user2@test.com" };

    Project project = new Project
    {
        Id = 300,
        Name = "Users Project",
        Description = "Test users",
        StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
    };

    ProjectRole role1 = new ProjectRole
    {
        RoleType = RoleType.ProjectMember,
        User = user1,
        Project = project
    };

    ProjectRole role2 = new ProjectRole
    {
        RoleType = RoleType.ProjectMember,
        User = user2,
        Project = project
    };

    project.ProjectRoles = new List<ProjectRole> { role1, role2 };
    _projectRepository.Add(project);

    List<User> users = _projectService.GetUsersFromProject(300);

    Assert.AreEqual(2, users.Count);
    Assert.IsTrue(users.Any(u => u.Name == "UserOne"));
    Assert.IsTrue(users.Any(u => u.Name == "UserTwo"));
}

[TestMethod]
public void GetAdministratorByProjectId_ShouldReturnCorrectResults()
{
    User adminUser = new User
    {
        Name = "Admin",
        LastName = "User",
        Email = "admin@test.com",
        Password = "Admin123!",
        BirthDate = DateTime.Now.AddYears(-30),
        Admin = true
    };

    Project project = new Project
    {
        Id = 500,
        Name = "Test Project",
        Description = "Test description",
        StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
    };

    ProjectRole adminRole = new ProjectRole
    {
        RoleType = RoleType.ProjectAdmin,
        User = adminUser,
        Project = project
    };

    project.ProjectRoles = new List<ProjectRole> { adminRole };
    _projectRepository.Add(project);

    User? result = _projectService.GetAdministratorByProjectId(500);
    Assert.IsNotNull(result);
    Assert.AreEqual("admin@test.com", result.Email);

    User? resultNotFound = _projectService.GetAdministratorByProjectId(999);
    Assert.IsNull(resultNotFound);

    Project projectNoAdmin = new Project
    {
        Id = 600,
        Name = "No Admin Project",
        Description = "Project without admin",
        StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
        ProjectRoles = new List<ProjectRole>()
    };
    _projectRepository.Add(projectNoAdmin);

    User? resultNoAdmin = _projectService.GetAdministratorByProjectId(600);
    Assert.IsNull(resultNoAdmin);
}

public void isLeadProjectTest()
{
    User leadUser = new User
    {
        Name = "Lead",
        LastName = "User",
        Email = "lead@admin,com",
    };
    _userRepository.Add(leadUser);
    
    Project project = new Project
    {
        Id = 700,
        Name = "Lead Project",
        Description = "Project with lead",
        StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
        ProjectRoles = new List<ProjectRole>()
    };
    
    ProjectRole leadRole = new ProjectRole
    {
        RoleType = RoleType.ProjectLead,
        User = leadUser,
        Project = project
    };

    ProjectDataDTO projectDataDto = new ProjectDataDTO()
    {
        Id = 700,
        Name = "Lead Project",
        Description = "Project with lead",
        StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
        Users = new List<string> { "lead@admin,com" }
    };
    
    project.ProjectRoles.Add(leadRole);
    
    _projectRepository.Add(project);
    
    bool isLead = _projectService.IsLeadProject(projectDataDto, "lead@admin,com");
}

[TestMethod]
public void GetProjectsLedByUser_ReturnsCorrectData()
{
    var leaderUser = new User
    {
        Name = "Juan",
        LastName = "Líder",
        Email = "juan.lider@test.com",
        Password = "Seguro123!",
        BirthDate = DateTime.Now.AddYears(-25),
        Admin = false
    };
    _userRepository.Add(leaderUser);

    var project = new Project
    {
        Id = 100,
        Name = "Proyecto de Prueba",
        StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), 
        Description = "Un proyecto de prueba"
    };

    var task = new Task
    {
        Title = "Tarea A",
        EarlyStart = new DateTime(2024, 3, 3),
        Duration = 5,
        Resources = new List<TaskResource>
        {
            new TaskResource
            {
                Resource = _resource
            }
        }
    };

    project.Tasks = new List<Task> { task };

    var role = new ProjectRole
    {
        RoleType = RoleType.ProjectLead,
        User = leaderUser,
        Project = project
    };

    project.ProjectRoles = new List<ProjectRole> { role };

    _projectRepository.Add(project);
    _taskRepository.Add(task);

    
    var projects = _projectService.GetProjectsLedByUser("juan.lider@test.com");
    var result = _projectService.MapProjectsToExporterDataDto(projects);

    Assert.AreEqual(1, result.Count);

    var exportedProject = result.First();
    Assert.AreEqual("Proyecto de Prueba", exportedProject.Name);
    Assert.AreEqual(DateOnly.FromDateTime(DateTime.Today.AddDays(1)), exportedProject.StartDate);

    Assert.AreEqual(1, exportedProject.Tasks.Count);

    var exportedTask = exportedProject.Tasks.First();
    Assert.AreEqual("Tarea A", exportedTask.Title);
    Assert.AreEqual(new DateTime(2024, 3, 3), exportedTask.StartDate);
    Assert.AreEqual("N", exportedTask.IsCritical);
    Assert.AreEqual(1, exportedTask.Resources.Count);
    Assert.AreEqual("Resource", exportedTask.Resources.First());
}

    [TestMethod]
    public void FromDtoShouldMapAllPropertiesCorrectly()
    {
        ProjectDataDTO dto = new ProjectDataDTO()
        {
            Id = 5,
            Name = "Test Project",
            Description = "This is a test project",
            StartDate = new DateOnly (2026, 1, 1),
            Users = new List<string>
            {
                "prodriguez@gmail.com",
                "test@gmail.com"
            }
        };

        User adminUser = new User()
        {
            Name = "Pedro",
            LastName = "Rodriguez",
            Email = "prodriguez@gmail.com",
            BirthDate = new DateTime(2003, 03, 14),
            Password = "Pedro1234@",
            Admin = true
        };

        User regularUser = new User()
        {
            Name = "Test",
            LastName = "User", 
            Email = "test@gmail.com",
            BirthDate = new DateTime(1995, 05, 20),
            Password = "Test123@",
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

        Project result = _projectService.FromDto(dto, projectRoles);
        
        Assert.AreEqual(dto.Id, result.Id);
        Assert.AreEqual(dto.Name, result.Name);
        Assert.AreEqual(dto.Description, result.Description);
        Assert.AreEqual(dto.StartDate, result.StartDate);
        Assert.AreEqual(2, result.ProjectRoles.Count);
        
        ProjectRole admin = result.ProjectRoles.FirstOrDefault(pr => pr.RoleType == RoleType.ProjectAdmin);
        Assert.IsNotNull(admin);
        Assert.AreEqual("Pedro", admin.User.Name);
        Assert.AreEqual("Rodriguez", admin.User.LastName);
        Assert.AreEqual("prodriguez@gmail.com", admin.User.Email);
    }
}