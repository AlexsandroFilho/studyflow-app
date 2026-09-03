using StudyFlow.Api.Domain.Interfaces.Rag;

namespace StudyFlow.Api.Services;

public sealed class ProcessadorIngestaoFonteAnatomiaWorker(IServiceScopeFactory scopeFactory, ILogger<ProcessadorIngestaoFonteAnatomiaWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = scopeFactory.CreateScope())
            await scope.ServiceProvider.GetRequiredService<IAdministracaoFonteAnatomiaService>().ReenfileirarInterrompidasAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var processou = await scope.ServiceProvider.GetRequiredService<IAdministracaoFonteAnatomiaService>().ProcessarProximaAsync(stoppingToken);
                await Task.Delay(processou ? TimeSpan.FromSeconds(2) : TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Falha no worker de ingestão de fontes.");
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}
