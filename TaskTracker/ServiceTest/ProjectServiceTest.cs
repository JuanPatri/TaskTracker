using Domain;
using Domain.Enums;
using DTOs.NotificationDTOs;
using DTOs.ProjectDTOs;
using DTOs.ResourceDTOs;
using DTOs.ResourceTypeDTOs;
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
    private Resource _resource;
    private NotificationRepository _notificationRepository;

    [TestInitialize]
    public void OnInitialize()
    {
        _projectRepository = new ProjectRepository();
        _taskRepository = new TaskRepository();
        _resourceRepository = new ResourceRepository();
        _resourceTypeRepository = new ResourceTypeRepository();
        _userRepository = new UserRepository();
        _notificationRepository = new NotificationRepository();
        _projectService = new ProjectService(_taskRepository, _projectRepository, _resourceRepository,
            _resourceTypeRepository, _userRepository, _notificationRepository);

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

    #region ProjectTest

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
    public void AddExclusiveResourceShouldAddCorrectly()
    {
        ResourceDataDto resourceDto = new ResourceDataDto()
            { Name = "Programmer Java", Description = "java", TypeResource = 1 };
        _projectService.AddExclusiveResourceToProject(35, resourceDto);

        Project updatedProject = _projectRepository.Find(p => p.Id == 35);
        Assert.AreEqual(1, updatedProject.ExclusiveResources.Count);
        Assert.AreEqual("Programmer Java", updatedProject.ExclusiveResources[0].Name);
    }

    [TestMethod]
    public void AddExclusiveResourceShouldThrowWhenProjectDoesNotExist()
    {
        ResourceDataDto resourceDto = new ResourceDataDto()
        {
            Name = "Nonexistent Project Resource",
            Description = "Description",
            TypeResource = 1
        };

        Assert.ThrowsException<ArgumentException>(() =>
            _projectService.AddExclusiveResourceToProject(999, resourceDto)
        );
    }

    [TestMethod]
    public void GetAllProjectsDTOs_ShouldReturnProjectsWithCorrectMapping()
    {
        List<ProjectDataDTO> projects = _projectService.GetAllProjectsDTOs();
        Assert.IsNotNull(projects);
        Assert.AreEqual(1, projects.Count);
    }

    [TestMethod]
    public void GetExclusiveResourcesForProjectShouldReturnExclusiveResources()
    {
        int projectId = 10;

        Resource exclusiveResource1 = new Resource
        {
            Name = "Printer",
            Description = "Laser printer",
            Type = new ResourceType { Id = 1, Name = "Hardware" }
        };

        Resource exclusiveResource2 = new Resource
        {
            Name = "Designer",
            Description = "Graphic Designer",
            Type = new ResourceType { Id = 2, Name = "Human" }
        };

        Project project = new Project
        {
            Id = projectId,
            Name = "Exclusive Project",
            Description = "Description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = new User { Name = "Admin", Email = "admin@example.com" },
            ExclusiveResources = new List<Resource> { exclusiveResource1, exclusiveResource2 }
        };

        _projectRepository.Add(project);

        List<GetResourceDto> result = _projectService.GetExclusiveResourcesForProject(projectId);

        Assert.IsTrue(result.Any(r => r.Name == "Printer"));
        Assert.IsTrue(result.Any(r => r.Name == "Designer"));
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
    public void DecreaseResourceQuantity_ResourceNotFound_ThrowsException()
    {
        Project project = new Project
        {
            Id = 1,
            Tasks = new List<Task>()
        };
        _projectRepository.Add(project);

        var exception = Assert.ThrowsException<InvalidOperationException>(() =>
            _projectService.DecreaseResourceQuantity(1, "NonExistent"));
    
        Assert.AreEqual("Resource not found or quantity is already 0", exception.Message);
    }

    [TestMethod]
    public void DecreaseResourceQuantity_QuantityIsZero_ThrowsException()
    {
        Resource resource = new Resource { Name = "Printer" };
        TaskResource taskResource = new TaskResource
        {
            Task = new Task(),
            Resource = resource,
            Quantity = 0  
        };

        Task task = new Task
        {
            Title = "Setup",
            Resources = new List<TaskResource> { taskResource }
        };

        Project project = new Project
        {
            Id = 1,
            Tasks = new List<Task> { task }
        };

        _projectRepository.Add(project);

        Assert.ThrowsException<InvalidOperationException>(() =>
            _projectService.DecreaseResourceQuantity(1, "Printer"));
    }

    [TestMethod]
    public void GetExclusiveResourcesForProjectShouldReturnEmptyWhenProjectNotFound()
    {
        List<GetResourceDto> result = _projectService.GetExclusiveResourcesForProject(999);
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetTasksForProjectWithIdShouldReturnEmptyWhenProjectNotFound()
    {
        List<GetTaskDTO> result = _projectService.GetTasksForProjectWithId(999);
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
    public void GetEstimatedProjectFinishDate_ShouldReturnCorrectFinishDate()
    {
        Task taskA = new Task { Title = "A", Duration = 2 };
        Task taskB = new Task { Title = "B", Duration = 3, Dependencies = new List<Task> { taskA } };
        Task taskC = new Task { Title = "C", Duration = 1, Dependencies = new List<Task> { taskB } };

        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        Project project = new Project
        {
            StartDate = new DateOnly(2025, 9, 12),

            Tasks = new List<Task> { taskA, taskB, taskC }
        };

        _projectService.CalculateEarlyTimes(project);
        DateTime finish = _projectService.GetEstimatedProjectFinishDate(project);


        Assert.AreEqual(new DateTime(2025, 9, 18), finish);
    }

    [TestMethod]
    public void GetProjectWithCriticalPathShouldReturnCorrectDTO()
    {
        Task taskA = new Task { Title = "A", Duration = 2 };
        Task taskB = new Task { Title = "B", Duration = 3, Dependencies = new List<Task> { taskA } };
        Task taskC = new Task { Title = "C", Duration = 1, Dependencies = new List<Task> { taskB } };

        _project.Tasks = new List<Task> { taskA, taskB, taskC };

        GetProjectDTO result = _projectService.GetProjectWithCriticalPath(35);

        CollectionAssert.AreEqual(new List<string> { "A", "B", "C" }, result.CriticalPathTitles);

        GetTaskDTO dtoA = result.Tasks.First(t => t.Title == "A");
        GetTaskDTO dtoB = result.Tasks.First(t => t.Title == "B");
        GetTaskDTO dtoC = result.Tasks.First(t => t.Title == "C");

        DateTime startDate = _project.StartDate.ToDateTime(new TimeOnly(0, 0));

        Assert.AreEqual(startDate, dtoA.EarlyStart);
        Assert.AreEqual(startDate.AddDays(2), dtoA.EarlyFinish);

        Assert.AreEqual(dtoA.EarlyFinish, dtoB.EarlyStart);
        Assert.AreEqual(dtoB.EarlyStart.AddDays(3), dtoB.EarlyFinish);

        Assert.AreEqual(dtoB.EarlyFinish, dtoC.EarlyStart);
        Assert.AreEqual(dtoC.EarlyStart.AddDays(1), dtoC.EarlyFinish);
    }
    
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

        List<UserDataDTO> result = _projectService.GetAllUsers();

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

        List<UserDataDTO> result = _projectService.GetAllUsers();

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
    public void CalculateEarlyTimesSimpleSequenceComputesCorrectStartAndFinish()
    {
        Task taskA = new Task { Title = "A", Duration = 2 };
        Task taskB = new Task { Title = "B", Duration = 3, Dependencies = new List<Task> { taskA } };
        Task taskC = new Task { Title = "C", Duration = 1, Dependencies = new List<Task> { taskB } };

        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        Project project = new Project
        {
            StartDate = startDate,
            Tasks = new List<Task> { taskA, taskB, taskC }
        };

        _projectService.CalculateEarlyTimes(project);

        DateTime baseStart = project.StartDate.ToDateTime(new TimeOnly(0, 0));

        Assert.AreEqual(baseStart, taskA.EarlyStart);
        Assert.AreEqual(baseStart.AddDays(2), taskA.EarlyFinish);

        Assert.AreEqual(taskA.EarlyFinish, taskB.EarlyStart);
        Assert.AreEqual(taskB.EarlyStart.AddDays(3), taskB.EarlyFinish);

        Assert.AreEqual(taskB.EarlyFinish, taskC.EarlyStart);
        Assert.AreEqual(taskC.EarlyStart.AddDays(1), taskC.EarlyFinish);
    }

    [TestMethod]
    public void CalculateLateTimesShouldComputeCorrectLateStartAndFinish()
    {
        Task taskA = new Task { Title = "A", Duration = 2 };
        Task taskB = new Task { Title = "B", Duration = 3, Dependencies = new List<Task> { taskA } };
        Task taskC = new Task { Title = "C", Duration = 1, Dependencies = new List<Task> { taskB } };

        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        Project project = new Project
        {
            StartDate = startDate,
            Tasks = new List<Task> { taskA, taskB, taskC }
        };

        _projectService.CalculateLateTimes(project);

        DateTime baseStart = project.StartDate.ToDateTime(new TimeOnly(0, 0));

        Assert.AreEqual(baseStart.AddDays(5), taskC.LateStart);
        Assert.AreEqual(baseStart.AddDays(6), taskC.LateFinish);

        Assert.AreEqual(baseStart.AddDays(2), taskB.LateStart);
        Assert.AreEqual(baseStart.AddDays(5), taskB.LateFinish);

        Assert.AreEqual(baseStart, taskA.LateStart);
        Assert.AreEqual(baseStart.AddDays(2), taskA.LateFinish);
    }


    [TestMethod]
    public void GetCriticalPathShouldReturnCorrectTasks()
    {
        Task taskA = new Task { Title = "A", Duration = 2 };
        Task taskB = new Task { Title = "B", Duration = 3, Dependencies = new List<Task> { taskA } };
        Task taskC = new Task { Title = "C", Duration = 1, Dependencies = new List<Task> { taskA } };
        Task taskD = new Task { Title = "D", Duration = 2, Dependencies = new List<Task> { taskB, taskC } };

        Project project = new Project
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            Tasks = new List<Task> { taskA, taskB, taskC, taskD }
        };

        List<Task> result = _projectService.GetCriticalPath(project);
        List<String> titles = result.Select(t => t.Title).ToList();

        Assert.AreEqual(3, result.Count);
        CollectionAssert.AreEquivalent(new List<string> { "A", "B", "D" }, titles);
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

        bool resultA = _projectService.IsTaskCriticalById(99, "A");
        bool resultB = _projectService.IsTaskCriticalById(99, "B");
        bool resultC = _projectService.IsTaskCriticalById(99, "C");

        Assert.IsTrue(resultA);
        Assert.IsTrue(resultB);
        Assert.IsTrue(resultC);
    }

    [TestMethod]
    public void IsTaskCriticalByProjectIdShouldReturnFalseIfProjectOrTaskNotFound()
    {
        bool resultMissingTask = _projectService.IsTaskCriticalById(99, "NotRealTask");
        Assert.IsFalse(resultMissingTask);

        bool resultMissingProject = _projectService.IsTaskCriticalById(999, "Whatever");
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

    #endregion

    [TestMethod]
    public void IsExclusiveResourceForProject_ShouldReturnCorrectExclusivity()
    {
        var exclusiveResource = new Resource
        {
            Id = 100,
            Name = "Exclusive Printer",
            Description = "Project exclusive printer",
            Type = _resource.Type
        };

        _project.ExclusiveResources = new List<Resource> { exclusiveResource };

        bool isExclusive1 = _projectService.IsExclusiveResourceForProject(exclusiveResource.Id, _project.Id);
        Assert.IsTrue(isExclusive1, "Should return true for exclusive resource in the project");

        bool isExclusive2 = _projectService.IsExclusiveResourceForProject(_resource.Id, _project.Id);
        Assert.IsFalse(isExclusive2, "Should return false for non-exclusive resource");

        bool isExclusive3 = _projectService.IsExclusiveResourceForProject(999, _project.Id);
        Assert.IsFalse(isExclusive3, "Should return false for non-existent resource");

        bool isExclusive4 = _projectService.IsExclusiveResourceForProject(exclusiveResource.Id, 999);
        Assert.IsFalse(isExclusive4, "Should return false for non-existent project");

        var anotherProject = new Project
        {
            Id = 50,
            Name = "Another Project",
            Description = "Different project",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = new User(),
            ExclusiveResources = new List<Resource> { exclusiveResource }
        };
        _projectRepository.Add(anotherProject);

        bool isExclusive5 = _projectService.IsExclusiveResourceForProject(exclusiveResource.Id, _project.Id);
        Assert.IsTrue(isExclusive5, "Should still return true - resource is in the target project");

        bool isExclusive6 = _projectService.IsExclusiveResourceForProject(exclusiveResource.Id, anotherProject.Id);
        Assert.IsTrue(isExclusive6, "Should return true for the other project that also has the resource");
    }
    
    [TestMethod]
    public void IsResourceAvailable_ShouldReturnCorrectAvailabilityBasedOnResourceUsage()
    {
        _resource.Quantity = 5;

        var taskStart = _project.StartDate.ToDateTime(new TimeOnly(0, 0));
        var taskEnd = taskStart.AddDays(3);

        _task.EarlyStart = taskStart;
        _task.EarlyFinish = taskEnd;
        _task.Status = Status.Blocked;

        var taskResource = new TaskResource
        {
            Task = _task,
            Resource = _resource,
            Quantity = 2
        };
        _task.Resources = new List<TaskResource> { taskResource };
        _project.Tasks = new List<Task> { _task };

        var newTaskStart = taskStart.AddDays(1);
        var newTaskEnd = newTaskStart.AddDays(2);

        bool isAvailable1 = _projectService.IsResourceAvailable(
            _resource.Id, _project.Id, true, newTaskStart, newTaskEnd, 2);

        Assert.IsTrue(isAvailable1, "Should be available: 5 total - 2 used = 3 available, requesting 2");

        bool isAvailable2 = _projectService.IsResourceAvailable(
            _resource.Id, _project.Id, true, newTaskStart, newTaskEnd, 4);

        Assert.IsFalse(isAvailable2, "Should not be available: 5 total - 2 used = 3 available, requesting 4");

        var noOverlapStart = taskEnd.AddDays(1);
        var noOverlapEnd = noOverlapStart.AddDays(2);

        bool isAvailable3 = _projectService.IsResourceAvailable(
            _resource.Id, _project.Id, true, noOverlapStart, noOverlapEnd, 5);

        Assert.IsTrue(isAvailable3, "Should be available: no overlap, all 5 units available");

        bool isAvailable4 = _projectService.IsResourceAvailable(
            _resource.Id, _project.Id, false, newTaskStart, newTaskEnd, 3);

        Assert.IsTrue(isAvailable4, "Should be available: non-exclusive, same calculation");

        _task.Status = Status.Completed;

        bool isAvailable5 = _projectService.IsResourceAvailable(
            _resource.Id, _project.Id, true, newTaskStart, newTaskEnd, 5);

        Assert.IsTrue(isAvailable5, "Should be available: completed task doesn't count");

        bool isAvailable6 = _projectService.IsResourceAvailable(
            999, _project.Id, true, newTaskStart, newTaskEnd, 1);

        Assert.IsFalse(isAvailable6, "Should return false for non-existent resource");
    }

    
     [TestMethod]
     public void GetResourcesWithName_ShouldReturnResourceWithCorrectQuantity()
     {
         Resource resource = new Resource { Name = "Laptop" };
         _resourceRepository.Add(resource);
    
         List<(int, string)> resourceList = new List<(int, string)> { (5, "Laptop") };
    
         List<(int, Resource)> result = _projectService.GetResourcesWithName(resourceList);
    
         Assert.AreEqual(1, result.Count);
         Assert.AreEqual(5, result[0].Item1);
         Assert.AreEqual("Laptop", result[0].Item2.Name);
     }

     [TestMethod]
     public void GetResourcesWithName_ShouldIgnoreNonexistentResources()
     {
         var resourceList = new List<(int, string)>
         {
             (2, "NonExistingResource")
         };
    
         var result = _projectService.GetResourcesWithName(resourceList);
    
         Assert.AreEqual(0, result.Count);
     }

    
     [TestMethod]
     public void GetResourcesWithNameShouldMapMultipleResources()
     {
         Resource res1 = new Resource { Name = "Mouse" };
         Resource res2 = new Resource { Name = "Keyboard" };
         _resourceRepository.Add(res1);
         _resourceRepository.Add(res2);
    
         List<(int, string)> resourceList = new List<(int, string)>
         {
             (1, "Mouse"),
             (3, "Keyboard")
         };
    
         List<(int, Resource)> result = _projectService.GetResourcesWithName(resourceList);
    
         Assert.AreEqual(2, result.Count);
         Assert.IsTrue(result.Any(r => r.Item2.Name == "Mouse" && r.Item1 == 1));
         Assert.IsTrue(result.Any(r => r.Item2.Name == "Keyboard" && r.Item1 == 3));
     }
     
     [TestMethod]
     public void DecreaseResourceQuantityShouldThrowWhenProjectNotFound()
     {
         Assert.ThrowsException<ArgumentException>(() =>
             _projectService.DecreaseResourceQuantity(999, "x"));
     }
    
     [TestMethod]
     public void DecreaseResourceQuantityShouldThrowWhenResourceQuantityIsZero()
     {
         TaskResource taskResource = new TaskResource()
         {
             Task = new Task(),
             Resource = new Resource(),
             Quantity = 0, 
         };
    
         Task task = new Task
         {
             Title = "Test Task",
             Resources = new List<TaskResource> { taskResource } 
         };
    
         Project project = new Project
         {
             Id = 101,
             Name = "Test Project",
             Tasks = new List<Task> { task },
             Description = "desc",
             StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
             Administrator = new User()
         };
    
         _projectRepository.Add(project);
    
         Assert.ThrowsException<InvalidOperationException>(() =>
             _projectService.DecreaseResourceQuantity(101, "Laptop"));
     }

     [TestMethod]
     public void GetResourcesWhitNameShouldReturnResources()
     {
         Resource newResource = new Resource();
         newResource.Name = "Resource1";
         _resourceRepository.Add(newResource);
     
         Resource newResource2 = new Resource();
         newResource2.Name = "Resource2";
         _resourceRepository.Add(newResource2);
     
     
         List<(int, string)> searchList = new List<(int, string)>()
         {
             (2, "Resource1")
         };
     
         List<(int, Resource)> resource = _projectService.GetResourcesWithName(searchList);
     
         Assert.AreEqual(1, resource.Count);
     }
    

    #region ResourceTypeTest

    

    [TestMethod]
    public void GetTasksForProjectWithIdTest()
    {
        Task task1 = new Task() { Title = "Task 1" };
        Task task2 = new Task() { Title = "Task 2" };

        _taskRepository.Add(task1);
        _taskRepository.Add(task2);

        Project project = new Project
        {
            Id = 1,
            Name = "Test Project",
            Tasks = new List<Task> { task1, task2 }
        };

        _projectRepository.Add(project);

        List<GetTaskDTO> tasks = _projectService.GetTasksForProjectWithId(1);

        Assert.AreEqual(2, tasks.Count);
    }

    [TestMethod]
    public void IsExclusiveResourceForProjectTest()
    {
        
    }
    #endregion

    #region NotificationTest

    [TestMethod]
    public void CreateNotificationService()
    {
        Assert.IsNotNull(_projectService);
    }

    [TestMethod]
    public void AddNotificationShouldReturnNotification()
    {
        var user = new User { Name = "User", Email = "user@example.com" };
        var project = new Project { Name = "Test Project" };
        _userRepository.Add(user);
        _projectRepository.Add(project);

        NotificationDataDTO notificationDto = new NotificationDataDTO()
        {
            Id = 1,
            Message = "Message test",
            Date = DateTime.Now.AddMinutes(1),
            Impact = 1,
            TypeOfNotification = TypeOfNotification.Delay,
            Projects = new List<string> { "Test Project" },
            Users = new List<string> { "user@example.com" }
        };

        var users = _userRepository.FindAll().Where(u => notificationDto.Users.Contains(u.Email)).ToList();
        var projects = _projectRepository.FindAll().Where(p => notificationDto.Projects.Contains(p.Name)).ToList();

        var notification =
            _notificationRepository.Add(Notification.FromDto(notificationDto, users, projects, new List<string>()));

        Assert.IsNotNull(notification);
        Assert.AreEqual(notificationDto.Message, notification.Message);
        Assert.AreEqual(notificationDto.Impact, notification.Impact);
        Assert.AreEqual(notificationDto.TypeOfNotification, notification.TypeOfNotification);
    }

    [TestMethod]
    public void IsTaskCritical_ShouldReturnTrueForCriticalTask_AndFalseForNonCriticalTask()
    {
        Task taskA = new Task { Title = "A", Duration = 2 };
        Task taskB = new Task { Title = "B", Duration = 3, Dependencies = new List<Task> { taskA } };
        Task taskC = new Task { Title = "C", Duration = 1, Dependencies = new List<Task> { taskA } };
        Task taskD = new Task { Title = "D", Duration = 2, Dependencies = new List<Task> { taskB, taskC } };

        Project project = new Project
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            Tasks = new List<Task> { taskA, taskB, taskC, taskD }
        };

        _projectService.CalculateEarlyTimes(project);
        _projectService.CalculateLateTimes(project);

        Assert.IsTrue(_projectService.IsTaskCritical(project, taskA.Title));
        Assert.IsTrue(_projectService.IsTaskCritical(project, taskB.Title));
        Assert.IsTrue(_projectService.IsTaskCritical(project, taskD.Title));
        Assert.IsFalse(_projectService.IsTaskCritical(project, taskC.Title));
    }

    [TestMethod]
    public void IsTaskCritical_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
        Project project = new Project
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            Tasks = new List<Task>()
        };

        bool result = _projectService.IsTaskCritical(project, "TareaInexistente");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsTaskCritical_ShouldReturnFalse_WhenProjectIsNull()
    {
        bool result = _projectService.IsTaskCritical(null, "AnyTask");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetNotificationTypeByImpact_ShouldReturnCorrectType()
    {
        var service = new ProjectService(null, null, null, null, null, null);

        Assert.AreEqual(TypeOfNotification.Delay, service.ObtenerTipoDeNotificacionPorImpacto(1));
        Assert.AreEqual(TypeOfNotification.DurationAdjustment, service.ObtenerTipoDeNotificacionPorImpacto(0));
        Assert.AreEqual(TypeOfNotification.DurationAdjustment, service.ObtenerTipoDeNotificacionPorImpacto(-5));
    }

    [TestMethod]
    public void CalculateImpact_ShouldReturnDifferenceBetweenDurations()
    {
        var service = new ProjectService(null, null, null, null, null, null);

        int impact = service.CalcularImpacto(5, 8);

        Assert.AreEqual(3, impact);

        impact = service.CalcularImpacto(10, 7);
        Assert.AreEqual(-3, impact);

        impact = service.CalcularImpacto(4, 4);
        Assert.AreEqual(0, impact);
    }

    [TestMethod]
    public void GetNewEstimatedEndDate_ShouldReturnEstimatedDate_WhenProjectExists()
    {
        var project = new Project
        {
            Id = 123,
            Name = "Test Project",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            Tasks = new List<Task>
            {
                new Task { Title = "Task1", Duration = 3 }
            }
        };
        _projectRepository.Add(project);

        var date = _projectService.GetNewEstimatedEndDate(123);

        var expected = project.StartDate.ToDateTime(new TimeOnly(0, 0)).AddDays(3);
        Assert.AreEqual(expected, date);
    }

    [TestMethod]
    public void GetUsersFromProject_ShouldReturnProjectUsers()
    {
        var user1 = new User { Name = "John", Email = "john@mail.com" };
        var user2 = new User { Name = "Anna", Email = "anna@mail.com" };
        _userRepository.Add(user1);
        _userRepository.Add(user2);

        var project = new Project
        {
            Id = 10,
            Name = "Project X",
            Users = new List<User> { user1, user2 }
        };
        _projectRepository.Add(project);

        var users = _projectService.GetUsersFromProject(10);

        Assert.AreEqual(2, users.Count);
        Assert.IsTrue(users.Any(u => u.Email == "john@mail.com"));
        Assert.IsTrue(users.Any(u => u.Email == "anna@mail.com"));
    }

    [TestMethod]
    public void GetUsersFromProject_ShouldReturnEmptyList_WhenProjectDoesNotExist()
    {
        var users = _projectService.GetUsersFromProject(999);

        Assert.IsNotNull(users);
        Assert.AreEqual(0, users.Count);
    }

    [TestMethod]
    public void GenerateNotificationMessage_ShouldReturnCorrectMessage_ByType()
    {
        var service = new ProjectService(null, null, null, null, null, null);
        var date = new DateTime(2024, 6, 10);

        var delayMessage = service.GenerateNotificationMessage(TypeOfNotification.Delay, "Task1", date);
        Assert.AreEqual(
            "The critical task 'Task1' has caused a delay. The new estimated project end date is 2024-06-10.",
            delayMessage);

        var adjustmentMessage =
            service.GenerateNotificationMessage(TypeOfNotification.DurationAdjustment, "Task2", date);
        Assert.AreEqual(
            "The duration of the critical task 'Task2' was adjusted. The new estimated project end date is 2024-06-10.",
            adjustmentMessage);

        var defaultMessage = service.GenerateNotificationMessage((TypeOfNotification)99, "Task3", date);
        Assert.AreEqual("The task 'Task3' has had a change. The new estimated project end date is 2024-06-10.",
            defaultMessage);
    }

    [TestMethod]
    public void CreateNotification_ShouldCreateAndReturnNotificationWithCorrectData()
    {
        var user = new User { Name = "John", Email = "john@mail.com" };
        var project = new Project
        {
            Id = 1,
            Name = "Project Test",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            Users = new List<User> { user },
            Tasks = new List<Task> { new Task { Title = "Task1", Duration = 5 } }
        };
        _userRepository.Add(user);
        _projectRepository.Add(project);

        int oldDuration = 5;
        int newDuration = 8;
        string taskTitle = "Task1";

        var notification = _projectService.CreateNotification(oldDuration, newDuration, 1, taskTitle);

        Assert.IsNotNull(notification);
        Assert.AreEqual(TypeOfNotification.Delay, notification.TypeOfNotification);
        Assert.AreEqual(3, notification.Impact);
        Assert.IsTrue(notification.Message.Contains("Task1"));
        Assert.AreEqual(1, notification.Users.Count);
        Assert.AreEqual("john@mail.com", notification.Users[0].Email);
    }

    [TestMethod]
    public void GetNotificationsForUser_ShouldReturnNotificationsForGivenEmail()
    {
        var user = new User { Name = "John", Email = "john@mail.com" };
        _userRepository.Add(user);

        var notification = new Notification
        {
            Message = "Test notification",
            TypeOfNotification = TypeOfNotification.Delay,
            Impact = 2,
            Date = DateTime.Now.AddMinutes(1),
            Users = new List<User> { user }
        };
        _notificationRepository.Add(notification);

        var notifications = _projectService.GetNotificationsForUser("john@mail.com");

        Assert.AreEqual(1, notifications.Count);
        Assert.AreEqual("Test notification", notifications[0].Message);
        Assert.AreEqual(TypeOfNotification.Delay, notifications[0].TypeOfNotification);
        Assert.AreEqual(2, notifications[0].Impact);
    }

    [TestMethod]
    public void GetNotificationsForUserShouldReturnEmptyListWhenNoNotificationsForUser()
    {
        var notifications = _projectService.GetNotificationsForUser("nonexistent@mail.com");

        Assert.IsNotNull(notifications);
        Assert.AreEqual(0, notifications.Count);
    }

    [TestMethod]
    public void MarkNotificationAsViewedAddsUserToViewedList()
    {
        var notification = new Notification
        {
            Id = 1,
            Message = "Test notification",
            TypeOfNotification = TypeOfNotification.Delay,
            Impact = 2,
            Date = DateTime.Now.AddMinutes(1),
            Projects = new List<Project>(),
            Users = new List<User>(),
            ViewedBy = new List<string>()
        };
        var notificationRepository = new NotificationRepository();
        notificationRepository.Add(notification);

        var service = new ProjectService(
            null, null, null, null, null, notificationRepository);

        service.MarkNotificationAsViewed(1, "user@email.com");

        var updated = notificationRepository.Find(n => n.Id == 1);
        Assert.IsTrue(updated.ViewedBy.Contains("user@email.com"));
    }

    [TestMethod]
    public void GetUnviewedNotificationsForUser_ShouldReturnOnlyUnviewedNotifications()
    {
        var user = new User { Name = "John", Email = "john@mail.com" };
        var notificationViewed = new Notification
        {
            Id = 1,
            Message = "Viewed notification",
            TypeOfNotification = TypeOfNotification.Delay,
            Impact = 1,
            Date = DateTime.Now.AddMinutes(1),
            Users = new List<User> { user },
            ViewedBy = new List<string> { "john@mail.com" }
        };
        var notificationUnviewed = new Notification
        {
            Id = 2,
            Message = "Unviewed notification",
            TypeOfNotification = TypeOfNotification.Delay,
            Impact = 2,
            Date = DateTime.Now.AddMinutes(2),
            Users = new List<User> { user },
            ViewedBy = new List<string>()
        };

        var notificationRepository = new NotificationRepository();
        notificationRepository.Add(notificationViewed);
        notificationRepository.Add(notificationUnviewed);

        var service = new ProjectService(
            null, null, null, null, null, notificationRepository);

        var unviewed = service.GetUnviewedNotificationsForUser("john@mail.com");

        Assert.AreEqual(1, unviewed.Count);
        Assert.AreEqual("Unviewed notification", unviewed[0].Message);
    }

    #endregion
}