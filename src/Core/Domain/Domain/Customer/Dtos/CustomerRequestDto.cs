namespace Domain.Customer.Dtos;

public class CustomerRequestDto
{
    public CustomerRequestDto()
    {
    }

    public CustomerRequestDto(
        string cpf,
        string name,
        string surname,
        string email,
        DateTimeOffset birthDate)
    {
        Cpf = cpf;
        Name = name;
        Surname = surname;
        Email = email;
        BirthDate = birthDate.DateTime;
    }

    public string Cpf { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }

    public static CustomerRequestDto ToDto(Entities.Customer customer)
    {
        return new CustomerRequestDto
        {
            Cpf = customer.Cpf,
            Name = customer.Name,
            Surname = customer.Surname,
            Email = customer.Email,
            BirthDate = customer.BirthDay,
        };
    }

    public static Entities.Customer ToEntity(CustomerRequestDto customerRequestDto)
    {
        return new Entities.Customer(
            customerRequestDto.Cpf,
            customerRequestDto.Name,
            customerRequestDto.Surname,
            customerRequestDto.Email,
            customerRequestDto.BirthDate,
            true);
    }
}
