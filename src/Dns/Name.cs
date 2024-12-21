using System.Text;

namespace codecrafters_dns_server.Dns;

public record Name(string Value)
{
    private const char DomainSeparator = '.';
    private const int VariableLengthSize = 1;

    public Span<byte> ToSpan()
    {
        var nameParts = Value.Split(DomainSeparator);

        var nameLength = nameParts.Sum(part => VariableLengthSize + part.Length);
        nameLength++; // Terminal zero byte

        Span<byte> bytes = new byte[nameLength];

        var position = 0;
        foreach (var part in nameParts)
        {
            bytes[position++] = (byte)part.Length;
            Encoding.ASCII.GetBytes(part, bytes[position..]);
            position += part.Length;
        }
        bytes[position] = 0;

        return bytes;
    }
}