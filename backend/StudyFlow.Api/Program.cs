using Scalar.AspNetCore;
using StudyFlow.Api.Configurations;
using StudyFlow.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddJsonSerializationConfiguration();
builder.Services.AddApplicationServices();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCorsConfiguration();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseCorsConfiguration();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<CurrentUsuarioMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
