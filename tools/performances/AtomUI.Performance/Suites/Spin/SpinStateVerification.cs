using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using AtomSpin = AtomUI.Desktop.Controls.Spin;
using AtomTextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunSpinStateVerification()
    {
        var failures = new List<string>();
        VerifySpinLazyMaskLifecycle(failures);
        VerifySpinIndicatorAnimationLifecycle(failures);
        VerifySpinIndicatorDefaultAlignment(failures);
        VerifySpinIndicatorBuiltInDotColor(failures);
        VerifySpinIndicatorTemplateOnlyKeepsBuiltInDots(failures);
        VerifySpinCustomIndicatorSizeSync(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Spin state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Spin state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifySpinLazyMaskLifecycle(ICollection<string> failures)
    {
        var spin = new AtomSpin
        {
            Width        = 220,
            Height       = 120,
            IsSpinning   = false,
            IsTipVisible = true,
            Tip          = "Loading...",
            Content      = new Border
            {
                Width  = 220,
                Height = 120
            }
        };

        using var realized = RealizeControl(spin);
        var maskLayout = FindVisualByName<Panel>(spin, "MaskLayout");
        var indicator  = FindVisualByName<SpinIndicator>(spin, "Indicator");
        Expect(maskLayout != null,
            "Spin should keep its static MaskLayout template part materialized.",
            failures);
        Expect(maskLayout?.IsVisible == false,
            "Non-spinning Spin should hide MaskLayout.",
            failures);
        Expect(indicator != null,
            "Spin should keep its static SpinIndicator template part materialized.",
            failures);
        Expect(indicator?.IsVisible == false,
            "Non-spinning Spin should hide SpinIndicator.",
            failures);
        Expect(indicator == null || GetSpinIndicatorAnimation(indicator) == null,
            "Hidden static SpinIndicator should not build animation.",
            failures);

        spin.IsSpinning = true;
        RefreshLayout(realized.Window);

        maskLayout = FindVisualByName<Panel>(spin, "MaskLayout");
        indicator  = FindVisualByName<SpinIndicator>(spin, "Indicator");
        Expect(maskLayout?.IsVisible == true,
            "Spinning Spin should show MaskLayout.",
            failures);
        Expect(indicator?.IsVisible == true,
            "Spinning Spin should show SpinIndicator.",
            failures);
        Expect(indicator == null || FindVisualByName<Control>(indicator, "BuiltInIndicatorLayout") != null,
            "Visible static SpinIndicator should materialize the built-in dot layout from the template.",
            failures);
        Expect(indicator == null || GetSpinIndicatorAnimation(indicator) == null,
            "Visible static SpinIndicator should not drive runtime rotation through an Avalonia styled-property animation.",
            failures);
        Expect(indicator == null || GetSpinIndicatorAnimationStyle(indicator) == null,
            "Visible static SpinIndicator should not attach a UI-thread animation style.",
            failures);
        Expect(indicator == null || GetSpinIndicatorCancellationTokenSource(indicator) == null,
            "Visible static SpinIndicator should not use a dispatcher cancellation loop for runtime animation.",
            failures);
        Expect(FindVisualByName<AtomTextBlock>(spin, "Tip") != null,
            "Spin should keep Tip TextBlock available from the static template.",
            failures);

        var updatedIcon = new LoadingOutlined();
        spin.SizeType        = SizeType.Large;
        spin.Tip             = "Still loading...";
        spin.IsTipVisible    = false;
        spin.CustomIndicator = updatedIcon;
        RefreshLayout(realized.Window);

        Expect(indicator?.SizeType == SizeType.Large,
            "Static SpinIndicator should sync SizeType changes from Spin.",
            failures);
        Expect(ReferenceEquals(indicator?.CustomIndicator, updatedIcon),
            "Static SpinIndicator should sync CustomIndicator changes from Spin.",
            failures);
        var tip = FindVisualByName<AtomTextBlock>(spin, "Tip");
        Expect(tip?.Text == "Still loading...",
            "Static Spin tip should sync Tip text changes from Spin.",
            failures);
        Expect(tip?.IsVisible == false,
            "Static Spin tip should sync IsTipVisible changes from Spin.",
            failures);

        spin.IsSpinning = false;
        RefreshLayout(realized.Window);

        maskLayout = FindVisualByName<Panel>(spin, "MaskLayout");
        indicator  = FindVisualByName<SpinIndicator>(spin, "Indicator");
        Expect(maskLayout?.IsVisible == false,
            "Spin should hide MaskLayout when spinning stops.",
            failures);
        Expect(indicator?.IsVisible == false,
            "Spin should hide SpinIndicator when spinning stops.",
            failures);
        Expect(indicator == null || GetSpinIndicatorCancellationTokenSource(indicator) == null,
            "Hidden static SpinIndicator should stop animation when spinning stops.",
            failures);
        Expect(indicator == null || GetSpinIndicatorAnimationStyle(indicator) == null,
            "Hidden static SpinIndicator should not keep a UI-thread animation style when spinning stops.",
            failures);

        spin.IsSpinning = true;
        RefreshLayout(realized.Window);
        indicator = FindVisualByName<SpinIndicator>(spin, "Indicator");
        Expect(indicator?.IsVisible == true,
            "Spin should show SpinIndicator when spinning starts again.",
            failures);
        Expect(indicator == null || GetSpinIndicatorAnimationStyle(indicator) == null,
            "Spin should restart without reattaching a UI-thread animation style.",
            failures);
    }

    private static void VerifySpinIndicatorAnimationLifecycle(ICollection<string> failures)
    {
        var indicator = new SpinIndicator
        {
            IsVisible = false
        };

        using var realized = RealizeControl(indicator);
        Expect(GetSpinIndicatorAnimation(indicator) == null,
            "Invisible SpinIndicator should not build animation during attach.",
            failures);

        indicator.IsVisible = true;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<Control>(indicator, "BuiltInIndicatorLayout") != null,
            "Visible SpinIndicator should use template-owned built-in dot visuals.",
            failures);
        Expect(GetSpinIndicatorAnimation(indicator) == null,
            "Visible SpinIndicator should not build an Avalonia styled-property animation.",
            failures);
        Expect(GetSpinIndicatorAnimationStyle(indicator) == null,
            "Visible SpinIndicator should not attach an animation style.",
            failures);
        Expect(GetSpinIndicatorCancellationTokenSource(indicator) == null,
            "Visible SpinIndicator should not keep a cancellation token source for runtime animation.",
            failures);
        Expect(!HasAbstractSpinIndicatorMember(indicator, "IndicatorAngle"),
            "SpinIndicator should not keep an IndicatorAngle property that invalidates Render every frame.",
            failures);

        indicator.MotionDuration = TimeSpan.FromMilliseconds(1234);
        RefreshLayout(realized.Window);
        Expect(GetSpinIndicatorAnimation(indicator) == null,
            "Changing MotionDuration should not materialize a UI-thread styled-property animation.",
            failures);
        Expect(GetSpinIndicatorAnimationStyle(indicator) == null,
            "Changing MotionDuration should keep compositor animation off the control Styles collection.",
            failures);

        indicator.IsVisible = false;
        RefreshLayout(realized.Window);
        Expect(GetSpinIndicatorCancellationTokenSource(indicator) == null,
            "Invisible SpinIndicator should stop animation.",
            failures);
        Expect(GetSpinIndicatorAnimationStyle(indicator) == null,
            "Invisible SpinIndicator should detach animation style.",
            failures);
    }

    private static void VerifySpinIndicatorDefaultAlignment(ICollection<string> failures)
    {
        var indicator = new SpinIndicator();
        using var realized = RealizeControl(indicator);

        Expect(indicator.HorizontalAlignment == HorizontalAlignment.Left,
            $"Standalone SpinIndicator should default to left alignment, actual {indicator.HorizontalAlignment}.",
            failures);
        Expect(indicator.VerticalAlignment == VerticalAlignment.Top,
            $"Standalone SpinIndicator should default to top alignment, actual {indicator.VerticalAlignment}.",
            failures);

        var spin = new AtomSpin
        {
            Width      = 120,
            Height     = 80,
            IsSpinning = true,
            Content    = new Border { Width = 120, Height = 80 }
        };
        using var spinRealized = RealizeControl(spin);
        var nestedIndicator = FindVisualByName<SpinIndicator>(spin, "Indicator");
        Expect(nestedIndicator?.HorizontalAlignment == HorizontalAlignment.Center,
            $"Spin template should keep its nested SpinIndicator centered, actual {nestedIndicator?.HorizontalAlignment}.",
            failures);
    }

    private static void VerifySpinCustomIndicatorSizeSync(ICollection<string> failures)
    {
        var indicator = new SpinIndicator
        {
            SizeType         = SizeType.Large,
            CustomIndicator = new LoadingOutlined()
        };

        using var realized = RealizeControl(indicator);
        var icon = indicator.GetSelfAndVisualDescendants().OfType<LoadingOutlined>().FirstOrDefault();
        Expect(icon != null,
            "Custom SpinIndicator should create the custom LoadingOutlined icon.",
            failures);
        if (icon == null)
        {
            return;
        }

        var largeWidth = icon.Width;
        indicator.SizeType = SizeType.Small;
        RefreshLayout(realized.Window);
        Expect(icon.Width < largeWidth && icon.Height < largeWidth,
            $"Custom SpinIndicator icon should shrink on SizeType change, large {largeWidth}, actual {icon.Width}x{icon.Height}.",
            failures);

        var presenter = GetSpinIndicatorCustomIndicatorPresenter(indicator);
        var builtInLayout = FindVisualByName<Control>(indicator, "BuiltInIndicatorLayout");
        Expect(presenter != null,
            "Custom SpinIndicator should keep the custom indicator presenter from the static template.",
            failures);
        Expect(presenter?.IsVisible == true,
            "Custom SpinIndicator should show the custom indicator presenter.",
            failures);
        Expect(builtInLayout?.IsVisible == false,
            "Custom SpinIndicator should hide the built-in dot layout.",
            failures);
        Expect(presenter?.RenderTransform == null,
            "Custom SpinIndicator should not update a RenderTransform from UI-thread angle changes.",
            failures);

        indicator.IsVisible = false;
        RefreshLayout(realized.Window);
        Expect(presenter?.RenderTransform == null,
            "Stopped custom SpinIndicator should still avoid UI-thread RenderTransform angle state.",
            failures);

        realized.Dispose();
        Expect(GetSpinIndicatorCustomIndicatorPresenter(indicator) == null,
            "Detached SpinIndicator should release custom indicator presenter reference.",
            failures);
    }

    private static void VerifySpinIndicatorBuiltInDotColor(ICollection<string> failures)
    {
        var indicator = new SpinIndicator();

        using var realized = RealizeControl(indicator);
        var dots = indicator.GetSelfAndVisualDescendants().OfType<Ellipse>().ToArray();
        Expect(dots.Length == 4,
            $"Built-in SpinIndicator should materialize 4 dots, actual {dots.Length}.",
            failures);

        var expectedColor = Color.Parse("#1677ff");
        for (var i = 0; i < dots.Length; i++)
        {
            var fill = dots[i].Fill as ISolidColorBrush;
            Expect(fill?.Color == expectedColor,
                $"Built-in SpinIndicator dot {i} should use ColorPrimary {expectedColor}, actual {DescribeBrush(dots[i].Fill)}.",
                failures);
            Expect(Math.Abs(dots[i].Opacity - 0.3) < 0.001,
                $"Built-in SpinIndicator dot {i} should start at opacity 0.3 before compositor animation, actual {dots[i].Opacity:0.###}.",
                failures);
        }

        var overriddenIndicator = new SpinIndicator();
        var overrideColor = Color.Parse("#ff4d4f");
        overriddenIndicator.Resources[SharedTokenKind.ColorPrimary] = Brush(overrideColor);

        using var overriddenRealized = RealizeControl(overriddenIndicator);
        var overriddenDots = overriddenIndicator.GetSelfAndVisualDescendants().OfType<Ellipse>().ToArray();
        for (var i = 0; i < overriddenDots.Length; i++)
        {
            var fill = overriddenDots[i].Fill as ISolidColorBrush;
            Expect(fill?.Color == overrideColor,
                $"Built-in SpinIndicator dot {i} should follow local ColorPrimary override {overrideColor}, actual {DescribeBrush(overriddenDots[i].Fill)}.",
                failures);
        }

        var explicitBrushIndicator = new SpinIndicator();
        var explicitColor          = Color.Parse("#52c41a");
        Expect(SetSpinIndicatorDotBgBrush(explicitBrushIndicator, Brush(explicitColor)),
            "SpinIndicator should keep the internal DotBgBrush theme contract for built-in dot color.",
            failures);

        using var explicitBrushRealized = RealizeControl(explicitBrushIndicator);
        var explicitBrushDots = explicitBrushIndicator.GetSelfAndVisualDescendants().OfType<Ellipse>().ToArray();
        for (var i = 0; i < explicitBrushDots.Length; i++)
        {
            var fill = explicitBrushDots[i].Fill as ISolidColorBrush;
            Expect(fill?.Color == explicitColor,
                $"Built-in SpinIndicator dot {i} should follow DotBgBrush {explicitColor}, actual {DescribeBrush(explicitBrushDots[i].Fill)}.",
                failures);
        }

        var runtimeColor = Color.Parse("#722ed1");
        Expect(SetSpinIndicatorDotBgBrush(explicitBrushIndicator, Brush(runtimeColor)),
            "SpinIndicator should accept runtime DotBgBrush updates.",
            failures);
        RefreshLayout(explicitBrushRealized.Window);
        for (var i = 0; i < explicitBrushDots.Length; i++)
        {
            var fill = explicitBrushDots[i].Fill as ISolidColorBrush;
            Expect(fill?.Color == runtimeColor,
                $"Built-in SpinIndicator dot {i} should update when DotBgBrush changes at runtime to {runtimeColor}, actual {DescribeBrush(explicitBrushDots[i].Fill)}.",
                failures);
        }
    }

    private static void VerifySpinIndicatorTemplateOnlyKeepsBuiltInDots(ICollection<string> failures)
    {
        var indicator = new SpinIndicator
        {
            CustomIndicatorTemplate = new FuncDataTemplate<object>((_, _) => new Border())
        };

        using var realized = RealizeControl(indicator);
        var builtInLayout = FindVisualByName<Control>(indicator, "BuiltInIndicatorLayout");
        var presenter = GetSpinIndicatorCustomIndicatorPresenter(indicator);
        Expect(builtInLayout?.IsVisible == true,
            "SpinIndicator should keep the built-in dot layout visible when only CustomIndicatorTemplate is set.",
            failures);
        Expect(presenter?.IsVisible == false,
            "SpinIndicator should not switch to an empty custom indicator when CustomIndicator is null.",
            failures);
    }

    private static object? GetSpinIndicatorAnimation(SpinIndicator indicator)
    {
        return GetPrivateField(indicator, "AtomUI.Controls.Commons.AbstractSpinIndicator", "_animation");
    }

    private static object? GetSpinIndicatorCancellationTokenSource(SpinIndicator indicator)
    {
        return GetPrivateField(indicator, "AtomUI.Controls.Commons.AbstractSpinIndicator", "_cancellationTokenSource");
    }

    private static object? GetSpinIndicatorAnimationStyle(SpinIndicator indicator)
    {
        return GetPrivateField(indicator, "AtomUI.Controls.Commons.AbstractSpinIndicator", "_animationStyle");
    }

    private static ContentPresenter? GetSpinIndicatorCustomIndicatorPresenter(SpinIndicator indicator)
    {
        return GetPrivateField(indicator,
            "AtomUI.Controls.Commons.AbstractSpinIndicator",
            "_customIndicatorPresenter") as ContentPresenter;
    }

    private static bool HasAbstractSpinIndicatorMember(SpinIndicator indicator, string memberName)
    {
        var type = indicator.GetType();
        while (type is not null)
        {
            if (type.FullName == "AtomUI.Controls.Commons.AbstractSpinIndicator")
            {
                const System.Reflection.BindingFlags flags =
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Static |
                    System.Reflection.BindingFlags.NonPublic;
                return type.GetField(memberName, flags) is not null ||
                       type.GetProperty(memberName, flags) is not null ||
                       type.GetField($"{memberName}Property", flags) is not null;
            }

            type = type.BaseType;
        }

        return false;
    }

    private static bool SetSpinIndicatorDotBgBrush(SpinIndicator indicator, IBrush brush)
    {
        var type = indicator.GetType();
        while (type is not null)
        {
            if (type.FullName == "AtomUI.Controls.Commons.AbstractSpinIndicator")
            {
                var property = type.GetField(
                    "DotBgBrushProperty",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                if (property?.GetValue(null) is not Avalonia.AvaloniaProperty avaloniaProperty)
                {
                    return false;
                }

                indicator.SetValue(avaloniaProperty, brush);
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }

}
