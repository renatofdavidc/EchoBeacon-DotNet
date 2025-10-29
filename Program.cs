using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProjetoChallengeMottu.Data;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Repositories;
using ProjetoChallengeMottu.Middleware;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configuração de repositórios
builder.Services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
builder.Services.AddScoped<IEchoBeaconRepository, EchoBeaconRepository>();
builder.Services.AddScoped<IMotoRepository, MotoRepository>();
builder.Services.AddScoped<ILocalizacaoMotoRepository, LocalizacaoMotoRepository>();
builder.Services.AddScoped<IAuditoriaMotoRepository, AuditoriaMotoRepository>();

// Configuração do DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração de versionamento da API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger com versionamento
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "EchoBeacon API",
        Version = "v1",
        Description = "API para gerenciamento de motos, dispositivos EchoBeacon, funcionários e localizações no pátio"
    });

    // Adicionar suporte para API Key no Swagger
    c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "API Key necessária para acessar os endpoints. Header: X-Api-Key",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Name = "X-Api-Key",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Reference = new Microsoft.OpenApi.Models.OpenApiReference
        {
            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
            Id = "ApiKey"
        },
        In = Microsoft.OpenApi.Models.ParameterLocation.Header
    };

    var securityRequirement = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { securityScheme, new[] { "ApiKey" } }
    };

    c.AddSecurityRequirement(securityRequirement);

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

// Configuração de Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database")
    .AddCheck("api", () => HealthCheckResult.Healthy("API está funcionando"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EchoBeacon API v1");
    });
}

// Middleware de autenticação com API Key
app.UseApiKeyAuthentication();

app.UseAuthorization();

// Endpoint de Health Checks
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        });
        await context.Response.WriteAsync(result);
    }
});

app.MapControllers();

app.Run();

// Torna a classe Program acessível para testes de integração
public partial class Program { }

