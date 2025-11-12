using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AtomUI.Controls;

using AvaloniaTextBox = Avalonia.Controls.TextBox;

public class TextArea : AvaloniaTextBox,
                        IControlSharedTokenResourcesHost,
                        IMotionAwareControl,
                        ISizeTypeAware
{
    #region 公共属性定义

    public static readonly StyledProperty<int> LinesProperty =
        AvaloniaProperty.Register<TextArea, int>(nameof(Lines), 2);
    
    public static readonly StyledProperty<bool> IsAutoSizeProperty =
        AvaloniaProperty.Register<TextArea, bool>(nameof(IsAutoSize), false);

    public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
        AddOnDecoratedBox.StyleVariantProperty.AddOwner<TextArea>();

    public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
        AddOnDecoratedBox.StatusProperty.AddOwner<TextArea>();

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<TextArea>();

    public static readonly StyledProperty<bool> IsEnableClearButtonProperty =
        AvaloniaProperty.Register<TextArea, bool>(nameof(IsEnableClearButton));

    public static readonly StyledProperty<IBrush?> WatermarkForegroundProperty =
        AvaloniaProperty.Register<TextArea, IBrush?>(nameof(WatermarkForeground));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<TextArea>();

    public static readonly StyledProperty<IDataTemplate?> InnerLeftContentTemplateProperty =
        AvaloniaProperty.Register<TextArea, IDataTemplate?>(nameof(InnerLeftContentTemplate));

    public static readonly StyledProperty<IDataTemplate?> InnerRightContentTemplateProperty =
        AvaloniaProperty.Register<TextArea, IDataTemplate?>(nameof(InnerRightContentTemplate));

    public int Lines
    {
        get => GetValue(LinesProperty);
        set
        {
            ValidateLinesValue(value);
            SetValue(LinesProperty, value);
        }
    }
    
    public bool IsAutoSize
    {
        get => GetValue(IsAutoSizeProperty);
        set => SetValue(IsAutoSizeProperty, value);
    }

    public AddOnDecoratedVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }

    public AddOnDecoratedStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public bool IsEnableClearButton
    {
        get => GetValue(IsEnableClearButtonProperty);
        set => SetValue(IsEnableClearButtonProperty, value);
    }

    public IBrush? WatermarkForeground
    {
        get => GetValue(WatermarkForegroundProperty);
        set => SetValue(WatermarkForegroundProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public IDataTemplate? InnerLeftContentTemplate
    {
        get => GetValue(InnerLeftContentTemplateProperty);
        set => SetValue(InnerLeftContentTemplateProperty, value);
    }

    public IDataTemplate? InnerRightContentTemplate
    {
        get => GetValue(InnerRightContentTemplateProperty);
        set => SetValue(InnerRightContentTemplateProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<TextArea, bool> IsEffectiveShowClearButtonProperty =
        AvaloniaProperty.RegisterDirect<TextArea, bool>(nameof(IsEffectiveShowClearButton),
            o => o.IsEffectiveShowClearButton,
            (o, v) => o.IsEffectiveShowClearButton = v);

    private bool _isEffectiveShowClearButton;

    internal bool IsEffectiveShowClearButton
    {
        get => _isEffectiveShowClearButton;
        set => SetAndRaise(IsEffectiveShowClearButtonProperty, ref _isEffectiveShowClearButton, value);
    }

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => LineEditToken.ID;
    Control IMotionAwareControl.PropertyBindTarget => this;

    #endregion

    private ScrollViewer? _scrollViewer;
    private IconButton? _clearButton;

    static TextArea()
    {
        AffectsMeasure<TextArea>(IsAutoSizeProperty, LinesProperty);
    }
    
    public TextArea()
    {
        this.RegisterResources();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Error, Status == AddOnDecoratedStatus.Error);
        PseudoClasses.Set(StdPseudoClass.Warning, Status == AddOnDecoratedStatus.Warning);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Outline, StyleVariant == AddOnDecoratedVariant.Outline);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Filled, StyleVariant == AddOnDecoratedVariant.Filled);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Borderless, StyleVariant == AddOnDecoratedVariant.Borderless);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == StatusProperty)
        {
            UpdatePseudoClasses();
        }
        
        if (change.Property == AcceptsReturnProperty ||
            change.Property == IsReadOnlyProperty ||
            change.Property == TextProperty ||
            change.Property == IsEnableClearButtonProperty)
        {
            ConfigureEffectiveShowClearButton();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var decorator = e.NameScope.Find<TextAreaDecoratedBox>(AddOnDecoratedBox.AddOnDecoratedBoxPart);
        if (decorator != null)
        {
            decorator.Owner = this;
        }
        
        _clearButton      = e.NameScope.Find<IconButton>(TextAreaThemeConstants.ClearButtonPart);
        if (_clearButton is not null)
        {
            _clearButton.Click += (sender, args) => { NotifyClearButtonClicked(); };
        }

        UpdatePseudoClasses();
    }

    internal void NotifyScrollViewerCreated(ScrollViewer scrollViewer)
    {
        _scrollViewer = scrollViewer;
        scrollViewer.ScrollChanged -= this.HandleScrollChanged;
        scrollViewer.ScrollChanged += this.HandleScrollChanged;
        this.SetScrollViewer(scrollViewer);
    }

    private void ValidateLinesValue(int lines)
    {
        var maxLines = int.MaxValue;
        if (MaxLines > 0)
        {
            maxLines = MaxLines;
        }

        if (lines > maxLines || lines < MinLines)
        {
            throw new ArgumentOutOfRangeException($"Lines must be between {MinLines} and {MaxLines}");
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        if (_scrollViewer != null)
        {
            if (!IsAutoSize)
            {
                if (Lines > 0 && double.IsNaN(Height))
                {
                    var height = double.NaN;
                    Lines = Math.Max(Lines, MinLines);
                    var fontSize = FontSize;
                    var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
                    var paragraphProperties = TextLayoutReflectionExtensions.CreateTextParagraphProperties(typeface, fontSize, null, default, default, null, default, LineHeight, default, FontFeatures);
                    var textLayout = new TextLayout(new LineTextSource(Lines), paragraphProperties);
                    var verticalSpace = this.GetVerticalSpaceBetweenScrollViewerAndPresenter();
                    height = Math.Ceiling(textLayout.Height + verticalSpace);
                    _scrollViewer.SetCurrentValue(MinHeightProperty, height);
                }
            }
        }
        return size;
    }
    
    private void ConfigureEffectiveShowClearButton()
    {
        if (!IsEnableClearButton)
        {
            IsEffectiveShowClearButton = false;
            return;
        }
        
        SetCurrentValue(IsEffectiveShowClearButtonProperty, !IsReadOnly && !string.IsNullOrEmpty(Text));
    }
    
    private class LineTextSource : ITextSource
    {
        private readonly int _lines;

        public LineTextSource(int lines)
        {
            _lines = lines;
        }

        public TextRun? GetTextRun(int textSourceIndex)
        {
            if (textSourceIndex >= _lines)
            {
                return null;
            }
            return new TextEndOfLine(1);
        }
    }
    
    protected virtual void NotifyClearButtonClicked()
    {
        Clear();
    }
}