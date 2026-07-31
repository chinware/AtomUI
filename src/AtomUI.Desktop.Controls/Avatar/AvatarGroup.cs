using System.Collections.Specialized;
using AtomUI.Controls;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

using ControlList = Avalonia.Controls.Controls;

public class AvatarGroup : TemplatedControl, IMotionAwareControl
{
    #region 公共属性定义
    
    public static readonly StyledProperty<IBrush?> FoldInfoAvatarBackgroundProperty = 
        AvaloniaProperty.Register<AvatarGroup, IBrush?>(nameof (FoldInfoAvatarBackground));
    
    public static readonly StyledProperty<IBrush?> FoldInfoAvatarForegroundProperty = 
        AvaloniaProperty.Register<AvatarGroup, IBrush?>(nameof (FoldInfoAvatarForeground));
    
    public static readonly StyledProperty<FlyoutTriggerType> FoldAvatarFlyoutTriggerTypeProperty =
        AvaloniaProperty.Register<AvatarGroup, FlyoutTriggerType>(nameof(FoldAvatarFlyoutTriggerType), FlyoutTriggerType.Hover);
    
    public static readonly StyledProperty<int?> MaxDisplayCountProperty =
        AvaloniaProperty.Register<AvatarGroup, int?>(nameof(MaxDisplayCount));
    
    public static readonly StyledProperty<CustomizableSizeType> SizeTypeProperty =
        Avatar.SizeTypeProperty.AddOwner<AvatarGroup>();
    
    public static readonly StyledProperty<double> SizeProperty =
        Avatar.SizeProperty.AddOwner<AvatarGroup>();
    
    public static readonly StyledProperty<AvatarShape> ShapeProperty =
        Avatar.ShapeProperty.AddOwner<AvatarGroup>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AvatarGroup>();
    
    public IBrush? FoldInfoAvatarBackground
    {
        get => GetValue(FoldInfoAvatarBackgroundProperty);
        set => SetValue(FoldInfoAvatarBackgroundProperty, value);
    }
    
    public IBrush? FoldInfoAvatarForeground
    {
        get => GetValue(FoldInfoAvatarForegroundProperty);
        set => SetValue(FoldInfoAvatarForegroundProperty, value);
    }
    
    public FlyoutTriggerType FoldAvatarFlyoutTriggerType
    {
        get => GetValue(FoldAvatarFlyoutTriggerTypeProperty);
        set => SetValue(FoldAvatarFlyoutTriggerTypeProperty, value);
    }
    
    public int? MaxDisplayCount
    {
        get => GetValue(MaxDisplayCountProperty);
        set => SetValue(MaxDisplayCountProperty, value);
    }
    
