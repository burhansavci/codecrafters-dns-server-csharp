namespace codecrafters_dns_server.Dns;

public enum Class
{
    /// <summary>
    /// the Internet
    /// </summary>
    IN = 1,

    /// <summary>
    /// the CSNET class (Obsolete - used only for examples in some obsolete RFCs)
    /// </summary>
    CS = 2,

    /// <summary>
    /// the CHAOS class
    /// </summary>
    CH = 3,

    /// <summary>
    /// Hesiod [Dyer 87]
    /// </summary>
    HS = 4
}