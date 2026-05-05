using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

public interface IDialogHost : IFocusScope
{
    double Width { get; set; }
    double MinWidth { get; set; }
    double MaxWidth { get; set; }
    double Height { get; set; }
    double MinHeight { get; set; }
    double MaxHeight { get; set; }
    bool Topmost { get; set; }
    ContentPresenter? Presenter { get; }
    Transform? Transform { get; set; }
    object? Content { get; }
    IDataTemplate? ContentTemplate { get; }
    void SetChild(Control? control);
    void Show();
    void Close(Action? callback = null);
}