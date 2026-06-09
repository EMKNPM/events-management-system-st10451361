using Xunit;
using ContractMaster.API.Models;

namespace ContractMaster.Tests.IntegrationTests;

public class ContractsApiTests : IntegrationTestBase
{
    [Fact]
    public void CanAddContractToDatabase()
    {
        // Arrange
        var contract = new Contract
        {
            ClientId = 1,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1),
            Status = ContractStatus.Active,
            ServiceLevel = "Premium"
        };

        // Act
        _context.Contracts.Add(contract);
        _context.SaveChanges();

        // Assert
        var savedContract = _context.Contracts.First();
        Assert.NotNull(savedContract);
        Assert.Equal("Premium", savedContract.ServiceLevel);
        Assert.Equal(ContractStatus.Active, savedContract.Status);
    }

    [Fact]
    public void CanUpdateContractStatus()
    {
        // Arrange
        var contract = new Contract
        {
            ClientId = 1,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1),
            Status = ContractStatus.Draft,
            ServiceLevel = "Basic"
        };
        _context.Contracts.Add(contract);
        _context.SaveChanges();

        // Act
        contract.Status = ContractStatus.Active;
        _context.SaveChanges();

        // Assert
        var updatedContract = _context.Contracts.First();
        Assert.Equal(ContractStatus.Active, updatedContract.Status);
    }
}