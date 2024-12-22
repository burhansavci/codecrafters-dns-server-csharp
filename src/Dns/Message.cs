namespace codecrafters_dns_server.Dns;

public record Message(Header Header, Question[] Questions, Answer[] Answers)
{
    public ReadOnlySpan<byte> ToReadonlySpan()
    {
        var headerBytes = Header.ToReadonlySpan();

        List<byte> questionsBytes = [];
        foreach (var question in Questions)
        {
            questionsBytes.AddRange(question.ToReadonlySpan().ToArray());
        }
        
        List<byte> answersBytes = [];
        foreach (var answer in Answers)
        {
            answersBytes.AddRange(answer.ToReadonlySpan().ToArray());
        }

        List<byte> messageBytes = [];
        messageBytes.AddRange(headerBytes.ToArray());
        messageBytes.AddRange(questionsBytes.ToArray());
        messageBytes.AddRange(answersBytes.ToArray());

        return messageBytes.ToArray();
    }
}