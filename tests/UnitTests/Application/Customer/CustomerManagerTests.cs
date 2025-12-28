using Application.Customer;
using Domain.Customer.Dtos;
using Domain.Customer.Entities;
using Domain.Customer.Ports.Out;
using FluentAssertions;
using Moq;
using Xunit;
using CustomerEntity = Domain.Customer.Entities.Customer;

namespace UnitTests.Application.Customer;

public class CustomerManagerTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly CustomerManager _customerManager;

    public CustomerManagerTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _customerManager = new CustomerManager(_customerRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnCustomerResponseDto()
    {
        // Arrange
        var request = new CustomerRequestDto
        {
            Cpf = "12345678909",
            Name = "Fulano",
            Surname = "Detal",
            Email = "teste@teste.com",
            BirthDate = new DateTime(1990, 1, 1)
        };

        _customerRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<CustomerEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerEntity c, CancellationToken ct) => c);

        // Act
        var result = await _customerManager.CreateAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        _customerRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<CustomerEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerEntity)null!);

        // Act
        var result = await _customerManager.GetByIdAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerNotFound_ShouldReturnErrorMessage()
    {
        // Arrange
        var updateDto = new CustomerUpdateDto { Id = 99 };
        _customerRepositoryMock
            .Setup(repo => repo.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerEntity)null!);

        // Act
        var result = await _customerManager.UpdateAsync(updateDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Error.Should().BeTrue();
        result.ErrorMessage.Should().Be("Customer not found.");
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerExists_ShouldCallUpdateAndReturnDto()
    {
        // Arrange
        var existingCustomer = new CustomerEntity("12345678909", "Velho", "Nome", "velho@email.com", DateTime.Now, true, id: 1);
        var updateDto = new CustomerUpdateDto
        {
            Id = 1,
            Cpf = "12345678909",
            Name = "Novo",
            Surname = "Nome",
            Email = "novo@email.com",
            IsActive = true
        };

        _customerRepositoryMock.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingCustomer);
        _customerRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<CustomerEntity>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingCustomer);

        // Act
        var result = await _customerManager.UpdateAsync(updateDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Novo");
        _customerRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<CustomerEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}