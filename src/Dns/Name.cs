using System.Buffers.Binary;
using System.Text;

namespace codecrafters_dns_server.Dns;

public record Name(string Value)
{
    private const char DomainSeparator = '.';
    private const int VariableLengthSize = 1;
    private const int PointerMask = 0b11000000;
    private const int OffsetMask = 0b00111111_11111111;

    public ReadOnlySpan<byte> ToReadonlySpan()
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

    public static (Name Name, int BytesRead) Parse(ReadOnlySpan<byte> bytes, int startPosition = 0)
    {
        var nameParts = new List<string>();
        var isPointerFollowed = false;
        var position = startPosition;
        var originalPosition = startPosition;

        while (bytes[originalPosition] != 0)
        {
            if ((bytes[originalPosition] & PointerMask) == PointerMask)
            {
                var offset = BinaryPrimitives.ReadInt16BigEndian(bytes[originalPosition..]);
                position = offset & OffsetMask; // Extract the offset value

                if (!isPointerFollowed)
                {
                    // Only advance the original position once when we first encounter a pointer
                    originalPosition += 2;
                    isPointerFollowed = true;
                }

                continue;
            }

            var labelLength = bytes[position++];
            var label = Encoding.ASCII.GetString(bytes[position..(position + labelLength)]);
            nameParts.Add(label);
            position += labelLength;

            if (!isPointerFollowed)
            {
                originalPosition = position;
            }
        }

        if (!isPointerFollowed)
        {
            originalPosition++; // Account for the terminal zero byte
        }

        return (new Name(string.Join(DomainSeparator, nameParts)), originalPosition - startPosition);
    }
}