using Domain.Employee.Ports.Out;
using Microsoft.EntityFrameworkCore;

namespace Infra.Database.SqlServer.Employee.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;
    public EmployeeRepository(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }

    /// <summary>
    /// Asynchronously creates a new employee record in the database.
    /// </summary>
    /// <param name="employee">The employee entity to be added to the database.</param>
    /// <returns>The created employee entity.</returns>
    /// <exception cref="DbUpdateException">Thrown if an error occurs while saving changes to the database.</exception>
    public async Task<Domain.Employee.Entities.Employee> CreateAsync(Domain.Employee.Entities.Employee employee, CancellationToken cancellationToken)
    {
        await _context.Employees.AddAsync(employee, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return employee;
    }

    /// <summary>
    /// Asynchronously deletes an employee record from the database.
    /// </summary>
    /// <param name="employee">The employee entity to be removed from the database.</param>
    /// <returns>The number of state entries written to the database.</returns>
    /// <exception cref="DbUpdateException">Thrown if an error occurs while saving changes to the database.</exception>
    public async Task<int> DeleteAsync(Domain.Employee.Entities.Employee employee, CancellationToken cancellationToken)
    {
        _context.Employees.Remove(employee);
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>  
    /// Asynchronously retrieves all employee records from the database.  
    /// </summary>  
    /// <returns>A list of all employee entities, or an empty list if no employees are found.</returns>  
    public async Task<List<Domain.Employee.Entities.Employee?>> GetAllAsync(CancellationToken cancellationToken, int skip = 0, int take = 10)
    {
        var employees = await _context.Employees
            .Skip(skip)
            .Take(take)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return employees;
    }

    public Task<Domain.Employee.Entities.Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    /// <summary>  
    /// Asynchronously retrieves an employee record from the database by its unique identifier.  
    /// </summary>  
    /// <param name="id">The unique identifier of the employee to retrieve.</param>  
    /// <returns>The employee entity if found; otherwise, null.</returns>  
    public async Task<Domain.Employee.Entities.Employee?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <summary>  
    /// Asynchronously updates an existing employee record in the database.  
    /// </summary>  
    /// <param name="employee">The employee entity with updated information to be saved to the database.</param>  
    /// <returns>The updated employee entity.</returns>  
    /// <exception cref="DbUpdateException">Thrown if an error occurs while saving changes to the database.</exception>  
    public async Task<Domain.Employee.Entities.Employee> UpdateAsync(Domain.Employee.Entities.Employee employee, CancellationToken cancellationToken)
    {
        _context.Update(employee);
        await _context.SaveChangesAsync(cancellationToken);

        return employee;
    }
}
