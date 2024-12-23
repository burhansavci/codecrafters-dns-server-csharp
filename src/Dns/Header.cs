using System.Buffers.Binary;

namespace codecrafters_dns_server.Dns;

public record Header(
    ushort Id,
    bool QueryResponseIndicator,
    byte OperationCode, // 4 bits
    bool AuthoritativeAnswer,
    bool Truncation,
    bool RecursionDesired,
    bool RecursionAvailable,
    byte Reserved, // 3 bits
    byte ResponseCode, // 4 bits
    ushort QuestionCount,
    ushort AnswerRecordCount,
    ushort AuthorityRecordCount,
    ushort AdditionalRecordCount)
{
    // The header section is always 12 bytes long.
    public const int Size = 12;

    public static Header Parse(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != Size) throw new ArgumentException("Header data must be 12 bytes long", nameof(bytes));
        
        Header header = new(
            Id: BinaryPrimitives.ReadUInt16BigEndian(bytes[..2]),
            QueryResponseIndicator: (bytes[2] & 0b10000000) != 0,
            OperationCode: (byte)((bytes[2] & 0b01111000) >> 3),
            AuthoritativeAnswer: (bytes[2] & 0b00000100) != 0,
            Truncation: (bytes[2] & 0b00000010) != 0,
            RecursionDesired: (bytes[2] & 0b00000001) != 0,
            RecursionAvailable: (bytes[3] & 0b10000000) != 0,
            Reserved: (byte)((bytes[3] & 0b01110000) >> 4),
            ResponseCode: (byte)(bytes[3] & 0b00001111),
            QuestionCount: BinaryPrimitives.ReadUInt16BigEndian(bytes[4..6]),
            AnswerRecordCount: BinaryPrimitives.ReadUInt16BigEndian(bytes[6..8]),
            AuthorityRecordCount: BinaryPrimitives.ReadUInt16BigEndian(bytes[8..10]),
            AdditionalRecordCount: BinaryPrimitives.ReadUInt16BigEndian(bytes[10..])
        );

        return header;
    }

    public ReadOnlySpan<byte> ToReadonlySpan()
    {
        Span<byte> bytes = new byte[Size];

        //Integers are encoded in big-endian format.
        BinaryPrimitives.WriteUInt16BigEndian(bytes, Id);

        bytes[2] = (byte)((QueryResponseIndicator ? 1 : 0) << 7);

        bytes[2] |= (byte)(OperationCode << 3);

        bytes[2] |= (byte)((AuthoritativeAnswer ? 1 : 0) << 2);

        bytes[2] |= (byte)((Truncation ? 1 : 0) << 1);

        bytes[2] |= (byte)(RecursionDesired ? 1 : 0);

        bytes[3] = (byte)((RecursionAvailable ? 1 : 0) << 7);

        bytes[3] |= (byte)(Reserved << 4);

        bytes[3] |= ResponseCode;

        BinaryPrimitives.WriteUInt16BigEndian(bytes[4..6], QuestionCount);

        BinaryPrimitives.WriteUInt16BigEndian(bytes[6..8], AnswerRecordCount);

        BinaryPrimitives.WriteUInt16BigEndian(bytes[8..10], AuthorityRecordCount);

        BinaryPrimitives.WriteUInt16BigEndian(bytes[10..], AdditionalRecordCount);

        return bytes;
    }
}