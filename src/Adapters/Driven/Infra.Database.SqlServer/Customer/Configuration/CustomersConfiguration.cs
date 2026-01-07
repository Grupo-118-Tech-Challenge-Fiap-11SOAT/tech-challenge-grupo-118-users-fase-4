using Microsoft.EntityFrameworkCore;

namespace Infra.Database.SqlServer.Customer.Configuration;

public class CustomersConfiguration : IEntityTypeConfiguration<Domain.Employee.Entities.Employee>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Domain.Employee.Entities.Employee> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(builder => builder.Id);
        builder.Property(builder => builder.Id)
            .UseIdentityColumn();

        builder.Property(builder => builder.Cpf)
            .HasMaxLength(11);

        builder.Property(builder => builder.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(builder => builder.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(builder => builder.BirthDay)
            .IsRequired();

        builder.Property(builder => builder.CreatedAt)
            .IsRequired()
            .HasColumnType("datetimeoffset")
            .HasDefaultValueSql("SYSDATETIMEOFFSET()")
            .ValueGeneratedOnAdd();

        builder.Property(builder => builder.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .HasDefaultValueSql("SYSDATETIMEOFFSET()")
            .ValueGeneratedOnUpdate();

        builder.Property(builder => builder.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(builder => builder.Email)
            .IsUnique()
            .HasDatabaseName("IX_Employees_Email");

        builder.HasIndex(builder => builder.Cpf)
            .IsUnique()
            .HasDatabaseName("IX_Employees_Cpf");
    }
}
