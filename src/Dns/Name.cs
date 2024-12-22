using System.Text;

namespace codecrafters_dns_server.Dns;

public record Name(string Value)
{
    private const char DomainSeparator = '.';
    private const int VariableLengthSize = 1;
    
    public static Name Parse(ReadOnlySpan<byte> bytes)
    {
        var position = 0;
        var nameParts = new List<string>();
        while (bytes[position] != 0)
        {
            var partLength = bytes[position];
            position++;
            var part = Encoding.ASCII.GetString(bytes[position..(position + partLength)]);
            nameParts.Add(part);
            position += partLength;
        }

        return new Name(string.Join(DomainSeparator, nameParts));
    }

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