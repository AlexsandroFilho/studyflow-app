using Scalar.AspNetCore;
using StudyFlow.Api.Configurations;
using StudyFlow.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddJsonSerializationConfiguration();
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCorsConfiguration();

var app = builder.Build();

if (args.FirstOrDefault()?.Equals("ingest-anatomia", StringComparison.OrdinalIgnoreCase) == true)
{
    using var scope = app.Services.CreateScope();
    var request = LerArgumentosIngestao(args.Skip(1).ToArray());
    var resultado = await scope.ServiceProvider.GetRequiredService<StudyFlow.Api.Domain.Interfaces.Rag.IIngestaoAnatomiaService>()
        .IngerirAsync(request);
    Console.WriteLine($"Fonte {resultado.FonteId} indexada com {resultado.QuantidadeChunks} chunks.");
    return;
}

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

static StudyFlow.Api.DTOs.FonteIngestaoRequest LerArgumentosIngestao(string[] args)
{
    var valores = args.Chunk(2).Where(x => x.Length == 2 && x[0].StartsWith("--"))
        .ToDictionary(x => x[0][2..], x => x[1], StringComparer.OrdinalIgnoreCase);
    string Obrigatorio(string nome) => valores.TryGetValue(nome, out var valor) && !string.IsNullOrWhiteSpace(valor)
        ? valor : throw new ArgumentException($"Informe --{nome} para a ingestão.");
    return new(Obrigatorio("file"), Obrigatorio("title"), valores.GetValueOrDefault("author"), Obrigatorio("version"), valores.GetValueOrDefault("subject"), valores.GetValueOrDefault("subsubject"));
}
