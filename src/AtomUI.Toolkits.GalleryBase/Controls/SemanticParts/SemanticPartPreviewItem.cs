using System.ComponentModel;
using System.Runtime.CompilerServices;
using AtomUI.Theme.Schema;

namespace AtomUI.Toolkits.GalleryBase.Controls;

internal sealed class SemanticPartPreviewItem : INotifyPropertyChanged
{
    private bool _isPinned;

    public SemanticPartPreviewItem(
        SemanticPartDescriptor descriptor,
        string description,
        string? codeSnippet)
    {
        Descriptor      = descriptor;
        Description     = description;
        CodeSnippet     = codeSnippet;
        Name            = descriptor.Name;
        Path            = descriptor.Path;
        Selector        = descriptor.SelectorClass is null ? "root" : $".{descriptor.SelectorClass}";
        SelectorRoute   = descriptor.SelectorRoute ?? "root";
        ContractType    = descriptor.ContractType.Name;
        StyleType       = descriptor.StyleType?.FullName ?? "-";
        Cardinality     = descriptor.Cardinality.ToString();
        Customization   = descriptor.Customization.ToString();
        Since           = descriptor.Since;
        IsCrossRoot     = descriptor.CrossVisualRoot;
        IsRuntimeCreated = descriptor.RuntimeCreated;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal SemanticPartDescriptor Descriptor { get; }

    internal string? CodeSnippet { get; }

    public string Name { get; }

    public string Path { get; }

    public string Selector { get; }

    public string SelectorRoute { get; }

    public string ContractType { get; }

    public string StyleType { get; }

    public string Cardinality { get; }

    public string Customization { get; }

    public string? Since { get; }

    public string Description { get; }

    public bool IsCrossRoot { get; }

    public bool IsRuntimeCreated { get; }

    public bool HasSince => !string.IsNullOrWhiteSpace(Since);

    public bool IsPinned
    {
        get => _isPinned;
        internal set => SetField(ref _isPinned, value);
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
