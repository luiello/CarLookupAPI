using System;

namespace CarLookup.Contracts.Responses;

/// <summary>
/// Response metadata
/// </summary>
public class ResponseMeta
{
    /// <summary>
    /// Response timestamp in UTC
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Request correlation ID
    /// </summary>
    public string RequestId { get; set; } = string.Empty;
}