using codecrafters_dns_server.Dns.Server;

var forwarderAddress = args.Length >= 2 ? args[1] : null;

var server = new DnsServer(2053, forwarderAddress);
await server.StartAsync();