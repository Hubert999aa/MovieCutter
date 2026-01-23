namespace JobsRunner.Options
{
    public sealed class RabbitMQSettings
    {
        public required string Host { get; init; }
        public required string VirtualHost { get; init; }
        public required string Username { get; init; }
        public required string Password { get; init; }
        public int Port { get; init; }
        public required QueueSettings Queues { get; init; }
    }

    public sealed class QueueSettings
    {
        public required DownloadVideoSettings DownloadVideoSettings { get; init; }
        public required CutVideoIntoFramesSettings CutVideoIntoFramesSettings { get; init; }
        public required CutVideoIntoPicesSettings CutVideoIntoPicesSettings { get; init; }
    }

    public sealed class DownloadVideoSettings : ConsumerQueueSettings { }
    public sealed class CutVideoIntoFramesSettings : ConsumerQueueSettings { }
    public sealed class CutVideoIntoPicesSettings : ConsumerQueueSettings { }

    public abstract class ConsumerQueueSettings
    {
        public required string Name { get; init; }
        public int PrefetchCount { get; init; }
        public int RetryCount { get; init; }
        public int RetryIntervalSeconds { get; init; }
    }
}
