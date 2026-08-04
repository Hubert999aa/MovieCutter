using Domain.HubsModels;
using Microsoft.AspNetCore.SignalR;

namespace MovieCutterAPI.Hubs
{
    public class OperationsProgressHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendOperationStatus(OperationStatusUpdate payload)
        {
            await Clients.All.SendAsync(
                "OperationStatus",
                payload);
        }

        public async Task SendOperationProgress(OperationProgressUpdate payload)
        {
            await Clients.All.SendAsync(
                "OperationProgress",
                payload);
        }
    }
}
