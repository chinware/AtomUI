using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;

namespace AtomUI.Controls;

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
    Visual? HostedVisualTreeRoot { get; }
    object? Content { get; }
    IDataTemplate? ContentTemplate { get; }
    event EventHandler<TemplateAppliedEventArgs>? TemplateApplied;
    void ConfigurePosition(DialogPositionRequest positionRequest);
    void SetChild(Control? control);
    void Show();
    void Close(Action? callback = null);
}