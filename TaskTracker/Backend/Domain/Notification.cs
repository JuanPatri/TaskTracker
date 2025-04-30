namespace Backend.Domain;

public class Notification
{
    private string _message;
    private DateTime _date;
    
    public string Message
    {
        get => _message;
        set 
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("The notification message cannot be empty");
            _message = value;
        } 
    }
    
    public DateTime Date
    {
        get => _date;
        set => _date = value;
    }
}