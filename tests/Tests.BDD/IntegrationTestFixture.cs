using Infra.Database.SqlServer;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Tests.BDD;

public class IntegrationTestFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        ConnectionString = _dbContainer.GetConnectionString();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
            })
            .Options;

        using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync() => await _dbContainer.DisposeAsync();
}