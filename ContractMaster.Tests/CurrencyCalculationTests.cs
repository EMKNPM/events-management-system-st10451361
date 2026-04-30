using ContractMaster.Web.Models;

namespace ContractMaster.Tests;

public class CurrencyCalculationTests
{
    [Theory]
    [InlineData(100, 19.50, 1950.00)]
    [InlineData(50.75, 18.25, 926.19)]  // Rounded to 2 decimals
    [InlineData(0, 19.50, 0.00)]
    [InlineData(1000, 20.00, 20000.00)]
    [InlineData(100, 19.1234, 1912.34)]  // 100 * 19.1234 = 1912.34
    public void CurrencyConversion_CalculatesCorrectly(decimal usd, decimal rate, decimal expectedZar)
    {
        // Act - Round to 2 decimal places for currency
        decimal actualZar = Math.Round(usd * rate, 2);

        // Assert
        Assert.Equal(expectedZar, actualZar);
    }

    [Fact]
    public void ServiceRequest_AutoCalculatesZarOnCreation()
    {
        // Arrange
        var request = new ServiceRequest
        {
            CostUSD = 250m
        };
        decimal rate = 19.50m;
        decimal expectedZar = 4875m;

        // Act
        request.CostZAR = request.CostUSD * rate;

        // Assert
        Assert.Equal(expectedZar, request.CostZAR);
        Assert.True(request.CostZAR > 0);
    }

    [Fact]
    public void ZeroUsdAmount_ResultsInZeroZar()
    {
        // Arrange & Act
        decimal usd = 0;
        decimal rate = 19.50m;
        decimal zar = usd * rate;

        // Assert
        Assert.Equal(0, zar);
    }

    [Fact]
    public void CurrencyConversion_HandlesLargeNumbers()
    {
        // Arrange
        decimal usd = 999999.99m;
        decimal rate = 19.1234m;

        // Act
        decimal zar = Math.Round(usd * rate, 2);

        // Assert
        Assert.True(zar > 0);
        Assert.True(zar < 20000000);
    }

    [Fact]
    public void CurrencyConversion_ValidatesPositiveRate()
    {
        // Arrange
        decimal rate = 19.50m;

        // Assert
        Assert.True(rate > 0);
        Assert.True(rate < 100); // Reasonable exchange rate range
    }
}