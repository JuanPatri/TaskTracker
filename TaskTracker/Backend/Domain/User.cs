namespace Backend.Domain;

public class User
{
    private string _name { get; set; }
    
    private string _lastName { get; set; }

    public string ValidateName
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The username cannot be empty");
            _name = value;
        }
    }
    
    public string ValidateLastName
    {
        get => _lastName;
        set
        {
            _lastName = value;
        }
    }

}