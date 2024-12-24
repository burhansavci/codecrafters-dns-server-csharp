namespace codecrafters_dns_server.Dns.Server.Builders;

public class DefaultDnsResponseBuilder : IDnsResponseBuilder
{
    public Task<Message> BuildResponseAsync(Message request)
    {
        var responseQuestions = request.Questions.Select(q => new Question(q.Name, q.Type, q.Class)).ToArray();

        var responseAnswers = request.Questions.Select(q => new Answer([new ResourceRecord(q.Name, q.Type, q.Class, 60, [8, 8, 8, 8])])).ToArray();

        var responseHeader = CreateResponseHeader(request, responseQuestions.Length, responseAnswers.Length);

        return Task.FromResult(new Message(responseHeader, responseQuestions, responseAnswers));
    }

    private static Header CreateResponseHeader(Message request, int questionCount, int answerCount) =>
        new(
            request.Header.Id,
            true,
            request.Header.OperationCode,
            false,
            false,
            request.Header.RecursionDesired,
            false,
            0,
            (byte)(request.Header.OperationCode == 0 ? 0 : 4), // 0 (no error) if OPCODE is 0 (standard query) else 4 (not implemented)
            (ushort)questionCount,
            (ushort)answerCount,
            0,
            0
        );
}