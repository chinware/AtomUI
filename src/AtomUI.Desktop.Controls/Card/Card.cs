using System.Collections.Specialized;
using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Reflection;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

public enum CardStyleVariant
{
    Outlined,
    Borderless
}

internal enum CardContentType
{
    Default,
    Meta,
    Grid,
    Tabs
}

public class Card : HeaderedContentControl,
                    ISizeTypeAware,
                    IMotionAwareControl
{
    #region 公共属性定义
    
    public static readonly StyledProperty<BoxShadows> BoxShadowProperty = 
        Border.BoxShadowProperty.AddOwner<Card>();
    
    public static readonly StyledProperty<object?> ExtraProperty = 
        AvaloniaProperty.Register<Card, object?>(nameof (Extra));
    
    public static readonly StyledProperty<IDataTemplate?> ExtraTemplateProperty = 
        AvaloniaProperty.Register<Card, IDataTemplate?>(nameof (ExtraTemplate));
    
    public static readonly StyledProperty<CardStyleVariant> StyleVariantProperty = 
        AvaloniaProperty.Register<Card, CardStyleVariant>(nameof (StyleVariant));
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty = 
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<Card>();
    
    public static readonly StyledProperty<bool> IsLoadingProperty = 
        AvaloniaProperty.Register<Card, bool>(nameof (IsLoading));
    
    public static readonly StyledProperty<bool> IsInnerModeProperty = 
        AvaloniaProperty.Register<Card, bool>(nameof (IsInnerMode));
    
    public static readonly StyledProperty<bool> IsHoverableProperty = 
        AvaloniaProperty.Register<Card, bool>(nameof (IsHoverable));
    
    public static readonly StyledProperty<object?> CoverProperty = 
        AvaloniaProperty.Register<Card, object?>(nameof (Cover));
    
    public static readonly StyledProperty<IDataTemplate?> CoverTemplateProperty = 
        AvaloniaProperty.Register<Card, IDataTemplate?>(nameof (CoverTemplate));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Card>();
    
    public BoxShadows BoxShadow
    {
        get => GetValue(BoxShadowProperty);
        set => SetValue(BoxShadowProperty, value);
    }
    
    public object? Extra
    {
        get => GetValue(ExtraProperty);
        set => SetValue(ExtraProperty, value);
    }
    
    public IDataTemplate? ExtraTemplate
    {
        get => GetValue(ExtraTemplateProperty);
        set => SetValue(ExtraTemplateProperty, value);
    }
    
    public CardStyleVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }
    
    public bool IsInnerMode
    {
        get => GetValue(IsInnerModeProperty);
        set => SetValue(IsInnerModeProperty, value);
    }
    
    public bool IsHoverable
    {
        get => GetValue(IsHoverableProperty);
        set => SetValue(IsHoverableProperty, value);
    }
    
    public object? Cover
    {
        get => GetValue(CoverProperty);
        set => SetValue(CoverProperty, value);
    }
    
    public IDataTemplate? CoverTemplate
    {
        get => GetValue(CoverTemplateProperty);
        set => SetValue(CoverTemplateProperty, value);
    }

    public Avalonia.Controls.Controls Actions { get; } = new ();
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion
    
    #region 内部属性定义
    
    internal static readonly DirectProperty<Card, Thickness> HeaderBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<Card, Thickness>(
            nameof(HeaderBorderThickness),
            o => o.HeaderBorderThickness,
            (o, v) => o.HeaderBorderThickness = v);
    
    internal static readonly DirectProperty<Card, Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<Card, Thickness>(
            nameof(EffectiveBorderThickness),
            o => o.EffectiveBorderThickness,
            (o, v) => o.EffectiveBorderThickness = v);
    
    internal static readonly DirectProperty<Card, CornerRadius> EffectiveCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<Card, CornerRadius>(
            nameof(EffectiveCornerRadius),
            o => o.EffectiveCornerRadius,
            (o, v) => o.EffectiveCornerRadius = v);
    
    internal static readonly DirectProperty<Card, CardContentType> ContentTypeProperty =
        AvaloniaProperty.RegisterDirect<Card, CardContentType>(
            nameof(ContentType),
            o => o.ContentType,
            (o, v) => o.ContentType = v);
    
    private Thickness _headerBorderThickness;

    internal Thickness HeaderBorderThickness
    {
        get => _headerBorderThickness;
        set => SetAndRaise(HeaderBorderThicknessProperty, ref _headerBorderThickness, value);
    }
    
    private Thickness _effectiveBorderThickness;

    internal Thickness EffectiveBorderThickness
    {
        get => _effectiveBorderThickness;
        set => SetAndRaise(EffectiveBorderThicknessProperty, ref _effectiveBorderThickness, value);
    }
    
    private CornerRadius _effectiveCornerRadius;

    internal CornerRadius EffectiveCornerRadius
    {
        get => _effectiveCornerRadius;
        set => SetAndRaise(EffectiveCornerRadiusProperty, ref _effectiveCornerRadius, value);
    }
    
    private CardContentType _contentType;

    internal CardContentType ContentType
    {
        get => _contentType;
        set => SetAndRaise(ContentTypeProperty, ref _contentType, value);
    }
    #endregion

    private DockPanel? _rootLayout;
    private Border? _cardContentFrame;
    private Border? _headerFrame;
    private DockPanel? _headerLayout;
    private ContentPresenter? _headerExtraPresenter;
    private ContentPresenter? _titlePresenter;
    private Border? _coverFrame;
    private ContentPresenter? _coverContentPresenter;
    private CardActionPanel? _cardActionPanel;
    private ContentPresenter? _contentPresenter;
    private Skeleton? _skeleton;
    private bool _isApplyingTemplate;
    
    static Card()
    {
        AffectsRender<Card>(IsHoverableProperty, IsInnerModeProperty, BoxShadowProperty);
        AffectsMeasure<Card>(SizeTypeProperty, StyleVariantProperty);
    }
    
    public Card()
    {
        this.RegisterTokenResourceScope(CardToken.ScopeProvider);
        Actions.CollectionChanged += HandleActionsChanged;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BorderThicknessProperty)
        {
            ConfigureHeaderBorderThickness();
            ConfigureContentBorderThickness();
            SyncHeaderFrameProperties();
        }
        else if (change.Property == CornerRadiusProperty)
        {
            ConfigureContentCornerRadius();
            SyncActionPanelProperties();
            SyncCoverFrameProperties();
        }
        else if (change.Property == HeaderProperty ||
                 change.Property == HeaderTemplateProperty ||
                 change.Property == ExtraProperty ||
                 change.Property == ExtraTemplateProperty)
        {
            UpdatePseudoClasses();
            UpdateHeaderFrame();
            SyncCoverFrameProperties();
        }
        else if (change.Property == CoverProperty ||
                 change.Property == CoverTemplateProperty)
        {
            UpdateCoverFrame();
        }
        else if (change.Property == ContentProperty)
        {
            ClearContentIntegration(change.OldValue);
            ConfigureContentType();
            ConfigureContentCornerRadius();
            ConfigureHeaderBorderThickness();
            SyncBodyPresenterProperties();
        }
        else if (change.Property == ContentTemplateProperty ||
                 change.Property == HorizontalContentAlignmentProperty ||
                 change.Property == VerticalContentAlignmentProperty)
        {
            SyncBodyPresenterProperties();
        }
        else if (change.Property == IsLoadingProperty)
        {
            UpdateBodyPresenter();
        }
        else if (change.Property == IsMotionEnabledProperty)
        {
            SyncActionPanelProperties();
        }
    }

    private void ConfigureContentType()
    {
        // 暂时只能探测 Content 直接指定的情况
        if (Content is CardMetaContent)
        {
            SetCurrentValue(ContentTypeProperty, CardContentType.Meta);
        }
        else if (Content is CardTabsContent cardTabsContent)
        {
            SetCurrentValue(ContentTypeProperty, CardContentType.Tabs);
            cardTabsContent[!IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
            cardTabsContent[!SizeTypeProperty]        = this[!SizeTypeProperty];
        }
        else if (Content is CardGridContent cardGridContent)
        {
            SetCurrentValue(ContentTypeProperty, CardContentType.Grid);
            cardGridContent[!IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
            cardGridContent[!SizeTypeProperty]        = this[!SizeTypeProperty];
        }
        else
        {
            SetCurrentValue(ContentTypeProperty, CardContentType.Default);
        }
    }

    private static void ClearContentIntegration(object? content)
    {
        if (content is CardTabsContent cardTabsContent)
        {
            cardTabsContent.ClearValue(CardTabsContent.IsMotionEnabledProperty);
            cardTabsContent.ClearValue(CardTabsContent.SizeTypeProperty);
        }
        else if (content is CardGridContent cardGridContent)
        {
            cardGridContent.ClearValue(CardGridContent.IsMotionEnabledProperty);
            cardGridContent.ClearValue(CardGridContent.SizeTypeProperty);
        }
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ConfigureContentBorderThickness();
    }
    
    private void ConfigureContentBorderThickness()
    {
        if (StyleVariant == CardStyleVariant.Outlined)
        {
            SetCurrentValue(EffectiveBorderThicknessProperty, BorderThickness);
        }
        else if (StyleVariant == CardStyleVariant.Borderless)
        {
            SetCurrentValue(EffectiveBorderThicknessProperty, new Thickness(0));
        }
    }

    private void ConfigureHeaderBorderThickness()
    {
        if (ContentType == CardContentType.Grid)
        {
            SetCurrentValue(HeaderBorderThicknessProperty, new Thickness(0));
        }
        else
        {
            SetCurrentValue(HeaderBorderThicknessProperty, new Thickness(0, 0, 0, BorderThickness.Bottom));
        }
    }

    private void ConfigureContentCornerRadius()
    {
        if (ContentType == CardContentType.Grid)
        {
            SetCurrentValue(EffectiveCornerRadiusProperty, new CornerRadius(CornerRadius.TopLeft, CornerRadius.TopRight, 0, 0));
        }
        else
        {
            SetCurrentValue(EffectiveCornerRadiusProperty, CornerRadius);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ReleaseHeaderFrame();
        ReleaseActionPanel();
        ReleaseCoverFrame();
        ReleaseBodyPresenter();

        _rootLayout       = e.NameScope.Find<DockPanel>("PART_RootLayout");
        _cardContentFrame = e.NameScope.Find<Border>("CardContent");
        _isApplyingTemplate = true;
        try
        {
            UpdatePseudoClasses();
            ConfigureContentType();
            ConfigureContentCornerRadius();
            ConfigureHeaderBorderThickness();
            ConfigureContentBorderThickness();
            UpdateHeaderFrame();
            UpdateCoverFrame();
            UpdateActionPanel();
            UpdateBodyPresenter();
        }
        finally
        {
            _isApplyingTemplate = false;
        }
        EnsureRootLayoutOrder();
    }

    private bool HasHeaderContent()
    {
        return Header != null ||
               HeaderTemplate != null ||
               Extra != null ||
               ExtraTemplate != null;
    }

    private bool HasTitleContent()
    {
        return Header != null || HeaderTemplate != null;
    }

    private bool HasExtraContent()
    {
        return Extra != null || ExtraTemplate != null;
    }

    private bool HasCoverContent()
    {
        return Cover != null || CoverTemplate != null;
    }

    private void UpdateHeaderFrame()
    {
        if (_rootLayout == null)
        {
            return;
        }

        if (!HasHeaderContent())
        {
            ReleaseHeaderFrame();
            RequestRootLayoutOrder();
            return;
        }

        EnsureHeaderFrame();
        UpdateHeaderPresenters();
        SyncHeaderFrameProperties();
        RequestRootLayoutOrder();
    }

    private void EnsureHeaderFrame()
    {
        if (_headerFrame != null)
        {
            return;
        }

        _headerFrame = new Border
        {
            Name = "HeaderFrame"
        };
        DockPanel.SetDock(_headerFrame, Dock.Top);
        _headerFrame.SetTemplatedParent(this);

        _headerLayout = new DockPanel
        {
            LastChildFill = true
        };
        _headerLayout.SetTemplatedParent(this);
        _headerFrame.Child = _headerLayout;
    }

    private void UpdateHeaderPresenters()
    {
        if (_headerLayout == null)
        {
            return;
        }

        if (HasExtraContent())
        {
            EnsureHeaderExtraPresenter();
            if (!_headerLayout.Children.Contains(_headerExtraPresenter!))
            {
                _headerLayout.Children.Insert(0, _headerExtraPresenter!);
            }
        }
        else
        {
            ReleaseHeaderExtraPresenter();
        }

        if (HasTitleContent())
        {
            EnsureTitlePresenter();
            if (!_headerLayout.Children.Contains(_titlePresenter!))
            {
                _headerLayout.Children.Add(_titlePresenter!);
            }
        }
        else
        {
            ReleaseTitlePresenter();
        }
    }

    private void EnsureHeaderExtraPresenter()
    {
        if (_headerExtraPresenter != null)
        {
            SyncHeaderExtraPresenterProperties();
            return;
        }

        _headerExtraPresenter = new ContentPresenter
        {
            Name              = "HeaderExtra",
            VerticalAlignment = VerticalAlignment.Center
        };
        DockPanel.SetDock(_headerExtraPresenter, Dock.Right);
        _headerExtraPresenter.SetTemplatedParent(this);
        SyncHeaderExtraPresenterProperties();
    }

    private void EnsureTitlePresenter()
    {
        if (_titlePresenter != null)
        {
            SyncTitlePresenterProperties();
            return;
        }

        _titlePresenter = new ContentPresenter
        {
            Name              = "TitlePresenter",
            VerticalAlignment = VerticalAlignment.Center
        };
        _titlePresenter.SetTemplatedParent(this);
        SyncTitlePresenterProperties();
    }

    private void SyncHeaderFrameProperties()
    {
        _headerFrame?.SetCurrentValue(Border.BorderThicknessProperty, HeaderBorderThickness);
        SyncHeaderExtraPresenterProperties();
        SyncTitlePresenterProperties();
    }

    private void SyncHeaderExtraPresenterProperties()
    {
        if (_headerExtraPresenter == null)
        {
            return;
        }

        _headerExtraPresenter.SetCurrentValue(ContentPresenter.ContentProperty, Extra);
        _headerExtraPresenter.SetCurrentValue(ContentPresenter.ContentTemplateProperty, ExtraTemplate);
    }

    private void SyncTitlePresenterProperties()
    {
        if (_titlePresenter == null)
        {
            return;
        }

        _titlePresenter.SetCurrentValue(ContentPresenter.ContentProperty, Header);
        _titlePresenter.SetCurrentValue(ContentPresenter.ContentTemplateProperty, HeaderTemplate);
    }

    private void ReleaseHeaderFrame()
    {
        ReleaseHeaderExtraPresenter();
        ReleaseTitlePresenter();
        if (_headerFrame != null)
        {
            _rootLayout?.Children.Remove(_headerFrame);
            _headerFrame.Child = null;
            _headerFrame.SetTemplatedParent(null);
            _headerFrame = null;
        }
        if (_headerLayout != null)
        {
            _headerLayout.Children.Clear();
            _headerLayout.SetTemplatedParent(null);
            _headerLayout = null;
        }
    }

    private void ReleaseHeaderExtraPresenter()
    {
        if (_headerExtraPresenter == null)
        {
            return;
        }

        _headerLayout?.Children.Remove(_headerExtraPresenter);
        _headerExtraPresenter.Content         = null;
        _headerExtraPresenter.ContentTemplate = null;
        _headerExtraPresenter.SetTemplatedParent(null);
        _headerExtraPresenter = null;
    }

    private void ReleaseTitlePresenter()
    {
        if (_titlePresenter == null)
        {
            return;
        }

        _headerLayout?.Children.Remove(_titlePresenter);
        _titlePresenter.Content         = null;
        _titlePresenter.ContentTemplate = null;
        _titlePresenter.SetTemplatedParent(null);
        _titlePresenter = null;
    }

    private void UpdateCoverFrame()
    {
        if (_rootLayout == null)
        {
            return;
        }

        if (!HasCoverContent())
        {
            ReleaseCoverFrame();
            RequestRootLayoutOrder();
            return;
        }

        EnsureCoverFrame();
        SyncCoverFrameProperties();
        RequestRootLayoutOrder();
    }

    private void EnsureCoverFrame()
    {
        if (_coverFrame != null)
        {
            return;
        }

        _coverFrame = new Border
        {
            Name         = "CoverFrame",
            ClipToBounds = true
        };
        DockPanel.SetDock(_coverFrame, Dock.Top);
        _coverFrame.SetTemplatedParent(this);

        _coverContentPresenter = new ContentPresenter
        {
            Name                       = "CoverContentPresenter",
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            VerticalContentAlignment   = VerticalAlignment.Stretch
        };
        _coverContentPresenter.SetTemplatedParent(this);
        _coverFrame.Child = _coverContentPresenter;
    }

    private void SyncCoverFrameProperties()
    {
        if (_coverFrame != null)
        {
            _coverFrame.SetCurrentValue(Border.CornerRadiusProperty,
                HasHeaderContent()
                    ? new CornerRadius()
                    : new CornerRadius(CornerRadius.TopLeft, CornerRadius.TopRight, 0, 0));
        }

        if (_coverContentPresenter != null)
        {
            _coverContentPresenter.SetCurrentValue(ContentPresenter.ContentProperty, Cover);
            _coverContentPresenter.SetCurrentValue(ContentPresenter.ContentTemplateProperty, CoverTemplate);
        }
    }

    private void ReleaseCoverFrame()
    {
        if (_coverFrame != null)
        {
            _rootLayout?.Children.Remove(_coverFrame);
            _coverFrame.Child = null;
            _coverFrame.SetTemplatedParent(null);
            _coverFrame = null;
        }
        if (_coverContentPresenter != null)
        {
            _coverContentPresenter.Content         = null;
            _coverContentPresenter.ContentTemplate = null;
            _coverContentPresenter.SetTemplatedParent(null);
            _coverContentPresenter = null;
        }
    }

    private void UpdateActionPanel()
    {
        if (_rootLayout == null)
        {
            return;
        }

        if (Actions.Count == 0)
        {
            ReleaseActionPanel();
            RequestRootLayoutOrder();
            return;
        }

        EnsureActionPanel();
        RequestRootLayoutOrder();
    }

    private void EnsureActionPanel()
    {
        if (_cardActionPanel != null)
        {
            SyncActionPanelProperties();
            return;
        }

        _cardActionPanel = new CardActionPanel
        {
            Name = "PART_ActionPanel"
        };
        DockPanel.SetDock(_cardActionPanel, Dock.Bottom);
        _cardActionPanel.SetTemplatedParent(this);
        SyncActionPanelProperties();
        foreach (var action in Actions.OfType<Control>())
        {
            _cardActionPanel.Actions.Add(action);
        }
    }

    private void SyncActionPanelProperties()
    {
        if (_cardActionPanel == null)
        {
            return;
        }

        _cardActionPanel.SetCurrentValue(TemplatedControl.CornerRadiusProperty, CornerRadius);
        _cardActionPanel.SetCurrentValue(CardActionPanel.IsMotionEnabledProperty, IsMotionEnabled);
    }

    private void ReleaseActionPanel()
    {
        if (_cardActionPanel == null)
        {
            return;
        }

        _cardActionPanel.Actions.Clear();
        _rootLayout?.Children.Remove(_cardActionPanel);
        _cardActionPanel.SetTemplatedParent(null);
        _cardActionPanel = null;
    }

    private void UpdateBodyPresenter()
    {
        if (_cardContentFrame == null)
        {
            return;
        }

        if (IsLoading)
        {
            ReleaseContentPresenter();
            EnsureSkeleton();
        }
        else
        {
            ReleaseSkeleton();
            EnsureContentPresenter();
        }
    }

    private void EnsureContentPresenter()
    {
        if (_contentPresenter != null)
        {
            SyncBodyPresenterProperties();
            return;
        }

        _contentPresenter = new ContentPresenter
        {
            Name                = "PART_ContentPresenter",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Stretch
        };
        _contentPresenter.SetTemplatedParent(this);
        _cardContentFrame!.Child = _contentPresenter;
        SyncBodyPresenterProperties();
    }

    private void EnsureSkeleton()
    {
        if (_skeleton != null)
        {
            SyncBodyPresenterProperties();
            return;
        }

        _skeleton = new Skeleton
        {
            IsLoading           = true,
            IsActive            = true,
            ParagraphRows       = 4,
            IsShowTitle         = false,
            ClipToBounds        = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Stretch
        };
        _skeleton.SetTemplatedParent(this);
        _cardContentFrame!.Child = _skeleton;
        SyncBodyPresenterProperties();
    }

    private void SyncBodyPresenterProperties()
    {
        if (_contentPresenter != null)
        {
            _contentPresenter.SetCurrentValue(ContentPresenter.ContentProperty, Content);
            _contentPresenter.SetCurrentValue(ContentPresenter.ContentTemplateProperty, ContentTemplate);
            _contentPresenter.SetCurrentValue(ContentPresenter.HorizontalContentAlignmentProperty, HorizontalContentAlignment);
            _contentPresenter.SetCurrentValue(ContentPresenter.VerticalContentAlignmentProperty, VerticalContentAlignment);
        }

        if (_skeleton != null)
        {
            _skeleton.SetCurrentValue(Skeleton.ContentProperty, Content);
            _skeleton.SetCurrentValue(Skeleton.ContentTemplateProperty, ContentTemplate);
            _skeleton.SetCurrentValue(Skeleton.HorizontalContentAlignmentProperty, HorizontalContentAlignment);
            _skeleton.SetCurrentValue(Skeleton.VerticalContentAlignmentProperty, VerticalContentAlignment);
        }
    }

    private void ReleaseBodyPresenter()
    {
        ReleaseContentPresenter();
        ReleaseSkeleton();
    }

    private void ReleaseContentPresenter()
    {
        if (_contentPresenter == null)
        {
            return;
        }

        if (_cardContentFrame?.Child == _contentPresenter)
        {
            _cardContentFrame.Child = null;
        }
        _contentPresenter.Content         = null;
        _contentPresenter.ContentTemplate = null;
        _contentPresenter.SetTemplatedParent(null);
        _contentPresenter = null;
    }

    private void ReleaseSkeleton()
    {
        if (_skeleton == null)
        {
            return;
        }

        if (_cardContentFrame?.Child == _skeleton)
        {
            _cardContentFrame.Child = null;
        }
        _skeleton.Content         = null;
        _skeleton.ContentTemplate = null;
        _skeleton.SetTemplatedParent(null);
        _skeleton = null;
    }

    private void EnsureRootLayoutOrder()
    {
        if (_rootLayout == null || _cardContentFrame == null)
        {
            return;
        }

        MoveRootChildToEnd(_headerFrame);
        MoveRootChildToEnd(_cardActionPanel);
        MoveRootChildToEnd(_coverFrame);
        MoveRootChildToEnd(_cardContentFrame);
    }

    private void RequestRootLayoutOrder()
    {
        if (!_isApplyingTemplate)
        {
            EnsureRootLayoutOrder();
        }
    }

    private void MoveRootChildToEnd(Control? control)
    {
        if (_rootLayout == null || control == null)
        {
            return;
        }

        var children     = _rootLayout.Children;
        var currentIndex = children.IndexOf(control);
        if (currentIndex == children.Count - 1)
        {
            return;
        }

        if (currentIndex >= 0)
        {
            children.RemoveAt(currentIndex);
        }
        children.Add(control);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(CardPseudoClass.Headerless, Header == null && HeaderTemplate == null && Extra == null && ExtraTemplate == null);
    }
    
    private void HandleActionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_cardActionPanel == null)
        {
            if (Actions.Count > 0)
            {
                UpdateActionPanel();
            }
            return;
        }

        if (_cardActionPanel != null)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _cardActionPanel.Actions.InsertRange(e.NewStartingIndex, e.NewItems!.OfType<Control>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _cardActionPanel.Actions.RemoveAll(e.OldItems!.OfType<Control>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (int index1 = 0; index1 < e.OldItems!.Count; ++index1)
                    {
                        int     index2  = index1 + e.OldStartingIndex;
                        Control newItem = (Control) e.NewItems![index1]!;
                        _cardActionPanel.Actions[index2] = newItem;
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    _cardActionPanel.Actions.MoveRange(e.OldStartingIndex, e.OldItems!.Count, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _cardActionPanel.Actions.Clear();
                    foreach (var action in Actions.OfType<Control>())
                    {
                        _cardActionPanel.Actions.Add(action);
                    }
                    break;
            }
        }
        if (Actions.Count == 0)
        {
            ReleaseActionPanel();
            RequestRootLayoutOrder();
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (IsMotionEnabled && IsHoverable)
        {
            this.DisableTransitions();
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (IsMotionEnabled && IsHoverable)
        {
            Dispatcher.Post(this.EnableTransitions);
        }
    }
}
