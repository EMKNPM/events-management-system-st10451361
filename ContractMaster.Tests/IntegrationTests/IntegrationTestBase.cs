using Xunit;
using Microsoft.EntityFrameworkCore;
using ContractMaster.API.Data;
using ContractMaster.API.Models;

namespace ContractMaster.Tests.IntegrationTests;

public class IntegrationTestBase : IDisposable
{
    protected readonly AppDbContext _context;

    public IntegrationTestBase()
    {
        // Create in-memory database options
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        // Seed test data
        _context.Database.EnsureCreated();
        SeedData();
    }

    private void SeedData()
    {
        if (!_context.Clients.Any())
        {
            _context.Clients.Add(new Client
            {
                Name = "Test Client",
                ContactDetails = "test@example.com",
                Region = "Test Region"
            });
            _context.SaveChanges();
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}