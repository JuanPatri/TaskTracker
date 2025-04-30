namespace Backend.Domain;

public class ReadingNotification
{
    private List<User> _user;
    
    public List<User> User
    {
        get => _user;
        set
        {
            if (value == null) throw new ArgumentNullException("The user cannot be null");
            _user = value;
        }
    }
}