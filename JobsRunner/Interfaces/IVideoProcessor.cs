using Domain.ConsumersContracts;

namespace JobsRunner.Interfaces
{
    public interface IVideoProcessor
    {
        public Task<bool> CutVideoPieceAsync(VideoPiece piece, string sourcePath, string newVideoPathWithoutExtension, string videoExtension, int videoNumber, CancellationToken cancellationToken);
        public Task<bool> CutVideoFramesAsync(int operationId, string sourcePath, string outputFolder, string videoName, CancellationToken cancellationToken, bool pushProgressNotifications = true);
    }
}
