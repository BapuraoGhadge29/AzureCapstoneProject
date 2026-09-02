using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using RetailBanking.Hubs;
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy
                .AllowAnyOrigin()//we can provide ui url here
                //.WithOrigins("http://localhost:5173","https://retailbankingui-a0dtbkhfdjd7hbh6.southeastasia-01.azurewebsites.net")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); 
        });
});

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

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddAzureWebAppDiagnostics();
});

builder.Services.AddTransient<IcustomerService, CustomerService>();
builder.Services.AddTransient<ILoanService, LoanService>();
builder.Services.AddTransient<IKycService, KycService>();
builder.Services.AddTransient<IRiskScoreService, RiskScoreService>();
builder.Services.AddTransient<IEligibilityService, EligibilityService>();
builder.Services.AddSingleton<NotificationPublisher>();

builder.Services.Configure<CreditRules>(builder.Configuration.GetSection("CreditRules"));


builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


builder.Services.AddControllers();
builder.Services.AddSignalR()
    .AddAzureSignalR(options =>
    {
        options.ConnectionString = builder.Configuration["SignalRConnectionstring"];
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseCors("AllowReact");
app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<CustomerHub>("/customerhub");
app.Run();

