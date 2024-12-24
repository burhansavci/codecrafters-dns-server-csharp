using System.Net;
using System.Net.Sockets;
using System.Text;
using codecrafters_dns_server.Dns.Server.Builders;

namespace codecrafters_dns_server.Dns.Server;

public class DnsServer
{
    private readonly UdpClient _udpClient;
    private readonly IDnsResponseBuilder _responseBuilder;

    public DnsServer(int port = 2053, string? forwarderAddress = null)
    {
        var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
        _udpClient = new UdpClient(endpoint);

        if (!string.IsNullOrEmpty(forwarderAddress))
        {
            var forwarder = new DnsForwarder(forwarderAddress);
            _responseBuilder = new ForwardingDnsResponseBuilder(forwarder);
        }
        else
        {
            _responseBuilder = new DefaultDnsResponseBuilder();
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var (request, sourceEndPoint) = await ReceiveRequestAsync();
                var response = await _responseBuilder.BuildResponseAsync(request);
                await SendResponseAsync(response, sourceEndPoint);
            }
        }
        catch (OperationCanceledException)
        {
            // Normal cancellation, ignore
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Error in DNS server: {ex}");
            throw;
        }
    }

    private async Task<(Message Request, IPEndPoint SourceEndPoint)> ReceiveRequestAsync()
    {
        var receivedResult = await _udpClient.ReceiveAsync();
        var request = Message.Parse(receivedResult.Buffer);

        Console.WriteLine($"Received {receivedResult.Buffer.Length} bytes from {receivedResult.RemoteEndPoint}: {Encoding.ASCII.GetString(receivedResult.Buffer)}");

        return (request, receivedResult.RemoteEndPoint);
    }

    private async Task SendResponseAsync(Message response, IPEndPoint sourceEndPoint)
    {
        var responseBytes = response.ToReadonlySpan().ToArray();
        await _udpClient.SendAsync(responseBytes, responseBytes.Length, sourceEndPoint);
    }
}