using Domain.Employee.ValueObjects;

namespace Domain.Employee.Dtos;

public class EmployeeRequestDto
{
    public EmployeeRequestDto()
    {
    }

    public EmployeeRequestDto(
        string cpf,
        string name,
        string surname,
        string email,
        DateTimeOffset birthDay,
        string password,
        EmployeeRole role)
    {
        Cpf = cpf;
        Name = name;
        Surname = surname;
        Email = email;
        BirthDay = birthDay;
        Password = password;
        Role = role;
    }

    public string Cpf { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public DateTimeOffset BirthDay { get; set; }
    public string Password { get; set; }
    public EmployeeRole Role { get; set; }

    public static EmployeeRequestDto ToDto(Entities.Employee employee)
    {
        return new EmployeeRequestDto
        {
            Cpf = employee.Cpf,
            Name = employee.Name,
            Surname = employee.Surname,
            Email = employee.Email,
            BirthDay = employee.BirthDay,
            Password = employee.Password,
            Role = employee.Role
        };
    }

    public static Entities.Employee ToEntity(EmployeeRequestDto employeeRequestDto)
    {
        return new Entities.Employee(
            employeeRequestDto.Cpf,
            employeeRequestDto.Name,
            employeeRequestDto.Surname,
            employeeRequestDto.Email,
            employeeRequestDto.BirthDay.DateTime,
            employeeRequestDto.Password,
            employeeRequestDto.Role,
            true);
    }
}
