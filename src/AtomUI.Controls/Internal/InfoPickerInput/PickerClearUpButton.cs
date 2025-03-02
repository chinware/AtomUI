using System.Reactive.Disposables;
using AtomUI.IconPkg;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

internal class PickerClearUpButton : TemplatedControl,
                                     ITokenResourceConsumer
{
    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;
    private CompositeDisposable? _tokenBindingsDisposable;
    
    public event EventHandler? ClearRequest;

    public static readonly StyledProperty<bool> IsInClearModeProperty =
        AvaloniaProperty.Register<PickerClearUpButton, bool>(nameof(IsInClearMode));
    
    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<PickerClearUpButton, Icon?>(nameof(Icon));

    public bool IsInClearMode
    {
        get => GetValue(IsInClearModeProperty);
        set => SetValue(IsInClearModeProperty, value);
    }
    
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    private IconButton? _clearButton;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _clearButton = e.NameScope.Get<IconButton>(PickerClearUpButtonTheme.ClearButtonPart);
        if (_clearButton is not null)
        {
            _clearButton.Click += (sender, args) => { ClearRequest?.Invoke(this, EventArgs.Empty); };
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingsDisposable = new CompositeDisposable();
    }
    
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
}