namespace Domain.Customer.Dtos;

public class CustomerResponseDto
{
    public CustomerResponseDto()
    {
    }

    public CustomerResponseDto(
        int id,
        string cpf,
        string name,
        string surname,
        string email,
        DateTime birthDate,
        bool isActive
        )
    {
        Id = id;
        Cpf = cpf;
        Name = name;
        Surname = surname;
        Email = email;
        BirthDate = birthDate;
        IsActive = isActive;
    }

    public int Id { get; set; }
    public string Cpf { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsActive { get; set; }
    public bool Error { get; set; }
    public string ErrorMessage { get; set; }

    public static CustomerResponseDto ToDto(Entities.Customer customer)
    {
        return new CustomerResponseDto
        (
            customer.Id,
            customer.Cpf,
            customer.Name,
            customer.Surname,
            customer.Email,
            customer.BirthDay,
            customer.IsActive
        );
    }
}
