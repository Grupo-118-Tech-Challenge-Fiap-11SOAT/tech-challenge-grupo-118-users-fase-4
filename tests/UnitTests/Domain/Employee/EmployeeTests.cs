using Domain.Employee.Dtos;
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

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldInitializeProperties_Correctly()
    {
        // Arrange
        string cpf = "33665360048";
        string name = "John";
        string surname = "Doe";
        string email = "john.doe@example.com";
        string password = "strongPassword";
        // Simulando um cargo (ajuste conforme seu Enum ou classe)
        var role = EmployeeRole.Admin;

        // Criamos uma data com Offset para testar se a propriedade aceita
        var birthDay = new DateTimeOffset(1990, 1, 1, 12, 0, 0, TimeSpan.FromHours(-3));

        // Act
        var dto = new EmployeeRequestDto(cpf, name, surname, email, birthDay, password, role);

        // Assert
        dto.Cpf.Should().Be(cpf);
        dto.Name.Should().Be(name);
        dto.Surname.Should().Be(surname);
        dto.Email.Should().Be(email);
        dto.BirthDay.Should().Be(birthDay);
        dto.Password.Should().Be(password);
        dto.Role.Should().Be(role);
    }

    [Fact]
    public void DefaultConstructor_ShouldCreateInstance_WithDefaultValues()
    {
        // Arrange & Act
        var dto = new EmployeeRequestDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Cpf.Should().BeNull();
        dto.BirthDay.Should().Be(default(DateTimeOffset));
    }

    #endregion

    #region ToEntity Tests

    [Fact]
    public void ToEntity_ShouldMapDtoToEntity_AndConvertDateTime_AndSetActive()
    {
        // Arrange
        var dto = new EmployeeRequestDto
        {
            Cpf = "33665360048",
            Name = "Maria",
            Surname = "Silva",
            Email = "maria@test.com",
            Password = "123",
            Role = EmployeeRole.Admin,
            BirthDay = new DateTimeOffset(2000, 5, 20, 10, 0, 0, TimeSpan.FromHours(-3))
        };

        // Act
        var entity = EmployeeRequestDto.ToEntity(dto);

        // Assert
        entity.Should().NotBeNull();
        entity.Should().BeOfType<EmployeeEntity>();

        entity.Cpf.Should().Be(dto.Cpf);
        entity.Name.Should().Be(dto.Name);
        entity.Surname.Should().Be(dto.Surname);
        entity.Email.Should().Be(dto.Email);
        entity.Password.Should().Be(dto.Password);
        entity.Role.Should().Be(dto.Role);

        // Validação Importante 1: O método ToEntity faz .DateTime, removendo o Offset.
        // Verificamos se a data armazenada na entidade é a data base do DTO.
        entity.BirthDay.Should().Be(dto.BirthDay.DateTime);

        // Validação Importante 2: O método ToEntity passa 'true' fixo no construtor.
        // Assumindo que a propriedade na entidade seja IsActive
        entity.IsActive.Should().BeTrue();
    }

    #endregion

    #region ToDto Tests

    [Fact]
    public void ToDto_ShouldMapEntityToDto_Correctly()
    {
        // Arrange
        // Criando uma entidade simulada (ajuste o construtor conforme sua Entidade real)
        var entity = new EmployeeEntity(
            "33665360048",
            "Carlos",
            "Oliveira",
            "carlos@test.com",
            new DateTime(1985, 8, 25), // Entidade usa DateTime simples
            "pass123",
            EmployeeRole.Manager,
            true
        );

        // Act
        var dto = EmployeeRequestDto.ToDto(entity);

        // Assert
        dto.Should().NotBeNull();
        dto.Cpf.Should().Be(entity.Cpf);
        dto.Name.Should().Be(entity.Name);
        dto.Surname.Should().Be(entity.Surname);
        dto.Email.Should().Be(entity.Email);
        dto.Password.Should().Be(entity.Password);
        dto.Role.Should().Be(entity.Role);

        // O DTO é DateTimeOffset. Ao converter de DateTime (Entity), 
        // ele assume o Offset local ou zero dependendo da configuração, 
        // mas o momento no tempo (.DateTime) deve ser igual.
        dto.BirthDay.DateTime.Should().Be(entity.BirthDay);
    }

    #endregion

    #region Property Setters

    [Fact]
    public void Properties_ShouldStoreValues_WhenSet()
    {
        // Teste para garantir cobertura dos Setters caso o construtor não seja usado
        // Arrange
        var dto = new EmployeeRequestDto();
        var date = DateTimeOffset.Now;

        // Act
        dto.Cpf = "33665360048";
        dto.Name = "Test";
        dto.Surname = "Unit";
        dto.Email = "test@mail.com";
        dto.BirthDay = date;
        dto.Password = "pass";
        dto.Role = EmployeeRole.Admin;

        // Assert
        dto.Cpf.Should().Be("33665360048");
        dto.Name.Should().Be("Test");
        dto.Surname.Should().Be("Unit");
        dto.Email.Should().Be("test@mail.com");
        dto.BirthDay.Should().Be(date);
        dto.Password.Should().Be("pass");
        dto.Role.Should().Be(EmployeeRole.Admin);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void DefaultConstructor_ShouldInitialize_WithDefaultValues()
    {
        // Arrange & Act
        var dto = new UpdateEmployeeDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(0);
        dto.Cpf.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithEntity_ShouldMapAllProperties()
    {
        // Arrange
        // Assumindo um construtor de entidade compatível para o teste
        var entity = new EmployeeEntity(
            "33665360048",
            "John",
            "Doe",
            "john@test.com",
            new DateTime(1990, 1, 1),
            "secretPass",
            EmployeeRole.Manager,
            true
        );

        // Act
        var dto = new UpdateEmployeeDto(entity);

        // Assert
        dto.Id.Should().Be(entity.Id);
        dto.Cpf.Should().Be(entity.Cpf);
        dto.Name.Should().Be(entity.Name);
        dto.Surname.Should().Be(entity.Surname);
        dto.Email.Should().Be(entity.Email);
        dto.BirthDate.Should().Be(entity.BirthDay);
        dto.Password.Should().Be(entity.Password);
        dto.Role.Should().Be(entity.Role);
        dto.IsActive.Should().Be(entity.IsActive);
    }

    #endregion

    #region ToDto Tests

    [Fact]
    public void ToDto_ShouldMapEntityToUpdateDto_Correctly()
    {
        // Arrange
        var entity = new EmployeeEntity(
            "98765432100",
            "Maria",
            "Silva",
            "maria@test.com",
            new DateTime(1985, 5, 20),
            "pass123",
            EmployeeRole.Admin,
            false
        );

        // Act
        var dto = UpdateEmployeeDto.ToDto(entity);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(entity.Id);
        dto.Cpf.Should().Be(entity.Cpf);
        dto.Name.Should().Be(entity.Name);
        dto.Surname.Should().Be(entity.Surname);
        dto.Email.Should().Be(entity.Email);
        dto.BirthDate.Should().Be(entity.BirthDay);
        dto.Password.Should().Be(entity.Password);
        dto.Role.Should().Be(entity.Role);
        dto.IsActive.Should().Be(entity.IsActive);
    }

    #endregion

    #region ToEntity Tests

    [Fact]
    public void ToEntity_ShouldMapDtoToEntity_WithId()
    {
        // Arrange
        var dto = new UpdateEmployeeDto
        {
            Id = 55,
            Cpf = "33665360048",
            Name = "Carlos",
            Surname = "Oliveira",
            Email = "carlos@test.com",
            BirthDate = new DateTime(2000, 1, 1),
            Password = "newPassword",
            Role = EmployeeRole.Admin,
            IsActive = true
        };

        // Act
        var entity = UpdateEmployeeDto.ToEntity(dto);

        // Assert
        entity.Should().NotBeNull();
        entity.Should().BeOfType<EmployeeEntity>();

        // Verifica se todas as propriedades foram passadas para o construtor da Entidade
        entity.Id.Should().Be(dto.Id);
        entity.Cpf.Should().Be(dto.Cpf);
        entity.Name.Should().Be(dto.Name);
        entity.Surname.Should().Be(dto.Surname);
        entity.Email.Should().Be(dto.Email);
        entity.BirthDay.Should().Be(dto.BirthDate);
        entity.Password.Should().Be(dto.Password);
        entity.Role.Should().Be(dto.Role);
        entity.IsActive.Should().Be(dto.IsActive);
    }

    #endregion
}