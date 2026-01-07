using Domain.Customer.Dtos;
using Domain.Customer.Ports.In;
using Domain.Customer.Ports.Out;

namespace Application.Customer;

public class CustomerManager : ICustomerManager
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerManager(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerResponseDto> CreateAsync(CustomerRequestDto customerRequestDto, CancellationToken cancellationToken)
    {
        var customer = new Domain.Customer.Entities.Customer(
            customerRequestDto.Cpf,
            customerRequestDto.Name,
            customerRequestDto.Surname,
            customerRequestDto.Email,
            customerRequestDto.BirthDate,
            true
            );

        var createdCustomer = await _customerRepository.CreateAsync(customer, cancellationToken);

        return CustomerResponseDto.ToDto(createdCustomer);
    }

    public async Task<CustomerResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer == null)
        {
            return null;
        }

        return CustomerResponseDto.ToDto(customer);
    }

    public async Task<CustomerResponseDto?> GetByCpfAsync(string cpf, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByCpfAsync(cpf, cancellationToken);
        if (customer == null)
        {
            return null;
        }

        return CustomerResponseDto.ToDto(customer);
    }

    public async Task<CustomerResponseDto?> UpdateAsync(CustomerUpdateDto customerUpdateDto, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(customerUpdateDto.Id, cancellationToken);

        if (customer == null)
        {
            return new CustomerResponseDto
            {
                ErrorMessage = "Customer not found.",
                Error = true
            };
        }

        customer.UpdateCustomer(
            customerUpdateDto.Cpf,
            customerUpdateDto.Name,
            customerUpdateDto.Surname,
            customerUpdateDto.Email,
            customerUpdateDto.BirthDate,
            customerUpdateDto.IsActive);

        var updatedCustomer = await _customerRepository.UpdateAsync(customer, cancellationToken);

        return CustomerResponseDto.ToDto(updatedCustomer);
    }
}
