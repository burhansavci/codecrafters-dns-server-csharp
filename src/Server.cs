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

    Console.WriteLine($"Received {receivedData.Length} bytes from {sourceEndPoint}: {receivedString}");

    Header header = Header.Parse(receivedData.AsSpan()[..Header.Size]);

    var name = Name.Parse(receivedData.AsSpan()[Header.Size..]);

    Console.WriteLine(name);

    Question question = new(name, Type.A, Class.IN);

    Answer answer = new([new ResourceRecord(name, Type.A, Class.IN, 60, [8, 8, 8, 8])]);

    Message message = new(header, [question], [answer]);

    // Create an empty response
    byte[] response = message.ToSpan().ToArray();

    // Send response
    udpClient.Send(response, response.Length, sourceEndPoint);
}