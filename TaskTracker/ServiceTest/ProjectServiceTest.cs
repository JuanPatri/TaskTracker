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

    [TestInitialize]
    public void OnInitialize()
    {
        _projectRepository = new ProjectRepository();
        _taskRepository = new TaskRepository();
        _resourceRepository = new ResourceRepository();
        _resourceTypeRepository = new ResourceTypeRepository();
        _userRepository = new UserRepository();

        _userService = new UserService(_userRepository);
        _criticalPathService = new CriticalPathService(_projectRepository, _taskRepository);

        _projectService = new ProjectService(_taskRepository, _projectRepository,
            _resourceTypeRepository, _userRepository, _userService, _criticalPathService);

        _taskService = new TaskService(_taskRepository, _resourceRepository, _projectRepository, _projectService,
            _criticalPathService);

        _project = new Project()
        {
            Id = 35,
            Name = "Test Project",
            Description = "Description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = new User()
        };
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
        ProjectDataDTO project = new ProjectDataDTO()
        {
            Id = 2,
            Name = "Project 2",
            Description = "Description of project 2",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = new UserDataDTO()
            {
                Name = "John",
                LastName = "Doe",
                Email = "john@example.com",
                BirthDate = new DateTime(1990, 01, 01),
                Password = "Pass123@",
                Admin = true
            }
        };
        Project? createdProject = _projectService.AddProject(project);
        Assert.IsNotNull(createdProject);
        Assert.AreEqual(_projectRepository.FindAll().Last(), createdProject);
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
            Administrator = new User()
        };

        _projectRepository.Add(project);

        GetProjectDTO projectToFind = new GetProjectDTO()
        {
            Id = 1,
            Name = "Project1"
        };
        var foundProject = _projectService.GetProject(projectToFind);
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
            Administrator = new User()
        };

        _projectRepository.Add(project);

        ProjectDataDTO projectUpdate = new ProjectDataDTO()
        {
            Id = 1,
            Name = "Project 1",
            Description = "Updated description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            Administrator = new UserDataDTO()
            {
                Name = "John",
                LastName = "Doe",
                Email = "john@example.com",
                BirthDate = new DateTime(1990, 01, 01),
                Password = "Pass123@",
                Admin = true
            }
        };
        var updatedProject = _projectService.UpdateProject(projectUpdate);
        Assert.IsNotNull(updatedProject);
        Assert.AreEqual(projectUpdate.Id, updatedProject.Id);
        Assert.AreEqual(projectUpdate.Name, updatedProject.Name);
    }

    [TestMethod]
    public void AddProjectShouldThrowExceptionWhenNameAlreadyExists()
    {
        var existingProject = new Project
        {
            Id = 10,
            Name = "DuplicateName",
            Description = "Desc",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = new User()
        };
        _projectRepository.Add(existingProject);

        var newProjectDto = new ProjectDataDTO
        {
            Name = "DuplicateName",
            Description = "New Desc",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            Administrator = new UserDataDTO
            {
                Name = "Admin",
                LastName = "Admin",
                Email = "admin@example.com",
                Password = "Admin123@",
                BirthDate = new DateTime(1990, 1, 1),
                Admin = true
            }
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
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = adminUser,
            Users = new List<User> { adminUser }
        };

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
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = admin,
            Users = new List<User> { member }
        };

        Project project2 = new Project
        {
            Id = 2,
            Name = "Project Two",
            Description = "Second Project",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = member,
            Users = new List<User> { member }
        };

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
        var user = new User { Name = "Ana", LastName = "Lopez", Email = "ana@example.com" };
        var admin = new User { Name = "Admin", LastName = "Root", Email = "admin@example.com", Admin = true };

        _userRepository.Add(user);
        _userRepository.Add(admin);

        var project = new Project
        {
            Id = 1,
            Name = "Test Project",
            Description = "Project description",
            StartDate = DateOnly.FromDateTime(DateTime.Now).AddDays(1),
            Administrator = admin,
            Users = new List<User> { user }
        };

        _projectRepository.Add(project);

        List<ProjectDataDTO> result = _projectService.ProjectsDataByUserEmail("ana@example.com");

        Assert.AreEqual(project.Id, result[0].Id);
        Assert.AreEqual(project.Name, result[0].Name);
        Assert.AreEqual("ana@example.com", result[0].Users?.FirstOrDefault());
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
        _project.Administrator = new User { Email = "admin@example.com" };
        var task = new Task { Title = "Test Task" };
        _project.Tasks.Add(task);


        string? email = _projectService.GetAdminEmailByTaskTitle("Test Task");

        Assert.IsNotNull(email);
        Assert.AreEqual("admin@example.com", email);
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
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = adminUser,
            Users = new List<User> { adminUser }
        };
        _projectRepository.Add(existingProject);

        ProjectDataDTO projectUpdateDto = new ProjectDataDTO
        {
            Id = 500,
            Name = "Updated Project Name",
            Description = "Updated description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            Administrator = new UserDataDTO
            {
                Name = adminUser.Name,
                LastName = adminUser.LastName,
                Email = adminUser.Email,
                Password = "",
                BirthDate = adminUser.BirthDate,
                Admin = adminUser.Admin
            },
            Users = new List<string> { adminUser.Email }
        };

        Project? updatedProject = _projectService.UpdateProject(projectUpdateDto);

        Assert.IsNotNull(updatedProject);
        Assert.AreEqual(projectUpdateDto.Name, updatedProject.Name);
        Assert.AreEqual(projectUpdateDto.Description, updatedProject.Description);
        Assert.AreEqual(adminUser.Password, updatedProject.Administrator.Password);
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
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = adminUser,
            Users = new List<User> { adminUser }
        };
        _projectRepository.Add(existingProject);

        ProjectDataDTO projectUpdateDto = new ProjectDataDTO
        {
            Id = 600,
            Name = "Updated Project With Users",
            Description = "Updated with more users",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
            Administrator = new UserDataDTO
            {
                Name = adminUser.Name,
                LastName = adminUser.LastName,
                Email = adminUser.Email,
                Password = adminUser.Password,
                BirthDate = adminUser.BirthDate,
                Admin = adminUser.Admin
            },
            Users = new List<string> { adminUser.Email, user1.Email, user2.Email }
        };

        Project? updatedProject = _projectService.UpdateProject(projectUpdateDto);

        Assert.IsNotNull(updatedProject);
        Assert.AreEqual(projectUpdateDto.Name, updatedProject.Name);
        Assert.AreEqual(3, updatedProject.Users.Count);
        Assert.IsTrue(updatedProject.Users.Any(u => u.Email == adminUser.Email));
        Assert.IsTrue(updatedProject.Users.Any(u => u.Email == user1.Email));
        Assert.IsTrue(updatedProject.Users.Any(u => u.Email == user2.Email));
    }

    [TestMethod]
    public void SelectedProject_PropertyWorksCorrectly()
    {
        Assert.IsNull(_projectService.SelectedProject);

        ProjectDataDTO projectDto = new ProjectDataDTO
        {
            Id = 1001,
            Name = "Selected Test Project"
        };

        _projectService.SelectedProject = projectDto;
        Assert.AreEqual(projectDto, _projectService.SelectedProject);

        _projectService.SelectedProject = null;
        Assert.IsNull(_projectService.SelectedProject);
    }

    [TestMethod]
    public void IsTaskCriticalByProjectIdShouldReturnTrueIfTaskIsCritical()
    {
        Task taskA = new Task { Title = "A", Duration = 2 };
        Task taskB = new Task { Title = "B", Duration = 3, Dependencies = new List<Task> { taskA } };
        Task taskC = new Task { Title = "C", Duration = 1, Dependencies = new List<Task> { taskB } };

        Project project = new Project
        {
            Id = 99,
            Name = "Critical Project",
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            Tasks = new List<Task> { taskA, taskB, taskC }
        };

        _projectRepository.Add(project);

        bool resultA = _taskService.IsTaskCriticalById(99, "A");
        bool resultB = _taskService.IsTaskCriticalById(99, "B");
        bool resultC = _taskService.IsTaskCriticalById(99, "C");

        Assert.IsTrue(resultA);
        Assert.IsTrue(resultB);
        Assert.IsTrue(resultC);
    }

    [TestMethod]
    public void IsTaskCriticalByProjectIdShouldReturnFalseIfProjectOrTaskNotFound()
    {
        bool resultMissingTask = _taskService.IsTaskCriticalById(99, "NotRealTask");
        Assert.IsFalse(resultMissingTask);

        bool resultMissingProject = _taskService.IsTaskCriticalById(999, "Whatever");
        Assert.IsFalse(resultMissingProject);
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
            Administrator = new User
            {
                Name = "Administrator User",
                Email = "admin@example.com"
            },
            Tasks = new List<Task>(),
            Users = new List<User>()
        };

        _projectRepository.Add(project);

        var initialProject = _projectRepository.Find(p => p.Id == 99);
        Assert.AreEqual(0, initialProject.Tasks.Count);

        _taskRepository.Add(Task.FromDto(taskDto, new List<TaskResource>(), new List<Task>()));
        _projectService.AddTaskToProject(taskDto, project.Id);

        var updatedProject = _projectRepository.Find(p => p.Id == 99);

        Assert.AreEqual(1, updatedProject.Tasks.Count);

        var addedTask = updatedProject.Tasks[0];
        Assert.IsNotNull(addedTask);
        Assert.AreEqual(taskDto.Title, addedTask.Title);
        Assert.AreEqual(taskDto.Description, addedTask.Description);
        Assert.AreEqual(taskDto.Duration, addedTask.Duration);
        Assert.AreEqual(taskDto.Status, addedTask.Status);
    }


    [TestMethod]
    public void GetNextExclusiveResourceId_NoExclusiveResources_ShouldReturnId1000()
    {
        var project = new Project
        {
            Id = 100,
            Name = "Project Without Exclusive Resources",
            Description = "Valid description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = new User(),
            ExclusiveResources = new List<Resource>()
        };
        _projectRepository.Add(project);

        var resourceDto = new ResourceDataDto
        {
            Name = "First Exclusive Resource",
            Description = "Description",
            Quantity = 1,
            TypeResource = 4
        };

        _projectService.AddExclusiveResourceToProject(100, resourceDto);

        var updatedProject = _projectRepository.Find(p => p.Id == 100);
        Assert.AreEqual(1, updatedProject.ExclusiveResources.Count);
        Assert.AreEqual(1000, updatedProject.ExclusiveResources[0].Id);
    }

    [TestMethod]
    public void GetNextExclusiveResourceId_WithExistingResources_ShouldReturnMaxIdPlusOne()
    {
        var existingResource1 = new Resource { Id = 1003, Name = "Resource1" };
        var existingResource2 = new Resource { Id = 1001, Name = "Resource2" };

        var project = new Project
        {
            Id = 101,
            Name = "Project With Exclusive Resources",
            Description = "Valid description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = new User(),
            ExclusiveResources = new List<Resource> { existingResource1, existingResource2 }
        };
        _projectRepository.Add(project);

        var resourceDto = new ResourceDataDto
        {
            Name = "New Exclusive Resource",
            Description = "Description",
            Quantity = 1,
            TypeResource = 4
        };

        _projectService.AddExclusiveResourceToProject(101, resourceDto);

        var updatedProject = _projectRepository.Find(p => p.Id == 101);
        Assert.AreEqual(3, updatedProject.ExclusiveResources.Count);
        var newResource = updatedProject.ExclusiveResources.FirstOrDefault(r => r.Name == "New Exclusive Resource");
        Assert.IsNotNull(newResource);
        Assert.AreEqual(1004, newResource.Id);
    }

    [TestMethod]
    public void GetNextExclusiveResourceId_AtLimit1999_ShouldThrowException()
    {
        var limitResource = new Resource { Id = 1999, Name = "Limit Resource" };

        var project = new Project
        {
            Id = 102,
            Name = "Project At Limit",
            Description = "Valid description",
            ExclusiveResources = new List<Resource> { limitResource }
        };
        _projectRepository.Add(project);

        var resourceDto = new ResourceDataDto
        {
            Name = "Resource Beyond Limit",
            Description = "Description",
            Quantity = 1,
            TypeResource = 4
        };

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            _projectService.AddExclusiveResourceToProject(102, resourceDto)
        );

        Assert.AreEqual("Too many exclusive resources. Max 999 exclusive resources allowed.", exception.Message);
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
            Administrator = admin,
            Tasks = new List<Task>()
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
            Administrator = new User(),
            Tasks = new List<Task> { taskA, taskB }
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
    public void GetProjectWithCriticalPath_ShouldReturnNull_WhenProjectNotFound()
    {
        GetProjectDTO result = _projectService.GetProjectWithCriticalPath(999);
        Assert.IsNull(result);
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
            Administrator = new User(),
            Tasks = new List<Task> { task1, task2 }
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
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = new User(),
            Users = new List<User> { user1, user2 }
        };
        _projectRepository.Add(project);

        List<User> users = _projectService.GetUsersFromProject(300);
    
        Assert.AreEqual(2, users.Count);
        Assert.IsTrue(users.Any(u => u.Name == "UserOne"));
        Assert.IsTrue(users.Any(u => u.Name == "UserTwo"));
    }

    [TestMethod]
    public void GetUsersFromProject_ShouldReturnEmpty_WhenProjectNotFound()
    {
        List<User> users = _projectService.GetUsersFromProject(999);
        Assert.AreEqual(0, users.Count);
    }

    [TestMethod]
    public void AddTaskToProject_ShouldThrowException_WhenTaskNotInRepository()
    {
        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Non Existent Task",
            Description = "Test",
            Duration = 1
        };

        InvalidOperationException exception = Assert.ThrowsException<InvalidOperationException>(() =>
            _projectService.AddTaskToProject(taskDto, _project.Id));

        Assert.AreEqual("Task must be added to repository before linking to project.", exception.Message);
    }

    [TestMethod]
    public void GetAdminEmailByTaskTitle_ShouldReturnNull_WhenTaskNotFound()
    {
        string result = _projectService.GetAdminEmailByTaskTitle("Non Existent Task");
        Assert.IsNull(result);
    }
}