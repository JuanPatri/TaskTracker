namespace Backend.Domain;

public class User
{
    private string _name { get; set; }
    
    private string _lastName { get; set; }
    
    private string _email { get; set; }

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The username cannot be empty");
            _name = value;
        }
    }
    
    public string LastName
    {
        get => _lastName;
        set
        {
            if(string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The user last name cannot be empty");
            _lastName = value;
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if (value == null) throw new ArgumentException("The email cannot be empty");
            _email = value;
        }
        
    } 
}