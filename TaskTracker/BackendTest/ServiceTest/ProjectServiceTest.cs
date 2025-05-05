using Backend.Domain;
using Task = Backend.Domain.Task;
using Backend.DTOs.ProjectDTOs;
using Backend.DTOs.UserDTOs;
using Backend.Repository;
using Backend.Service;
using Backend.DTOs.TaskDTOs;
using Backend.Domain.Enums;

namespace BackendTest.ServiceTest;

[TestClass]
public class ProjectServiceTest
{
    private ProjectService _projectService;
    private ProjectRepository _projectRepository;
    private Project _project;
    private TaskRepository _taskRepository;
    private ResourceRepository _resourceRepository;
    private TaskService _taskService;
    private Task _task;

    [TestInitialize]
    public void OnInitialize()
    {
        _projectRepository = new ProjectRepository();
        _projectService = new ProjectService(_projectRepository);
        _taskRepository = new TaskRepository();
        _resourceRepository = new ResourceRepository();
        _taskService = new TaskService(_taskRepository, _projectRepository, _resourceRepository);
    
        Project project = new Project() { Id = 35, Name = "Test Project" };
        _projectRepository.Add(project);

        _task = new Task() { Title = "Test Task", };
        _taskRepository.Add(_task);
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
            StartDate = DateTime.Now.AddDays(1),
            FinishDate = DateTime.Now.AddDays(10),
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
        
        Project project = new Project()
        {
            Id = 1,
            Name = "Project 1",
            Description = "Description of project 1",
            StartDate = DateTime.Now.AddDays(1),
            FinishDate = DateTime.Now.AddDays(10),
            Administrator = new User()
        };
        
        _projectRepository.Add(project);
        
        Assert.AreEqual(_projectRepository.FindAll().Count, 1);   
        
        GetProjectDTO projectToDelete = new GetProjectDTO()
        {
            Id = 1,
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
            StartDate = DateTime.Now.AddDays(1),
            FinishDate = DateTime.Now.AddYears(1),
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
            StartDate = DateTime.Now.AddDays(1),
            FinishDate = DateTime.Now.AddDays(10),
            Administrator = new User()
        };
        
        _projectRepository.Add(project);
        
        ProjectDataDTO projectUpdate = new ProjectDataDTO()
        {
            Id = 1,
            Name = "Project 1",
            Description = "Updated description",
            StartDate = DateTime.Now.AddDays(2),
            FinishDate = DateTime.Now.AddDays(12),
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
    public void AddTaskToRepository()
    {
        TaskDataDTO taskDto = new TaskDataDTO();
        taskDto.Title = "Test Task";
        taskDto.Description = "This is a test task.";
        taskDto.Duration = TimeSpan.FromHours(1);
        taskDto.Status = Status.Pending;
        taskDto.Dependencies = new List<string>(){"Task1", "Task2"};
        taskDto.Resources = new List<(int, string)>(){(1, "Resource1"), (2, "Resource2")};

        Task? task = _taskService.AddTask(taskDto);
    
        Assert.IsNotNull(task);
        Assert.AreEqual(_taskRepository.FindAll().Last(), task);
    }
    
    [TestMethod]
    public void FindTaskByTitleReturnTask()
    {
        TaskDataDTO taskDto = new TaskDataDTO();
        taskDto.Title = "Test Task";
        Assert.AreEqual(_taskService.GetTaskByTitle(taskDto.Title), _task);
    }

    [TestMethod]
    public void FindAllTasksReturnsAllTasks()
    {
        List<Task> tasks = _taskService.GetAllTasks();
        Assert.AreEqual(1, tasks.Count);
        
        TaskDataDTO task2 = new TaskDataDTO();
        task2.Title = "Test Task 2";
        task2.Description = "This is a test task.";
        task2.Duration = TimeSpan.FromHours(1);
        task2.Status = Status.Pending;
        _taskService.AddTask(task2);
        tasks = _taskService.GetAllTasks();
        
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
        taskDto.Duration = TimeSpan.FromHours(4);
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

        _taskService.UpdateTask(taskDto);

        Assert.AreEqual("New Description", _task.Description);
    }
    
    [TestMethod]
    public void RemoveTaskShouldDeleteTask()
    {
        Assert.AreEqual(1, _taskRepository.FindAll().Count());
        
        GetTaskDTO taskDto = new GetTaskDTO();
        taskDto.Title = "Test Task";
        _taskService.RemoveTask(taskDto);
        
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
        
        List<(int, Resource)> resource = _taskService.GetResourcesWithName(searchList);
        
        Assert.AreEqual(1, resource.Count);
    }
    
    [TestMethod]
    public void GetTaskDependenciesWithTitleShouldReturnTask()
    {

        Task dependencyTask1 = new Task();
        dependencyTask1.Title = "Task1";
        _taskRepository.Add(dependencyTask1);
    
        Task dependencyTask2 = new Task();
        dependencyTask2.Title = "Task2";
        _taskRepository.Add(dependencyTask2);
    
        Task taskWithDependency = new Task();
        taskWithDependency.Title = "MainTask";
        taskWithDependency.Dependencies = new List<Task> { dependencyTask1 };
        _taskRepository.Add(taskWithDependency);
    
        Task taskWithoutSearchedDependency = new Task();
        taskWithoutSearchedDependency.Title = "AnotherTask";
        taskWithoutSearchedDependency.Dependencies = new List<Task> { dependencyTask2 };
        _taskRepository.Add(taskWithoutSearchedDependency);
    
        List<string> searchList = new List<string>()
        {
            "Task1"  
        }; 
    
        List<Task> tasks = _taskService.GetTaskDependenciesWithTitle(searchList);
    
        Assert.AreEqual(1, tasks.Count);
        Assert.AreEqual("MainTask", tasks[0].Title); 
    }

    [TestMethod]
    public void GetResourcesWithName_ShouldReturnResourceWithCorrectQuantity()
    {
        Resource resource = new Resource { Name = "Laptop" };
        _resourceRepository.Add(resource);

        List<(int, string)> resourceList = new List<(int, string)> { (5, "Laptop") };
        
        List<(int, Resource)> result = _taskService.GetResourcesWithName(resourceList);

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
        
        var result = _taskService.GetResourcesWithName(resourceList);

        Assert.AreEqual(0, result.Count);
    }
    [TestMethod]
    public void GetResourcesWithName_ShouldMapMultipleResources()
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

        List<(int,Resource)> result = _taskService.GetResourcesWithName(resourceList);

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(r => r.Item2.Name == "Mouse" && r.Item1 == 1));
        Assert.IsTrue(result.Any(r => r.Item2.Name == "Keyboard" && r.Item1 == 3));
    }



}