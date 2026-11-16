using BudgetCalculator.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IInsuranceService, InsuranceService>();

var app = builder.Build();

app.MapControllers();

app.Run();
