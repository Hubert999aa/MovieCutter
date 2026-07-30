namespace Domain.BusinessEnums
{
    public enum OperationType
    {
        Undefined = 0,
        DownloadOnly = 1,
        CuttingIntoPiecesOnly = 2,
        CuttingIntoFramesOnly = 3,
        DownloadPiecesAndFrames = 4,
        CuttingIntoPiecesAndFrames = 5
    }
}
