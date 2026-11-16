using BudgetCalculator.Api.Models;
using BudgetCalculator.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCalculator.Api.Controllers;

[ApiController]
[Route("api/budget")]
public class BudgetController : ControllerBase
{
    private readonly IInsuranceService _insurance;

    public BudgetController(IInsuranceService insurance)
    {
        _insurance = insurance;
    }

    [HttpPost("calculate")]
    public IActionResult Calculate([FromBody] BudgetRequest request)
    {
        if (request.Days <= 0) return BadRequest("Количество дней должно быть больше нуля.");

        var total = request.DailyCost * request.Days;

        // было total *= 5 (500%), теперь страховку считает отдельный сервис (+5%)
        if (request.IncludeInsurance)
        {
            total = _insurance.AddInsurance(total);
        }

        return Ok(new { TotalAmount = total });
    }
}
