using Domain.Employee.Exceptions;
using Domain.Employee.ValueObjects;
using FluentAssertions;
using EmployeeEntity = Domain.Employee.Entities.Employee;

namespace UnitTests.Domain.Employee;

public class EmployeeTests
{
    [Fact]
    [Trait("Category", "Entity")]
    public void Constructor_WithValidData_ShouldCreateEmployee()
    {
        // Arrange
        var cpf = "12345678909";
        var name = "Lucas";
        var surname = "Oliveira";
        var email = "lucas@empresa.com";
        var birthday = new DateTime(1995, 5, 20);
        var password = "SafePassword123";
        var role = EmployeeRole.Admin; // Assumindo que este Enum/ValueObject existe
        var isActive = true;

        // Act
        var employee = new EmployeeEntity(cpf, name, surname, email, birthday, password, role, isActive);

        // Assert
        employee.Cpf.Should().Be(cpf);
        employee.Name.Should().Be(name);
        employee.Password.Should().Be(password);
        employee.Role.Should().Be(role);
        employee.Id.Should().Be(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithEmptyCpf_ShouldThrowCpfNullOrEmptyException(string invalidCpf)
    {
        // Act
        Action act = () => new EmployeeEntity(invalidCpf, "Name", "Surname", "test@test.com", DateTime.Now, "pass", EmployeeRole.Admin, true);

        // Assert
        act.Should().Throw<CpfNullOrEmptyException>();
    }

    [Fact]
    public void Constructor_WithBirthDayMinValue_ShouldThrowBirthDayMinValueException()
    {
        // Act
        Action act = () => new EmployeeEntity("12345678909", "Name", "Surname", "test@test.com", DateTime.MinValue, "pass", EmployeeRole.Admin, true);

        // Assert
        act.Should().Throw<BirthDayMinValueException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_WithEmptyPassword_ShouldThrowPasswordNullOrEmptyException(string invalidPassword)
    {
        // Act
        Action act = () => new EmployeeEntity("12345678909", "Name", "Surname", "test@test.com", DateTime.Now, invalidPassword, EmployeeRole.Admin, true);

        // Assert
        act.Should().Throw<PasswordNullOrEmptyException>();
    }

    [Fact]
    public void SetPassword_ShouldUpdatePasswordProperty()
    {
        // Arrange
        var employee = new EmployeeEntity("12345678909", "Name", "Surname", "test@test.com", DateTime.Now, "oldPass", EmployeeRole.Admin, true);
        var newPassword = "newSecurePassword";

        // Act
        employee.SetPassword(newPassword);

        // Assert
        employee.Password.Should().Be(newPassword);
    }

    [Fact]
    public void UpdateEmployee_WithValidData_ShouldUpdateAllProperties()
    {
        // Arrange
        var employee = new EmployeeEntity("12345678909", "Old", "Name", "old@test.com", DateTime.Now, "pass", EmployeeRole.Admin, true);
        var updatedName = "New Name";
        var updatedRole = EmployeeRole.Manager;

        // Act
        employee.UpdateEmployee("12345678909", updatedName, "Surname", "new@test.com", DateTime.Now, "newPass", updatedRole, false);

        // Assert
        employee.Name.Should().Be(updatedName);
        employee.Role.Should().Be(updatedRole);
        employee.IsActive.Should().BeFalse();
    }
}