namespace Backend.Domain;

public class User
{
    private string _name { get; set; }

    public string ValidateName
    {
        get => _name;
        set
        {
            if (value == null) throw new ArgumentException("The username cannot be empty");
            _name = value;
        }
    }
}