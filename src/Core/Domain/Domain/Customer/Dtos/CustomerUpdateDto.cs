namespace Domain.Customer.Dtos;

public class CustomerUpdateDto
{
    public CustomerUpdateDto()
    {
    }

    public CustomerUpdateDto(Entities.Customer customer)
    {
        Id = customer.Id;
        IsActive = customer.IsActive;
        Cpf = customer.Cpf;
        Name = customer.Name;
        Email = customer.Email;
        BirthDate = customer.BirthDay;
    }

    public int Id { get; set; }
    public string Cpf { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsActive { get; set; }

    public static CustomerUpdateDto ToDto(Entities.Customer customer)
    {
        return new CustomerUpdateDto
        {
            Id = customer.Id,
            Cpf = customer.Cpf,
            Name = customer.Name,
            Surname = customer.Surname,
            Email = customer.Email,
            BirthDate = customer.BirthDay,
            IsActive = customer.IsActive
        };
    }

    public static Entities.Customer ToEntity(CustomerUpdateDto customerUpdateDto)
    {
        return new Entities.Customer(
            customerUpdateDto.Cpf,
            customerUpdateDto.Name,
            customerUpdateDto.Surname,
            customerUpdateDto.Email,
            customerUpdateDto.BirthDate,
            customerUpdateDto.IsActive,
            customerUpdateDto.Id
            );
    }
}
