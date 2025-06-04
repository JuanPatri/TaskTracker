using Domain;
using DTOs.TaskDTOs;
using Enums;
using Repository;

namespace Service;

public class NotificationService
{
    private readonly IRepository<Notification> _notificationRepository;
    private int _notificationId;
    private readonly ProjectService _projectService;

    public NotificationService(IRepository<Notification> notificationRepository, ProjectService projectService){
        _notificationRepository = notificationRepository;
        _notificationId = 1;
        _projectService = projectService;
    }

    public TypeOfNotification ObtenerTipoDeNotificacionPorImpacto(int impacto)
    {
        if (impacto > 0)
            return TypeOfNotification.Delay;
        else
            return TypeOfNotification.DurationAdjustment;
    }

    public int CalcularImpacto(int duracionVieja, int duracionNueva)
    {
        return duracionNueva - duracionVieja;
    }

    public DateTime GetNewEstimatedEndDate(int projectId)
    {
        var proyecto = _projectService.GetProjectById(projectId);
        if (proyecto == null)
            throw new ArgumentException("Proyecto no encontrado");

        return _projectService.GetEstimatedProjectFinishDate(proyecto);
    }
    
    public string GenerateNotificationMessage(TypeOfNotification type, string taskTitle, DateTime newEstimatedEndDate)
    {
        switch (type)
        {
            case TypeOfNotification.Delay:
                return
                    $"The critical task '{taskTitle}' has caused a delay. The new estimated project end date is {newEstimatedEndDate:yyyy-MM-dd}.";
            case TypeOfNotification.DurationAdjustment:
                return
                    $"The duration of the critical task '{taskTitle}' was adjusted. The new estimated project end date is {newEstimatedEndDate:yyyy-MM-dd}.";
            default:
                return
                    $"The task '{taskTitle}' has had a change. The new estimated project end date is {newEstimatedEndDate:yyyy-MM-dd}.";
        }
    }

    public Notification CreateNotification(int duracionVieja, int duracionNueva, int projectId, string taskTitle)
    {
        int impacto = CalcularImpacto(duracionVieja, duracionNueva);
        TypeOfNotification tipo = ObtenerTipoDeNotificacionPorImpacto(impacto);
        DateTime nuevaFechaFin = GetNewEstimatedEndDate(projectId);
        List<User> users = _projectService.GetUsersFromProject(projectId);
        string message = GenerateNotificationMessage(tipo, taskTitle, nuevaFechaFin);

        var notificacion = new Notification
        {
            Id = _notificationId++,
            Message = message,
            TypeOfNotification = tipo,
            Impact = impacto,
            Date = nuevaFechaFin,
            Users = users,
        };

        return _notificationRepository.Add(notificacion);
    }

    public List<Notification> GetNotificationsForUser(string email)
    {
        return _notificationRepository
            .FindAll()
            .Where(n => n.Users != null && n.Users.Any(u => u.Email == email))
            .Where(n => n.ViewedBy == null || !n.ViewedBy.Contains(email))
            .ToList();
    }

    public void MarkNotificationAsViewed(int notificationId, string userEmail)
    {
        var notification = _notificationRepository.Find(n => n.Id == notificationId);
        if (notification != null && !notification.ViewedBy.Contains(userEmail))
        {
            notification.ViewedBy.Add(userEmail);
            if (notification.Projects == null)
            {
                notification.Projects = new List<Project>();
            }

            _notificationRepository.Update(notification);
        }
    }

    public List<Notification> GetUnviewedNotificationsForUser(string email)
    {
        return _notificationRepository
            .FindAll()
            .Where(n => n.Users != null && n.Users.Any(u => u.Email == email) && !n.ViewedBy.Contains(email))
            .ToList();
    }
}