    public CustomizableSizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }
    
    public AvatarShape Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    [Content] public ControlList Children { get; } = new();

    #endregion
    
    #region 内部属性定义
    
    internal static readonly StyledProperty<double> GroupSpaceProperty = 
        AvaloniaProperty.Register<AvatarGroup, double>(nameof (GroupSpace));
    
    internal static readonly StyledProperty<double> GroupOverlappingProperty = 
        AvaloniaProperty.Register<AvatarGroup, double>(nameof (GroupOverlapping));
    
    internal double GroupSpace
    {
        get => GetValue(GroupSpaceProperty);
        set => SetValue(GroupSpaceProperty, value);
    }
    
    internal double GroupOverlapping
    {
        get => GetValue(GroupOverlappingProperty);
        set => SetValue(GroupOverlappingProperty, value);
    }
    #endregion
    
    private Avatar? _foldCountAvatar;
    private FlyoutHost? _foldCountFlyout;
    private StackPanel? _foldCountStackPanel;

    static AvatarGroup()
    {
        AffectsMeasure<AvatarGroup>(GroupOverlappingProperty, GroupSpaceProperty);
    }
    
    public AvatarGroup()
    {
        Children.CollectionChanged += ChildrenChanged;
        this.ConfigureMotionBindingStyle();
    }

    protected virtual void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            throw new NotSupportedException();
        }

        RebuildChildrenPresentation();
        InvalidateMeasureOnChildrenChanged();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        RebuildChildrenPresentation();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ReleaseFoldInfo();
    }

    private protected virtual void InvalidateMeasureOnChildrenChanged()
    {
        InvalidateMeasure();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        // 理论上是统一的，我们的孩子都一样大
        var count      = LogicalChildren.Count;
        if (count == 0)
        {
            return size.WithWidth(0);
        }
        var totalWidth = count * size.Width - (count - 1) * GroupOverlapping;
        return size.WithWidth(totalWidth);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var offsetX = 0.0;
        for (var i = 0; i < LogicalChildren.Count; i++)
        {
            var child = LogicalChildren[i];
            if (child is Control avatar)
            {
                var childSize = avatar.DesiredSize;
                avatar.Arrange(new Rect(offsetX, 0, childSize.Width, childSize.Height));
                offsetX += childSize.Width - GroupOverlapping;
            }
        }
        return finalSize;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == FoldAvatarFlyoutTriggerTypeProperty)
        {
            ConfigureFoldAvatarCursor();
        }
        else if (change.Property == MaxDisplayCountProperty)
        {
            RebuildChildrenPresentation();
            InvalidateMeasureOnChildrenChanged();
        }
    }

    private void RebuildChildrenPresentation()
    {
        ClearPresentedChildren();
        ConfigureChildren();
        UpdateChildrenZIndex();

        var visibleCount = GetVisibleChildrenCount();
        for (var i = 0; i < visibleCount; ++i)
        {
            LogicalChildren.Add(Children[i]);
            VisualChildren.Add(Children[i]);
        }

        ConfigureFoldInfo(visibleCount);
    }

    private void ClearPresentedChildren()
    {
        _foldCountStackPanel?.Children.Clear();
        LogicalChildren.Clear();
        VisualChildren.Clear();
    }

    private void ConfigureChildren()
    {
        foreach (var child in Children)
        {
            if (child is Avatar avatar)
            {
                ConfigureAvatar(avatar);
            }
        }
    }

    private void UpdateChildrenZIndex()
    {
        for (var i = 0; i < Children.Count; i++)
        {
            Children[i].ZIndex = i + 1;
        }
    }

    private int GetVisibleChildrenCount()
    {
        if (!MaxDisplayCount.HasValue || Children.Count <= MaxDisplayCount.Value)
        {
            return Children.Count;
        }

        return Math.Max(0, MaxDisplayCount.Value);
    }

    private void ConfigureFoldInfo(int visibleCount)
    {
        if (!ShouldFold(visibleCount))
        {
            ReleaseFoldInfo();
            return;
        }

        var foldCountFlyout = GetFoldCountFlyout();
        var foldCountAvatar = GetFoldCountAvatar();
        foldCountAvatar.Text = $"+{Children.Count - visibleCount}";
        LogicalChildren.Add(foldCountFlyout);
        VisualChildren.Add(foldCountFlyout);

        for (var i = visibleCount; i < Children.Count; ++i)
        {
            _foldCountStackPanel!.Children.Add(Children[i]);
        }
    }

    private bool ShouldFold(int visibleCount)
    {
        return MaxDisplayCount.HasValue && Children.Count > visibleCount;
    }

    private void BindAvatarProperties(Avatar avatar)
    {
        avatar[!BorderThicknessProperty] = this[!BorderThicknessProperty];
        avatar[!ShapeProperty]           = this[!ShapeProperty];
        avatar[!SizeProperty]            = this[!SizeProperty];
        avatar[!SizeTypeProperty]        = this[!SizeTypeProperty];
    }

    private void ConfigureAvatar(Avatar avatar)
    {
        BindAvatarProperties(avatar);
        avatar[!IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
    }

    private Avatar GetFoldCountAvatar()
    {
        if (_foldCountAvatar == null)
        {
            _foldCountAvatar = new Avatar();

            BindAvatarProperties(_foldCountAvatar);
            _foldCountAvatar[!ForegroundProperty] = this[!FoldInfoAvatarForegroundProperty];
            _foldCountAvatar[!BackgroundProperty] = this[!FoldInfoAvatarBackgroundProperty];
        }

        return _foldCountAvatar;
    }

    private FlyoutHost GetFoldCountFlyout()
    {
        if (_foldCountFlyout == null)
        {
            var foldCountAvatar = GetFoldCountAvatar();
            _foldCountStackPanel             = new StackPanel();
            _foldCountStackPanel.Orientation = Orientation.Horizontal;

            _foldCountFlyout                       = new FlyoutHost();
            _foldCountFlyout.ZIndex                = Int32.MaxValue;
            _foldCountFlyout.Content               = foldCountAvatar;
            _foldCountFlyout.ShouldUseOverlayPopup = true;

            _foldCountStackPanel[!StackPanel.SpacingProperty] = this[!GroupSpaceProperty];
            _foldCountFlyout[!FlyoutHost.TriggerProperty]     = this[!FoldAvatarFlyoutTriggerTypeProperty];

            _foldCountFlyout.Flyout = new Flyout
            {
                Content = _foldCountStackPanel
            };
            ConfigureFoldAvatarCursor();
        }

        return _foldCountFlyout;
    }

    private void ReleaseFoldInfo()
    {
        _foldCountStackPanel?.Children.Clear();
        if (_foldCountFlyout is { } foldCountFlyout)
        {
            if (foldCountFlyout.Flyout is { IsOpen: true } flyout)
            {
                flyout.Hide();
            }
            LogicalChildren.Remove(foldCountFlyout);
            VisualChildren.Remove(foldCountFlyout);
            foldCountFlyout.Flyout  = null;
            foldCountFlyout.Content = null;
        }

        _foldCountAvatar     = null;
        _foldCountFlyout     = null;
        _foldCountStackPanel = null;
    }

    private void ConfigureFoldAvatarCursor()
    {
        if (_foldCountAvatar == null)
        {
            return;
        }

        if (FoldAvatarFlyoutTriggerType == FlyoutTriggerType.Click)
        {
            _foldCountAvatar.Cursor = new Cursor(StandardCursorType.Hand);
        }
        else
        {
            _foldCountAvatar.Cursor = new Cursor(StandardCursorType.Arrow);
        }
    }
}
