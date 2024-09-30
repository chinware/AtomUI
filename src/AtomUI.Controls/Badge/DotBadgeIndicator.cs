using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class DotBadgeIndicator : Control
{
    internal static readonly StyledProperty<IBrush?> BadgeDotColorProperty =
        AvaloniaProperty.Register<DotBadgeIndicator, IBrush?>(
            nameof(BadgeDotColor));

    internal static readonly StyledProperty<IBrush?> BadgeShadowColorProperty =
        AvaloniaProperty.Register<DotBadgeIndicator, IBrush?>(
            nameof(BadgeShadowColor));

    internal static readonly StyledProperty<double> BadgeShadowSizeProperty =
        AvaloniaProperty.Register<DotBadgeIndicator, double>(
            nameof(BadgeShadowSize));

    internal IBrush? BadgeDotColor
    {
        get => GetValue(BadgeDotColorProperty);
        set => SetValue(BadgeDotColorProperty, value);
    }

    internal IBrush? BadgeShadowColor
    {
        get => GetValue(BadgeShadowColorProperty);
        set => SetValue(BadgeShadowColorProperty, value);
    }

    public double BadgeShadowSize
    {
        get => GetValue(BadgeShadowSizeProperty);
        set => SetValue(BadgeShadowSizeProperty, value);
    }

    private BoxShadows _boxShadows;

    static DotBadgeIndicator()
    {
        AffectsRender<DotBadgeIndicator>(BadgeDotColorProperty, BadgeShadowSizeProperty);
    }

    public sealed override void ApplyTemplate()
    {
        base.ApplyTemplate();
        BuildBoxShadow();
        
        TokenResourceBinder.CreateTokenBinding(this, BadgeShadowSizeProperty, BadgeTokenResourceKey.BadgeShadowSize);
        TokenResourceBinder.CreateTokenBinding(this, BadgeShadowColorProperty, BadgeTokenResourceKey.BadgeShadowColor);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == BadgeShadowSizeProperty ||
            e.Property == BadgeShadowColorProperty)
        {
            BuildBoxShadow(true);
        }
    }

    public override void Render(DrawingContext context)
    {
        context.DrawRectangle(BadgeDotColor, null, new Rect(Bounds.Size), Bounds.Width / 2, Bounds.Height / 2,
            _boxShadows);
    }

    private void BuildBoxShadow(bool force = false)
    {
        if (_boxShadows == default || force)
        {
            if (BadgeShadowColor is not null)
            {
                _boxShadows = new BoxShadows(new BoxShadow
                {
                    OffsetX = 0,
                    OffsetY = 0,
                    Blur    = 0,
                    Spread  = BadgeShadowSize,
                    Color   = ((SolidColorBrush)BadgeShadowColor).Color
                });
            }
        }
    }
}