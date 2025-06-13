using BusinessLogicTest.Context;
using Domain;
using DTOs.NotificationDTOs;
using Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Service;
using Task = Domain.Task;

namespace ServiceTest;

[TestClass]
public class NotificationServiceTest
{
    private UserRepository _userRepository;
    private ProjectRepository _projectRepository;
    private NotificationService _notificationService;
    private NotificationRepository _notificationRepository;
    private ProjectService _projectService;
    private ResourceTypeRepository _resourceTypeRepository;
    private UserService _userService;
    private CriticalPathService _criticalPathService;
    private TaskRepository _taskRepository;
    private SqlContext _sqlContext;
    
    [TestInitialize]
    public void OnInitializated()
    {
        _sqlContext = SqlContextFactory.CreateMemoryContext();
        _userRepository = new UserRepository(_sqlContext);
        _projectRepository = new ProjectRepository(_sqlContext);
        _notificationRepository = new NotificationRepository();
        _resourceTypeRepository = new ResourceTypeRepository();
        _taskRepository = new TaskRepository();
        _userService = new UserService(_userRepository);
        _criticalPathService = new CriticalPathService(_projectRepository, _taskRepository);
        _projectService = new ProjectService(_taskRepository, _projectRepository, _resourceTypeRepository, _userRepository, _userService, _criticalPathService);
        _notificationService = new NotificationService(_notificationRepository, _projectService);
    }
    
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
            _notificationRepository.Add(_notificationService.FromDto(notificationDto, users, projects, new List<string>()));

        Assert.IsNotNull(notification);
        Assert.AreEqual(notificationDto.Message, notification.Message);
        Assert.AreEqual(notificationDto.Impact, notification.Impact);
        Assert.AreEqual(notificationDto.TypeOfNotification, notification.TypeOfNotification);
    }

    [TestMethod]
    public void GetNotificationTypeByImpact_ShouldReturnCorrectType()
    {
        var service = new ProjectService(null, null, null, null, null, null);

        Assert.AreEqual(TypeOfNotification.Delay, _notificationService.ObtenerTipoDeNotificacionPorImpacto(1));
        Assert.AreEqual(TypeOfNotification.DurationAdjustment, _notificationService.ObtenerTipoDeNotificacionPorImpacto(0));
        Assert.AreEqual(TypeOfNotification.DurationAdjustment, _notificationService.ObtenerTipoDeNotificacionPorImpacto(-5));
    }

    [TestMethod]
    public void CalculateImpact_ShouldReturnDifferenceBetweenDurations()
    {
        var service = new ProjectService(null, null, null, null, null, null);

        int impact = _notificationService.CalcularImpacto(5, 8);

        Assert.AreEqual(3, impact);

        impact = _notificationService.CalcularImpacto(10, 7);
        Assert.AreEqual(-3, impact);

        impact = _notificationService.CalcularImpacto(4, 4);
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

        var date = _notificationService.GetNewEstimatedEndDate(123);

        var expected = project.StartDate.ToDateTime(new TimeOnly(0, 0)).AddDays(3);
        Assert.AreEqual(expected, date);
    }

    [TestMethod]
    public void GetUsersFromProject_ShouldReturnProjectUsers()
    {
        User user1 = new User { Name = "John", Email = "john@mail.com" };
        User user2 = new User { Name = "Anna", Email = "anna@mail.com" };
        _userRepository.Add(user1);
        _userRepository.Add(user2);

        Project project = new Project
        {
            Id = 10,
            Name = "Project X",
            Description = "Test description",
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

        List<User> users = _projectService.GetUsersFromProject(10);

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

        var delayMessage = _notificationService.GenerateNotificationMessage(TypeOfNotification.Delay, "Task1", date);
        Assert.AreEqual(
            "The critical task 'Task1' has caused a delay. The new estimated project end date is 2024-06-10.",
            delayMessage);

        var adjustmentMessage =
            _notificationService.GenerateNotificationMessage(TypeOfNotification.DurationAdjustment, "Task2", date);
        Assert.AreEqual(
            "The duration of the critical task 'Task2' was adjusted. The new estimated project end date is 2024-06-10.",
            adjustmentMessage);

        var defaultMessage = _notificationService.GenerateNotificationMessage((TypeOfNotification)99, "Task3", date);
        Assert.AreEqual("The task 'Task3' has had a change. The new estimated project end date is 2024-06-10.",
            defaultMessage);
    }

    [TestMethod]
    public void CreateNotification_ShouldCreateAndReturnNotificationWithCorrectData()
    {
        User user = new User { Name = "John", Email = "john@mail.com" };
    
        Project project = new Project
        {
            Id = 1,
            Name = "Project Test",
            Description = "Test description",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            Tasks = new List<Task> { new Task { Title = "Task1", Duration = 5 } }
        };

        ProjectRole userRole = new ProjectRole
        {
            RoleType = RoleType.ProjectMember,
            User = user,
            Project = project
        };

        project.ProjectRoles = new List<ProjectRole> { userRole };
    
        _userRepository.Add(user);
        _projectRepository.Add(project);

        int oldDuration = 5;
        int newDuration = 8;
        string taskTitle = "Task1";

        var notification = _notificationService.CreateNotification(oldDuration, newDuration, 1, taskTitle);

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

        var notifications = _notificationService.GetNotificationsForUser("john@mail.com");

        Assert.AreEqual(1, notifications.Count);
        Assert.AreEqual("Test notification", notifications[0].Message);
        Assert.AreEqual(TypeOfNotification.Delay, notifications[0].TypeOfNotification);
        Assert.AreEqual(2, notifications[0].Impact);
    }

    [TestMethod]
    public void GetNotificationsForUserShouldReturnEmptyListWhenNoNotificationsForUser()
    {
        var notifications = _notificationService.GetNotificationsForUser("nonexistent@mail.com");

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
        
        _notificationRepository.Add(notification);
        
        _notificationService.MarkNotificationAsViewed(1, "user@email.com");

        var updated = _notificationRepository.Find(n => n.Id == 1);
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

        _notificationRepository.Add(notificationViewed);
        _notificationRepository.Add(notificationUnviewed);

        var unviewed = _notificationService.GetUnviewedNotificationsForUser("john@mail.com");

        Assert.AreEqual(1, unviewed.Count);
        Assert.AreEqual("Unviewed notification", unviewed[0].Message);
    }
}