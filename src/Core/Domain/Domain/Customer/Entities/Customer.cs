using Domain.Base.Entities;
using Domain.Base.Exceptions;
using Domain.Base.Extensions;

namespace Domain.Customer.Entities;

public class Customer : Person
{
    public Customer()
    {
    }

    public Customer(string cpf,
        string name,
        string surname,
        string email,
        DateTime birthday,
        bool isActive,
        int id = 0)
    {
        Cpf = cpf;
        Name = name;
        Surname = surname;
        Email = email;
        BirthDay = birthday;
        IsActive = isActive;
        CreatedAt = DateTimeOffset.Now;

        if (id != 0)
            Id = id;

        ValidateCustomer();
    }

    public void UpdateCustomer(string cpf,
        string name,
        string surname,
        string email,
        DateTime birthday,
        bool isActive)
    {
        Cpf = cpf;
        Name = name;
        Surname = surname;
        Email = email;
        BirthDay = birthday;
        IsActive = isActive;
        UpdatedAt = DateTimeOffset.Now;

        ValidateCustomer();
    }

    private void ValidateCustomer()
    {
        if (!Cpf.IsValidCpf())
        {
            throw new InvalidCpfException();
        }

        if (!Email.IsValidEmail())
        {
            throw new InvalidEmailException();
        }
    }
}
