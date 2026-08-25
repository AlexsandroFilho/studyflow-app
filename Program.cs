using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StudyFlow.Api.Data;
using StudyFlow.Api.Data.Repositories;
using StudyFlow.Api.Domain.Interfaces.Conexao;
using StudyFlow.Api.Domain.Interfaces.Notas;
using StudyFlow.Api.Domain.Interfaces.Temas;
using StudyFlow.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseNpgsql(connectionString));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ITemaRepository, TemaRepository>();
builder.Services.AddScoped<ITemaService, TemaService>();
builder.Services.AddScoped<INotaRepository, NotaRepository>();
builder.Services.AddScoped<INotaService, NotaService>();
builder.Services.AddScoped<IConexaoNotaRepository, ConexaoNotaRepository>();
builder.Services.AddScoped<IConexaoNotaService, ConexaoNotaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
