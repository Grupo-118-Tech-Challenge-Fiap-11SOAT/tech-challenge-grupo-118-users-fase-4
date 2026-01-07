using Domain.Base.Entities;
using Domain.Base.Exceptions;
using Domain.Base.Extensions;
using Domain.Employee.Exceptions;
using Domain.Employee.ValueObjects;

namespace Domain.Employee.Entities;

public class Employee : Person
{
    public string Password { get; protected set; }
    public EmployeeRole Role { get; protected set; }

    public Employee(string cpf,
        string name,
        string surname,
        string email,
        DateTime birthday,
        string password,
        EmployeeRole role,
        bool isActive,
        int id = 0)
    {
        Cpf = cpf;
        Name = name;
        Surname = surname;
        Email = email;
        BirthDay = birthday;
        Password = password;
        Role = role;
        IsActive = isActive;

        if (id != 0)
            Id = id;

        ValidateEmployee();
    }

    protected Employee()
    {
    }

    private void ValidateEmployee()
    {
        if (string.IsNullOrEmpty(Cpf))
        {
            throw new CpfNullOrEmptyException();
        }

        if (!Cpf.IsValidCpf())
        {
            throw new InvalidCpfException();
        }

        if (string.IsNullOrEmpty(Name))
        {
            throw new NameNullOrEmptyException();
        }

        if (string.IsNullOrEmpty(Surname))
        {
            throw new SurnameNullOrEmptyException();
        }

        if (string.IsNullOrEmpty(Email))
        {
            throw new EmailNullOrEmptyException();
        }

        if (!Email.IsValidEmail())
        {
            throw new InvalidEmailException();
        }

        if (BirthDay == DateTime.MinValue)
        {
            throw new BirthDayMinValueException();
        }

        if (string.IsNullOrEmpty(Password))
        {
            throw new PasswordNullOrEmptyException();
        }
    }

    public void SetPassword(string password)
    {
        this.Password = password;
    }

    public void UpdateEmployee(string cpf,
        string name,
        string surname,
        string email,
        DateTime birthday,
        string password,
        EmployeeRole role,
        bool isActive)
    {
        Cpf = cpf;
        Name = name;
        Surname = surname;
        Email = email;
        BirthDay = birthday;
        Password = password;
        Role = role;
        IsActive = isActive;
        UpdatedAt = DateTimeOffset.Now;

        ValidateEmployee();
    }
}
