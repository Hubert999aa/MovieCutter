using JobsRunner.Interfaces;

public class SignalRInitializer : IHostedService
{
    private readonly IOperationStatusManager _service;

    public SignalRInitializer(IOperationStatusManager service)
    {
        _service = service;
    }

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        await _service.StartAsync();
    }

    public Task StopAsync(
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}