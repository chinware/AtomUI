using System.Reactive.Disposables;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.IconPkg;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class MessageBoxDialog : Dialog
{
    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<MessageBoxDialog, Icon?>(nameof (Icon));
    
    public static readonly StyledProperty<MessageBoxStyle> StyleProperty =
        MessageBox.StyleProperty.AddOwner<MessageBoxDialog>();
    
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public MessageBoxStyle Style
    {
        get => GetValue(StyleProperty);
        set => SetValue(StyleProperty, value);
    }
    
    private protected override DialogHost CreateDialogHost(TopLevel topLevel, Dialog dialog)
    {
        return new MessageBoxDialogHost(topLevel, this);
    }
    
    private protected override OverlayDialogHost CreateOverlayDialogHost(DialogLayer dialogLayer, Dialog dialog)
    {
        return new MessageBoxOverlayDialogHost(dialogLayer, this);
    }

    private protected override void RelayDialogHostBindings(CompositeDisposable disposables, DialogHost dialogHost)
    {
        base.RelayDialogHostBindings(disposables, dialogHost);
        disposables.Add(BindUtils.RelayBind(this, IconProperty, dialogHost, MessageBoxDialogHost.StyleIconProperty));
        disposables.Add(BindUtils.RelayBind(this, StyleProperty, dialogHost, MessageBoxDialogHost.StyleProperty));
    }

    private protected override void RelayOverlayDialogBindings(CompositeDisposable disposables,
                                                               OverlayDialogHost dialogHost)
    {
        base.RelayOverlayDialogBindings(disposables, dialogHost);
        disposables.Add(BindUtils.RelayBind(this, IconProperty, dialogHost, MessageBoxOverlayDialogHost.StyleIconProperty));
        disposables.Add(BindUtils.RelayBind(this, StyleProperty, dialogHost, MessageBoxOverlayDialogHost.StyleProperty));
    }
}