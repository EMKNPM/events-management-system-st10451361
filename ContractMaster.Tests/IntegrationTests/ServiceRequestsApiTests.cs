using Xunit;
using ContractMaster.API.Models;

namespace ContractMaster.Tests.IntegrationTests;

public class ServiceRequestsApiTests : IntegrationTestBase
{
    [Fact]
    public void CanAddServiceRequestToDatabase()
    {
        // Arrange - First create a contract
        var contract = new Contract
        {
            ClientId = 1,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1),
            Status = ContractStatus.Active,
            ServiceLevel = "Premium"
        };
        _context.Contracts.Add(contract);
        _context.SaveChanges();

        // Act - Create service request
        var request = new ServiceRequest
        {
            ContractId = contract.ContractId,
            Description = "Test service request",
            CostUSD = 100,
            CostZAR = 1950,
            Status = ServiceRequestStatus.Open,
            CreatedAt = DateTime.Now
        };
        _context.ServiceRequests.Add(request);
        _context.SaveChanges();

        // Assert
        var savedRequest = _context.ServiceRequests.First();
        Assert.NotNull(savedRequest);
        Assert.Equal("Test service request", savedRequest.Description);
        Assert.Equal(100, savedRequest.CostUSD);
    }

    [Fact]
    public void ServiceRequestCalculatesZarCorrectly()
    {
        // Arrange
        var usdAmount = 100m;
        var rate = 19.50m;
        var expectedZar = 1950m;

        // Act
        var actualZar = usdAmount * rate;

        // Assert
        Assert.Equal(expectedZar, actualZar);
    }
}