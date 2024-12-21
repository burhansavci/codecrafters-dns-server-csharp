namespace codecrafters_dns_server.Dns;

public record Message(Header Header, Question[] Questions, Answer[] Answers)
{
    public Span<byte> ToSpan()
    {
        Span<byte> headerBytes = Header.ToSpan();

        List<byte> questionsBytes = [];
        foreach (var question in Questions)
        {
            questionsBytes.AddRange(question.ToSpan().ToArray());
        }
        
        List<byte> answersBytes = [];
        foreach (var answer in Answers)
        {
            answersBytes.AddRange(answer.ToSpan().ToArray());
        }

        List<byte> messageBytes = [];
        messageBytes.AddRange(headerBytes.ToArray());
        messageBytes.AddRange(questionsBytes.ToArray());
        messageBytes.AddRange(answersBytes.ToArray());

        return messageBytes.ToArray();
    }
}