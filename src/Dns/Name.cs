using System.Buffers.Binary;
using System.Text;

namespace codecrafters_dns_server.Dns;

public record Name(string Value)
{
    private const char DomainSeparator = '.';
    private const int VariableLengthSize = 1;
    private const int PointerMask = 0b11000000;
    private const int OffsetMask = 0b00111111_11111111;
    private const int PointerSize = 2;

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
        var parsingState = new ParsingState(startPosition);

        while (!IsEndOfName(bytes, parsingState.OriginalPosition) || (parsingState.IsPointerFollowed && !IsEndOfName(bytes, parsingState.CurrentPosition)))
        {
            if (IsPointer(bytes, parsingState.OriginalPosition))
            {
                HandlePointer(bytes, parsingState);
                continue;
            }

            nameParts.Add(ParsePart(bytes, parsingState));

            if (!parsingState.IsPointerFollowed)
                parsingState.OriginalPosition = parsingState.CurrentPosition;
        }

        if (!parsingState.IsPointerFollowed)
            parsingState.OriginalPosition++; // Account for the terminal zero byte

        var domainName = new Name(string.Join(DomainSeparator, nameParts));
        return (domainName, parsingState.OriginalPosition - startPosition);
    }

    private static bool IsEndOfName(ReadOnlySpan<byte> bytes, int position) => bytes[position] == 0;

    private static bool IsPointer(ReadOnlySpan<byte> bytes, int position) => (bytes[position] & PointerMask) == PointerMask;

    private static void HandlePointer(ReadOnlySpan<byte> bytes, ParsingState state)
    {
        var offset = BinaryPrimitives.ReadInt16BigEndian(bytes[state.OriginalPosition..]);
        state.CurrentPosition = offset & OffsetMask;

        if (!state.IsPointerFollowed)
        {
            state.OriginalPosition += PointerSize;
            state.IsPointerFollowed = true;
        }
    }

    private static string ParsePart(ReadOnlySpan<byte> bytes, ParsingState state)
    {
        var partLength = bytes[state.CurrentPosition++];

        var part = Encoding.ASCII.GetString(bytes[state.CurrentPosition..(state.CurrentPosition + partLength)]);
        state.CurrentPosition += partLength;
        return part;
    }

    private class ParsingState(int startPosition)
    {
        public int CurrentPosition { get; set; } = startPosition;
        public int OriginalPosition { get; set; } = startPosition;
        public bool IsPointerFollowed { get; set; }
    }
}