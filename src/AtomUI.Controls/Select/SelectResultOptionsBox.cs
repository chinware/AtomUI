using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AtomUI.Controls;

internal class SelectResultOptionsBox : TemplatedControl
{
    public static readonly DirectProperty<SelectResultOptionsBox, IList<SelectOption>?> SelectedOptionsProperty =
        AvaloniaProperty.RegisterDirect<SelectResultOptionsBox, IList<SelectOption>?>(
            nameof(SelectedOptions),
            o => o.SelectedOptions,
            (o, v) => o.SelectedOptions = v);
    
    public static readonly StyledProperty<SelectMode> ModeProperty =
        AvaloniaProperty.Register<SelectResultOptionsBox, SelectMode>(nameof(Mode));
    
    public static readonly StyledProperty<bool> IsSearchEnabledProperty =
        Select.IsSearchEnabledProperty.AddOwner<SelectResultOptionsBox>();
    
    private IList<SelectOption>? _selectedOptions;

    public IList<SelectOption>? SelectedOptions
    {
        get => _selectedOptions;
        set => SetAndRaise(SelectedOptionsProperty, ref _selectedOptions, value);
    }
    
    public SelectMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }
    
    public bool IsSearchEnabled
    {
        get => GetValue(IsSearchEnabledProperty);
        set => SetValue(IsSearchEnabledProperty, value);
    }

    private WrapPanel? _defaultPanel;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedOptionsProperty)
        {
            HandleSelectedOptionsChanged();
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        e.Handled = false;
    }
    
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        e.Handled = false;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _defaultPanel = e.NameScope.Find<WrapPanel>(SelectResultOptionsBoxThemeConstants.DefaultPanelPart);
    }

    private void HandleSelectedOptionsChanged()
    {
        if (_defaultPanel != null)
        {
            _defaultPanel.Children.Clear();
            if (_selectedOptions != null)
            {
                foreach (var option in _selectedOptions)
                {
                    _defaultPanel.Children.Add(new Tag()
                    {
                        TagText = option.Header,
                        Bordered = false
                    });
                }
            }
        }
    }
}