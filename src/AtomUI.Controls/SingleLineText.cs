using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class SingleLineText : Control
{
    #region 公共属性定义

    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        Border.BackgroundProperty.AddOwner<SingleLineText>();

    public static readonly StyledProperty<Thickness> PaddingProperty =
        Decorator.PaddingProperty.AddOwner<SingleLineText>();
    
    public static readonly StyledProperty<double> FontSizeProperty =
        TextElement.FontSizeProperty.AddOwner<SingleLineText>();

    public static readonly StyledProperty<FontFamily> FontFamilyProperty =
        TextElement.FontFamilyProperty.AddOwner<SingleLineText>();
    
    public static readonly StyledProperty<FontStyle> FontStyleProperty =
        TextElement.FontStyleProperty.AddOwner<SingleLineText>();

    public static readonly StyledProperty<FontWeight> FontWeightProperty =
        TextElement.FontWeightProperty.AddOwner<SingleLineText>();

    public static readonly StyledProperty<FontStretch> FontStretchProperty =
        TextElement.FontStretchProperty.AddOwner<SingleLineText>();

    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        TextElement.ForegroundProperty.AddOwner<SingleLineText>();

    public static readonly StyledProperty<double> BaselineOffsetProperty =
        AvaloniaProperty.Register<SingleLineText, double>(nameof(BaselineOffset));
    
    public static readonly AttachedProperty<double> LineHeightProperty =
        AvaloniaProperty.RegisterAttached<SingleLineText, Control, double>(
            nameof(LineHeight),
            double.NaN,
            validate: IsValidLineHeight,
            inherits: true);

    public static readonly StyledProperty<double> LetterSpacingProperty =
        AvaloniaProperty.Register<SingleLineText, double>(
            nameof(LetterSpacing),
            0);

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<SingleLineText, string?>(nameof(Text));

    public static readonly StyledProperty<TextAlignment> TextAlignmentProperty =
        AvaloniaProperty.Register<SingleLineText, TextAlignment>(
            nameof(TextAlignment),
            defaultValue: TextAlignment.Start);

    public static readonly StyledProperty<TextTrimming> TextTrimmingProperty =
        AvaloniaProperty.Register<SingleLineText, TextTrimming>(nameof(TextTrimming),
            defaultValue: TextTrimming.None);

    public static readonly StyledProperty<TextDecorationCollection?> TextDecorationsProperty =
        Inline.TextDecorationsProperty.AddOwner<SingleLineText>();

    public static readonly StyledProperty<FontFeatureCollection?> FontFeaturesProperty =
        TextElement.FontFeaturesProperty.AddOwner<SingleLineText>();
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        AvaloniaProperty.Register<SingleLineText, SizeType>(nameof(SizeType), SizeType.Middle);
    
    public TextLayout TextLayout => _textLayout ??= CreateTextLayout(Text);

    public Thickness Padding
    {
        get => GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    [Content]
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public FontFamily FontFamily
    {
        get => GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
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

    public FontStretch FontStretch
    {
        get => GetValue(FontStretchProperty);
        set => SetValue(FontStretchProperty, value);
    }
    
    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }
    
    public double LetterSpacing
    {
        get => GetValue(LetterSpacingProperty);
        set => SetValue(LetterSpacingProperty, value);
    }

    public TextTrimming TextTrimming
    {
        get => GetValue(TextTrimmingProperty);
        set => SetValue(TextTrimmingProperty, value);
    }

    public TextAlignment TextAlignment
    {
        get => GetValue(TextAlignmentProperty);
        set => SetValue(TextAlignmentProperty, value);
    }

    public TextDecorationCollection? TextDecorations
    {
        get => GetValue(TextDecorationsProperty);
        set => SetValue(TextDecorationsProperty, value);
    }

    public FontFeatureCollection? FontFeatures
    {
        get => GetValue(FontFeaturesProperty);
        set => SetValue(FontFeaturesProperty, value);
    }

    protected override bool BypassFlowDirectionPolicies => true;

    public double BaselineOffset
    {
        get => GetValue(BaselineOffsetProperty);
        set => SetValue(BaselineOffsetProperty, value);
    }
    
    public double LineHeight
    {
        get => GetValue(LineHeightProperty);
        set => SetValue(LineHeightProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    #endregion
    
    private TextLayout? _textLayout;
    private Size _constraint = new(double.NaN, double.NaN);
    
    static SingleLineText()
    {
        ClipToBoundsProperty.OverrideDefaultValue<SingleLineText>(true);
        AffectsRender<SingleLineText>(BackgroundProperty, ForegroundProperty);
    }

    private static bool IsValidLineHeight(double lineHeight) => double.IsNaN(lineHeight) || lineHeight > 0;
    
    public sealed override void Render(DrawingContext context)
    {
        RenderCore(context);
    }

    private protected virtual void RenderCore(DrawingContext context)
    {
        var background = Background;
        if (background != null)
        {
            context.FillRectangle(background, new Rect(Bounds.Size));
        }
        var scale      = LayoutHelper.GetLayoutScale(this);
        var padding    = LayoutHelper.RoundLayoutThickness(Padding, scale, scale);
        var top        = padding.Top;
        var textHeight = TextLayout.Height;
        if (Bounds.Height >= textHeight)
        {
            switch (VerticalAlignment)
            {
                case VerticalAlignment.Center:
                    top += (Bounds.Height - textHeight + TextLayout.OverhangTrailing) / 2;
                    break;

                case VerticalAlignment.Bottom:
                    top += Bounds.Height - textHeight;
                    break;
            }
        }

        RenderTextLayout(context, new Point(padding.Left, top));
    }

    protected virtual void RenderTextLayout(DrawingContext context, Point origin)
    {
        TextLayout.Draw(context, origin + new Point(TextLayout.OverhangLeading, 0));
    }

    protected virtual TextLayout CreateTextLayout(string? text)
    {
        var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
        var defaultProperties = new GenericTextRunProperties(
            typeface,
            FontFeatures,
            FontSize,
            TextDecorations,
            Foreground);

        var paragraphProperties = new GenericTextParagraphProperties(FlowDirection,
            IsMeasureValid ? TextAlignment : TextAlignment.Left, true, false,
            defaultProperties, TextWrapping.NoWrap, LineHeight, 0, LetterSpacing);

        ITextSource textSource;

        textSource = new SimpleTextSource(text ?? "", defaultProperties);

        var maxSize = GetMaxSizeFromConstraint();

        return new TextLayout(
            textSource,
            paragraphProperties,
            TextTrimming,
            maxSize.Width,
            maxSize.Height,
            1);
    }

    private protected Size GetMaxSizeFromConstraint()
    {
        var maxWidth  = double.IsNaN(_constraint.Width) ? 0.0 : _constraint.Width;
        var maxHeight = double.IsNaN(_constraint.Height) ? 0.0 : _constraint.Height;
        return new Size(maxWidth, maxHeight);
    }
    
    protected void InvalidateTextLayout()
    {
        InvalidateVisual();
        InvalidateMeasure();
    }

    protected override void OnMeasureInvalidated()
    {
        _textLayout?.Dispose();
        _textLayout = null;
        base.OnMeasureInvalidated();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var scale        = LayoutHelper.GetLayoutScale(this);
        var padding      = LayoutHelper.RoundLayoutThickness(Padding, scale, scale);
        var deflatedSize = availableSize.Deflate(padding);

        if (_constraint != deflatedSize)
        {
            //Reset TextLayout when the constraint is not matching.
            _textLayout?.Dispose();
            _textLayout = null;
            _constraint = deflatedSize;
            //Force arrange so text will be properly aligned.
            InvalidateArrange();
        }
        
        //This implicitly recreated the TextLayout with a new constraint if we previously reset it.
        var textLayout = TextLayout;
        var size = LayoutHelper.RoundLayoutSizeUp(new Size(textLayout.TextLines.First().Width, textLayout.Height + textLayout.OverhangTrailing / 2).Inflate(padding),
            1, 1);
        return size;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var scale   = LayoutHelper.GetLayoutScale(this);
        var padding = LayoutHelper.RoundLayoutThickness(Padding, scale, scale);

        var availableSize = finalSize.Deflate(padding);

        //ToDo: Introduce a text run cache to be able to reuse shaped runs etc.
        _textLayout?.Dispose();
        _textLayout = null;
        _constraint = availableSize;
        return finalSize;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == FontWeightProperty ||
            change.Property == FontWeightProperty ||
            change.Property == FontFamilyProperty ||
            change.Property == FontStretchProperty ||
            change.Property == TextTrimmingProperty ||
            change.Property == TextAlignmentProperty ||
            change.Property == FlowDirectionProperty ||
            change.Property == PaddingProperty ||
            change.Property == LetterSpacingProperty ||
            change.Property == TextProperty ||
            change.Property == TextDecorationsProperty ||
            change.Property == FontFeaturesProperty ||
            change.Property == ForegroundProperty ||
            change.Property == FontSizeProperty ||
            change.Property == LineHeightProperty)
        {
            InvalidateTextLayout();
        }
    }
    
    protected readonly record struct SimpleTextSource : ITextSource
    {
        private readonly string _text;
        private readonly TextRunProperties _defaultProperties;

        public SimpleTextSource(string text, TextRunProperties defaultProperties)
        {
            _text              = text;
            _defaultProperties = defaultProperties;
        }

        public TextRun GetTextRun(int textSourceIndex)
        {
            if (textSourceIndex > _text.Length)
            {
                return new TextEndOfParagraph();
            }

            var runText = _text.AsMemory(textSourceIndex);

            if (runText.IsEmpty)
            {
                return new TextEndOfParagraph();
            }

            return new TextCharacters(runText, _defaultProperties);
        }
    }
}
