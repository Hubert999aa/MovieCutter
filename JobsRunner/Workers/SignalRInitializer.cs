using JobsRunner.Interfaces;

public class SignalRInitializer : IHostedService
{
    private readonly IOperationStatusManager _service;
    private readonly ILogger<SignalRInitializer> _logger;

    public SignalRInitializer(IOperationStatusManager service, ILogger<SignalRInitializer> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _service.StartAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Nie udało się połączyć z SignalR: {Message}", ex.Message);
        } 
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}