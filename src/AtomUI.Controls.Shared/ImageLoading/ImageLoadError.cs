namespace AtomUI.Controls;

public sealed record ImageLoadError(
    ImageLoadErrorCode Code,
    string Message,
    int? HttpStatus = null,
    string? SourceDisplayName = null,
    Exception? Exception = null);
