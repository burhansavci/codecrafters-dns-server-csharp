using System.Buffers.Binary;
using System.Text;

namespace codecrafters_dns_server.Dns;

public record Question(string Name, QuestionType Type, QuestionClass Class)
{
    private const char DomainSeparator = '.';

    private const int TypeSize = 2;

    private const int ClassSize = 2;

    private const int VariableLengthSize = 1;

    public Span<byte> ToSpan()
    {
        var nameParts = Name.Split(DomainSeparator);

        var nameLength = nameParts.Sum(part => VariableLengthSize + part.Length);
        nameLength++; // Terminal zero byte

        var totalLength = nameLength + TypeSize + ClassSize;

        Span<byte> bytes = new byte[totalLength];

        var position = 0;
        foreach (var part in nameParts)
        {
            bytes[position++] = (byte)part.Length;
            Encoding.ASCII.GetBytes(part, bytes[position..]);
            position += part.Length;
        }
        bytes[position++] = 0;

        BinaryPrimitives.WriteInt16BigEndian(bytes[position..], (short)Type);
        position += TypeSize;

        BinaryPrimitives.WriteInt16BigEndian(bytes[position..], (short)Class);

        return bytes;
    }
}