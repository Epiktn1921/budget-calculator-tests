namespace BudgetCalculator.Api.Services;

public interface IInsuranceService
{
    // возвращает сумму вместе со страховкой
    decimal AddInsurance(decimal amount);
}
