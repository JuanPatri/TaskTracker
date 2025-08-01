﻿using Domain;
using DTOs.ProjectDTOs;
using DTOs.TaskDTOs;
using Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using RepositoryTest.Context;
using Service;
using Task = Domain.Task;

namespace ServiceTest;

[TestClass]
public class CriticalPathServiceTest
{
    private CriticalPathService _criticalPathService;
    private ProjectRepository _projectRepository;
    private TaskRepository _taskRepository;
    private ProjectService _projectService;
    private ResourceTypeRepository _resourceTypeRepository;
    private UserRepository _userRepository;
    private UserService _userService;
    private Project _project;
    private SqlContext _sqlContext;

    [TestInitialize]
    public void OnInitializated()
    {
        _sqlContext = SqlContextFactory.CreateMemoryContext();
        _projectRepository = new ProjectRepository(_sqlContext);
        _taskRepository = new TaskRepository(_sqlContext);
        _resourceTypeRepository = new ResourceTypeRepository(_sqlContext);
        _userRepository = new UserRepository(_sqlContext);
        _userService = new UserService(_userRepository);
        _criticalPathService = new CriticalPathService(_projectRepository, _taskRepository);
        _projectService = new ProjectService(_taskRepository, _projectRepository, _resourceTypeRepository,
            _userRepository, _userService, _criticalPathService);

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
    }

    [TestMethod]
    public void GetEstimatedProjectFinishDate_ShouldReturnCorrectFinishDate()
    {
        Task taskA = new Task { Title = "A", Duration = 2 };
        Task taskB = new Task { Title = "B", Duration = 3 };
        Task taskC = new Task { Title = "C", Duration = 1 };

        TaskDependency dependencyB = new TaskDependency
        {
            Id = 1,
            Task = taskB,
            Dependency = taskA
        };

        TaskDependency dependencyC = new TaskDependency
        {
            Id = 2,
            Task = taskC,
            Dependency = taskB
        };

        taskB.Dependencies = new List<TaskDependency> { dependencyB };
        taskC.Dependencies = new List<TaskDependency> { dependencyC };

        Project project = new Project
        {
            StartDate = new DateOnly(2025, 9, 12),
            Tasks = new List<Task> { taskA, taskB, taskC }
        };

        _criticalPathService.CalculateEarlyTimes(project);
        DateTime finish = _projectService.GetEstimatedProjectFinishDate(project);

        Assert.AreEqual(new DateTime(2025, 9, 18), finish);
    }

    [TestMethod]
    public void CalculateEarlyTimesSimpleSequenceComputesCorrectStartAndFinish()
    {
        Task taskA = new Task { Title = "A", Duration = 2 };
        Task taskB = new Task { Title = "B", Duration = 3 };
        Task taskC = new Task { Title = "C", Duration = 1 };

        TaskDependency dependencyB = new TaskDependency
        {
            Id = 1,
            Task = taskB,
            Dependency = taskA
        };

        TaskDependency dependencyC = new TaskDependency
        {
            Id = 2,
            Task = taskC,
            Dependency = taskB
        };

        taskB.Dependencies = new List<TaskDependency> { dependencyB };
        taskC.Dependencies = new List<TaskDependency> { dependencyC };

        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        Project project = new Project
        {
            StartDate = startDate,
            Tasks = new List<Task> { taskA, taskB, taskC }
        };

        _criticalPathService.CalculateEarlyTimes(project);

        DateTime baseStart = project.StartDate.ToDateTime(new TimeOnly(0, 0));

        Assert.AreEqual(baseStart, taskA.EarlyStart);
        Assert.AreEqual(baseStart.AddDays(2), taskA.EarlyFinish);

        Assert.AreEqual(taskA.EarlyFinish, taskB.EarlyStart);
        Assert.AreEqual(taskB.EarlyFinish, taskC.EarlyStart);

        Assert.AreEqual(taskB.EarlyFinish, taskC.EarlyStart);
        Assert.AreEqual(taskC.EarlyStart.AddDays(1), taskC.EarlyFinish);
    }

