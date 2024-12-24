namespace codecrafters_dns_server.Dns.Server.Builders;

public class ForwardingDnsResponseBuilder(DnsForwarder forwarder) : IDnsResponseBuilder
{
    public async Task<Message> BuildResponseAsync(Message request)
    {
        var forwardResponses = new List<Message>();

        foreach (var question in request.Questions)
        {
            var forwardRequest = CreateForwardRequest(request, question);
            var forwardResponse = await forwarder.ForwardRequestAsync(forwardRequest);
            forwardResponses.Add(forwardResponse);
        }

        return CreateCombinedResponse(request, forwardResponses);
    }

    private static Message CreateForwardRequest(Message request, Question question)
    {
        var header = new Header(
            request.Header.Id,
            false,
            request.Header.OperationCode,
            false,
            false,
            request.Header.RecursionDesired,
            false,
            0,
            (byte)(request.Header.OperationCode == 0 ? 0 : 4), // 0 (no error) if OPCODE is 0 (standard query) else 4 (not implemented),
            1,
            0,
            0,
            0
        );

        return new Message(header, [question], []);
    }

    private static Message CreateCombinedResponse(Message request, List<Message> forwardResponses)
    {
        var header = new Header(
            request.Header.Id,
            true,
            request.Header.OperationCode,
            false,
            false,
            request.Header.RecursionDesired,
            false,
            0,
            (byte)(request.Header.OperationCode == 0 ? 0 : 4), // 0 (no error) if OPCODE is 0 (standard query) else 4 (not implemented),
            (ushort)request.Questions.Length,
            (ushort)forwardResponses.SelectMany(r => r.Answers).Count(),
            0,
            0
        );

        return new Message(
            header,
            request.Questions,
            forwardResponses.SelectMany(r => r.Answers).ToArray()
        );
    }
}