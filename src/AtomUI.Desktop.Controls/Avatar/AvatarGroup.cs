using System.Collections.Specialized;
using AtomUI.Controls;
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
                if (!MaxDisplayCount.HasValue || LogicalChildren.Count < MaxDisplayCount.Value)
                {
                    var startingIndex = e.NewStartingIndex;
                    foreach (var item in e.NewItems!.OfType<Control>())
                    {
                        LogicalChildren.Insert(startingIndex, item);
                        VisualChildren.Insert(startingIndex, item);
                        startingIndex++;
                    }
                }
                break;

            case NotifyCollectionChangedAction.Move:
                if (!MaxDisplayCount.HasValue)
                {
                    LogicalChildren.MoveRange(e.OldStartingIndex, e.OldItems!.Count, e.NewStartingIndex);
                    VisualChildren.MoveRange(e.OldStartingIndex, e.OldItems!.Count, e.NewStartingIndex);
                }
                else
                {
                    if (e.OldStartingIndex < MaxDisplayCount.Value && 
                        e.OldStartingIndex + e.OldItems!.Count <= MaxDisplayCount.Value &&
                        e.NewStartingIndex <= MaxDisplayCount.Value)
                    {
                        LogicalChildren.MoveRange(e.OldStartingIndex, e.OldItems!.Count, e.NewStartingIndex);
                        VisualChildren.MoveRange(e.OldStartingIndex, e.OldItems!.Count, e.NewStartingIndex);
                    }
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                var removedItems = e.OldItems!.OfType<Control>();
                LogicalChildren.RemoveAll(removedItems);
                VisualChildren.RemoveAll(removedItems);
                break;

            case NotifyCollectionChangedAction.Replace:
                if (!MaxDisplayCount.HasValue)
                {
                    for (var i = 0; i < e.OldItems!.Count; ++i)
                    {
                        var index = i + e.OldStartingIndex;
                        var child = (Control)e.NewItems![i]!;
                        LogicalChildren[index] = child;
                        VisualChildren[index]  = child;
                    }
                }
                else
                {
                    for (var i = 0; i < e.OldItems!.Count; ++i)
                    {
                        var index = i + e.OldStartingIndex;
                        if (index < MaxDisplayCount.Value)
                        {
                            var child = (Control)e.NewItems![i]!;
                            LogicalChildren[index] = child;
                            VisualChildren[index]  = child;
                        }
                    }
                }
                
                break;

            case NotifyCollectionChangedAction.Reset:
                throw new NotSupportedException();
        }

        UpdateChildrenZIndex(e);
        ConfigureFoldInfo();
        InvalidateMeasureOnChildrenChanged();
    }

    private void UpdateChildrenZIndex(NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Replace:
                // Update ZIndex for all children after the change
                var startIndex = e.Action == NotifyCollectionChangedAction.Remove
                    ? e.OldStartingIndex
                    : e.NewStartingIndex;
                for (var i = startIndex; i < Children.Count; i++)
                {
                    Children[i].ZIndex = i + 1;
                }
                break;

            case NotifyCollectionChangedAction.Move:
                // Update ZIndex for affected range
                var minIndex = Math.Min(e.OldStartingIndex, e.NewStartingIndex);
                var maxIndex = Math.Max(e.OldStartingIndex + e.OldItems!.Count,
                                       e.NewStartingIndex + e.OldItems!.Count);
                for (var i = minIndex; i < Math.Min(maxIndex, Children.Count); i++)
                {
                    Children[i].ZIndex = i + 1;
                }
                break;
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
        var foldCountAvatar = GetFoldCountAvatar();
        if (_foldCountFlyout == null)
        {
            _foldCountFlyout                       = new FlyoutHost();
            _foldCountFlyout.ZIndex                = Int32.MaxValue;
            _foldCountFlyout.Content               = foldCountAvatar;
            _foldCountFlyout.ShouldUseOverlayPopup = true;
            _foldCountStackPanel                   = new StackPanel();
            _foldCountStackPanel.Orientation       = Orientation.Horizontal;
            
            _foldCountStackPanel[!StackPanel.SpacingProperty] = this[!GroupSpaceProperty];
            _foldCountFlyout[!FlyoutHost.TriggerProperty]     = this[!FoldAvatarFlyoutTriggerTypeProperty];
            
            _foldCountFlyout.Flyout = new Flyout
            {
                Content = _foldCountStackPanel
            };
        }

        ConfigureFoldInfo();
    }
    
    private void ConfigureFoldInfo()
    {
        var foldCountAvatar = GetFoldCountAvatar();
        if (_foldCountFlyout != null && _foldCountStackPanel != null)
        {
            if (MaxDisplayCount.HasValue && Children.Count > MaxDisplayCount.Value)
            {
                foldCountAvatar.Text = $"+{Children.Count - MaxDisplayCount.Value}";
                LogicalChildren.Add(_foldCountFlyout);
                VisualChildren.Add(_foldCountFlyout);
                for (var i = MaxDisplayCount.Value; i < Children.Count; ++i)
                {
                    _foldCountStackPanel.Children.Add(Children[i]);
                }
            }
            else
            {
                LogicalChildren.Remove(_foldCountFlyout);
                VisualChildren.Remove(_foldCountFlyout);
                _foldCountStackPanel.Children.Clear();
            }
        }
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
    }

    private void ConfigureFoldAvatarCursor()
    {
        var foldCountAvatar = GetFoldCountAvatar();
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