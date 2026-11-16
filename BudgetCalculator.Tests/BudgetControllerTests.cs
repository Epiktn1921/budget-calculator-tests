using BudgetCalculator.Api.Controllers;
using BudgetCalculator.Api.Models;
using BudgetCalculator.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BudgetCalculator.Tests;

public class BudgetControllerTests
{
    // вместо настоящего сервиса страховки подставляю мок,
    // чтобы тестировать только контроллер
    private readonly Mock<IInsuranceService> _insuranceMock = new();
    private readonly BudgetController _controller;

    public BudgetControllerTests()
    {
        _controller = new BudgetController(_insuranceMock.Object);
    }

    // достаю TotalAmount из ответа (там анонимный объект, поэтому через GetProperty)
    private decimal GetTotal(IActionResult result)
    {
        var ok = Assert.IsType<OkObjectResult>(result);
        var value = ok.Value!;
        return (decimal)value.GetType().GetProperty("TotalAmount")!.GetValue(value)!;
    }

    [Fact]
    public void Calculate_100x5_NoInsurance_Returns500()
    {
        var request = new BudgetRequest { DailyCost = 100, Days = 5, IncludeInsurance = false };

        var result = _controller.Calculate(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Equal(500m, GetTotal(result));
    }

    [Fact]
    public void Calculate_NoInsurance_DoesNotCallInsuranceService()
    {
        var request = new BudgetRequest { DailyCost = 100, Days = 5, IncludeInsurance = false };

        _controller.Calculate(request);

        _insuranceMock.Verify(s => s.AddInsurance(It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Calculate_WithInsurance_PassesSumToServiceAndReturnsItsResult()
    {
        _insuranceMock.Setup(s => s.AddInsurance(500m)).Returns(525m);
        var request = new BudgetRequest { DailyCost = 100, Days = 5, IncludeInsurance = true };

        var result = _controller.Calculate(request);

        Assert.Equal(525m, GetTotal(result));
        _insuranceMock.Verify(s => s.AddInsurance(500m), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Calculate_DaysZeroOrNegative_ReturnsBadRequest(int days)
    {
        var request = new BudgetRequest { DailyCost = 100, Days = days, IncludeInsurance = true };

        var result = _controller.Calculate(request);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, bad.StatusCode);
        Assert.Equal("Количество дней должно быть больше нуля.", bad.Value);
        // до страховки дело не дошло
        _insuranceMock.Verify(s => s.AddInsurance(It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void Calculate_OneDay_ReturnsDailyCost()
    {
        var request = new BudgetRequest { DailyCost = 250, Days = 1 };

        Assert.Equal(250m, GetTotal(_controller.Calculate(request)));
    }

    [Fact]
    public void Calculate_ZeroCost_ReturnsZero()
    {
        var request = new BudgetRequest { DailyCost = 0, Days = 10 };

        Assert.Equal(0m, GetTotal(_controller.Calculate(request)));
    }

    [Fact]
    public void Calculate_CostWithKopecks_CountsExactly()
    {
        var request = new BudgetRequest { DailyCost = 99.99m, Days = 3 };

        Assert.Equal(299.97m, GetTotal(_controller.Calculate(request)));
    }
}
