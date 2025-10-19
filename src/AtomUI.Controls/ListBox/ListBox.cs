using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace AtomUI.Controls;

using AvaloniaListBox = Avalonia.Controls.ListBox;

public class ListBox : AvaloniaListBox,
                       IMotionAwareControl,
                       IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<ListBox>();

    public static readonly StyledProperty<bool> DisabledItemHoverEffectProperty =
        AvaloniaProperty.Register<ListBox, bool>(nameof(DisabledItemHoverEffect));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ListBox>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public bool DisabledItemHoverEffect
    {
        get => GetValue(DisabledItemHoverEffectProperty);
        set => SetValue(DisabledItemHoverEffectProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion
    
    #region 内部属性定义

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ListBoxToken.ID;

    #endregion
    
    private readonly Dictionary<ListBoxItem, CompositeDisposable> _itemsBindingDisposables = new();
    private IDisposable? _borderThicknessDisposable;

    public ListBox()
    {
        this.RegisterResources();
        LogicalChildren.CollectionChanged += HandleCollectionChanged;
    }
    
    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is ListBoxItem listBoxItem)
                    {
                        if (_itemsBindingDisposables.TryGetValue(listBoxItem, out var disposable))
                        {
                            disposable.Dispose();
                            _itemsBindingDisposables.Remove(listBoxItem);
                        }
                    }
                }
            }
        }
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new ListBoxItem();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return base.ArrangeOverride(finalSize.Deflate(new Thickness(BorderThickness.Left,
            BorderThickness.Top,
            BorderThickness.Right,
            BorderThickness.Bottom)));
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is ListBoxItem listBoxItem)
        {
            var disposables = new CompositeDisposable(3);
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, listBoxItem, ListBoxItem.IsMotionEnabledProperty));
            disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, listBoxItem, ListBoxItem.SizeTypeProperty));
            disposables.Add(BindUtils.RelayBind(this, DisabledItemHoverEffectProperty, listBoxItem,
                ListBoxItem.DisabledItemHoverEffectProperty));
            if (_itemsBindingDisposables.TryGetValue(listBoxItem, out var oldDisposables))
            {
                oldDisposables.Dispose();
                _itemsBindingDisposables.Remove(listBoxItem);
            }
            _itemsBindingDisposables.Add(listBoxItem, disposables);
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _borderThicknessDisposable = TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _borderThicknessDisposable?.Dispose();
    }
}