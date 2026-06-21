using AtomUI.Controls.Primitives;
using AtomUI.Icons.AntDesign;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.TreeSelectControl;

public class TreeSelectBehaviorTests
{
    static TreeSelectBehaviorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Template_Binds_Right_AddOn_Count_And_Handle_State_From_Axaml()
    {
        var jack = new TreeItemNode
        {
            Header = "Jack",
            Value  = "jack"
        };
        var lucy = new TreeItemNode
        {
            Header = "Lucy",
            Value  = "lucy"
        };
        var rightAddOn  = new TextBlock { Text = "extra" };
        var suffixIcon  = new DownOutlined();
        var loadingIcon = new LoadingOutlined();
        var treeSelect = new Desktop.Controls.TreeSelect
        {
            Width                   = 240,
            IsMultiple              = true,
            IsFilterEnabled         = true,
            IsAllowClear            = true,
            IsMotionEnabled         = false,
            MaxCount                = 3,
            IsShowMaxCountIndicator = true,
            SelectedItems           = [jack, lucy],
            ContentRightAddOn       = rightAddOn,
            SuffixIcon              = suffixIcon,
            SuffixLoadingIcon       = loadingIcon
        };

        ShowInWindow(treeSelect, () =>
        {
            var indicator = GetVisualDescendant<SelectMaxCountIndicator>(treeSelect, "PART_SelectMaxCountIndicator");
            indicator.MaxCount.ShouldBe(3);
            indicator.SelectedCount.ShouldBe(2);
            indicator.IsVisible.ShouldBeTrue();

            var contentPresenter = GetVisualDescendant<ContentPresenter>(treeSelect, "PART_ContentRightAddOnPresenter");
            contentPresenter.Content.ShouldBeSameAs(rightAddOn);
            contentPresenter.IsVisible.ShouldBeTrue();

            var handle = GetVisualDescendant<SelectHandle>(treeSelect, "PART_SelectHandle");
            handle.OpenIndicator.ShouldBeSameAs(suffixIcon);
            handle.LoadingIcon.ShouldBeSameAs(loadingIcon);
            handle.IsFilterEnabled.ShouldBeTrue();
            handle.IsMotionEnabled.ShouldBeFalse();
            handle.IsAllowClear.ShouldBeTrue();
            handle.IsSelectionEmpty.ShouldBeFalse();
        });
    }

    [Fact]
    public void Template_Relays_AddOnDecoratedBox_Input_State_To_Handle()
    {
        var treeSelect = new Desktop.Controls.TreeSelect
        {
            Width = 240
        };

        ShowInWindow(treeSelect, () =>
        {
            var addOnBox = GetVisualDescendant<AddOnDecoratedBox>(
                treeSelect,
                AddOnDecoratedBox.AddOnDecoratedBoxPart);
            var handle = GetVisualDescendant<SelectHandle>(treeSelect, "PART_SelectHandle");

            addOnBox.IsInnerBoxHover   = true;
            addOnBox.IsInnerBoxPressed = true;

            handle.IsInputHover.ShouldBeTrue();
            handle.IsInputPressed.ShouldBeTrue();
        });
    }

    [Fact]
    public void Custom_Size_Single_Selected_Text_Is_Vertically_Centered()
    {
        var selectedNode = new TreeItemNode
        {
            Header = "My leaf",
            Value  = "my-leaf"
        };
        var treeSelect = new Desktop.Controls.TreeSelect
        {
            Width         = 240,
            Height        = 38,
            FontSize      = 15,
            SizeType      = CustomizableSizeType.Custom,
            IsAllowClear  = true,
            SelectedItem  = selectedNode,
            ItemsSource   = [selectedNode],
            PlaceholderText = "Please select"
        };

        ShowInWindow(treeSelect, () =>
        {
            var addOnBox = GetVisualDescendant<AddOnDecoratedBox>(
                treeSelect,
                AddOnDecoratedBox.AddOnDecoratedBoxPart);
            var textPresenter = GetVisualDescendant<TextPresenter>(treeSelect, "PART_TextPresenter");

            var textTop = textPresenter.TranslatePoint(default, addOnBox);
            textTop.ShouldNotBeNull();

            var textCenterY = textTop.Value.Y + textPresenter.Bounds.Height / 2;
            var boxCenterY  = addOnBox.Bounds.Height / 2;

            Math.Abs(textCenterY - boxCenterY).ShouldBeLessThanOrEqualTo(1.0);
        });
    }

    private static T GetVisualDescendant<T>(Control control, string name)
        where T : Control
    {
        var descendant = control.GetVisualDescendants()
                                .OfType<T>()
                                .SingleOrDefault(x => x.Name == name);
        descendant.ShouldNotBeNull();
        return descendant;
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var overlayPanel = new ScopeAwareOverlayLayerPanel
        {
            Width  = 420,
            Height = 320
        };
        overlayPanel.Children.Add(content);

        var visualLayerManager = new VisualLayerManager
        {
            Child = overlayPanel
        };
        EnablePopupOverlayLayer(visualLayerManager);

        var window = new AvaloniaWindow
        {
            Width   = 420,
            Height  = 320,
            Content = visualLayerManager
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }

    private static void EnablePopupOverlayLayer(VisualLayerManager visualLayerManager)
    {
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }
}
