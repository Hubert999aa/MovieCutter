using Domain.ConsumersContracts;

namespace JobsRunner.Interfaces
{
    public interface IVideoProcessor
    {
        public Task<bool> CutVideoPieceAsync(VideoPiece piece, string sourcePath, string newVideoPathWithoutExtension, string videoExtension, int videoNumber, CancellationToken cancellationToken);
    }
}
