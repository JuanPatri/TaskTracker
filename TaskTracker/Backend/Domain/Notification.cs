namespace Backend.Domain;

public class Notification
{
    private string _message;
    
    public string Message
    {
        get => _message;
        set => _message = value;
    }
}