using Application.Employee;
using Domain.Employee.Dtos;
using Domain.Employee.Exceptions;
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
    private readonly EmployeeManager _manager;

    public EmployeeManagerTests()
    {
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _passwordManagerMock = new Mock<IPasswordManager>();
        _manager = new EmployeeManager(_employeeRepositoryMock.Object, _passwordManagerMock.Object);
    }

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var requestDto = new EmployeeRequestDto
        {
            Name = "John",
            Surname = "Doe",
            Cpf = "59605667029",
            Email = "john@test.com",
            Password = "plainPassword",
            BirthDay = DateTimeOffset.Now.AddYears(-20),
            Role = EmployeeRole.Admin,
        };

        // Configurar o Mock do PasswordManager para lidar com o parâmetro 'out'
        string hashedPassword = "hashed_secret_password";
        _passwordManagerMock
            .Setup(x => x.CreatePasswordHash(requestDto.Password, out hashedPassword));

        // Configurar o Repo para retornar uma entidade criada
        // Nota: Como não posso instanciar facilmente o retorno de ToEntity sem saber a estrutura interna,
        // configuro o mock para aceitar qualquer Employee e retornar um Employee válido.
        var createdEntity = new EmployeeEntity(requestDto.Cpf, requestDto.Name, requestDto.Surname, requestDto.Email, requestDto.BirthDay.UtcDateTime, requestDto.Password, requestDto.Role, true);

        _employeeRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<EmployeeEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdEntity);

        // Act
        var result = await _manager.CreateAsync(requestDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Error.Should().BeFalse();
        result.Name.Should().Be(requestDto.Name);

        // Verifica se o hash de senha foi gerado
        _passwordManagerMock.Verify(x => x.CreatePasswordHash(requestDto.Password, out hashedPassword), Times.Once);
        // Verifica se salvou no banco
        _employeeRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<EmployeeEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnError_WhenDomainExceptionIsThrown()
    {
        // Este teste é crucial para cobrir o bloco catch (Exception ex) when (...)

        // Arrange
        var requestDto = new EmployeeRequestDto { Name = "Invalid", Cpf = "59605667029", Surname = "Invalid", Email = "email@email.com", BirthDay = DateTime.Now.AddYears(-20), Password = "123", Role = EmployeeRole.Admin }; // Dados irrelevantes pois vamos forçar o erro

        // Simulamos que o repositório (ou a lógica interna da entidade antes de chegar no repo) lançou uma exceção de domínio
        _employeeRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<EmployeeEntity>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NameNullOrEmptyException());

        // Precisamos garantir que o PasswordManager não quebre o teste antes de chegar no repo
        string hash = "hash";
        _passwordManagerMock.Setup(x => x.CreatePasswordHash(It.IsAny<string>(), out hash));

        // Act
        var result = await _manager.CreateAsync(requestDto, CancellationToken.None);

        // Assert
        result.Error.Should().BeTrue();
        result.ErrorMessage.Should().Contain("Name was null or empty.");
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ShouldReturnSuccess_WhenEmployeeExists()
    {
        // Arrange
        var updateDto = new UpdateEmployeeDto
        {
            Id = 1,
            Name = "New Name",
            Surname = "New Surname",
            Email = "email@email.com",
            Cpf = "59605667029",
            Password = "newPass",
            Role = EmployeeRole.Admin,
            BirthDate = DateTime.Now
        };

        var existingEmployee = new EmployeeEntity("59605667029", "Old", "Name", "email@email.com", DateTime.Now, "123", EmployeeRole.Admin, true);

        // Mock: Encontra o funcionário
        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(updateDto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEmployee);

        // Mock: Atualiza e retorna o funcionário atualizado
        _employeeRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<EmployeeEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmployeeEntity e, CancellationToken t) => e);

        // Act
        var result = await _manager.UpdateAsync(updateDto, CancellationToken.None);

        // Assert
        result.Error.Should().BeFalse();
        result.Name.Should().Be(updateDto.Name);

        _employeeRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<EmployeeEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnError_WhenEmployeeNotFound()
    {
        // Arrange
        var updateDto = new UpdateEmployeeDto { Id = 99 };

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(updateDto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmployeeEntity?)null);

        // Act
        var result = await _manager.UpdateAsync(updateDto, CancellationToken.None);

        // Assert
        result.Error.Should().BeTrue();
        result.ErrorMessage.Should().Be("Employee not found.");

        _employeeRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<EmployeeEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnError_WhenDomainExceptionIsThrown()
    {
        // Arrange
        var updateDto = new UpdateEmployeeDto { Id = 1, Name = "" }; // Nome vazio deve disparar erro
        var existingEmployee = new EmployeeEntity("59605667029", "Old", "Name", "email@email.com", DateTime.Now, "123", EmployeeRole.Admin, true);

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(updateDto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEmployee);

        // Forçamos o erro no UpdateAsync do repositório para simular falha de validação da Entidade
        _employeeRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<EmployeeEntity>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CpfNullOrEmptyException());

        // Act
        var result = await _manager.UpdateAsync(updateDto, CancellationToken.None);

        // Assert
        result.Error.Should().BeTrue();
        result.ErrorMessage.Should().Contain("CPF was null or empty.");
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_ShouldReturnAffectedRows_WhenEmployeeExists()
    {
        // Arrange
        int id = 1;
        var existingEmployee = new EmployeeEntity("59605667029", "Name", "Surname", "email@email.com", DateTime.Now, "123", EmployeeRole.Admin, true);

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEmployee);

        _employeeRepositoryMock
            .Setup(x => x.DeleteAsync(existingEmployee, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _manager.DeleteAsync(id, CancellationToken.None);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnZero_WhenEmployeeDoesNotExist()
    {
        // Arrange
        int id = 99;
        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmployeeEntity?)null);

        // Act
        var result = await _manager.DeleteAsync(id, CancellationToken.None);

        // Assert
        result.Should().Be(0);
        _employeeRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<EmployeeEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDto_WhenFound()
    {
        // Arrange
        int id = 1;
        var employee = new EmployeeEntity("59605667029", "Name", "Surname", "email@email.com", DateTime.Now, "123", EmployeeRole.Admin, true);

        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        // Act
        var result = await _manager.GetByIdAsync(id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Name");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        int id = 99;
        _employeeRepositoryMock
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmployeeEntity?)null);

        // Act
        var result = await _manager.GetByIdAsync(id, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedList()
    {
        // Arrange
        var employees = new List<EmployeeEntity>
            {
                new EmployeeEntity("59605667029", "User1", "Sur1", "mail1@email.com", DateTime.Now, "123", EmployeeRole.Admin, true),
                new EmployeeEntity("76966417009", "User2", "Sur2", "mail2@email.com", DateTime.Now, "123", EmployeeRole.Admin, true)
            };

        _employeeRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>(), 0, 10))
            .ReturnsAsync(employees);

        // Act
        var result = await _manager.GetAllAsync(CancellationToken.None, 0, 10);

        // Assert
        result.Should().HaveCount(2);
        result[0].Cpf.Should().Be("59605667029");
        result[1].Cpf.Should().Be("76966417009");
    }

    #endregion
}