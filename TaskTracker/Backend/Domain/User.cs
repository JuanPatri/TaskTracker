using System.Text.RegularExpressions;

namespace Backend.Domain;

public class User
{
    private string _name { get; set; }
    
    private string _lastName { get; set; }
    
    private string _email { get; set; }

    private DateTime _birthDate { get; set; }
    
    private string _password { get; set; }
    
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
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The email cannot be empty");
            if (!Regex.IsMatch(value, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")) throw new ArgumentException("Error in the email format");
            _email = value;
        }
        
    } 
    
    public DateTime BirthDate
    {
        get => _birthDate;
        set 
        {
            if (value == default(DateTime)) throw new ArgumentException("The birth date cannot be empty");
            _birthDate = value;
        }
    }
    
    public string Password
    {
        get => _password;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Any(char.IsUpper) || !value.Any(char.IsLower) ||
                !value.Any(c => char.IsSymbol(c) || char.IsPunctuation(c)) || !value.Any(char.IsNumber) )
            {
                throw new ArgumentException("The password format is not correct");
            }

            _password = value;
        }
    }
}