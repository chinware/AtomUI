using Avalonia.Controls.Templates;

namespace AtomUI.Desktop.Controls;

public sealed class OtpLineEditSeparatorContext
{
    public OtpLineEditSeparatorContext(
        int cellIndex,
        int separatorIndex,
        object? content,
        IDataTemplate? contentTemplate)
    {
        CellIndex       = cellIndex;
        SeparatorIndex = separatorIndex;
        Content        = content;
        ContentTemplate = contentTemplate;
    }

    public int CellIndex { get; }

    public int SeparatorIndex { get; }

    public object? Content { get; }

    public IDataTemplate? ContentTemplate { get; }
}
