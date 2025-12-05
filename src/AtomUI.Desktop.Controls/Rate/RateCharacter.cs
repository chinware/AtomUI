using AtomUI.Media;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal class RateCharacter : Control
{
    #region 公共属性定义

    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        Border.BackgroundProperty.AddOwner<TextBlock>();
    
    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        TextElement.ForegroundProperty.AddOwner<RateCharacter>();
    
    public static readonly StyledProperty<FontFamily> FontFamilyProperty =
        TextElement.FontFamilyProperty.AddOwner<RateCharacter>();

    public static readonly StyledProperty<double> FontSizeProperty =
        TextElement.FontSizeProperty.AddOwner<RateCharacter>();
    
    public static readonly StyledProperty<FontStyle> FontStyleProperty =
        TextElement.FontStyleProperty.AddOwner<RateCharacter>();
    
    public static readonly StyledProperty<FontWeight> FontWeightProperty =
        TextElement.FontWeightProperty.AddOwner<RateCharacter>();
    
    public static readonly StyledProperty<char?> CharacterProperty =
        AvaloniaProperty.Register<RateCharacter, char?>(nameof(Character));
    
    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }
    
    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }
    
    public FontFamily FontFamily
    {
        get => GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }
    
    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }
    
    public FontStyle FontStyle
    {
        get => GetValue(FontStyleProperty);
        set => SetValue(FontStyleProperty, value);
    }
    
    public FontWeight FontWeight
    {
        get => GetValue(FontWeightProperty);
        set => SetValue(FontWeightProperty, value);
    }
    
    public char? Character
    {
        get => GetValue(CharacterProperty);
        set => SetValue(CharacterProperty, value);
    }
    #endregion

    static RateCharacter()
    {
        AffectsMeasure<RateCharacter>(CharacterProperty, FontSizeProperty, FontStyleProperty, FontWeightProperty);
        AffectsRender<RateCharacter>(BackgroundProperty, ForegroundProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return TextUtils.CalculateTextSize($"{Character}", FontSize, FontFamily, FontStyle, FontWeight);
    }

    public sealed override void Render(DrawingContext context)
    {
    }
}