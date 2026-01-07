namespace Domain.Employee.Ports.Out;

public interface IEmployeeRepository
{
    Task<Entities.Employee> CreateAsync(Entities.Employee employee, CancellationToken cancellationToken);
    Task<Entities.Employee> UpdateAsync(Entities.Employee employee, CancellationToken cancellationToken);
    Task<int> DeleteAsync(Entities.Employee employee, CancellationToken cancellationToken);
    Task<Entities.Employee?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<List<Entities.Employee?>> GetAllAsync(CancellationToken cancellationToken, int skip = 0, int take = 10);
    Task<Entities.Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
