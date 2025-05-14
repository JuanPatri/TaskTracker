using System.Text.RegularExpressions;
using Backend.DTOs.UserDTOs;

namespace Backend.Domain;

public class User
{
    private string _name;

    private string _lastName;

    private string _email;

    private DateTime _birthDate= DateTime.Today;

    private string _password;
    
    public bool Admin { get; set; }
    
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The username cannot be empty");
            _name = value;
            
            if(value.Any(char.IsDigit)) throw new ArgumentException("The username cannot contain digits");
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
                !value.Any(c => char.IsSymbol(c) || char.IsPunctuation(c)) || !value.Any(char.IsNumber) || value.Length < 8)
            {
                throw new ArgumentException("The password format is not correct");
            }
            
            _password = value;
        }
    }
    
    public static User FromDto(UserDataDTO userDataDto)
    {
        return new User()
        {
            Name = userDataDto.Name,
            LastName = userDataDto.LastName,
            Email = userDataDto.Email,
            BirthDate = userDataDto.BirthDate,
            Password = userDataDto.Password,
            Admin = userDataDto.Admin
        };
    }
}