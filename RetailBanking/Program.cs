using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using RetailBanking.Interfaces;
using RetailBanking.Models;
using RetailBanking.Repository;
using RetailBanking.Repository.Interfaces;
using RetailBanking.Services;

var builder = WebApplication.CreateBuilder(args);

//var keyVaultUrl = new Uri("https://team2bankinkv.vault.azure.net/");
//builder.Configuration.AddAzureKeyVault(keyVaultUrl, new DefaultAzureCredential());

// Add services to the container.
builder.Services.AddDbContext<RetailBankingDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Retail Banking API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter JWT Token. Example: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddTransient<IcustomerService, CustomerService>();
builder.Services.AddTransient<ILoanService, LoanService>();
builder.Services.AddTransient<IKycService, KycService>();
builder.Services.AddTransient<IRiskScoreService, RiskScoreService>();
builder.Services.AddTransient<IEligibilityService, EligibilityService>();
builder.Services.AddSingleton<EventGridService>();

builder.Services.Configure<CreditRules>(builder.Configuration.GetSection("CreditRules"));

builder.Services.AddControllers();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

