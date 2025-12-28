using Application.Customer;
using Domain.Customer.Dtos;
using FluentAssertions;
using Infra.Database.SqlServer;
using Infra.Database.SqlServer.Customer.Repositories;
using Microsoft.EntityFrameworkCore;
using Reqnroll;
using Tests.BDD;

[Binding]
public class CustomerStepDefinitions : IClassFixture<IntegrationTestFixture>
{
    private static CustomerManager _customerManager;
    private static AppDbContext _context;

    private CustomerRequestDto _requestDto;
    private CustomerResponseDto _responseDto;
    private static IntegrationTestFixture _fixture;

    [BeforeTestRun]
    public static async Task BeforeTestRun()
    {
        _fixture = new IntegrationTestFixture();
        await _fixture.InitializeAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_fixture.ConnectionString)
            .Options;

        _context = new AppDbContext(options);
        var repository = new CustomerRepository(_context);
        _customerManager = new CustomerManager(repository);
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        await _fixture.DisposeAsync();
    }

    [Given(@"que o sistema de persistência está operacional")]
    [Given(@"the persistence system is operational")]
    public void GivenSistemaOperacional()
    {
        _context.Database.CanConnect().Should().BeTrue();
    }

    [Given(@"que eu preenchi os dados de um novo cliente com CPF ""(.*)"" e nome ""(.*)""")]
    [Given(@"I filled the data of a new customer with CPF ""(.*)"" and name ""(.*)""")]
    public void GivenDadosNovoCliente(string cpf, string nome)
    {
        _requestDto = new CustomerRequestDto
        {
            Cpf = cpf,
            Name = nome,
            Surname = "Teste",
            Email = "teste@email.com",
            BirthDate = DateTime.Now.AddYears(-20)
        };
    }

    [When(@"eu confirmar o cadastro")]
    [When(@"I confirm the registration")]
    public async Task WhenConfirmarCadastro()
    {
        _responseDto = await _customerManager.CreateAsync(_requestDto, CancellationToken.None);
    }

    [Then(@"o cliente deve ser salvo no banco de dados")]
    [Then(@"the customer should be saved in the database")]
    public async Task ThenSalvoNoBanco()
    {
        var customerInDb = await _context.Customers.AnyAsync(c => c.Cpf == _requestDto.Cpf);
        customerInDb.Should().BeTrue();
    }

    [Then(@"o sistema deve retornar um DTO com o nome ""(.*)"" e um ID válido")]
    [Then(@"the system should return a DTO with the name ""(.*)"" and a valid ID")]
    public void ThenRetornoValido(string nome)
    {
        _responseDto.Name.Should().Be(nome);
        _responseDto.Id.Should().BeGreaterThan(0);
    }

    [Given(@"que não existe um cliente cadastrado com o ID (.*)")]
    [Given(@"there is no customer registered with ID (.*)")]
    public async Task GivenClienteInexistente(int id)
    {
        var exists = await _context.Customers.AnyAsync(c => c.Id == id);
        exists.Should().BeFalse();
    }

    [When(@"eu tentar atualizar o cliente (.*) para o nome ""(.*)""")]
    [When(@"I try to update customer (.*) to name ""(.*)""")]
    public async Task WhenTentarAtualizar(int id, string novoNome)
    {
        var updateDto = new CustomerUpdateDto { Id = id, Name = novoNome };
        _responseDto = await _customerManager.UpdateAsync(updateDto, CancellationToken.None);
    }

    [Then(@"o sistema deve retornar um erro com a mensagem ""(.*)""")]
    [Then(@"the system should return an error with message ""(.*)""")]
    public void ThenErroMensagem(string msg)
    {
        _responseDto.Error.Should().BeTrue();
        _responseDto.ErrorMessage.Should().Be(msg);
    }
}