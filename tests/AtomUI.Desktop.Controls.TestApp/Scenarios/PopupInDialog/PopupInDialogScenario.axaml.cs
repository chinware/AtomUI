using AtomUI.Controls.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls.TestApp.Scenarios.PopupInDialog;

using AtomButton = AtomUI.Desktop.Controls.Button;
using AtomComboBox = AtomUI.Desktop.Controls.ComboBox;
using AtomMenuItem = AtomUI.Desktop.Controls.MenuItem;

public partial class PopupInDialogScenario : UserControl
{
    public PopupInDialogScenario()
    {
        InitializeComponent();
        DialogComboBox.PropertyChanged += HandleDialogComboBoxPropertyChanged;
    }

    private void HandleOpenBasicDialog(object? sender, RoutedEventArgs e)
    {
        BasicDialog.IsOpen = true;
        Dispatcher.UIThread.Post(() =>
        {
            BasicOwnershipText.Text = DescribeTopLevel(DialogComboBox);
            LastStatusText.Text = "已打开原始 ComboBox Dialog";
        });
    }

    private void HandleDialogComboBoxSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DialogComboBox.SelectedItem is not AtomUI.Desktop.Controls.ComboBoxItem item)
        {
            return;
        }

        var selection = item.Content?.ToString() ?? "尚未选择";
        DialogSelectionText.Text = selection;
        LastStatusText.Text = $"ComboBox 选择：{selection}";
    }

    private void HandleOpenDropDownFromCode(object? sender, RoutedEventArgs e)
    {
        DialogComboBox.SetCurrentValue(AtomComboBox.IsDropDownOpenProperty, true);
    }

    private void HandleDialogComboBoxPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property != AtomComboBox.IsDropDownOpenProperty)
        {
            return;
        }

        DropDownStateText.Text = $"IsDropDownOpen = {DialogComboBox.IsDropDownOpen}";
    }

    private void HandleOpenSelectorFamily(object? sender, RoutedEventArgs e)
    {
        var comboBox = new AtomComboBox { Width = 320, PlaceholderText = "ComboBox" };
        comboBox.Items.Add(new AtomUI.Desktop.Controls.ComboBoxItem { Content = "Combo A" });
        comboBox.Items.Add(new AtomUI.Desktop.Controls.ComboBoxItem { Content = "Combo B" });

        var select = new AtomUI.Desktop.Controls.Select
        {
            Width = 320,
            PlaceholderText = "Select",
            OptionsSource =
            [
                new AtomUI.Desktop.Controls.SelectOption { Header = "Select A" },
                new AtomUI.Desktop.Controls.SelectOption { Header = "Select B" }
            ]
        };
        var cascader = new AtomUI.Desktop.Controls.Cascader
        {
            Width = 320,
            PlaceholderText = "Cascader",
            OptionsSource = new AtomUI.Desktop.Controls.ICascaderOption[]
            {
                new AtomUI.Desktop.Controls.CascaderOption { Header = "Cascader A", Value = "A", IsLeaf = true },
                new AtomUI.Desktop.Controls.CascaderOption { Header = "Cascader B", Value = "B", IsLeaf = true }
            }
        };
        var treeSelect = new AtomUI.Desktop.Controls.TreeSelect
        {
            Width = 320,
            PlaceholderText = "TreeSelect",
            ItemsSource =
            [
                new AtomUI.Desktop.Controls.TreeItemNode { Header = "Tree A", Value = "A", IsLeaf = true },
                new AtomUI.Desktop.Controls.TreeItemNode { Header = "Tree B", Value = "B", IsLeaf = true }
            ]
        };

        OpenFamilyDialog("选择器", [
            CreateLabeledControl("ComboBox", comboBox),
            CreateLabeledControl("Select", select),
            CreateLabeledControl("Cascader", cascader),
            CreateLabeledControl("TreeSelect", treeSelect)
        ]);
    }

    private void HandleOpenSuggestionFamily(object? sender, RoutedEventArgs e)
    {
        var options = new AtomUI.Desktop.Controls.IAutoCompleteOption[]
        {
            new AtomUI.Desktop.Controls.AutoCompleteOption { Header = "Alpha", Content = "Alpha" },
            new AtomUI.Desktop.Controls.AutoCompleteOption { Header = "Alpine", Content = "Alpine" }
        };
        var autoComplete = new AtomUI.Desktop.Controls.AutoComplete
        {
            Width = 320,
            PlaceholderText = "输入 Al",
            OptionsSource = options
        };
        var searchEdit = new AtomUI.Desktop.Controls.AutoCompleteSearchEdit
        {
            Width = 320,
            PlaceholderText = "输入 Al",
            OptionsSource = options
        };
        var textArea = new AtomUI.Desktop.Controls.AutoCompleteTextArea
        {
            Width = 480,
            PlaceholderText = "输入 Al",
            OptionsSource = options
        };
        var mentions = new AtomUI.Desktop.Controls.Mentions
        {
            Width = 480,
            PlaceholderText = "输入 @",
            OptionsSource =
            [
                new AtomUI.Desktop.Controls.MentionOption { Header = "atom", Value = "atom" },
                new AtomUI.Desktop.Controls.MentionOption { Header = "avalonia", Value = "avalonia" }
            ]
        };

        OpenFamilyDialog("自动建议", [
            CreateLabeledControl("AutoComplete", autoComplete),
            CreateLabeledControl("AutoCompleteSearchEdit", searchEdit),
            CreateLabeledControl("AutoCompleteTextArea", textArea),
            CreateLabeledControl("Mentions", mentions)
        ]);
    }

    private void HandleOpenPickerFamily(object? sender, RoutedEventArgs e)
    {
        OpenFamilyDialog("日期 / 颜色 Picker", [
            CreateLabeledControl("DatePicker", new AtomUI.Desktop.Controls.DatePicker { Width = 320 }),
            CreateLabeledControl("TimePicker", new AtomUI.Desktop.Controls.TimePicker { Width = 320 }),
            CreateLabeledControl("RangeDatePicker", new AtomUI.Desktop.Controls.RangeDatePicker { Width = 480 }),
            CreateLabeledControl("RangeTimePicker", new AtomUI.Desktop.Controls.RangeTimePicker { Width = 480 }),
            CreateLabeledControl("ColorPicker", new AtomUI.Desktop.Controls.ColorPicker { Width = 320 }),
            CreateLabeledControl("GradientColorPicker", new AtomUI.Desktop.Controls.GradientColorPicker { Width = 320 })
        ]);
    }

    private void HandleOpenPrimitiveFamily(object? sender, RoutedEventArgs e)
    {
        var directPopupButton = new AtomButton { Content = "Direct Popup" };
        var directPopup = new AtomUI.Desktop.Controls.Popup
        {
            PlacementTarget = directPopupButton,
            IsLightDismissEnabled = true,
            Child = new Border
            {
                Padding = new Thickness(16),
                Child = new TextBlock { Text = "Direct Popup content" }
            }
        };
        directPopupButton.Click += (_, _) => directPopup.IsOpen = !directPopup.IsOpen;
        var directPopupHost = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            Children = { directPopupButton, directPopup }
        };

        var flyoutButton = new AtomButton { Content = "Flyout" };
        var flyout = new AtomUI.Desktop.Controls.Flyout
        {
            Content = new TextBlock { Text = "Flyout content" }
        };
        flyoutButton.Click += (_, _) => flyout.ShowAt(flyoutButton);

        var menuFlyoutButton = new AtomButton { Content = "MenuFlyout" };
        var menuFlyout = CreateMenuFlyout("Menu action A", "Menu action B");
        menuFlyoutButton.Click += (_, _) => menuFlyout.ShowAt(menuFlyoutButton);

        var toolTipButton = new AtomButton { Content = "点击切换 ToolTip" };
        AtomUI.Desktop.Controls.ToolTip.SetTip(toolTipButton, "ToolTip content");
        toolTipButton.Click += (_, _) => AtomUI.Desktop.Controls.ToolTip.SetIsOpen(
            toolTipButton,
            !AtomUI.Desktop.Controls.ToolTip.GetIsOpen(toolTipButton));

        var contextMenuButton = new AtomButton { Content = "右键打开 ContextMenu" };
        var contextMenu = new AtomUI.Desktop.Controls.ContextMenu();
        contextMenu.Items.Add(new AtomMenuItem { Header = "Context action A" });
        contextMenu.Items.Add(new AtomMenuItem { Header = "Context action B" });
        contextMenuButton.ContextMenu = contextMenu;

        var dropdownButton = new AtomUI.Desktop.Controls.DropdownButton
        {
            Content = "DropdownButton",
            TriggerType = AtomUI.Desktop.Controls.FlyoutTriggerType.Click,
            DropdownFlyout = CreateMenuFlyout("Dropdown A", "Dropdown B")
        };
        var splitButton = new AtomUI.Desktop.Controls.SplitButton
        {
            Content = "SplitButton",
            TriggerType = AtomUI.Desktop.Controls.FlyoutTriggerType.Click,
            Flyout = new AtomUI.Desktop.Controls.Flyout
            {
                Content = new TextBlock { Text = "SplitButton Flyout" }
            }
        };
        var popupConfirm = new AtomUI.Desktop.Controls.PopupConfirm
        {
            Content = new AtomButton { Content = "PopupConfirm" },
            Title = "确认操作？",
            ConfirmContent = "点击 OK 或 Cancel 验证关闭。"
        };

        OpenFamilyDialog("Popup / Flyout / ToolTip", [
            CreateLabeledControl("Popup", directPopupHost),
            CreateLabeledControl("Flyout", flyoutButton),
            CreateLabeledControl("MenuFlyout", menuFlyoutButton),
            CreateLabeledControl("ToolTip", toolTipButton),
            CreateLabeledControl("ContextMenu", contextMenuButton),
            CreateLabeledControl("DropdownButton", dropdownButton),
            CreateLabeledControl("SplitButton", splitButton),
            CreateLabeledControl("PopupConfirm", popupConfirm)
        ]);
    }

    private void HandleOpenMenuAndTourFamily(object? sender, RoutedEventArgs e)
    {
        var parentMenuItem = new AtomMenuItem { Header = "Menu parent" };
        parentMenuItem.Items.Add(new AtomMenuItem { Header = "Menu child A" });
        parentMenuItem.Items.Add(new AtomMenuItem { Header = "Menu child B" });
        var menu = new AtomUI.Desktop.Controls.Menu { Width = 420 };
        menu.Items.Add(parentMenuItem);

        var navParent = new AtomUI.Desktop.Controls.NavMenuNode { Header = "Nav parent" };
        navParent.Children.Add(new AtomUI.Desktop.Controls.NavMenuNode { Header = "Nav child A" });
        navParent.Children.Add(new AtomUI.Desktop.Controls.NavMenuNode { Header = "Nav child B" });
        var navMenu = new AtomUI.Desktop.Controls.NavMenu
        {
            Width = 320,
            Mode = AtomUI.Desktop.Controls.NavMenuMode.Vertical,
            ShouldUseOverlayPopup = true
        };
        navMenu.Items.Add(navParent);

        var tourTarget = new AtomButton { Content = "Tour target", Width = 160 };
        var tour = new AtomUI.Desktop.Controls.Tour { IsShowMask = false };
        tour.Steps.Add(new AtomUI.Desktop.Controls.TourStep
        {
            Target = tourTarget,
            Title = "Tour in Dialog",
            Content = "关闭后 Dialog 必须保持可交互。"
        });
        var tourButton = new AtomButton { Content = "打开 Tour" };
        tourButton.Click += (_, _) => tour.IsOpen = true;
        var tourHost = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            Children = { tourTarget, tourButton, tour }
        };

        OpenFamilyDialog("Menu / NavMenu / Tour", [
            CreateLabeledControl("Menu / MenuItem", menu),
            CreateLabeledControl("NavMenu", navMenu),
            CreateLabeledControl("Tour", tourHost)
        ]);
    }

    private void HandleOpenDelegatedConsumerFamily(object? sender, RoutedEventArgs e)
    {
        var avatarGroup = new AtomUI.Desktop.Controls.AvatarGroup
        {
            MaxDisplayCount = 2,
            FoldAvatarFlyoutTriggerType = AtomUI.Desktop.Controls.FlyoutTriggerType.Click
        };
        avatarGroup.Children.Add(new AtomUI.Desktop.Controls.Avatar { Text = "A" });
        avatarGroup.Children.Add(new AtomUI.Desktop.Controls.Avatar { Text = "B" });
        avatarGroup.Children.Add(new AtomUI.Desktop.Controls.Avatar { Text = "C" });
        avatarGroup.Children.Add(new AtomUI.Desktop.Controls.Avatar { Text = "D" });

        var transferItems = Enumerable.Range(1, 6)
                                      .Select(index => new ListItemData
                                      {
                                          ItemKey = $"transfer-{index}",
                                          Content = $"Transfer item {index}"
                                      })
                                      .ToList();
        var transfer = new AtomUI.Desktop.Controls.ListTransfer
        {
            Width = 700,
            Height = 260,
            PageSize = 3,
            ItemsSource = transferItems
        };

        var tabs = new AtomUI.Desktop.Controls.TabControl { Width = 360 };
        foreach (var index in Enumerable.Range(1, 12))
        {
            tabs.Items.Add(new AtomUI.Desktop.Controls.TabItem
            {
                Header = $"Long tab {index}",
                Content = $"Tab content {index}"
            });
        }

        var dataGrid = new AtomUI.Desktop.Controls.DataGrid
        {
            AutoGenerateColumns = false,
            CanUserFilterColumns = true,
            Width = 680,
            Height = 240,
            ItemsSource = new[]
            {
                new PopupGridRow("Alpha", "London"),
                new PopupGridRow("Beta", "New York"),
                new PopupGridRow("Gamma", "Tokyo")
            }
        };
        dataGrid.Columns.Add(new AtomUI.Desktop.Controls.DataGridTextColumn
        {
            Header = "Name (click filter icon)",
            Binding = new Binding(nameof(PopupGridRow.Name)),
            FilterMemberPath = nameof(PopupGridRow.Name),
            Filters = new[]
            {
                new AtomUI.Desktop.Controls.DataGridFilterItem { Text = "Alpha", Value = "Alpha" },
                new AtomUI.Desktop.Controls.DataGridFilterItem { Text = "Beta", Value = "Beta" },
                new AtomUI.Desktop.Controls.DataGridFilterItem { Text = "Gamma", Value = "Gamma" }
            },
            Width = new AtomUI.Desktop.Controls.DataGridLength(1, AtomUI.Desktop.Controls.DataGridLengthUnitType.Star)
        });
        dataGrid.Columns.Add(new AtomUI.Desktop.Controls.DataGridTextColumn
        {
            Header = "Address",
            Binding = new Binding(nameof(PopupGridRow.Address)),
            Width = new AtomUI.Desktop.Controls.DataGridLength(1, AtomUI.Desktop.Controls.DataGridLengthUnitType.Star)
        });

        OpenFamilyDialog("间接 Popup 消费者", [
            CreateLabeledControl("AvatarGroup（点击 +2）", avatarGroup),
            CreateLabeledControl("Transfer（点击标题栏选择箭头）", transfer),
            CreateLabeledControl("TabControl（点击溢出菜单）", tabs),
            CreateLabeledControl("DataGrid（点击列头过滤图标）", dataGrid)
        ]);
    }

    private void OpenFamilyDialog(string title, IEnumerable<Control> controls)
    {
        FamilyContentHost.Children.Clear();
        var ownershipText = new TextBlock
        {
            Text = "TopLevel：Dialog 尚未附着",
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        };
        FamilyContentHost.Children.Add(new TextBlock
        {
            FontSize = 20,
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            Text = title
        });
        FamilyContentHost.Children.Add(new TextBlock
        {
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Text = "逐个点击弹层；选择或关闭后继续操作同一 Dialog。"
        });
        FamilyContentHost.Children.Add(ownershipText);
        foreach (var control in controls)
        {
            FamilyContentHost.Children.Add(control);
        }

        FamilyDialog.Title = title;
        FamilyDialog.IsOpen = true;
        LastStatusText.Text = $"已打开 {title} Dialog";
        Dispatcher.UIThread.Post(() => ownershipText.Text = DescribeTopLevel(FamilyContentHost));
    }

    private static Control CreateLabeledControl(string label, Control control)
    {
        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                new TextBlock { FontWeight = Avalonia.Media.FontWeight.SemiBold, Text = label },
                control
            }
        };
    }

    private static AtomUI.Desktop.Controls.MenuFlyout CreateMenuFlyout(params string[] headers)
    {
        var flyout = new AtomUI.Desktop.Controls.MenuFlyout();
        foreach (var header in headers)
        {
            flyout.Items.Add(new AtomMenuItem { Header = header });
        }

        return flyout;
    }

    private static string DescribeTopLevel(Control control)
    {
        var topLevel = TopLevel.GetTopLevel(control);
        return topLevel is null
            ? "TopLevel：<none>（失败）"
            : $"TopLevel：{topLevel.GetType().Name}（已附着）";
    }
}

[GenerateDataMemberAccessors]
internal sealed partial record PopupGridRow(string Name, string Address);
