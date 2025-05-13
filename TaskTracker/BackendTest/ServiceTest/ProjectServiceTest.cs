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
            FinishDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
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
            FinishDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
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
            FinishDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
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
            FinishDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
            Administrator = new User()
        };

        _projectRepository.Add(project);

        ProjectDataDTO projectUpdate = new ProjectDataDTO()
        {
            Id = 1,
            Name = "Project 1",
            Description = "Updated description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            FinishDate = DateOnly.FromDateTime(DateTime.Now.AddDays(12)),
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
            FinishDate = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            Administrator = new User()
        };
        _projectRepository.Add(existingProject);

        var newProjectDto = new ProjectDataDTO
        {
            Name = "DuplicateName",
            Description = "New Desc",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            FinishDate = DateOnly.FromDateTime(DateTime.Now.AddDays(6)),
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
            FinishDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
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
            Description = "Project with exclusive resources",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            FinishDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
            Administrator = new User { Name = "Admin", Email = "admin@example.com" },
            ExclusiveResources = new List<Resource> { exclusiveResource1, exclusiveResource2 }
        };

        _projectRepository.Add(project);

        List<GetResourceDto> result = _projectService.GetExclusiveResourcesForProject(projectId);

        Assert.IsTrue(result.Any(r => r.Name == "Printer"));
        Assert.IsTrue(result.Any(r => r.Name == "Designer"));
    }

    [TestMethod]
    public void GetProjectsByUserEmailNotAdminShouldReturnProjectsWhereUserIsMemberButNotAdmin()
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
            FinishDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
            Administrator = admin,
            Users = new List<User> { member }
        };

        Project project2 = new Project
        {
            Id = 2,
            Name = "Project Two",
            Description = "Second Project",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            FinishDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
            Administrator = member, // user is admin here
            Users = new List<User> { member }
        };

        _projectRepository.Add(project1);
        _projectRepository.Add(project2);

        List<GetProjectDTO> result = _projectService.GetProjectsByUserEmailNotAdmin(userEmail);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Project One", result[0].Name);
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
            FinishDate = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
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
            FinishDate = DateOnly.FromDateTime(DateTime.Now).AddDays(10),
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
            StartDate = startDate,
            Tasks = new List<Task> { taskA, taskB, taskC }
        };

        _projectService.CalculateEarlyTimes(project);
        DateTime finish = _projectService.GetEstimatedProjectFinishDate(project);

        DateTime expectedFinish = startDate.ToDateTime(new TimeOnly(0, 0))
            .AddDays(taskA.Duration + taskB.Duration + taskC.Duration);
        
        Assert.AreEqual(expectedFinish, finish);
    }

    #endregion

    #region TaskTest

    [TestMethod]
    public void AddTaskToRepository()
    {
        TaskDataDTO taskDto = new TaskDataDTO();
        taskDto.Title = "Test Taskk";
        taskDto.Description = "This is a test task.";
        taskDto.Duration = 1;
        taskDto.Status = Status.Pending;
        taskDto.Dependencies = new List<string>() { "Task1", "Task2" };
        taskDto.Resources = new List<(int, string)>() { (1, "Resource1"), (2, "Resource2") };

        Task? task = _projectService.AddTask(taskDto);

        Assert.IsNotNull(task);
        Assert.AreEqual(_taskRepository.FindAll().Last(), task);
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
            FinishDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1)),
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
            FinishDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
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
            Message = "Message test",
            Date = DateTime.Now.AddMinutes(1),
            Impact = 1,
            TypeOfNotification = TypeOfNotification.Delay,
            Projects = new List<string> { "Test Project" },
            Users = new List<string> { "user@example.com" }
        };
    
        var users = _userRepository.FindAll().Where(u => notificationDto.Users.Contains(u.Email)).ToList();
        var projects = _projectRepository.FindAll().Where(p => notificationDto.Projects.Contains(p.Name)).ToList();
    
        var notification = _notificationRepository.Add(Notification.FromDto(notificationDto, users, projects));
    
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
        bool result = _projectService.IsTaskCritical(null, "CualquierTarea");
        Assert.IsFalse(result);
    }
    
    [TestMethod]
    public void ObtenerTipoDeNotificacionPorImpacto_ShouldReturnCorrectType()
    {
        var service = new ProjectService(null, null, null, null, null, null);

        Assert.AreEqual(TypeOfNotification.Delay, service.ObtenerTipoDeNotificacionPorImpacto(1));
        Assert.AreEqual(TypeOfNotification.DurationAdjustment, service.ObtenerTipoDeNotificacionPorImpacto(0));
        Assert.AreEqual(TypeOfNotification.DurationAdjustment, service.ObtenerTipoDeNotificacionPorImpacto(-5));
    }
    
    [TestMethod]
    public void CalcularImpacto_DeberiaRetornarDiferenciaEntreDuraciones()
    {
        var service = new ProjectService(null, null, null, null, null, null);

        int impacto = service.CalcularImpacto(5, 8);

        Assert.AreEqual(3, impacto);

        impacto = service.CalcularImpacto(10, 7);
        Assert.AreEqual(-3, impacto);

        impacto = service.CalcularImpacto(4, 4);
        Assert.AreEqual(0, impacto);
    }
    
    [TestMethod]
    public void GetNewEstimatedEndDate_DeberiaRetornarFechaEstimada_CuandoProyectoExiste()
    {
        var project = new Project
        {
            Id = 123,
            Name = "Proyecto Test",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            Tasks = new List<Task>
            {
                new Task { Title = "Tarea1", Duration = 3 }
            }
        };
        _projectRepository.Add(project);
    
        var fecha = _projectService.GetNewEstimatedEndDate(123);
    
        var esperado = project.StartDate.ToDateTime(new TimeOnly(0, 0)).AddDays(3);
        Assert.AreEqual(esperado, fecha);
    }
    
    [TestMethod]
    public void GetUsersFromProject_DeberiaRetornarUsuariosDelProyecto()
    {
        var user1 = new User { Name = "Juan", Email = "juan@mail.com" };
        var user2 = new User { Name = "Ana", Email = "ana@mail.com" };
        _userRepository.Add(user1);
        _userRepository.Add(user2);
    
        var project = new Project
        {
            Id = 10,
            Name = "Proyecto X",
            Users = new List<User> { user1, user2 }
        };
        _projectRepository.Add(project);
    
        var usuarios = _projectService.GetUsersFromProject(10);
    
        Assert.AreEqual(2, usuarios.Count);
        Assert.IsTrue(usuarios.Any(u => u.Email == "juan@mail.com"));
        Assert.IsTrue(usuarios.Any(u => u.Email == "ana@mail.com"));
    }
    
    
    #endregion
}
    