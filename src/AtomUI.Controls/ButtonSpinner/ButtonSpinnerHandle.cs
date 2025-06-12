using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class ButtonSpinnerHandle : TemplatedControl
{
    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ButtonSpinnerHandle>();
    
    public static readonly StyledProperty<Location> ButtonSpinnerLocationProperty =
        ButtonSpinner.ButtonSpinnerLocationProperty.AddOwner<ButtonSpinnerHandle>();
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public Location ButtonSpinnerLocation
    {
        get => GetValue(ButtonSpinnerLocationProperty);
        set => SetValue(ButtonSpinnerLocationProperty, value);
    }
    
    public IconButton? IncreaseButton { get; private set; }
    public IconButton? DecreaseButton { get; private set; }
    
    public event EventHandler? ButtonsCreated;
    
    private Border? _spinnerHandleDecorator;
    
    private void SetupSpinnerHandleCornerRadius()
    {
        if (_spinnerHandleDecorator is not null)
        {
            if (ButtonSpinnerLocation == Location.Left)
            {
                _spinnerHandleDecorator.CornerRadius = new CornerRadius(CornerRadius.TopLeft,
                    0,
                    0,
                    CornerRadius.BottomLeft);
            }
            else
            {
                _spinnerHandleDecorator.CornerRadius = new CornerRadius(0,
                    CornerRadius.TopRight,
                    CornerRadius.BottomRight,
                    0);
            }
        }
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CornerRadiusProperty || change.Property == ButtonSpinnerLocationProperty)
        {
            SetupSpinnerHandleCornerRadius();
        }
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _spinnerHandleDecorator = e.NameScope.Find<Border>(ButtonSpinnerThemeConstants.SpinnerHandleDecoratorPart);
        IncreaseButton          = e.NameScope.Find<IconButton>(ButtonSpinnerThemeConstants.IncreaseButtonPart);
        DecreaseButton          = e.NameScope.Find<IconButton>(ButtonSpinnerThemeConstants.DecreaseButtonPart);
        ButtonsCreated?.Invoke(this, EventArgs.Empty);
        SetupSpinnerHandleCornerRadius();
    }
}