using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public class SubmitButton : Button
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsWatchValidateResultProperty =
        AvaloniaProperty.Register<SubmitButton, bool>(nameof(IsWatchValidateResult), false);
    
    public bool IsWatchValidateResult
    {
        get => GetValue(IsWatchValidateResultProperty);
        set => SetValue(IsWatchValidateResultProperty, value);
    }
    #endregion
    
    #region 公共事件定义

    public static readonly RoutedEvent<RoutedEventArgs> SubmitEvent =
        RoutedEvent.Register<SubmitButton, RoutedEventArgs>(nameof(Submit), RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs>? Submit
    {
        add => AddHandler(SubmitEvent, value);
        remove => RemoveHandler(SubmitEvent, value);
    }
    #endregion
    
    static SubmitButton()
    {
        Form.IsFormValidProperty.Changed.AddClassHandler<SubmitButton>((button, args) => button.HandleFormValidChanged(args.GetNewValue<bool>()));
    }

    protected override void OnClick()
    {
        base.OnClick();
        RaiseEvent(new RoutedEventArgs(SubmitEvent));
    }

    private void HandleFormValidChanged(bool newValue)
    {
        if (IsWatchValidateResult)
        {
            SetValue(IsEnabledProperty, newValue);
        }
        else
        {
            SetValue(IsEnabledProperty, false);
        }
    }
}