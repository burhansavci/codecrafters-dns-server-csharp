namespace codecrafters_dns_server.Dns;

public enum Type
{
    /// <summary>
    /// a host address
    /// </summary>
    A = 1,

    /// <summary>
    /// an authoritative name server
    /// </summary>
    NS = 2,

    /// <summary>
    /// a mail destination (Obsolete - use MX)
    /// </summary>
    MD = 3,

    /// <summary>
    /// a mail forwarder (Obsolete - use MX)
    /// </summary>
    MF = 4,

    /// <summary>
    /// the canonical name for an alias
    /// </summary>
    CNAME = 5,

    /// <summary>
    /// marks the start of a zone of authority
    /// </summary>
    SOA = 6,

    /// <summary>
    /// a mailbox domain name (EXPERIMENTAL)
    /// </summary>
    MB = 7,

    /// <summary>
    /// a mail group member (EXPERIMENTAL)
    /// </summary>
    MG = 8,

    /// <summary>
    /// a mail rename domain name (EXPERIMENTAL)
    /// </summary>
    MR = 9,

    /// <summary>
    ///  10 a null RR (EXPERIMENTAL)
    /// </summary>
    NULL = 10,

    /// <summary>
    /// a well known service description
    /// </summary>
    WKS = 11,

    /// <summary>
    /// a domain name pointer
    /// </summary>
    PTR = 12,

    /// <summary>
    /// host information
    /// </summary>
    HINFO = 13,

    /// <summary>
    /// mailbox or mail list information
    /// </summary>
    MINFO = 14,

    /// <summary>
    /// mail exchange
    /// </summary>
    MX = 15,

    /// <summary>
    /// text strings
    /// </summary>
    TXT = 16
}