namespace codecrafters_dns_server.Dns;

public record Message(Header Header)
{
    public Span<byte> ToSpan()
    {
        return Header.ToSpan();
    }
}
