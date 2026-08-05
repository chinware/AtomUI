using System.Reactive.Disposables;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUI.Desktop.Controls;

internal sealed class NavMenuGroupItem : ItemsControl
{
    internal static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<NavMenuGroupItem, object?>(nameof(Header));

    internal static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.Register<NavMenuGroupItem, IDataTemplate?>(nameof(HeaderTemplate));

    internal static readonly DirectProperty<NavMenuGroupItem, NavMenuMode> ModeProperty =
        AvaloniaProperty.RegisterDirect<NavMenuGroupItem, NavMenuMode>(
            nameof(Mode),
            item => item.Mode,
            (item, value) => item.Mode = value);

    internal static readonly DirectProperty<NavMenuGroupItem, bool> IsDarkStyleProperty =
        AvaloniaProperty.RegisterDirect<NavMenuGroupItem, bool>(
            nameof(IsDarkStyle),
            item => item.IsDarkStyle,
            (item, value) => item.IsDarkStyle = value);

    internal static readonly DirectProperty<NavMenuGroupItem, bool> IsInlineCollapsedProperty =
        AvaloniaProperty.RegisterDirect<NavMenuGroupItem, bool>(
            nameof(IsInlineCollapsed),
            item => item.IsInlineCollapsed,
            (item, value) => item.IsInlineCollapsed = value);

    internal static readonly DirectProperty<NavMenuGroupItem, bool> IsTopLevelProperty =
        AvaloniaProperty.RegisterDirect<NavMenuGroupItem, bool>(
            nameof(IsTopLevel),
            item => item.IsTopLevel);

    internal static readonly StyledProperty<bool> IsItemBackgroundEnabledProperty =
        AvaloniaProperty.Register<NavMenuGroupItem, bool>(nameof(IsItemBackgroundEnabled), true);

    internal static readonly StyledProperty<double> EntryItemSpacingProperty =
        NavMenu.EntryItemSpacingProperty.AddOwner<NavMenuGroupItem>();

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<NavMenuGroupItem>();

    internal static readonly StyledProperty<bool> ShouldUseOverlayPopupProperty =
        AvaloniaProperty.Register<NavMenuGroupItem, bool>(nameof(ShouldUseOverlayPopup));

    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel());

    private CompositeDisposable? _entryBindingDisposables;
    private NavMenuMode _mode;
    private bool _isDarkStyle;
    private bool _isInlineCollapsed;
    private bool _isTopLevel;

    internal object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    internal IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    internal NavMenuMode Mode
    {
        get => _mode;
        set => SetAndRaise(ModeProperty, ref _mode, value);
    }

    internal bool IsDarkStyle
    {
        get => _isDarkStyle;
        set => SetAndRaise(IsDarkStyleProperty, ref _isDarkStyle, value);
    }

    internal bool IsInlineCollapsed
    {
        get => _isInlineCollapsed;
        set => SetAndRaise(IsInlineCollapsedProperty, ref _isInlineCollapsed, value);
    }

    internal bool IsTopLevel
    {
        get => _isTopLevel;
        private set => SetAndRaise(IsTopLevelProperty, ref _isTopLevel, value);
    }

    internal bool IsItemBackgroundEnabled
    {
        get => GetValue(IsItemBackgroundEnabledProperty);
        set => SetValue(IsItemBackgroundEnabledProperty, value);
    }

    internal double EntryItemSpacing
    {
        get => GetValue(EntryItemSpacingProperty);
        set => SetValue(EntryItemSpacingProperty, value);
    }

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    internal bool ShouldUseOverlayPopup
    {
        get => GetValue(ShouldUseOverlayPopupProperty);
        set => SetValue(ShouldUseOverlayPopupProperty, value);
    }

    internal NavMenu? OwnerMenu { get; private set; }
    internal NavMenuItem? SemanticParentItem { get; private set; }
    internal int Level { get; private set; }

    static NavMenuGroupItem()
    {
        ItemsPanelProperty.OverrideDefaultValue<NavMenuGroupItem>(DefaultPanel);
        FocusableProperty.OverrideDefaultValue<NavMenuGroupItem>(false);
    }

    internal CompositeDisposable ResetEntryBindingDisposables()
    {
        _entryBindingDisposables?.Dispose();
        _entryBindingDisposables = new CompositeDisposable();
        return _entryBindingDisposables;
    }

    internal void ClearEntryBindingDisposables()
    {
        _entryBindingDisposables?.Dispose();
        _entryBindingDisposables = null;
    }

    internal void UpdateEntryContext(
        NavMenu? ownerMenu,
        NavMenuItem? semanticParentItem,
        int level,
        bool isTopLevel)
    {
        OwnerMenu          = ownerMenu;
        SemanticParentItem = semanticParentItem;
        Level              = level;
        IsTopLevel         = isTopLevel;
    }

    internal void ClearEntryContext()
    {
        OwnerMenu          = null;
        SemanticParentItem = null;
        Level              = 0;
        IsTopLevel         = false;
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NavMenuEntryContainerCoordinator.NeedsContainer(this, item, index, out recycleKey);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return NavMenuEntryContainerCoordinator.CreateContainer(item, index, recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        NavMenuEntryContainerCoordinator.PrepareContainer(this, container, item, index);
    }

    protected override void ClearContainerForItemOverride(Control container)
    {
        NavMenuEntryContainerCoordinator.ClearContainer(this, container);
        base.ClearContainerForItemOverride(container);
    }
}
