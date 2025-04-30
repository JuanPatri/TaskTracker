namespace Backend.Domain;

public class ReadingNotification
{
    private List<User> _user;
    
    public List<User> User
    {
        get => _user;
        set
        {
            _user = value;
        }
    }
}