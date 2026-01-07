namespace Domain.Customer.Ports.Out;

public interface ICustomerRepository
{
    Task<Entities.Customer> CreateAsync(Entities.Customer customer, CancellationToken cancellationToken);
    Task<Entities.Customer> UpdateAsync(Entities.Customer customer, CancellationToken cancellationToken);
    Task<Entities.Customer?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Entities.Customer?> GetByCpfAsync(string Cpf, CancellationToken cancellationToken);
}
