using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using AtomDrawer = AtomUI.Desktop.Controls.Drawer;
using AtomDrawerPlacement = AtomUI.Desktop.Controls.DrawerPlacement;
using ScenarioTabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.Drawer;

public partial class DrawerShowCase : GalleryReactiveUserControl<DrawerViewModel>
{
    public const string LanguageId = nameof(DrawerShowCase);

    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public DrawerShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _scenarioController.Attach(DataContext);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _scenarioController.Detach();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        _scenarioController.UpdateDataContext(DataContext);
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new DrawerApiDataGrid(),
            DesignTokenScenario => new DrawerDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Drawer scenario: {scenario}")
        };
    }

    private void HandleOpenLargeSizeDrawer(object? sender, RoutedEventArgs e)
    {
        if (TryFindTemplateControl(sender, "PresetSizeDrawer", out AtomDrawer drawer))
        {
            drawer.SizeType = CustomizableSizeType.Large;
            drawer.IsOpen   = true;
        }
    }

    private void HandleOpenCustomSizeDrawer(object? sender, RoutedEventArgs e)
    {
        if (TryFindTemplateControl(sender, "PresetSizeDrawer", out AtomDrawer drawer))
        {
            drawer.SizeType   = CustomizableSizeType.Custom;
            drawer.DialogSize = new Dimension(400);
            drawer.IsOpen     = true;
        }
    }

    private void HandleOpenCustomPercentageSizeDrawer(object? sender, RoutedEventArgs e)
    {
        if (TryFindTemplateControl(sender, "PresetSizeDrawer", out AtomDrawer drawer))
        {
            drawer.SizeType   = CustomizableSizeType.Custom;
            drawer.DialogSize = new Dimension(50, DimensionUnitType.Percentage);
            drawer.IsOpen     = true;
        }
    }

    private void HandleOpenDefaultSizeDrawer(object? sender, RoutedEventArgs e)
    {
        if (TryFindTemplateControl(sender, "PresetSizeDrawer", out AtomDrawer drawer))
        {
            drawer.SizeType = CustomizableSizeType.Small;
            drawer.IsOpen   = true;
        }
    }

    private void HandleOpenMultilevelLevelTwoDrawer(object? sender, RoutedEventArgs e)
    {
        if (TryFindTemplateControl(sender, "MultiLevelDrawerLevelTwo", out AtomDrawer drawer))
        {
            drawer.IsOpen = true;
        }
    }

    private void HandleMultiLevelPlacementChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        var option = args.CheckedOption;
        if (option.IsChecked == true && option.Tag is AtomDrawerPlacement placement)
        {
            if (DataContext is DrawerViewModel vm)
            {
                vm.MultiLevelPlacement = placement;
            }
        }
    }

    private void HandleExtraAndFooterPlacementChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        var option = args.CheckedOption;
        if (option.IsChecked == true && option.Tag is AtomDrawerPlacement placement)
        {
            if (DataContext is DrawerViewModel vm)
            {
                vm.ExtraAndFooterPlacement = placement;
            }
        }
    }

    private void HandleCustomPlacementChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        var option = args.CheckedOption;
        if (option.IsChecked == true && option.Tag is AtomDrawerPlacement placement)
        {
            if (DataContext is DrawerViewModel vm)
            {
                vm.CustomPlacement = placement;
            }
        }
    }

    private static bool TryFindTemplateControl<T>(object? source, string name, out T control)
        where T : Control
    {
        var current = source as Control;
        while (current is not null)
        {
            if (current is T directControl &&
                directControl.Name == name)
            {
                control = directControl;
                return true;
            }

            var descendantControl = FindDescendantByName<T>(current, name);
            if (descendantControl is not null)
            {
                control = descendantControl;
                return true;
            }

            current = current.Parent as Control;
        }

        control = null!;
        return false;
    }

    private static T? FindDescendantByName<T>(Control root, string name)
        where T : Control
    {
        if (root is T typedRoot && typedRoot.Name == name)
        {
            return typedRoot;
        }

        return root.GetVisualDescendants().OfType<T>().FirstOrDefault(control => control.Name == name)
               ?? root.GetLogicalDescendants().OfType<T>().FirstOrDefault(control => control.Name == name);
    }
}
