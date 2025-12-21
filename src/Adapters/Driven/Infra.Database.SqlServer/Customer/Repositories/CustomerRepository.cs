using Domain.Customer.Entities;
using Domain.Customer.Ports.Out;
using Microsoft.EntityFrameworkCore;

namespace Infra.Database.SqlServer.Customer.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _dbContext;

    public CustomerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Domain.Customer.Entities.Customer> CreateAsync(Domain.Customer.Entities.Customer customer, CancellationToken cancellationToken)
    {
        await _dbContext.Customers.AddAsync(customer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return customer;
    }

    public async Task<Domain.Customer.Entities.Customer> UpdateAsync(Domain.Customer.Entities.Customer customer, CancellationToken cancellationToken)
    {
        _dbContext.Update(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return customer;
    }

    public async Task<Domain.Customer.Entities.Customer?> GetByIdAsync(int id, CancellationToken cancellationToken) => await _dbContext.Customers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Domain.Customer.Entities.Customer?> GetByCpfAsync(string cpf, CancellationToken cancellationToken) => await _dbContext.Customers.FirstOrDefaultAsync(x => x.Cpf == cpf, cancellationToken);
}
