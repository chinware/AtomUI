namespace AtomUI.Controls;

public interface ICheckableTagOption
{
    object Value { get; }

    object? Content { get; }
}

public class CheckableTagOption : ICheckableTagOption
{
    public object Value { get; set; } = null!;

    public object? Content { get; set; }
}
