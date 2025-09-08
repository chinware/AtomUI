using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace AtomUI.Controls;

public interface IDialogHost : IDisposable, IFocusScope
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
    event EventHandler<TemplateAppliedEventArgs>? TemplateApplied;
    void ConfigurePosition(DialogPositionRequest positionRequest);
    void SetChild(Control? control);
    void Show();
    void Hide();
    void TakeFocus();
}