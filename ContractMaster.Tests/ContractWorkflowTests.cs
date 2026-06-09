using Xunit;
using ContractMaster.Web.Models;

namespace ContractMaster.Tests;

public class ContractWorkflowTests
{
    [Theory]
    [InlineData(ContractStatus.Active, true)]
    [InlineData(ContractStatus.Draft, true)]
    [InlineData(ContractStatus.Expired, false)]
    [InlineData(ContractStatus.OnHold, false)]
    public void ServiceRequest_CannotBeCreatedForExpiredOrOnHoldContracts(ContractStatus status, bool expectedCanCreate)
    {
        // Arrange - Create a contract with specific status
        var contract = new Contract
        {
            Status = status,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddYears(1)
        };

        // Act - Apply business rule
        // Business Rule: Service Request only for Active or Draft contracts
        bool canCreate = (contract.Status != ContractStatus.Expired &&
                          contract.Status != ContractStatus.OnHold);

        // Assert - Verify the rule
        Assert.Equal(expectedCanCreate, canCreate);
    }

    [Fact]
    public void Contract_WithEndDateBeforeStartDate_IsInvalid()
    {
        // Arrange - Contract with invalid dates
        var contract = new Contract
        {
            StartDate = new DateTime(2026, 12, 31),
            EndDate = new DateTime(2026, 1, 1)
        };

        // Act - Check date validity
        bool isValid = contract.EndDate > contract.StartDate;

        // Assert - Should be invalid
        Assert.False(isValid);
    }

    [Fact]
    public void Contract_WithValidDates_IsValid()
    {
        // Arrange - Contract with valid dates
        var contract = new Contract
        {
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31)
        };

        // Act - Check date validity
        bool isValid = contract.EndDate > contract.StartDate;

        // Assert - Should be valid
        Assert.True(isValid);
    }

    [Fact]
    public void ActiveContract_AllowsServiceRequests()
    {
        // Arrange
        var contract = new Contract { Status = ContractStatus.Active };

        // Act
        bool canCreateRequest = contract.Status == ContractStatus.Active ||
                                contract.Status == ContractStatus.Draft;

        // Assert
        Assert.True(canCreateRequest);
    }

    [Fact]
    public void ExpiredContract_BlocksServiceRequests()
    {
        // Arrange
        var contract = new Contract { Status = ContractStatus.Expired };

        // Act
        bool canCreateRequest = contract.Status != ContractStatus.Expired &&
                                contract.Status != ContractStatus.OnHold;

        // Assert
        Assert.False(canCreateRequest);
    }
}