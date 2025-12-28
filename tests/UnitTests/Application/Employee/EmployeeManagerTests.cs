using Application.Employee;
using Domain.Employee.Dtos;
using Domain.Employee.Ports.Out;
using Domain.Employee.ValueObjects;
using FluentAssertions;
using Moq;
using EmployeeEntity = Domain.Employee.Entities.Employee;

namespace UnitTests.Application.Employee;

public class EmployeeManagerTests
{
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IPasswordManager> _passwordManagerMock;
    private readonly EmployeeManager _employeeManager;

    public EmployeeManagerTests()
    {
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _passwordManagerMock = new Mock<IPasswordManager>();
        _employeeManager = new EmployeeManager(_employeeRepositoryMock.Object, _passwordManagerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldHashPasswordAndReturnDto()
    {
        // Arrange
        var request = new EmployeeRequestDto
        {
            Cpf = "12345678909",
            Name = "Jose",
            Surname = "Silva",
            Email = "jose@email.com",
            BirthDay = new DateTime(1980, 1, 1),
            Password = "plainPassword",
            Role = EmployeeRole.Admin
        };

        string outHash = "hashedPassword";
        _passwordManagerMock
            .Setup(m => m.CreatePasswordHash(It.IsAny<string>(), out outHash));

        _employeeRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<EmployeeEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmployeeEntity e, CancellationToken ct) => e);

        // Act
        var result = await _employeeManager.CreateAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Error.Should().BeFalse();
        _passwordManagerMock.Verify(m => m.CreatePasswordHash("plainPassword", out It.Ref<string>.IsAny), Times.Once);
        _employeeRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<EmployeeEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenDomainExceptionOccurs_ShouldReturnErrorDto()
    {
        // Arrange
        var request = new EmployeeRequestDto { Cpf = "" }; // Cpf vazio deve disparar exceção no ToEntity

        // Act
        var result = await _employeeManager.CreateAsync(request, CancellationToken.None);

        // Assert
        result.Error.Should().BeTrue();
        result.ErrorMessage.Should().Contain("Message:");
    }

    [Fact]
    public async Task DeleteAsync_WhenEmployeeExists_ShouldReturnAffectedRows()
    {
        // Arrange
        var employee = new EmployeeEntity("12345678909", "Nome", "Sobre", "e@e.com", DateTime.Now, "hash", EmployeeRole.Admin, true, 1);
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        _employeeRepositoryMock.Setup(r => r.DeleteAsync(employee, It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _employeeManager.DeleteAsync(1, CancellationToken.None);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task DeleteAsync_WhenEmployeeNotFound_ShouldReturnZero()
    {
        // Arrange
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((EmployeeEntity)null!);

        // Act
        var result = await _employeeManager.DeleteAsync(999, CancellationToken.None);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnListOfDtos()
    {
        // Arrange
        var list = new List<EmployeeEntity> {
            new EmployeeEntity("12345678909", "E1", "S1", "e1@e.com", DateTime.Now, "p1", EmployeeRole.Admin, true, 1),
            new EmployeeEntity("98765432100", "E2", "S2", "e2@e.com", DateTime.Now, "p2", EmployeeRole.Admin, true, 2)
        };
        _employeeRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(list);

        // Act
        var result = await _employeeManager.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("E1");
        result[1].Cpf.Should().Be("98765432100");
    }

    [Fact]
    public async Task UpdateAsync_WhenEmployeeNotFound_ShouldReturnErrorMessage()
    {
        // Arrange
        var updateDto = new UpdateEmployeeDto { Id = 1 };
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((EmployeeEntity)null!);

        // Act
        var result = await _employeeManager.UpdateAsync(updateDto, CancellationToken.None);

        // Assert
        result.Error.Should().BeTrue();
        result.ErrorMessage.Should().Be("Employee not found.");
    }
}