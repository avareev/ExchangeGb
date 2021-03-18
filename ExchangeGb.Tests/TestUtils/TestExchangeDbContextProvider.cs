using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;
using ExchangeGb.Data;
using Microsoft.EntityFrameworkCore;

namespace ExchangeGb.Tests.TestUtils
{
    public class TestExchangeDbContextProvider : IAsyncDisposable
    {
        private readonly PostgreSqlTestcontainer _testContainer;

        private TestExchangeDbContextProvider(PostgreSqlTestcontainer testContainer)
        {
            _testContainer = testContainer;
        }

        public static async Task<TestExchangeDbContextProvider> GetInstanceAsync()
        {
            var testContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                .WithDatabase(new PostgreSqlTestcontainerConfiguration
                {
                    Database = "db",
                    Username = "postgres",
                    Password = "postgres",
                }).Build();
            await testContainer.StartAsync();
            return new TestExchangeDbContextProvider(testContainer);
        }

        public ExchangeDbContext GetDbContext()
        {
            var dbContext = new ExchangeDbContext(new DbContextOptionsBuilder<ExchangeDbContext>()
                .UseNpgsql(_testContainer.ConnectionString)
                .Options);
            dbContext.Database.Migrate();
            return dbContext;
        }

        public ValueTask DisposeAsync()
        {
            return _testContainer.DisposeAsync();
        }
    }
}