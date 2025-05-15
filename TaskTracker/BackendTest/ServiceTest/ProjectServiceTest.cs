using Backend.Domain;
using Task = Backend.Domain.Task;
using Backend.DTOs.ProjectDTOs;
using Backend.DTOs.UserDTOs;
using Backend.Repository;
using Backend.Service;
using Backend.DTOs.TaskDTOs;
using Backend.Domain.Enums;
using Backend.DTOs.NotificationDTOs;
using Backend.DTOs.ResourceDTOs;
using Backend.DTOs.ResourceTypeDTOs;

namespace BackendTest.ServiceTest;

[TestClass]
public class
    ProjectServiceTest
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
    public void DecreaseResourceQuantityShouldDecreaseQuantityByOne()
    {
        Resource resource = new Resource { Name = "Printer" };
        Task task = new Task
        {
            Title = "Setup",
            Resources = new List<(int, Resource)> { (3, resource) }
        };

        Project project = new Project
        {
            Id = 1,
            Name = "Office Setup",
            Description = "Setup project",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = new User { Email = "admin@example.com" },
            Tasks = new List<Task> { task },
            Users = new List<User>()
        };

        _projectRepository.Add(project);

        _projectService.DecreaseResourceQuantity(1, "Printer");

        Project updatedProject = _projectRepository.Find(p => p.Id == 1);
        int updatedQty = updatedProject.Tasks[0].Resources.First().Item1;

        Assert.AreEqual(2, updatedQty);
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
            Resources = new List<(int, string)>()
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

    #endregion

    #region TaskTest

    [TestMethod]
    public void AddTask_WithExistingDependenciesAndResources_ShouldAddTaskWithCorrectDependenciesAndResources()
    {
        Task dependency1 = new Task { Title = "Dependency1" };
        Task dependency2 = new Task { Title = "Dependency2" };
        _taskRepository.Add(dependency1);
        _taskRepository.Add(dependency2);

        Resource resource1 = new Resource
        {
            Name = "TestResource1",
            Description = "Resource for testing",
            Type = new ResourceType { Id = 1, Name = "Type1" }
        };
        Resource resource2 = new Resource
        {
            Name = "TestResource2",
            Description = "Another resource",
            Type = new ResourceType { Id = 2, Name = "Type2" }
        };
        _resourceRepository.Add(resource1);
        _resourceRepository.Add(resource2);

        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Task With Real Dependencies And Resources",
            Description = "Task that has real dependencies and resources",
            Duration = 3,
            Status = Status.Pending,
            Dependencies = new List<string> { "Dependency1", "Dependency2" },
            Resources = new List<(int, string)> { (2, "TestResource1"), (3, "TestResource2") }
        };

        Task addedTask = _projectService.AddTask(taskDto);

        Assert.IsNotNull(addedTask);
        Assert.AreEqual("Task With Real Dependencies And Resources", addedTask.Title);

        Assert.IsNotNull(addedTask.Dependencies);
        Assert.AreEqual(2, addedTask.Dependencies.Count);
        Assert.IsTrue(addedTask.Dependencies.Any(d => d.Title == "Dependency1"));
        Assert.IsTrue(addedTask.Dependencies.Any(d => d.Title == "Dependency2"));

        Assert.IsNotNull(addedTask.Resources);
        Assert.AreEqual(2, addedTask.Resources.Count);
        Assert.IsTrue(addedTask.Resources.Any(r => r.Item2.Name == "TestResource1" && r.Item1 == 2));
        Assert.IsTrue(addedTask.Resources.Any(r => r.Item2.Name == "TestResource2" && r.Item1 == 3));

        Task? taskInRepo = _taskRepository.Find(t => t.Title == "Task With Real Dependencies And Resources");
        Assert.IsNotNull(taskInRepo);
        Assert.AreEqual(addedTask, taskInRepo);
    }

    [TestMethod]
    public void AddTask_WithDuplicateTitle_ShouldThrowException()
    {
        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Test Task",
            Description = "This is a duplicate task",
            Duration = 2,
            Status = Status.Pending,
            Dependencies = new List<string>(),
            Resources = new List<(int, string)>()
        };

        Assert.ThrowsException<Exception>(() => _projectService.AddTask(taskDto));
    }

    [TestMethod]
    public void AddTask_WithNonExistentDependenciesAndResources_ShouldAddTaskWithEmptyCollections()
    {
        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Task With Nonexistent Dependencies",
            Description = "This task refers to non-existent dependencies and resources",
            Duration = 2,
            Status = Status.Pending,
            Dependencies = new List<string> { "NonexistentDep1", "NonexistentDep2" },
            Resources = new List<(int, string)> { (1, "NonexistentRes1"), (2, "NonexistentRes2") }
        };

        Task addedTask = _projectService.AddTask(taskDto);

        Assert.IsNotNull(addedTask);

        Assert.AreEqual(0, addedTask.Dependencies.Count);

        Assert.AreEqual(0, addedTask.Resources.Count);
    }

    [TestMethod]
    public void FindTaskByTitleReturnTask()
    {
        TaskDataDTO taskDto = new TaskDataDTO();
        taskDto.Title = "Test Task";
        Assert.AreEqual(_projectService.GetTaskByTitle(taskDto.Title), _task);
    }

    [TestMethod]
    public void FindAllTasksReturnsAllTasks()
    {
        List<Task> tasks = _projectService.GetAllTasks();
        Assert.AreEqual(1, tasks.Count);

        TaskDataDTO task2 = new TaskDataDTO();
        task2.Title = "Test Task 2";
        task2.Description = "This is a test task.";
        task2.Duration = 1;
        task2.Status = Status.Pending;
        _projectService.AddTask(task2);
        tasks = _projectService.GetAllTasks();

        Assert.AreEqual(2, tasks.Count);
    }

    [TestMethod]
    public void UpdateTaskShouldUpdateTask()
    {
        Project project = new Project() { Id = 45, Name = "Updated Test Project" };
        _projectRepository.Add(project);

        TaskDataDTO taskDto = new TaskDataDTO();
        taskDto.Title = "Test Task";
        taskDto.Description = "New Description";
        taskDto.Duration = 1;
        taskDto.Status = Status.Blocked;
        taskDto.Dependencies = new List<string>() { "Task1", "Task2" };

        _taskRepository.Add(new Task()
        {
            Title = "Task1"
        });

        _taskRepository.Add(new Task()
        {
            Title = "Task2"
        });

        _task.Description = "Description";
        Assert.AreEqual(_task.Description, "Description");

        _projectService.UpdateTask(taskDto);

        Assert.AreEqual("New Description", _task.Description);
    }

    [TestMethod]
    public void CanMarkTaskAsCompleted_ReturnsFalse_WhenTaskDoesNotExist()
    {
        TaskDataDTO nonExistentTaskDto = new TaskDataDTO
        {
            Title = "Non-Existent Task",
            Description = "This task does not exist in the repository"
        };

        bool result = _projectService.CanMarkTaskAsCompleted(nonExistentTaskDto);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void CanMarkTaskAsCompleted_ReturnsTrue_WhenAllDependenciesAreCompleted()
    {
        Task dependency1 = new Task
        {
            Title = "Dependency 1",
            Status = Status.Completed
        };
        Task dependency2 = new Task
        {
            Title = "Dependency 2",
            Status = Status.Completed
        };
        _taskRepository.Add(dependency1);
        _taskRepository.Add(dependency2);

        Task taskWithCompletedDependencies = new Task
        {
            Title = "Task With Completed Dependencies",
            Dependencies = new List<Task> { dependency1, dependency2 }
        };
        _taskRepository.Add(taskWithCompletedDependencies);

        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Task With Completed Dependencies"
        };

        bool result = _projectService.CanMarkTaskAsCompleted(taskDto);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CanMarkTaskAsCompleted_ReturnsFalse_WhenAnyDependencyIsNotCompleted()
    {
        Task completedDependency = new Task
        {
            Title = "Completed Dependency",
            Status = Status.Completed
        };
        Task pendingDependency = new Task
        {
            Title = "Pending Dependency",
            Status = Status.Pending
        };
        _taskRepository.Add(completedDependency);
        _taskRepository.Add(pendingDependency);

        Task taskWithMixedDependencies = new Task
        {
            Title = "Task With Mixed Dependencies",
            Dependencies = new List<Task> { completedDependency, pendingDependency }
        };
        _taskRepository.Add(taskWithMixedDependencies);

        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Task With Mixed Dependencies"
        };

        bool result = _projectService.CanMarkTaskAsCompleted(taskDto);


        Assert.IsFalse(result);
    }

    [TestMethod]
    public void RemoveTaskShouldDeleteTask()
    {
        Assert.AreEqual(1, _taskRepository.FindAll().Count());

        GetTaskDTO taskDto = new GetTaskDTO();
        taskDto.Title = "Test Task";
        _projectService.RemoveTask(taskDto);

        Assert.AreEqual(0, _taskRepository.FindAll().Count());
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

    [TestMethod]
    public void GetTaskDependenciesWithTitleShouldReturnTask()
    {
        Task dependencyTask1 = new Task { Title = "Task1" };
        Task dependencyTask2 = new Task { Title = "Task2" };
        _taskRepository.Add(dependencyTask1);
        _taskRepository.Add(dependencyTask2);

        Task taskWithDependency = new Task
        {
            Title = "MainTask",
            Dependencies = new List<Task> { dependencyTask1 }
        };
        _taskRepository.Add(taskWithDependency);

        Task taskWithoutSearchedDependency = new Task
        {
            Title = "AnotherTask",
            Dependencies = new List<Task> { dependencyTask2 }
        };
        _taskRepository.Add(taskWithoutSearchedDependency);

        List<string> searchList = new List<string> { "Task1" };

        List<Task> tasks = _projectService.GetTaskDependenciesWithTitleTask(searchList);

        Assert.AreEqual(1, tasks.Count);
        Assert.AreEqual("Task1", tasks[0].Title);
    }

    [TestMethod]
    public void GetTaskDependenciesWithTitleTask_ShouldReturnMatchingTasks()
    {
        Task dependencyTask1 = new Task { Title = "Task1" };
        Task dependencyTask2 = new Task { Title = "Task2" };
        _taskRepository.Add(dependencyTask1);
        _taskRepository.Add(dependencyTask2);

        Task taskWithDependency = new Task
        {
            Title = "MainTask",
            Dependencies = new List<Task> { dependencyTask1 }
        };
        _taskRepository.Add(taskWithDependency);

        Task taskWithoutSearchedDependency = new Task
        {
            Title = "AnotherTask",
            Dependencies = new List<Task> { dependencyTask2 }
        };
        _taskRepository.Add(taskWithoutSearchedDependency);

        List<string> searchList = new List<string> { "Task1" };

        List<Task> tasks = _projectService.GetTaskDependenciesWithTitleTask(searchList);

        Assert.AreEqual(1, tasks.Count);
        Assert.AreEqual("Task1", tasks[0].Title);
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
    public void AddTaskToProjectShouldAddTaskToProjectTasksList()
    {
        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Project Task",
            Description = "Description of the project task",
            Duration = 1,
            Status = Status.Pending,
            Dependencies = new List<string>(),
            Resources = new List<(int, string)>()
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

        _projectService.AddTask(taskDto);
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
    public void DecreaseResourceQuantityShouldThrowWhenProjectNotFound()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _projectService.DecreaseResourceQuantity(999, "x"));
    }

    [TestMethod]
    public void DecreaseResourceQuantityShouldThrowWhenResourceQuantityIsZero()
    {
        Task task = new Task
        {
            Title = "Test Task",
            Resources = new List<(int, Resource)> { (0, new Resource { Name = "Laptop" }) }
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
    public void ValidateTaskStatusTest()
    {
        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Test Task",
            Description = "This is a test task.",
            Dependencies = new List<string>() { "Task1", "Task2" }
        };

        Status status = Status.Completed;

        bool isValid = _projectService.ValidateTaskStatus("Test Task", status);
        Assert.IsFalse(isValid);
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

    #endregion

    #region ResourceTest

    [TestMethod]
    public void CreateResourceService()
    {
        Assert.IsNotNull(_projectService);
    }

    [TestMethod]
    public void AddResourceShouldReturnResource()
    {
        ResourceDataDto resource = new ResourceDataDto()
        {
            Name = "name",
            Description = "description",
            TypeResource = 1
        };

        Resource? createdResource = _projectService.AddResource(resource);
        Assert.IsNotNull(createdResource);
        Assert.AreEqual(_resourceRepository.FindAll().Last(), createdResource);
    }

    [TestMethod]
    public void AddResourceShouldThrowExceptionIfResourceAlreadyExists()
    {
        ResourceDataDto resource = new ResourceDataDto()
        {
            Name = "Resource",
            Description = "description",
            TypeResource = 1
        };

        Assert.ThrowsException<Exception>(() => _projectService.AddResource(resource));
    }

    [TestMethod]
    public void RemoveResourceShouldRemoveResource()
    {
        Assert.AreEqual(_resourceRepository.FindAll().Count, 1);

        GetResourceDto resourceToDelete = new GetResourceDto()
        {
            Name = "Resource"
        };

        _projectService.RemoveResource(resourceToDelete);

        Assert.AreEqual(_resourceRepository.FindAll().Count, 0);
    }

    [TestMethod]
    public void GetResourceReturnResource()
    {
        GetResourceDto resourceToFind = new GetResourceDto()
        {
            Name = "Resource"
        };

        Assert.AreEqual(_projectService.GetResource(resourceToFind), _resource);
    }

    [TestMethod]
    public void GetAllResourceReturnAllResource()
    {
        Resource newResource = new Resource()
        {
            Name = "Resource2",
            Description = "Description",
            Type = new ResourceType()
            {
                Id = 4,
                Name = "Type"
            }
        };
        _resourceRepository.Add(newResource);
        List<Resource> resources = _projectService.GetAllResources();

        Assert.IsTrue(resources.Any(r => r.Name == "Resource"));
        Assert.IsTrue(resources.Any(r => r.Name == "Resource2"));
    }

    [TestMethod]
    public void UpdateResourceShouldModifyResourceData()
    {
        Assert.AreEqual(_resource.Description, "Description");

        ResourceDataDto resourceDTO = new ResourceDataDto()
        {
            Name = "Resource",
            Description = "new description",
            TypeResource = 2
        };

        _projectService.UpdateResource(resourceDTO);
        Assert.AreEqual(_resource.Description, "new description");
    }

    #endregion

    #region ResourceTypeTest

    [TestMethod]
    public void CreateResourceTypeService()
    {
        Assert.IsNotNull(_projectService);
    }

    [TestMethod]
    public void AddResourceTypeShouldReturnResource()
    {
        ResourceTypeDto resourceType = new ResourceTypeDto()
        {
            Id = 4,
            Name = "name"
        };

        ResourceType? createdResourceType = _projectService.AddResourceType(resourceType);
        Assert.IsNotNull(createdResourceType);
        Assert.AreEqual(_resourceTypeRepository.FindAll().Last(), createdResourceType);
    }

    [TestMethod]
    public void AddResourceTypeShouldThrowExceptionIfResourceAlreadyExists()
    {
        ResourceTypeDto resourceType = new ResourceTypeDto()
        {
            Id = 4,
            Name = "Human"
        };

        Assert.ThrowsException<Exception>(() => _projectService.AddResourceType(resourceType));
    }

    [TestMethod]
    public void RemoveResourceTypeShouldRemoveResourceType()
    {
        Assert.AreEqual(_resourceTypeRepository.FindAll().Count, 4);

        ResourceTypeDto resourceToDelete = new ResourceTypeDto()
        {
            Id = 1,
            Name = "Human",
        };

        _projectService.RemoveResourceType(resourceToDelete);

        Assert.AreEqual(_resourceTypeRepository.FindAll().Count, 3);
    }

    [TestMethod]
    public void GetResourceTypeReturnResourceType()
    {
        ResourceTypeDto resourceToFind = new ResourceTypeDto()
        {
            Id = 1,
            Name = "Resource"
        };

        Assert.AreEqual(_projectService.GetResourceType(resourceToFind).Name, "Human");
    }

    [TestMethod]
    public void GetAllResourceTypeReturnAllResourceType()
    {
        List<ResourceType> resourcesTypes = _projectService.GetAllResourcesType();

        Assert.IsTrue(resourcesTypes.Any(r => r.Name == "Human"));
        Assert.IsTrue(resourcesTypes.Any(r => r.Name == "Software"));
    }

    [TestMethod]
    public void UpdateResourceTypeShouldModifyResourceTypeData()
    {
        Assert.AreEqual(_resourceTypeRepository.Find(r => r.Id == 1).Name, "Human");

        ResourceTypeDto resourceTypeDTO = new ResourceTypeDto()
        {
            Id = 1,
            Name = "Resource"
        };

        _projectService.UpdateResourceType(resourceTypeDTO);
        Assert.AreEqual(_resourceTypeRepository.Find(r => r.Id == 1).Name, "Resource");
    }

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
    public void GetResourcesForSystemShouldReturnAllResourceNames()
    {
        Resource additionalResource = new Resource()
        {
            Name = "Additional Resource",
            Description = "Additional description",
            Type = new ResourceType()
            {
                Id = 3,
                Name = "Additional Type"
            }
        };
        _resourceRepository.Add(additionalResource);

        List<GetResourceDto> resources = _projectService.GetResourcesForSystem();

        Assert.IsNotNull(resources);
        Assert.AreEqual(2, resources.Count);
        Assert.IsTrue(resources.Any(r => r.Name == "Resource"));
        Assert.IsTrue(resources.Any(r => r.Name == "Additional Resource"));
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