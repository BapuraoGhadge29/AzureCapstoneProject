using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RetailBanking.Interfaces;
using RetailBanking.Models;
using RetailBanking.Repository;
using RetailBanking.Repository.Interfaces;
using RetailBanking.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<RetailBankingDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IcustomerService, CustomerService>();
builder.Services.AddTransient<ILoanService, LoanService>();
builder.Services.AddTransient<IKycService, KycService>();
builder.Services.AddTransient<IRiskScoreService, RiskScoreService>();
builder.Services.AddTransient<IEligibilityService, EligibilityService>();

builder.Services.Configure<CreditRules>(builder.Configuration.GetSection("CreditRules"));

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
