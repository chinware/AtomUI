using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Utils;
using Avalonia;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

internal class DialogCaptionButton : AvaloniaButton
{
    #region 公共属性定义

    public static readonly StyledProperty<Icon?> NormalIconProperty =
        AvaloniaProperty.Register<CaptionButton, Icon?>(nameof(NormalIcon));
    
    public static readonly StyledProperty<Icon?> CheckedIconProperty =
        AvaloniaProperty.Register<CaptionButton, Icon?>(nameof(CheckedIcon));
    
    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<CaptionButton, double>(nameof(IconWidth));

    public static readonly StyledProperty<double> IconHeightProperty = 
        AvaloniaProperty.Register<CaptionButton, double>(nameof(IconHeight));
    
    public static readonly StyledProperty<bool> IsCheckedProperty = 
        AvaloniaProperty.Register<CaptionButton, bool>(nameof(IsChecked), defaultBindingMode: BindingMode.TwoWay, defaultValue:false);
    
    public Icon? NormalIcon
    {
        get => GetValue(NormalIconProperty);
        set => SetValue(NormalIconProperty, value);
    }
    
    public Icon? CheckedIcon
    {
        get => GetValue(CheckedIconProperty);
        set => SetValue(CheckedIconProperty, value);
    }

    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public double IconHeight
    {
        get => GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }
    
    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    #endregion
    
    #region 内部属性定义
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DialogCaptionButton>();
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);
            }
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureTransitions(false);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Transitions = null;
    }

    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions = [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty)
                ];
            }
        }
        else
        {
            Transitions = null;
        }
    }
}