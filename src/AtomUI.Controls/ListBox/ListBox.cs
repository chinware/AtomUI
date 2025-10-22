using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.IconPkg;
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
    
    public static readonly StyledProperty<bool> IsBorderlessProperty =
        AvaloniaProperty.Register<ListBox, bool>(nameof(IsBorderless), false);
    
    public static readonly StyledProperty<bool> IsShowSelectedIndicatorProperty =
        AvaloniaProperty.Register<ListBox, bool>(nameof(IsShowSelectedIndicator), false);
    
    public static readonly StyledProperty<Icon?> SelectedIndicatorProperty =
        AvaloniaProperty.Register<ListBox, Icon?>(nameof(SelectedIndicator));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ListBox>();

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
    
    public bool IsShowSelectedIndicator
    {
        get => GetValue(IsShowSelectedIndicatorProperty);
        set => SetValue(IsShowSelectedIndicatorProperty, value);
    }
    
    public bool IsBorderless
    {
        get => GetValue(IsBorderlessProperty);
        set => SetValue(IsBorderlessProperty, value);
    }
    
    public Icon? SelectedIndicator
    {
        get => GetValue(SelectedIndicatorProperty);
        set => SetValue(SelectedIndicatorProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion
    
    #region 内部属性定义
    
    internal static readonly DirectProperty<ListBox, Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<ListBox, Thickness>(nameof(EffectiveBorderThickness),
            o => o.EffectiveBorderThickness,
            (o, v) => o.EffectiveBorderThickness = v);
    
    private Thickness _effectiveBorderThickness;

    internal Thickness EffectiveBorderThickness
    {
        get => _effectiveBorderThickness;
        set => SetAndRaise(EffectiveBorderThicknessProperty, ref _effectiveBorderThickness, value);
    }

    protected override Type StyleKeyOverride { get; } = typeof(ListBox);
    
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
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is ListBoxItem listBoxItem)
        {
            var disposables = new CompositeDisposable(4);
            
            if (item != null && item is not Visual)
            {
                if (!listBoxItem.IsSet(ListBoxItem.ContentProperty))
                {
                    listBoxItem.SetCurrentValue(ListBoxItem.ContentProperty, item);
                }

                if (item is IListBoxItemData listBoxItemData)
                {
                    if (!listBoxItem.IsSet(ListBoxItem.IsSelectedProperty))
                    {
                        listBoxItem.SetCurrentValue(ListBoxItem.IsSelectedProperty, listBoxItemData.IsSelected);
                    }
                    if (!listBoxItem.IsSet(ListBoxItem.IsEnabledProperty))
                    {
                        listBoxItem.SetCurrentValue(IsEnabledProperty, listBoxItemData.IsEnabled);
                    }
                }
            }
            
            if (ItemTemplate != null)
            {
                disposables.Add(BindUtils.RelayBind(this, ItemTemplateProperty, listBoxItem, ListBoxItem.ContentTemplateProperty));
            }
            
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
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type ListBoxItem.");
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BorderThicknessProperty ||
            change.Property == IsBorderlessProperty)
        {
            ConfigureEffectiveBorderThickness();
        }
    }

    private void ConfigureEffectiveBorderThickness()
    {
        if (IsBorderless)
        {
            SetCurrentValue(EffectiveBorderThicknessProperty, new Thickness(0));
        }
        else
        {
            SetCurrentValue(EffectiveBorderThicknessProperty, BorderThickness);
        }
    }
}