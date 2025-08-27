using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

using ControlList = Avalonia.Controls.Controls;

public class AvatarGroup : TemplatedControl, IMotionAwareControl, IControlSharedTokenResourcesHost, IResourceBindingManager
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
    
    public static readonly StyledProperty<AvatarSizeType> SizeTypeProperty =
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
    
    public AvatarSizeType SizeType
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

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ButtonToken.ID;
    
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable { get; set; }
    #endregion
    
    private Avatar? _foldCountAvatar;
    private FlyoutHost? _foldCountFlyout;
    private StackPanel? _foldCountStackPanel;
    private Dictionary<Control, CompositeDisposable> _itemDisposables = new();
    private CompositeDisposable? _foldCoundAvatarDisposables;
    private CompositeDisposable? _flyoutDisposables;

    static AvatarGroup()
    {
        AffectsMeasure<AvatarGroup>(GroupOverlappingProperty, GroupSpaceProperty);
    }
    
    public AvatarGroup()
    {
        Children.CollectionChanged += ChildrenChanged;
        this.RegisterResources();
        this.ConfigureMotionBindingStyle();
    }

    protected virtual void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                var tobeAdded = e.NewItems!.OfType<Control>().ToList();
                foreach (var item in tobeAdded)
                {
                    if (item is Avatar avatar)
                    {
                        ConfigureAvatar(avatar);
                    }
                }
                if (!MaxDisplayCount.HasValue || LogicalChildren.Count < MaxDisplayCount.Value)
                {
                    var startingIndex = e.NewStartingIndex;
                    foreach (var item in tobeAdded)
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
                var items = e.OldItems!.OfType<Control>().ToList();
                LogicalChildren.RemoveAll(items);
                VisualChildren.RemoveAll(items);
                foreach (var child in items)
                {
                    if (_itemDisposables.TryGetValue(child, out var disposable))
                    {
                        disposable.Dispose();
                    }
                    _itemDisposables.Remove(child);
                }
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

        var zindex = 1;
        foreach (var child in Children)
        {
            child.ZIndex = zindex++;
        }

        ConfigureFoldInfo();
        InvalidateMeasureOnChildrenChanged();
    }

    private void ConfigureAvatar(Avatar avatar)
    {
        var disposable = new CompositeDisposable();
        disposable.Add(BindUtils.RelayBind(this, BorderThicknessProperty, avatar, BorderThicknessProperty));
        disposable.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, avatar, IsMotionEnabledProperty));
        disposable.Add(BindUtils.RelayBind(this, ShapeProperty, avatar, ShapeProperty));
        disposable.Add(BindUtils.RelayBind(this, SizeProperty, avatar, SizeProperty));
        disposable.Add(BindUtils.RelayBind(this, SizeTypeProperty, avatar, SizeTypeProperty));
        _itemDisposables.Add(avatar, disposable);
    }

    private Avatar GetFoldCountAvatar()
    {
        if (_foldCountAvatar == null)
        {
            _foldCountAvatar = new Avatar();
            _foldCoundAvatarDisposables?.Dispose();
            _foldCoundAvatarDisposables = new CompositeDisposable();
            _foldCoundAvatarDisposables.Add(BindUtils.RelayBind(this, BorderThicknessProperty, _foldCountAvatar, BorderThicknessProperty));
            _foldCoundAvatarDisposables.Add(BindUtils.RelayBind(this, ShapeProperty, _foldCountAvatar, ShapeProperty));
            _foldCoundAvatarDisposables.Add(BindUtils.RelayBind(this, SizeProperty, _foldCountAvatar, SizeProperty));
            _foldCoundAvatarDisposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, _foldCountAvatar, SizeTypeProperty));
            _foldCoundAvatarDisposables.Add(BindUtils.RelayBind(this, FoldInfoAvatarForegroundProperty, _foldCountAvatar, Avatar.ForegroundProperty));
            _foldCoundAvatarDisposables.Add(BindUtils.RelayBind(this, FoldInfoAvatarBackgroundProperty, _foldCountAvatar, Avatar.BackgroundProperty));
        }
       
        return _foldCountAvatar;
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        
        var foldCountAvatar = GetFoldCountAvatar();
        if (_foldCountFlyout == null)
        {
            _flyoutDisposables?.Dispose();
            _flyoutDisposables               = new CompositeDisposable();
            _foldCountFlyout                 = new FlyoutHost();
            _foldCountFlyout.ZIndex          = Int32.MaxValue;
            _foldCountFlyout.AnchorTarget    = foldCountAvatar;
            _foldCountStackPanel             = new StackPanel();
            _foldCountStackPanel.Orientation = Orientation.Horizontal;
            _flyoutDisposables.Add(BindUtils.RelayBind(this, GroupSpaceProperty, _foldCountStackPanel, StackPanel.SpacingProperty));
            _flyoutDisposables.Add(BindUtils.RelayBind(this, FoldAvatarFlyoutTriggerTypeProperty, _foldCountFlyout, FlyoutHost.TriggerProperty));
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
        if (_foldCountFlyout != null)
        {
            if (MaxDisplayCount.HasValue && Children.Count > MaxDisplayCount.Value)
            {
                foldCountAvatar.Text = $"+{Children.Count - MaxDisplayCount.Value}";
                LogicalChildren.Add(_foldCountFlyout);
                VisualChildren.Add(_foldCountFlyout);
                for (var i = MaxDisplayCount.Value; i < Children.Count; ++i)
                {
                    _foldCountStackPanel?.Children.Add(Children[i]);
                }
            }
            else
            {
                LogicalChildren.Remove(_foldCountFlyout);
                VisualChildren.Remove(_foldCountFlyout);
                _foldCountStackPanel?.Children.Clear();
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
        var offetX = 0.0;
        for (var i = 0; i < LogicalChildren.Count; i++)
        {
            var child = LogicalChildren[i];
            if (child is Control avatar)
            {
                var childSize = avatar.DesiredSize;
                avatar.Arrange(new Rect(offetX, 0, childSize.Width, childSize.Height));
                offetX += childSize.Width - GroupOverlapping;
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