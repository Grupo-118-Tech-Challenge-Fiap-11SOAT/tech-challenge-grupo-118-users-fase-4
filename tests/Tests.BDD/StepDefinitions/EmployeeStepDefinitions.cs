using Application.Employee;
using Domain.Employee.Dtos;
using FluentAssertions;
using Infra.Database.SqlServer;
using Infra.Database.SqlServer.Employee.Repositories;
using Infra.Password;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reqnroll;
using Tests.BDD;

[Binding]
public class EmployeeStepDefinitions : IClassFixture<IntegrationTestFixture>
{
    private static EmployeeManager? _employeeManager;
    private static PasswordManager? _passwordManager;
    private static AppDbContext? _context;
    private static IConfiguration? _configuration;

    private EmployeeRequestDto? _requestDto;
    private EmployeeResponseDto? _responseDto;
    private int _deleteResult;
    private static IntegrationTestFixture? _fixture;

    [BeforeTestRun]
    public static async Task BeforeTestRun()
    {
        _fixture = new IntegrationTestFixture();
        await _fixture.InitializeAsync();

        var myConfigurationSettings = new Dictionary<string, string>
        {
            {"Security:Key", "ea9083a270357e95a1bdb53188a66bb4"},
        };

        // Cria o IConfiguration diretamente da memória
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfigurationSettings)
            .Build();


        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_fixture.ConnectionString)
            .Options;

        _context = new AppDbContext(options);
        var repository = new EmployeeRepository(_context);
        _passwordManager = new PasswordManager(_configuration);
        _employeeManager = new EmployeeManager(repository, _passwordManager);
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        await _fixture.DisposeAsync();
    }

    [Given(@"I have the data of a new employee:")]
    public void GivenDadosFuncionario(Table table)
    {
        var row = table.Rows.ToDictionary(r => r[0], r => r[1]);
        _requestDto = new EmployeeRequestDto
        {
            Cpf = row["Cpf"],
            Name = row["Name"],
            Surname = row.ContainsKey("Surname") ? row["Surname"] : "Teste",
            Email = row.ContainsKey("Email") ? row["Email"] : "teste@teste.com",
            Password = row["Password"],
            BirthDay = DateTime.Now.AddYears(-25)
        };
    }

    [When(@"I request the employee registration")]
    public async Task WhenSolicitarCadastro()
    {
        _responseDto = await _employeeManager.CreateAsync(_requestDto, CancellationToken.None);
    }

    [Then(@"the employee should be persisted with the encrypted password")]
    public async Task ThenPersistidoComSenha()
    {
        var employeeInDb = await _context.Employees.FirstOrDefaultAsync(e => e.Cpf == _requestDto.Cpf);
        employeeInDb.Should().NotBeNull();
        employeeInDb.Password.Should().NotBeNullOrEmpty();
    }

    [Then(@"the system should return the data for ""(.*)"" with a generated ID")]
    public void ThenRetornoDados(string nome)
    {
        _responseDto.Name.Should().Be(nome);
        _responseDto.Id.Should().BeGreaterThan(0);
    }

    [Given(@"there is no employee with ID (.*)")]
    public async Task GivenInexistente(int id)
    {
        var emp = await _context.Employees.FindAsync(id);
        if (emp != null)
        {
            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();
        }
    }

    [When(@"I request deletion of employee (.*)")]
    public async Task WhenExcluir(int id)
    {
        _deleteResult = await _employeeManager.DeleteAsync(id, CancellationToken.None);
    }

    [Then(@"the system should report that (.*) records were affected")]
    public void ThenRegistrosAfetados(int result)
    {
        _deleteResult.Should().Be(result);
    }

    [Then(@"the system should return an error containing the message ""(.*)""")]
    public void ThenErroMensagem(string msgParte)
    {
        _responseDto.Error.Should().BeTrue();
        _responseDto.ErrorMessage.Should().Contain(msgParte);
    }
}