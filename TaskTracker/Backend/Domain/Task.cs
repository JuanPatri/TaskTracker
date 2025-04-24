namespace Backend.Domain;
using Enums;
public class Task
{
    private string _title = String.Empty; 
    private string _description = String.Empty;
    private DateOnly _date = DateOnly.FromDateTime(DateTime.Now);
    private TimeSpan _durationTask;
    private Status _status = Status.Pending;
    private DateTime _earliestStartDate;
    private DateTime _earliestEndDate;
    
    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The title name cannot be empty");
            _title = value;
        }
    }

    public string Description
    {
        get => _description;
        set => _description = value;
    }
    public DateOnly Date
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

    public TimeSpan DurationTask
    {
        get => _durationTask;
        set => _durationTask = value;
    }

    public Status Status
    {
        get => _status;
        set => _status = value;
    }
    
    public DateTime EarliestStartDate
    {
        get => _earliestStartDate;
        set
        {
            DateTime today = DateTime.Now;
            if (value < today)
                throw new ArgumentException("The date cannot be in the past.");
            
            _earliestStartDate = value;
        } 
    }

    public DateTime EarliestEndDate
    {
        get => _earliestEndDate;
        set
        {
            DateTime today = DateTime.Now;
            if (value < today)
                throw new ArgumentException("The date cannot be in the past.");
            _earliestEndDate = value;
        }
        
    }
    
    }