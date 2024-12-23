namespace codecrafters_dns_server.Dns;

public record Message(Header Header, Question[] Questions, Answer[] Answers)
{
    public static Message Parse(ReadOnlySpan<byte> bytes)
    {
        var header = Header.Parse(bytes[..Header.Size]);
        var position = Header.Size;

        var questions = new Question[header.QuestionCount];
        for (int i = 0; i < header.QuestionCount; i++)
        {
            var (question, bytesRead) = Question.Parse(bytes, position);
            questions[i] = question;
            position += bytesRead;
        }

        return new Message(header, questions, []);
    }

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