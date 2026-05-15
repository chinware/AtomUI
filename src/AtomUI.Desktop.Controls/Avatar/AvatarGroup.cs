using System.Collections.Specialized;
using AtomUI.Controls;
using AtomUI.Reflection;
using AtomUI.Theme;
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
    private bool _isAttachedToVisualTree;

    static AvatarGroup()
    {
        AffectsMeasure<AvatarGroup>(GroupOverlappingProperty, GroupSpaceProperty);
    }
    
    public AvatarGroup()
    {
        Children.CollectionChanged += ChildrenChanged;
        this.RegisterTokenResourceScope(AvatarToken.ScopeProvider);
        this.ConfigureMotionBindingStyle();
    }

    protected virtual void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                foreach (var item in e.NewItems!.OfType<Control>())
                {
                    if (item is Avatar avatar)
                    {
                        ConfigureAvatar(avatar);
                    }
                }
                break;

            case NotifyCollectionChangedAction.Replace:
                foreach (var item in e.NewItems!.OfType<Control>())
                {
                    if (item is Avatar avatar)
                    {
                        ConfigureAvatar(avatar);
                    }
                }
                break;

        }

        UpdateChildrenZIndex();
        MarkFoldInfoDirty();
        InvalidateMeasureOnChildrenChanged();
    }

    private void UpdateChildrenZIndex()
    {
        for (var i = 0; i < Children.Count; i++)
        {
            Children[i].ZIndex = i + 1;
        }
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

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _isAttachedToVisualTree = true;
        ConfigureFoldInfo();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isAttachedToVisualTree = false;
        ReleaseFoldInfrastructure();
        base.OnDetachedFromVisualTree(e);
    }

    private void MarkFoldInfoDirty()
    {
        if (_isAttachedToVisualTree)
        {
            ConfigureFoldInfo();
        }
    }
    
    private void ConfigureFoldInfo()
    {
        var shouldFold = MaxDisplayCount.HasValue && Children.Count > MaxDisplayCount.Value;
        var visibleCount = shouldFold
            ? Math.Max(0, MaxDisplayCount!.Value)
            : Children.Count;

        RebuildVisibleChildren(visibleCount);
        if (!shouldFold)
        {
            ReleaseFoldInfrastructure();
            return;
        }

        var (foldHost, stackPanel) = EnsureFoldInfrastructure();
        var foldCountAvatar = GetFoldCountAvatar();
        foldCountAvatar.Text = $"+{Children.Count - visibleCount}";
        AddFoldHostToGroup(foldHost);
        for (var i = visibleCount; i < Children.Count; ++i)
        {
            stackPanel.Children.Add(Children[i]);
        }
    }

    private void RebuildVisibleChildren(int visibleCount)
    {
        ClearFoldStackPanel();
        RemoveFoldHostFromGroup();
        foreach (var child in Children)
        {
            LogicalChildren.Remove(child);
            VisualChildren.Remove(child);
        }

        var count = Math.Min(visibleCount, Children.Count);
        for (var i = 0; i < count; i++)
        {
            LogicalChildren.Add(Children[i]);
            VisualChildren.Add(Children[i]);
        }
    }

    private (FlyoutHost FoldHost, StackPanel StackPanel) EnsureFoldInfrastructure()
    {
        if (_foldCountFlyout != null &&
            _foldCountStackPanel != null)
        {
            return (_foldCountFlyout, _foldCountStackPanel);
        }

        ReleaseFoldInfrastructure();

        var foldCountAvatar = GetFoldCountAvatar();
        var flyout = new FlyoutHost
        {
            ZIndex                = Int32.MaxValue,
            Content               = foldCountAvatar,
            ShouldUseOverlayPopup = true
        };
        flyout.SetTemplatedParent(this);
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };
        stackPanel[!StackPanel.SpacingProperty] = this[!GroupSpaceProperty];
        flyout[!FlyoutHost.TriggerProperty]     = this[!FoldAvatarFlyoutTriggerTypeProperty];
        flyout.Flyout = new Flyout
        {
            Content = stackPanel
        };
        _foldCountFlyout     = flyout;
        _foldCountStackPanel = stackPanel;
        ConfigureFoldAvatarCursor();
        return (flyout, stackPanel);
    }

    private void AddFoldHostToGroup(FlyoutHost flyout)
    {
        if (!LogicalChildren.Contains(flyout))
        {
            LogicalChildren.Add(flyout);
        }
        if (!VisualChildren.Contains(flyout))
        {
            VisualChildren.Add(flyout);
        }
    }

    private void RemoveFoldHostFromGroup()
    {
        if (_foldCountFlyout == null)
        {
            return;
        }

        LogicalChildren.Remove(_foldCountFlyout);
        VisualChildren.Remove(_foldCountFlyout);
    }

    private void ClearFoldStackPanel()
    {
        _foldCountStackPanel?.Children.Clear();
    }

    private void ReleaseFoldInfrastructure()
    {
        if (_foldCountFlyout == null &&
            _foldCountStackPanel == null &&
            _foldCountAvatar == null)
        {
            return;
        }

        ClearFoldStackPanel();
        RemoveFoldHostFromGroup();
        if (_foldCountFlyout != null)
        {
            if (_foldCountFlyout.Flyout is { } flyout)
            {
                flyout.Content = null!;
            }
            _foldCountFlyout.Flyout  = null;
            _foldCountFlyout.Content = null;
            _foldCountFlyout.SetTemplatedParent(null);
        }
        _foldCountAvatar     = null;
        _foldCountFlyout     = null;
        _foldCountStackPanel = null;
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
            MarkFoldInfoDirty();
            InvalidateMeasureOnChildrenChanged();
        }
    }

    private void ConfigureFoldAvatarCursor()
    {
        var foldCountAvatar = _foldCountAvatar;
        if (foldCountAvatar == null)
        {
            return;
        }
        if (FoldAvatarFlyoutTriggerType == FlyoutTriggerType.Click)
        {
            foldCountAvatar.Cursor = new Cursor(StandardCursorType.Hand);
        }
        else
        {
            foldCountAvatar.Cursor = new Cursor(StandardCursorType.Arrow);
        }
    }
}
