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
        set 
        {
            if (value < DateTime.Now) throw new ArgumentException("The notification date cannot be in the past");
            if (value > DateTime.Now.AddMinutes(1)) throw new ArgumentException("The notification date cannot be in the future");
            _date = value;
        }
    }
}