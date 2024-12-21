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
    public Span<byte> ToSpan()
    {
        // The header section is always 12 bytes long. Integers are encoded in big-endian format.
        Span<byte> bytes = new(new byte[12]);

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