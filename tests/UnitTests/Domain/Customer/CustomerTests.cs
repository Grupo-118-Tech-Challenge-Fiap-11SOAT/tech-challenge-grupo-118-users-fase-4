using Domain.Base.Exceptions;
using CustomerEntity = Domain.Customer.Entities.Customer;
using FluentAssertions;

namespace UnitTests.Domain.Customer;

public class CustomerTests
{
    [Fact]
    [Trait("Category", "Entity")]
    public void Constructor_WithValidData_ShouldCreateCustomer()
    {
        // Arrange
        var cpf = "12345678909"; // Assumindo que a extensão valide este formato
        var name = "João";
        var surname = "Silva";
        var email = "joao@email.com";
        var birthday = new DateTime(1990, 1, 1);
        var isActive = true;

        // Act
        var customer = new CustomerEntity(cpf, name, surname, email, birthday, isActive);

        // Assert
        customer.Cpf.Should().Be(cpf);
        customer.Name.Should().Be(name);
        customer.Email.Should().Be(email);
        customer.CreatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(1));
        customer.Id.Should().Be(0); // Default quando não passado
    }

    [Fact]
    public void Constructor_WithInvalidCpf_ShouldThrowInvalidCpfException()
    {
        // Arrange
        var invalidCpf = "123";

        // Act
        Action act = () => new CustomerEntity(invalidCpf, "Name", "Surname", "email@test.com", DateTime.Now, true);

        // Assert
        act.Should().Throw<InvalidCpfException>();
    }

    [Fact]
    public void Constructor_WithInvalidEmail_ShouldThrowInvalidEmailException()
    {
        // Arrange
        var invalidEmail = "email-invalido";

        // Act
        Action act = () => new CustomerEntity("12345678909", "Name", "Surname", invalidEmail, DateTime.Now, true);

        // Assert
        act.Should().Throw<InvalidEmailException>();
    }

    [Fact]
    public void UpdateCustomer_WithValidData_ShouldUpdatePropertiesAndSetUpdatedAt()
    {
        // Arrange
        var customer = new CustomerEntity("12345678909", "Original", "Name", "old@email.com", DateTime.Now, true);
        var newName = "Updated Name";
        var newEmail = "new@email.com";

        // Act
        customer.UpdateCustomer("12345678909", newName, "Name", newEmail, DateTime.Now, true);

        // Assert
        customer.Name.Should().Be(newName);
        customer.Email.Should().Be(newEmail);
        customer.UpdatedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithSpecificId_ShouldAssignIdCorrectly()
    {
        // Arrange & Act
        var customer = new CustomerEntity("12345678909", "Name", "Surname", "test@test.com", DateTime.Now, true, id: 99);

        // Assert
        customer.Id.Should().Be(99);
    }
}
