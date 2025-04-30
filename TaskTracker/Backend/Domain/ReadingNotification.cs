namespace Backend.Domain;

public class ReadingNotification
{
    private List<User> _user;
    private List<Notification> _notification;
    private bool _read;
    
    public List<User> User
    {
        get => _user;
        set
        {
            if (value == null) throw new ArgumentNullException("The user cannot be null");
            _user = value;
        }
    }
    
    public List<Notification> Notification
    {
        get => _notification;
        set 
        {
            if (value == null) throw new ArgumentNullException("The notification cannot be null");
            _notification = value;
        }
    }
    
    public bool Read
    {
        get => _read;
        set
        {
            _read = value;
        }
    }
}