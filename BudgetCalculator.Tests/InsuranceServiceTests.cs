using BudgetCalculator.Api.Services;

namespace BudgetCalculator.Tests;

// тут уже настоящий сервис, проверяю что страховка именно 5%
public class InsuranceServiceTests
{
    private readonly InsuranceService _service = new();

    [Theory]
    [InlineData(500, 525)]
    [InlineData(1000, 1050)]
    [InlineData(0, 0)]
    public void AddInsurance_Adds5Percent(decimal amount, decimal expected)
    {
        Assert.Equal(expected, _service.AddInsurance(amount));
    }

    [Fact]
    public void AddInsurance_RoundsToKopecks()
    {
        // 301.50 * 1.05 = 316.575 -> 316.58
        Assert.Equal(316.58m, _service.AddInsurance(301.50m));
    }
}
