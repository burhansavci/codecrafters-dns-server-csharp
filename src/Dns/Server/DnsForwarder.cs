using System.Net;
using System.Net.Sockets;

namespace codecrafters_dns_server.Dns.Server;

public class DnsForwarder
{
    private readonly UdpClient _client;
    private readonly IPEndPoint _endpoint;

    public DnsForwarder(string address)
    {
        var parts = address.Split(':');
        if (parts.Length != 2)
            throw new ArgumentException("Forwarder address must be in format 'ip:port'", nameof(address));

        var ip = IPAddress.Parse(parts[0]);
        var port = int.Parse(parts[1]);

        _endpoint = new IPEndPoint(ip, port);
        _client = new UdpClient();

        Console.WriteLine($"Forwarding requests to {address}");
    }

    public async Task<Message> ForwardRequestAsync(Message request)
    {
        var requestBytes = request.ToReadonlySpan().ToArray();
        await _client.SendAsync(requestBytes, requestBytes.Length, _endpoint);

        var result = await _client.ReceiveAsync();
        return Message.Parse(result.Buffer);
    }
}