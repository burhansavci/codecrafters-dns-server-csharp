using System.Buffers.Binary;

namespace codecrafters_dns_server.Dns;

public record Header(
    short Id,
    bool QueryResponseIndicator,
    byte OperationCode, // 4 bits
    bool AuthoritativeAnswer,
    bool Truncation,
    bool RecursionDesired,
    bool RecursionAvailable,
    byte Reserved, // 3 bits
    byte ResponseCode, // 4 bits
    short QuestionCount,
    short AnswerRecordCount,
    short AuthorityRecordCount,
    short AdditionalRecordCount)
{
    public static Header FromBytes(byte[] data)
    {
        var dataSpan = data[..12].AsSpan();

        var opCode = (byte)((dataSpan[2] & 0b01111000) >> 3);

        Header header = new(
            Id: BinaryPrimitives.ReadInt16BigEndian(dataSpan),
            QueryResponseIndicator: true,
            OperationCode: opCode,
            AuthoritativeAnswer: (dataSpan[2] & 0b00000100) != 0,
            Truncation: (dataSpan[2] & 0b00000010) != 0,
            RecursionDesired: (dataSpan[2] & 0b00000001) != 0,
            RecursionAvailable: (dataSpan[3] & 0b10000000) != 0,
            Reserved: (byte)((dataSpan[3] & 0b01110000) >> 4),
            ResponseCode: (byte)(opCode == 0 ? 0 : 4), // 0 (no error) if OPCODE is 0 (standard query) else 4 (not implemented)
            QuestionCount: BinaryPrimitives.ReadInt16BigEndian(dataSpan[4..]),
            AnswerRecordCount: 1,
            AuthorityRecordCount: BinaryPrimitives.ReadInt16BigEndian(dataSpan[8..]),
            AdditionalRecordCount: BinaryPrimitives.ReadInt16BigEndian(dataSpan[10..])
        );

        return header;
    }

    public Span<byte> ToSpan()
    {
        // The header section is always 12 bytes long. Integers are encoded in big-endian format.
        Span<byte> bytes = new byte[12];

        BinaryPrimitives.WriteInt16BigEndian(bytes, Id);

        bytes[2] = (byte)((QueryResponseIndicator ? 1 : 0) << 7);

        bytes[2] |= (byte)(OperationCode << 3);

        bytes[2] |= (byte)((AuthoritativeAnswer ? 1 : 0) << 2);

        bytes[2] |= (byte)((Truncation ? 1 : 0) << 1);

        bytes[2] |= (byte)(RecursionDesired ? 1 : 0);

        bytes[3] = (byte)((RecursionAvailable ? 1 : 0) << 7);

        bytes[3] |= (byte)(Reserved << 4);

        bytes[3] |= ResponseCode;

        BinaryPrimitives.WriteInt16BigEndian(bytes[4..], QuestionCount);

        BinaryPrimitives.WriteInt16BigEndian(bytes[6..], AnswerRecordCount);

        BinaryPrimitives.WriteInt16BigEndian(bytes[8..], AuthorityRecordCount);

        BinaryPrimitives.WriteInt16BigEndian(bytes[10..], AdditionalRecordCount);

        return bytes;
    }
}