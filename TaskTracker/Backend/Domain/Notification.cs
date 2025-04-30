namespace Backend.Domain;

public class Notification
{
    private string _message;
    
    public string Message
    {
        get => _message;
        set 
        {
            if (value == null) throw new ArgumentNullException("The notification message cannot be empty");
            _message = value;
        } 
    }
}