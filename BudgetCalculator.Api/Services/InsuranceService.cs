namespace BudgetCalculator.Api.Services;

public class InsuranceService : IInsuranceService
{
    private const decimal Rate = 0.05m; // страховка 5%

    public decimal AddInsurance(decimal amount)
    {
        var result = amount + amount * Rate;
        return Math.Round(result, 2, MidpointRounding.AwayFromZero); // до копеек
    }
}
