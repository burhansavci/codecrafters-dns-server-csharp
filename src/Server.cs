using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_dns_server.Dns;
using Type = codecrafters_dns_server.Dns.Type;

// Resolve UDP address
IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
int port = 2053;
IPEndPoint udpEndPoint = new(ipAddress, port);

// Create UDP socket
UdpClient udpClient = new(udpEndPoint);

while (true)
{
    // Receive data
    IPEndPoint sourceEndPoint = new(IPAddress.Any, 0);
    byte[] receivedData = udpClient.Receive(ref sourceEndPoint);
    string receivedString = Encoding.ASCII.GetString(receivedData);

    Console.WriteLine($"Received {receivedData.Length} bytes and {receivedString.Length} length of string from {sourceEndPoint}: {receivedString}");

    var request = Message.Parse(receivedData);

    var responseQuestions = new Question[request.Questions.Length];
    for (var i = 0; i < request.Questions.Length; i++)
    {
        var question = request.Questions[i];
        responseQuestions[i] = new Question(question.Name, question.Type, question.Class);
    }

    var responseAnswers = new Answer[request.Questions.Length];
    for (var i = 0; i < request.Questions.Length; i++)
    {
        var question = request.Questions[i];
        responseAnswers[i] = new Answer([new ResourceRecord(question.Name, question.Type, question.Class, 60, [8, 8, 8, 8])]);
    }

    Header responseHeader = new(request.Header.Id,
        true,
        request.Header.OperationCode,
        false,
        false,
        request.Header.RecursionDesired,
        false,
        0,
        (byte)(request.Header.OperationCode == 0 ? 0 : 4), // 0 (no error) if OPCODE is 0 (standard query) else 4 (not implemented)
        (ushort)responseQuestions.Length,
        (ushort)responseAnswers.Length,
        0,
        0);

    Message message = new(responseHeader, responseQuestions, responseAnswers);

    // Create an empty response
    byte[] response = message.ToReadonlySpan().ToArray();

    // Send response
    udpClient.Send(response, response.Length, sourceEndPoint);
}