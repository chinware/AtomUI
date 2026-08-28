using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// 星号等分隔符字符的字形墨迹绘制在字体行盒上半区、且墨迹相对字符 advance 偏左，
/// 这是字体设计决定的排版事实；Avalonia 公共 API 无法取得字形墨迹边界做自动居中。
/// 该行为以设计 token 提供的"相对字号的光学补偿比例"生成 RenderTransform 平移，
/// 随 FontSize 自动缩放，主题与使用者均可按字体覆盖。
/// </summary>
public class OtpSeparatorGlyphAlignment
{
    private OtpSeparatorGlyphAlignment()
    {
    }

    public static readonly AttachedProperty<double> XProperty =
        AvaloniaProperty.RegisterAttached<OtpSeparatorGlyphAlignment, TemplatedControl, double>("X");

    public static readonly AttachedProperty<double> YProperty =
        AvaloniaProperty.RegisterAttached<OtpSeparatorGlyphAlignment, TemplatedControl, double>("Y");

    private static readonly AttachedProperty<bool> SubscribedProperty =
        AvaloniaProperty.RegisterAttached<OtpSeparatorGlyphAlignment, TemplatedControl, bool>("Subscribed");

    static OtpSeparatorGlyphAlignment()
    {
        // XAML 对 attached 属性直接 SetValue（不经过 SetX/SetY），必须走属性系统回调
        XProperty.Changed.AddClassHandler<TemplatedControl>((control, _) =>
        {
            EnsureSubscribed(control);
            Apply(control);
        });
        YProperty.Changed.AddClassHandler<TemplatedControl>((control, _) =>
        {
            EnsureSubscribed(control);
            Apply(control);
        });
    }

    public static double GetX(TemplatedControl control)
    {
        return control.GetValue(XProperty);
    }

    public static void SetX(TemplatedControl control, double value)
    {
        control.SetValue(XProperty, value);
        EnsureSubscribed(control);
        Apply(control);
    }

    public static double GetY(TemplatedControl control)
    {
        return control.GetValue(YProperty);
    }

    public static void SetY(TemplatedControl control, double value)
    {
        control.SetValue(YProperty, value);
        EnsureSubscribed(control);
        Apply(control);
    }

    private static void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Sender is TemplatedControl control
            && change.Property != SubscribedProperty)
        {
            EnsureSubscribed(control);
            Apply(control);
        }
    }

    private static void EnsureSubscribed(TemplatedControl control)
    {
        if (!control.GetValue(SubscribedProperty))
        {
            control.SetValue(SubscribedProperty, true);
            control.PropertyChanged += OnControlPropertyChanged;
        }
    }

    private static void OnControlPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == TemplatedControl.FontSizeProperty && sender is TemplatedControl control)
        {
            Apply(control);
        }
    }

    private static void Apply(TemplatedControl control)
    {
        var offsetX = control.GetValue(XProperty) * control.FontSize;
        var offsetY = control.GetValue(YProperty) * control.FontSize;

        if (offsetX == 0 && offsetY == 0)
        {
            control.RenderTransform = null;
            return;
        }

        var existing = control.RenderTransform as TranslateTransform;
        if (existing is null)
        {
            existing = new TranslateTransform();
            control.RenderTransform = existing;
        }

        existing.X = offsetX;
        existing.Y = offsetY;
    }
}
