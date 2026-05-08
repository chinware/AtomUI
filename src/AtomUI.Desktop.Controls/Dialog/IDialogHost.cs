using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using System.Reactive.Disposables;

namespace AtomUI.Desktop.Controls;

public interface IDialogHost : IFocusScope
{
    bool Topmost { get; set; }
    object? Content { get; set; }
    IDataTemplate? ContentTemplate { get; set; }
    AvaloniaList<DialogButton> CustomButtons { get; }
    event EventHandler<TemplateAppliedEventArgs>? TemplateApplied;
    void BindDialog(Dialog dialog, CompositeDisposable disposables);
    void AttachPlacement(Control placementTarget);
    void UpdateSizing();
    void UpdatePlacement();
    void Show();
    void Close(Action? callback = null);
}
