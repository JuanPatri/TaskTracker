namespace Backend.Domain;

public class Task
{
    private string _title { get; set; }
    private DateOnly _date { get; set; }

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
            _date = value;

        }
    }
}