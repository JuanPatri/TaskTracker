using Backend.Domain.Enums;

namespace Backend.Domain;

public class Notification
{
    private string _message;
    private DateTime _date;
    private TypeOfNotification _validateTypeOfNotification;
    private int _impact;
    
    private const int MinImpact = 1;
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
            _date = value;
        }
    }
    
    public TypeOfNotification ValidateTypeOfNotification
    {
        get => _validateTypeOfNotification;
        set => _validateTypeOfNotification = value;
    }
    
    public int Impact
    {
        get => _impact;
        set
        {
            if (value < MinImpact) throw new ArgumentException("The impact must be greater than 0");
            _impact = value;
        }
    }
}