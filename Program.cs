using Microsoft.EntityFrameworkCore;
using ProjetoChallengeMottu.Data;
using ProjetoChallengeMottu.Interfaces;
using ProjetoChallengeMottu.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IEchoBeaconRepository, EchoBeaconRepository>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IMotoRepository, MotoRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
