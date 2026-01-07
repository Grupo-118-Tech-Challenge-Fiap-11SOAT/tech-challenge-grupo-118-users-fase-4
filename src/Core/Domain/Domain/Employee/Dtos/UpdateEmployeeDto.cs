using Domain.Employee.ValueObjects;

namespace Domain.Employee.Dtos;

public class UpdateEmployeeDto
{
    public UpdateEmployeeDto()
    {
    }

    public UpdateEmployeeDto(Entities.Employee employee)
    {
        Id = employee.Id;
        IsActive = employee.IsActive;
        Cpf = employee.Cpf;
        Name = employee.Name;
        Surname = employee.Surname;
        Email = employee.Email;
        BirthDate = employee.BirthDay;
        Password = employee.Password;
        Role = employee.Role;
    }

    public int Id { get; set; }
    public string Cpf { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public string Password { get; set; }
    public EmployeeRole Role { get; set; }
    public bool IsActive { get; set; }

    public static UpdateEmployeeDto ToDto(Entities.Employee employee)
    {
        return new UpdateEmployeeDto
        {
            Id = employee.Id,
            IsActive = employee.IsActive,
            Cpf = employee.Cpf,
            Name = employee.Name,
            Surname = employee.Surname,
            Email = employee.Email,
            BirthDate = employee.BirthDay,
            Password = employee.Password,
            Role = employee.Role
        };
    }

    public static Entities.Employee ToEntity(UpdateEmployeeDto employeeDto)
    {
        return new Entities.Employee(employeeDto.Cpf,
            employeeDto.Name,
            employeeDto.Surname,
            employeeDto.Email,
            employeeDto.BirthDate,
            employeeDto.Password,
            employeeDto.Role,
            employeeDto.IsActive,
            employeeDto.Id);
    }
}
