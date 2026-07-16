using AtomUI.Theme.TokenSystem;
using AtomUI.Theme;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Styling;

namespace AtomUI.Controls.DesignTokens
{
    public enum IconTokenKind
    {
        FallbackColor,
        SecondaryFillColor,
        SecondaryStrokeColor,
        StrokeLineCap,
        StrokeLineJoin,
        StrokeWidth
    }

    public class IconTokenResourceExtension : TokenResourceExtension<IconTokenKind>
    {
        public IconTokenResourceExtension()
        {
        }

        public IconTokenResourceExtension(IconTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class IconTokenSharedTokenResourceExtension : ControlSharedTokenResourceExtension
    {
        public IconTokenSharedTokenResourceExtension(SharedTokenKind kind) : base("AtomUI", "Icon", kind)
        {
        }
    }
}