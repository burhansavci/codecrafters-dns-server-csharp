namespace codecrafters_dns_server.Dns.Server.Builders;

public interface IDnsResponseBuilder
{
    Task<Message> BuildResponseAsync(Message request);
}