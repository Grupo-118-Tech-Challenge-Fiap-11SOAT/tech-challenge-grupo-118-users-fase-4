using Application.Customer;
using Domain.Customer.Dtos;
using Domain.Customer.Ports.Out;
using FluentAssertions;
using Moq;
using CustomerEntity = Domain.Customer.Entities.Customer;

namespace UnitTests.Application.Customer;

public class CustomerManagerTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly CustomerManager _manager;

    public CustomerManagerTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _manager = new CustomerManager(_customerRepositoryMock.Object);
    }

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_ShouldReturnResponseDto_WhenSuccessful()
    {
        // Arrange
        var requestDto = new CustomerRequestDto
        {
            Cpf = "15887241071",
            Name = "John",
            Surname = "Doe",
            Email = "john@test.com",
            BirthDate = DateTime.Now.AddYears(-20)
        };

        // Simulamos a entidade que seria criada e retornada pelo banco
        var createdEntity = new CustomerEntity(
            requestDto.Cpf, requestDto.Name, requestDto.Surname,
            requestDto.Email, requestDto.BirthDate, true);

        // É importante configurar o Mock para aceitar qualquer objeto Customer
        _customerRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<CustomerEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdEntity);

        // Act
        var result = await _manager.CreateAsync(requestDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Cpf.Should().Be(requestDto.Cpf); // Assumindo que o ToDto mapeia corretamente
        result.Name.Should().Be(requestDto.Name);

        // Verificamos se o método do repositório foi chamado exatamente 1 vez
        _customerRepositoryMock.Verify(x => x.CreateAsync(
            It.Is<CustomerEntity>(c => c.Cpf == requestDto.Cpf && c.Email == requestDto.Email),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDto_WhenCustomerExists()
    {
        // Arrange
        int id = 1;
        var entity = new CustomerEntity("15887241071", "John", "Doe", "email@email.com", DateTime.Now, true);
        // Se a entidade tiver Setter privado para ID, talvez precise de reflection ou construtor, 
        // mas aqui assumimos que o objeto retornado pelo mock é suficiente.

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _manager.GetByIdAsync(id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("John");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
    {
        // Arrange
        int id = 99;
        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerEntity?)null);

        // Act
        var result = await _manager.GetByIdAsync(id, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetByCpfAsync

    [Fact]
    public async Task GetByCpfAsync_ShouldReturnDto_WhenCustomerExists()
    {
        // Arrange
        string cpf = "15887241071";
        var entity = new CustomerEntity(cpf, "Maria", "Silva", "maria@test.com", DateTime.Now, true);

        _customerRepositoryMock
            .Setup(x => x.GetByCpfAsync(cpf, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _manager.GetByCpfAsync(cpf, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Cpf.Should().Be(cpf);
    }

    [Fact]
    public async Task GetByCpfAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
    {
        // Arrange
        string cpf = "15887241071";
        _customerRepositoryMock
            .Setup(x => x.GetByCpfAsync(cpf, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerEntity?)null);

        // Act
        var result = await _manager.GetByCpfAsync(cpf, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ShouldReturnError_WhenCustomerNotFound()
    {
        // Arrange
        var updateDto = new CustomerUpdateDto { Id = 99 };

        // Mock retorna null ao buscar pelo ID
        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(updateDto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerEntity?)null);

        // Act
        var result = await _manager.UpdateAsync(updateDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Error.Should().BeTrue();
        result.ErrorMessage.Should().Be("Customer not found.");

        // Garante que o método Update do repositório NUNCA foi chamado
        _customerRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CustomerEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAndReturnDto_WhenCustomerExists()
    {
        // Arrange
        var updateDto = new CustomerUpdateDto
        {
            Id = 1,
            Cpf = "15887241071",
            Name = "Updated Name",
            Surname = "Updated Surname",
            Email = "updated@email.com",
            BirthDate = DateTime.Now,
            IsActive = true
        };

        // Cliente existente no banco (dados antigos)
        var existingCustomer = new CustomerEntity("15887241071", "Old Name", "Old Surname", "old@email.com", DateTime.Now, true);

        // 1. Mock do GetById retorna o cliente existente
        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(updateDto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCustomer);

        // 2. Mock do Update retorna o cliente já modificado
        // Note: O método UpdateCustomer na entidade void altera o objeto existingCustomer em memória.
        _customerRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<CustomerEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerEntity c, CancellationToken t) => c); // Retorna o próprio objeto passado

        // Act
        var result = await _manager.UpdateAsync(updateDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Error.Should().BeFalse(); // Assume default false
        result.Name.Should().Be(updateDto.Name);
        result.Email.Should().Be(updateDto.Email);

        // Verificação crucial: O método Update do repositório foi chamado com os dados NOVOS?
        _customerRepositoryMock.Verify(x => x.UpdateAsync(
            It.Is<CustomerEntity>(c => c.Name == updateDto.Name && c.Cpf == updateDto.Cpf),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}