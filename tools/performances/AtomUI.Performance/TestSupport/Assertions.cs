using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static void Expect(bool condition, string message, ICollection<string> failures)
    {
        if (!condition)
        {
            failures.Add(message);
        }
    }

    private static T? FindVisualByName<T>(Control root, string name)
        where T : Control
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<T>()
                   .FirstOrDefault(control => control.Name == name);
    }

    private static bool IsZeroCornerRadius(CornerRadius cornerRadius)
    {
        return cornerRadius.TopLeft == 0 &&
               cornerRadius.TopRight == 0 &&
               cornerRadius.BottomLeft == 0 &&
               cornerRadius.BottomRight == 0;
    }

    private static void ExpectBrush(IBrush? actual, IBrush? expected, string label, ICollection<string> failures)
    {
        if (BrushEquals(actual, expected))
        {
            return;
        }

        failures.Add($"{label}: expected {DescribeBrush(expected)}, actual {DescribeBrush(actual)}.");
    }

    private static bool BrushEquals(IBrush? actual, IBrush? expected)
    {
        if (ReferenceEquals(actual, expected))
        {
            return true;
        }
        if (actual is ISolidColorBrush actualSolid && expected is ISolidColorBrush expectedSolid)
        {
            return actualSolid.Color == expectedSolid.Color &&
                   Math.Abs(actualSolid.Opacity - expectedSolid.Opacity) < 0.001;
        }
        return Equals(actual, expected);
    }

    private static string DescribeBrush(IBrush? brush)
    {
        return brush switch
        {
            null => "<null>",
            ISolidColorBrush solid => $"{solid.Color} opacity {solid.Opacity:0.###}",
            _ => brush.ToString() ?? brush.GetType().Name
        };
    }

    private static void ResetEffectiveBrushState(AddOnDecoratedBox box, InputControlStyleVariant styleVariant)
    {
        box.SetCurrentValue(InputElement.IsEnabledProperty, true);
        box.SetCurrentValue(AddOnDecoratedBox.StyleVariantProperty, styleVariant);
        box.SetCurrentValue(AddOnDecoratedBox.StatusProperty, InputControlStatus.Default);
        box.IsInnerBoxHover   = false;
        box.IsInnerBoxPressed = false;
    }

    private static EffectiveBrushSet ApplyTestBrushes(AddOnDecoratedBox box)
    {
        var brushes = new EffectiveBrushSet();
        box.InnerBoxDefaultBorderBrush        = brushes.DefaultBorder;
        box.InnerBoxHoverBorderBrush          = brushes.HoverBorder;
        box.InnerBoxActiveBorderBrush         = brushes.ActiveBorder;
        box.InnerBoxFilledBackground          = brushes.FilledBackground;
        box.InnerBoxFilledBorderBrush         = brushes.FilledBorder;
        box.InnerBoxFilledHoverBackground     = brushes.FilledHoverBackground;
        box.InnerBoxActiveBackground          = brushes.ActiveBackground;
        box.InnerBoxDisabledBackground        = brushes.DisabledBackground;
        box.InnerBoxErrorBorderBrush          = brushes.ErrorBorder;
        box.InnerBoxErrorHoverBorderBrush     = brushes.ErrorHoverBorder;
        box.InnerBoxErrorBackground           = brushes.ErrorBackground;
        box.InnerBoxErrorFilledBorderBrush    = brushes.ErrorFilledBorder;
        box.InnerBoxErrorHoverBackground      = brushes.ErrorHoverBackground;
        box.InnerBoxWarningBorderBrush        = brushes.WarningBorder;
        box.InnerBoxWarningHoverBorderBrush   = brushes.WarningHoverBorder;
        box.InnerBoxWarningBackground         = brushes.WarningBackground;
        box.InnerBoxWarningFilledBorderBrush  = brushes.WarningFilledBorder;
        box.InnerBoxWarningHoverBackground    = brushes.WarningHoverBackground;
        return brushes;
    }

    private static IBrush Brush(Color color)
    {
        return new SolidColorBrush(color);
    }
}