    [TestMethod]
    public void GetCriticalPathShouldReturnCorrectTasks()
    {
        Task taskA = new Task { Title = "A", Duration = 2 };
        Task taskB = new Task { Title = "B", Duration = 3 };
        Task taskC = new Task { Title = "C", Duration = 1 };
        Task taskD = new Task { Title = "D", Duration = 2 };

        TaskDependency dependencyB = new TaskDependency
        {
            Id = 1,
            Task = taskB,
            Dependency = taskA
        };

        TaskDependency dependencyC = new TaskDependency
        {
            Id = 2,
            Task = taskC,
            Dependency = taskA
        };

        TaskDependency dependencyD1 = new TaskDependency
        {
            Id = 3,
            Task = taskD,
            Dependency = taskB
        };

        TaskDependency dependencyD2 = new TaskDependency
        {
            Id = 4,
            Task = taskD,
            Dependency = taskC
        };

        taskB.Dependencies = new List<TaskDependency> { dependencyB };
        taskC.Dependencies = new List<TaskDependency> { dependencyC };
        taskD.Dependencies = new List<TaskDependency> { dependencyD1, dependencyD2 };

        Project project = new Project
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            Tasks = new List<Task> { taskA, taskB, taskC, taskD }
        };

        List<Task> result = _criticalPathService.GetCriticalPath(project);
        List<String> titles = result.Select(t => t.Title).ToList();

        Assert.AreEqual(3, result.Count);
        CollectionAssert.AreEquivalent(new List<string> { "A", "B", "D" }, titles);
    }

    [TestMethod]
    public void GetProjectWithCriticalPathShouldReturnCorrectDTO()
    {
        Task taskA = new Task
        {
            Title = "A",
            Duration = 2,
            Description = "Task A",
            Status = Status.Pending,
            EarlyStart = DateTime.MinValue,
            EarlyFinish = DateTime.MinValue,
            LateStart = DateTime.MinValue,
            LateFinish = DateTime.MinValue
        };

        Task taskB = new Task
        {
            Title = "B",
            Duration = 3,
            Description = "Task B",
            Status = Status.Pending,
            EarlyStart = DateTime.MinValue,
            EarlyFinish = DateTime.MinValue,
            LateStart = DateTime.MinValue,
            LateFinish = DateTime.MinValue
        };

        Task taskC = new Task
        {
            Title = "C",
            Duration = 1,
            Description = "Task C",
            Status = Status.Pending,
            EarlyStart = DateTime.MinValue,
            EarlyFinish = DateTime.MinValue,
            LateStart = DateTime.MinValue,
            LateFinish = DateTime.MinValue
        };

        TaskDependency dependencyB = new TaskDependency
        {
            Task = taskB,
            Dependency = taskA
        };

        TaskDependency dependencyC = new TaskDependency
        {
            Task = taskC,
            Dependency = taskB
        };

        taskB.Dependencies = new List<TaskDependency> { dependencyB };
        taskC.Dependencies = new List<TaskDependency> { dependencyC };

        _project.Id = 35;
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

    [TestMethod]
    public void CalculateLateTimesShouldComputeCorrectLateStartAndFinish()
    {
        Task taskA = new Task { Title = "A", Duration = 2 };
        Task taskB = new Task { Title = "B", Duration = 3 };
        Task taskC = new Task { Title = "C", Duration = 1 };

        TaskDependency dependencyB = new TaskDependency
        {
            Id = 1,
            Task = taskB,
            Dependency = taskA
        };

        TaskDependency dependencyC = new TaskDependency
        {
            Id = 2,
            Task = taskC,
            Dependency = taskB
        };

        taskB.Dependencies = new List<TaskDependency> { dependencyB };
        taskC.Dependencies = new List<TaskDependency> { dependencyC };

        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        Project project = new Project
        {
            StartDate = startDate,
            Tasks = new List<Task> { taskA, taskB, taskC }
        };

        _criticalPathService.CalculateLateTimes(project);

        DateTime baseStart = project.StartDate.ToDateTime(new TimeOnly(0, 0));

        Assert.AreEqual(baseStart.AddDays(5), taskC.LateStart);
        Assert.AreEqual(baseStart.AddDays(6), taskC.LateFinish);

        Assert.AreEqual(baseStart.AddDays(2), taskB.LateStart);
        Assert.AreEqual(baseStart.AddDays(5), taskB.LateFinish);

        Assert.AreEqual(baseStart, taskA.LateStart);
        Assert.AreEqual(baseStart.AddDays(2), taskA.LateFinish);
    }
}