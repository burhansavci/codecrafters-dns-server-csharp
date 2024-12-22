using System.Buffers.Binary;

namespace codecrafters_dns_server.Dns;

public record Answer(List<ResourceRecord> ResourceRecords)
{
    public ReadOnlySpan<byte> ToReadonlySpan()
    {
        List<byte> resourceRecordsBytes = [];
        foreach (var resourceRecord in ResourceRecords)
        {
            resourceRecordsBytes.AddRange(resourceRecord.ToReadonlySpan().ToArray());
        }

        return resourceRecordsBytes.ToArray();
    }
}

public record ResourceRecord(Name Name, Type Type, Class Class, int TimeToLive, byte[] Data)
{
    private const int TypeSize = 2;
    private const int ClassSize = 2;
    private const int TimeToLiveSize = 4;
    private const int DataLengthSize = 2;

    public ReadOnlySpan<byte> ToReadonlySpan()
    {
        var nameBytes = Name.ToReadonlySpan();
        var totalLength = nameBytes.Length + TypeSize + ClassSize + TimeToLiveSize + DataLengthSize + Data.Length;

        Span<byte> bytes = new byte[totalLength];

        var position = 0;
        nameBytes.CopyTo(bytes);
        position += nameBytes.Length;

        BinaryPrimitives.WriteInt16BigEndian(bytes[position..], (short)Type);
        position += TypeSize;

        BinaryPrimitives.WriteInt16BigEndian(bytes[position..], (short)Class);
        position += ClassSize;

        BinaryPrimitives.WriteInt32BigEndian(bytes[position..], TimeToLive);
        position += TimeToLiveSize;

        BinaryPrimitives.WriteInt16BigEndian(bytes[position..], (short)Data.Length);
        position += DataLengthSize;

        Data.CopyTo(bytes[position..]);

        return bytes;
    }
}