namespace AtomUI.Controls.Commons;

internal sealed class CheckableTagOptionItem
{
    public CheckableTagOptionItem(object source, object value, object? content)
    {
        Source  = source;
        Value   = value;
        Content = content;
    }

    public object Source { get; }

    public object Value { get; }

    public object? Content { get; }
}
