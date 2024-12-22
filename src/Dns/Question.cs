using System.Buffers.Binary;

namespace codecrafters_dns_server.Dns;

public record Question(Name Name, Type Type, Class Class)
{
    private const int TypeSize = 2;
    private const int ClassSize = 2;

    public ReadOnlySpan<byte> ToReadonlySpan()
    {
        var nameBytes = Name.ToReadonlySpan();
        var totalLength = nameBytes.Length + TypeSize + ClassSize;

        Span<byte> bytes = new byte[totalLength];

        var position = 0;
        nameBytes.CopyTo(bytes);
        position += nameBytes.Length;

        BinaryPrimitives.WriteInt16BigEndian(bytes[position..], (short)Type);
        position += TypeSize;

        BinaryPrimitives.WriteInt16BigEndian(bytes[position..], (short)Class);

        return bytes;
    }
}