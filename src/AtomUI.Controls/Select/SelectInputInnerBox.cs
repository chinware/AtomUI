using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AtomUI.Controls;

internal class SelectInputInnerBox : AddOnDecoratedInnerBox
{
    public static readonly StyledProperty<SelectMode> ModeProperty =
        AvaloniaProperty.Register<SelectInputInnerBox, SelectMode>(nameof(Mode));
    
    public SelectMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    #region 内部属性定义

    internal static readonly DirectProperty<SelectInputInnerBox, bool> IsSearchEnabledProperty =
        AvaloniaProperty.RegisterDirect<SelectInputInnerBox, bool>(nameof(IsSearchEnabled),
            o => o.IsSearchEnabled,
            (o, v) => o.IsSearchEnabled = v);
    
    private bool _isSearchEnabled;

    internal bool IsSearchEnabled
    {
        get => _isSearchEnabled;
        set => SetAndRaise(IsSearchEnabledProperty, ref _isSearchEnabled, value);
    }
    
    #endregion
    
    internal SelectInput? OwnerInput;
    private SelectHandle? _selectHandle;

    protected override void NotifyClearButtonClicked()
    {
        OwnerInput?.Clear();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ModeProperty)
        {
            ConfigureEffectiveInnerBoxPadding();
        }
    }
    
    protected override void ConfigureEffectiveInnerBoxPadding()
    {
        if (Mode == SelectMode.Multiple)
        {
            SetCurrentValue(EffectiveInnerBoxPaddingProperty, new Thickness(InnerBoxPadding.Left / 2, InnerBoxPadding.Top, InnerBoxPadding.Right, InnerBoxPadding.Bottom));
        }
        else
        {
            SetCurrentValue(EffectiveInnerBoxPaddingProperty, InnerBoxPadding);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _selectHandle = e.NameScope.Find<SelectHandle>(SelectInputThemeConstants.HandlePart);
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        if (_selectHandle != null)
        {
            _selectHandle.IsInputHover = true;
        }
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        if (_selectHandle != null)
        {
            _selectHandle.IsInputHover = false;
        }
    }
}