namespace Domain.TechnicalModels
{
    public class RabbitMQBaseSettings
    {
        public required string Host { get; init; }
        public required string VirtualHost { get; init; }
        public required string Username { get; init; }
        public required string Password { get; init; }
        public int Port { get; init; }
    }
}
