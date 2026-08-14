using Application.Interfaces;
using Domain.BusinessEnums;
using Domain.HubsModels;
using JobsRunner.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace JobsRunner.Services
{
    public class OperationStatusManagerService : IOperationStatusManager, IAsyncDisposable
    {
        private readonly HubConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;

        public OperationStatusManagerService(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _connection = new HubConnectionBuilder()
                .WithUrl(configuration["SignalR:ConnectionUrl"])
                .WithAutomaticReconnect(new[] {
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                })
                .Build();
        }

        public async Task UpdateOperationStatus(int operationId, OperationStatus status)
        {
            using var scope = _scopeFactory.CreateScope();

            var _databaseContext = scope.ServiceProvider
                .GetRequiredService<IMovieCutterDatabase>();

            var operation = await _databaseContext.Operations.SingleOrDefaultAsync(p => p.IdOperation == operationId);
            if (operation == null) return;

            this.SendStatusMessage(operation.IdOperation, status.ToString());

            operation.OperationStatus = status;
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateOperationProgress(int operationId, VideoProcess processType, int progress = 0)
        {
            using var scope = _scopeFactory.CreateScope();

            var _databaseContext = scope.ServiceProvider
                .GetRequiredService<IMovieCutterDatabase>();

            var operation = await _databaseContext.Operations.SingleOrDefaultAsync(p => p.IdOperation == operationId);
            if (operation == null) return;

            var operationType = operation.OperationType;
            var scaledProgress = 0;

            switch (processType)
            {
                case VideoProcess.DownloadingMetadata:
                    scaledProgress = operationType == OperationType.DownloadOnly ? 10 : 5;
                    break;
                case VideoProcess.Downloading:
                    scaledProgress = operationType == OperationType.DownloadOnly ? this.ScaleProgresPercent(progress, 0, 100, 10, 100) : this.ScaleProgresPercent(progress, 0, 100, 5, 40);
                    break;
                case VideoProcess.CuttingIntoPieces:
                    scaledProgress = operationType == OperationType.CuttingIntoPiecesOnly ? progress : this.ScaleProgresPercent(progress, 0, 100, 41, 50);
                    break;
                case VideoProcess.CuttingIntoFrames:
                    scaledProgress = operationType == OperationType.CuttingIntoFramesOnly ? progress : this.ScaleProgresPercent(progress, 0, 100, 51, 100);
                    break;
            }

            this.SendProgressMessage(operation.IdOperation, scaledProgress);

            operation.ProgressPercentage = scaledProgress;
            await _databaseContext.SaveChangesAsync();
        }

        private async void SendProgressMessage(int operationId, int percent)
        {
            var payload = new OperationProgressUpdate { IdOperation = operationId, ProgressPercent = percent };
            await _connection.InvokeAsync("SendOperationProgress", payload);
        }

        private async void SendStatusMessage(int operationId, string status)
        {
            var payload = new OperationStatusUpdate { IdOperation = operationId, OperationStatus = status };
            await _connection.InvokeAsync("SendOperationStatus", payload);
        }

        private int ScaleProgresPercent(double value, double fromMin, double fromMax, double toMin, double toMax)
        {
            return (int)Math.Round(
                ((value - fromMin) * (toMax - toMin) / (fromMax - fromMin)) + toMin
            );
        }

        public async Task StartAsync()
        {
            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _connection.DisposeAsync();
        }
    }
}
