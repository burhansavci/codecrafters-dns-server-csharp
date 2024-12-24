using System.Buffers.Binary;

namespace codecrafters_dns_server.Dns;

public record Answer(List<ResourceRecord> ResourceRecords)
{
    public static (Answer Answer, int BytesRead) Parse(ReadOnlySpan<byte> bytes, int startPosition = 0)
    {
        var resourceRecords = new List<ResourceRecord>();
        var position = startPosition;

        while (position < bytes.Length)
        {
            var (resourceRecord, bytesRead) = ResourceRecord.Parse(bytes, position);
            resourceRecords.Add(resourceRecord);
            position += bytesRead;
        }

        return (new Answer(resourceRecords), position - startPosition);
    }

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

    public static (ResourceRecord ResourceRecord, int BytesRead) Parse(ReadOnlySpan<byte> bytes, int startPosition = 0)
    {
        var (name, bytesRead) = Name.Parse(bytes, startPosition);
        var position = startPosition + bytesRead;

        var type = (Type)BinaryPrimitives.ReadUInt16BigEndian(bytes[position..]);
        position += TypeSize;

        var @class = (Class)BinaryPrimitives.ReadUInt16BigEndian(bytes[position..]);
        position += ClassSize;

        var timeToLive = BinaryPrimitives.ReadInt32BigEndian(bytes[position..]);
        position += TimeToLiveSize;

        var dataLength = BinaryPrimitives.ReadUInt16BigEndian(bytes[position..]);
        position += DataLengthSize;

        var data = bytes[position..(position + dataLength)].ToArray();
        position += dataLength;

        return (new ResourceRecord(name, type, @class, timeToLive, data), position - startPosition);
    }

    public ReadOnlySpan<byte> ToReadonlySpan()
    {
        var nameBytes = Name.ToReadonlySpan();
        var totalLength = nameBytes.Length + TypeSize + ClassSize + TimeToLiveSize + DataLengthSize + Data.Length;

        Span<byte> bytes = new byte[totalLength];

        var position = 0;
        nameBytes.CopyTo(bytes);
        position += nameBytes.Length;

        BinaryPrimitives.WriteUInt16BigEndian(bytes[position..], (ushort)Type);
        position += TypeSize;

        BinaryPrimitives.WriteUInt16BigEndian(bytes[position..], (ushort)Class);
        position += ClassSize;

        BinaryPrimitives.WriteInt32BigEndian(bytes[position..], TimeToLive);
        position += TimeToLiveSize;

        BinaryPrimitives.WriteUInt16BigEndian(bytes[position..], (ushort)Data.Length);
        position += DataLengthSize;

        Data.CopyTo(bytes[position..]);

        return bytes;
    }
}