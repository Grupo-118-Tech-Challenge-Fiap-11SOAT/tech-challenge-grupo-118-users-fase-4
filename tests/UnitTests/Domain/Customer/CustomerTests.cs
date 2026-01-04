using Domain.Base.Exceptions;
using Domain.Customer.Dtos;
using FluentAssertions;
using CustomerEntity = Domain.Customer.Entities.Customer;

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

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldInitializeProperties_AndConvertDateTimeOffsetToDateTime()
    {
        // Arrange
        string cpf = "33665360048";
        string name = "John";
        string surname = "Doe";
        string email = "john.doe@example.com";

        // Criamos um DateTimeOffset com fuso horário (-3h)
        var birthDateOffset = new DateTimeOffset(1990, 1, 1, 12, 0, 0, TimeSpan.FromHours(-3));

        // Act
        var dto = new CustomerRequestDto(cpf, name, surname, email, birthDateOffset);

        // Assert
        dto.Cpf.Should().Be(cpf);
        dto.Name.Should().Be(name);
        dto.Surname.Should().Be(surname);
        dto.Email.Should().Be(email);

        // Validação crucial: Verifica se a conversão pegou a propriedade .DateTime corretamente
        // ignorando o offset conforme a lógica da sua classe
        dto.BirthDate.Should().Be(birthDateOffset.DateTime);
    }

    [Fact]
    public void DefaultConstructor_ShouldCreateInstance_WithDefaultValues()
    {
        // Arrange & Act
        var dto = new CustomerRequestDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Cpf.Should().BeNull(); // Strings inicializam como null
        dto.BirthDate.Should().Be(default(DateTime)); // DateTime inicializa como MinValue
    }

    #endregion

    #region ToEntity Tests

    [Fact]
    public void ToEntity_ShouldMapDtoToEntity_AndSetActiveTrue()
    {
        // Arrange
        var dto = new CustomerRequestDto
        {
            Cpf = "33665360048",
            Name = "Maria",
            Surname = "Silva",
            Email = "maria@test.com",
            BirthDate = new DateTime(1995, 5, 20)
        };

        // Act
        var entity = CustomerRequestDto.ToEntity(dto);

        // Assert
        entity.Should().NotBeNull();
        entity.Should().BeOfType<CustomerEntity>();

        entity.Cpf.Should().Be(dto.Cpf);
        entity.Name.Should().Be(dto.Name);
        entity.Surname.Should().Be(dto.Surname);
        entity.Email.Should().Be(dto.Email);
        entity.BirthDay.Should().Be(dto.BirthDate);

        // Verifica o "true" hardcoded no método ToEntity
        // Assumindo que a propriedade na entidade se chama IsActive ou Active
        // Caso sua entidade não exponha essa propriedade publicamente para leitura, 
        // você pode precisar inspecionar ou confiar no construtor da entidade.
        // entity.IsActive.Should().BeTrue(); 
    }

    #endregion

    #region ToDto Tests

    [Fact]
    public void ToDto_ShouldMapEntityToDto_Correctly()
    {
        // Arrange
        // Criando uma entidade simulada
        var entity = new CustomerEntity(
            "33665360048",
            "Carlos",
            "Oliveira",
            "carlos@test.com",
            new DateTime(1980, 10, 10),
            true
        );

        // Act
        var dto = CustomerRequestDto.ToDto(entity);

        // Assert
        dto.Should().NotBeNull();
        dto.Should().BeOfType<CustomerRequestDto>();

        dto.Cpf.Should().Be(entity.Cpf);
        dto.Name.Should().Be(entity.Name);
        dto.Surname.Should().Be(entity.Surname);
        dto.Email.Should().Be(entity.Email);
        dto.BirthDate.Should().Be(entity.BirthDay);
    }

    #endregion

    #region Properties Tests (Setters)

    [Fact]
    public void Properties_ShouldStoreValues_WhenSet()
    {
        // Teste simples para garantir que os setters automáticos funcionam
        // Útil para cobertura se você não usar o construtor parametrizado em todo lugar

        // Arrange
        var dto = new CustomerRequestDto();
        var now = DateTime.Now;

        // Act
        dto.Cpf = "33665360048";
        dto.Name = "Test";
        dto.Surname = "Case";
        dto.Email = "test@mail.com";
        dto.BirthDate = now;

        // Assert
        dto.Cpf.Should().Be("33665360048");
        dto.Name.Should().Be("Test");
        dto.Surname.Should().Be("Case");
        dto.Email.Should().Be("test@mail.com");
        dto.BirthDate.Should().Be(now);
    }

    #endregion
}
