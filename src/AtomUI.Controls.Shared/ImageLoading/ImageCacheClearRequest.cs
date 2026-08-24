namespace AtomUI.Controls;

public sealed record ImageCacheClearRequest
{
    public string? CachePartition { get; init; }

    public bool ClearDecodedMemory { get; init; } = true;

    public bool ClearEncodedMemory { get; init; } = true;

    public bool ClearPersistent { get; init; } = true;

    public bool CancelInFlight { get; init; } = true;
}
