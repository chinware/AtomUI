namespace AtomUI.Desktop.Controls;

public sealed class DataGridLocalSourceOptions
{
    public int PreferredRangeSize { get; init; } = 128;

    public int MaximumRangeSize { get; init; } = 512;

    public int MaximumProjectionCount { get; init; } = 8;

    internal void Validate()
    {
        if (MaximumRangeSize is < 32 or > 4096)
        {
            throw new ArgumentOutOfRangeException(nameof(MaximumRangeSize));
        }
        if (PreferredRangeSize < 32 || PreferredRangeSize > MaximumRangeSize)
        {
            throw new ArgumentOutOfRangeException(nameof(PreferredRangeSize));
        }
        if (MaximumProjectionCount is < 1 or > 64)
        {
            throw new ArgumentOutOfRangeException(nameof(MaximumProjectionCount));
        }
    }
}
