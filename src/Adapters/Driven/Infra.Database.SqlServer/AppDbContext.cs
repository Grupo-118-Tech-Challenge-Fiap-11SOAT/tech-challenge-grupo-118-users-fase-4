using Infra.Database.SqlServer.Customer.Configuration;
using Infra.Database.SqlServer.Employee.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Infra.Database.SqlServer;

public class AppDbContext : DbContext
{
    public DbSet<Domain.Customer.Entities.Customer> Customers { get; set; }
    public DbSet<Domain.Employee.Entities.Employee> Employees { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CustomersConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
    }
}
