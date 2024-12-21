namespace codecrafters_dns_server.Dns;

public record Message(Header Header, Question[] Questions)
{
    public Span<byte> ToSpan()
    {
        Span<byte> headerBytes = Header.ToSpan();

        List<byte> questionsBytes = [];
        foreach (var question in Questions)
        {
            questionsBytes.AddRange(question.ToSpan().ToArray());
        }

        List<byte> messageBytes = [];
        messageBytes.AddRange(headerBytes.ToArray());
        messageBytes.AddRange(questionsBytes.ToArray());

        return messageBytes.ToArray();
    }
}