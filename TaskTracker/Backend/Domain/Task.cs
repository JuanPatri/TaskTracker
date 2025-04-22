namespace Backend.Domain;
using Backend.Domain.Enums;
public class Task
{
    private string _title { get; set; }
    private DateOnly _date { get; set; }
    private TimeSpan _durationTask { get; set; }
    private Status _status { get; set; }
    public string ValidateTitle
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The title name cannot be empty");
            _title = value;
        }
    }

    public DateOnly ValidateDate
    {
        get => _date;
        set
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            if (value > today)
                throw new ArgumentException("The date cannot be in the future.");
            
            _date = value;
        }
    }

    public TimeSpan ValidateDurationTask
    {
        get => _durationTask;
        set => _durationTask = value;
    }

    public Status ValidateStatus
    {
        get => _status;
        set => _status = value;
    }
}