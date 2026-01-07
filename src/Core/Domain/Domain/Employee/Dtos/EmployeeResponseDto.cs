using Domain.Employee.ValueObjects;

namespace Domain.Employee.Dtos;

public class EmployeeResponseDto
{
    public EmployeeResponseDto()
    {

    }

    public EmployeeResponseDto(
        int id,
        string cpf,
        string name,
        string surname,
        string email,
        DateTimeOffset birthdate,
        EmployeeRole role,
        bool isActive)
    {
        Id = id;
        Cpf = cpf;
        Name = name;
        Surname = surname;
        Email = email;
        BirthDate = birthdate;
        Role = role;
        IsActive = isActive;
    }

    public int Id { get; set; }
    public string Cpf { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public DateTimeOffset BirthDate { get; set; }
    public EmployeeRole Role { get; set; }
    public bool IsActive { get; set; }
    public bool Error { get; set; }
    public string ErrorMessage { get; set; }

    public static EmployeeResponseDto ToDto(Entities.Employee employee, bool error = false, string errorMessage = "")
    {
        return new EmployeeResponseDto
        {
            Id = employee.Id,
            Cpf = employee.Cpf,
            Name = employee.Name,
            Surname = employee.Surname,
            Email = employee.Email,
            BirthDate = employee.BirthDay,
            Role = employee.Role,
            IsActive = employee.IsActive,
            Error = error,
            ErrorMessage = errorMessage
        };
    }
}
