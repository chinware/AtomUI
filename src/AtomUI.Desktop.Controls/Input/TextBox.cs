using AtomUI.Animations;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

public class TextBox : AbstractTextInput
{
    #region 内部属性定义

    internal static readonly DirectProperty<TextBox, CornerRadius> EffectiveCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<TextBox, CornerRadius>(
            nameof(EffectiveCornerRadius),
            o => o.EffectiveCornerRadius,
            (o, value) => o.EffectiveCornerRadius = value);

    private CornerRadius _effectiveCornerRadius;

    internal CornerRadius EffectiveCornerRadius
    {
        get => _effectiveCornerRadius;
        private set => SetAndRaise(EffectiveCornerRadiusProperty, ref _effectiveCornerRadius, value);
    }

    #endregion

    public TextBox()
    {
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CornerRadiusProperty ||
            change.Property == CompactSpaceItemPositionProperty ||
            change.Property == CompactSpaceOrientationProperty)
        {
            ConfigureCornerRadius();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var innerBoxDecorator = e.NameScope.Find<PixelAlignedBorder>("InnerBoxDecorator");
        if (innerBoxDecorator is not null)
        {
            innerBoxDecorator.DisableTransitions();
            innerBoxDecorator.Dispatcher.Post(innerBoxDecorator.EnableTransitions);
        }

        ConfigureCornerRadius();
    }

    private void ConfigureCornerRadius()
    {
        EffectiveCornerRadius = CompactSpace.CalculateEffectiveCornerRadius(
            CornerRadius,
            IsUsedInCompactSpace,
            CompactSpaceItemPosition,
            CompactSpaceOrientation);
    }
}
