using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Interactivity;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public class MyTabItemData : TabItemData
{
    public object? Content { get; init; }
}

public partial class TabControlShowCase : ReactiveUserControl<TabControlViewModel>
{
    public TabControlShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is TabControlViewModel viewModel)
            {
                viewModel.TabItemDataSource.Add(new MyTabItemData()
                {
                    Header  = "Tab 1",
                    Content = "Tab Content 1",
                    Icon    = new WechatFilled()
                });

                viewModel.TabItemDataSource.Add(new MyTabItemData()
                {
                    Header     = "Tab 2",
                    Content    = "Tab Content 2",
                    IsClosable = true,
                    Icon       = new LinuxOutlined()
                });

                viewModel.TabStripItemDataSource.Add(new TabItemData()
                {
                    Header = "Tab 1"
                });
                viewModel.TabStripItemDataSource.Add(new TabItemData()
                {
                    Header = "Tab 2"
                });

                PositionTabStripOptionGroup.OptionCheckedChanged     += viewModel.HandleTabStripPlacementOptionCheckedChanged;
                PositionCardTabStripOptionGroup.OptionCheckedChanged += viewModel.HandleCardTabStripPlacementOptionCheckedChanged;
                SizeTypeTabStripOptionGroup.OptionCheckedChanged     += viewModel.HandleTabStripSizeTypeOptionCheckedChanged;
                AddTabDemoStrip.AddTabRequest                        += HandleTabStripAddTabRequest;

                PositionTabControlOptionGroup.OptionCheckedChanged     += viewModel.HandleTabControlPlacementOptionCheckedChanged;
                PositionCardTabControlOptionGroup.OptionCheckedChanged += viewModel.HandleCardTabControlPlacementOptionCheckedChanged;
                SizeTypeTabControlOptionGroup.OptionCheckedChanged     += viewModel.HandleTabControlSizeTypeOptionCheckedChanged;
                AddTabDemoTabControl.AddTabRequest                     += HandleTabControlAddTabRequest;

                Disposable.Create(() =>
                {
                    PositionTabStripOptionGroup.OptionCheckedChanged     -= viewModel.HandleTabStripPlacementOptionCheckedChanged;
                    PositionCardTabStripOptionGroup.OptionCheckedChanged -= viewModel.HandleCardTabStripPlacementOptionCheckedChanged;
                    SizeTypeTabStripOptionGroup.OptionCheckedChanged     -= viewModel.HandleTabStripSizeTypeOptionCheckedChanged;
                    AddTabDemoStrip.AddTabRequest                        -= HandleTabStripAddTabRequest;

                    PositionTabControlOptionGroup.OptionCheckedChanged     -= viewModel.HandleTabControlPlacementOptionCheckedChanged;
                    PositionCardTabControlOptionGroup.OptionCheckedChanged -= viewModel.HandleCardTabControlPlacementOptionCheckedChanged;
                    SizeTypeTabControlOptionGroup.OptionCheckedChanged     -= viewModel.HandleTabControlSizeTypeOptionCheckedChanged;
                    AddTabDemoTabControl.AddTabRequest                     -= HandleTabControlAddTabRequest;

                    viewModel.TabItemDataSource      = new();
                    viewModel.TabStripItemDataSource = new();
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }

    private void HandleTabStripAddTabRequest(object? sender, RoutedEventArgs args)
    {
        var index = AddTabDemoStrip.ItemCount;
        AddTabDemoStrip.Items.Add(new TabStripItem
        {
            Content    = $"new tab {index}",
            IsClosable = true
        });
    }

    private void HandleTabControlAddTabRequest(object? sender, RoutedEventArgs args)
    {
        var index = AddTabDemoTabControl.ItemCount;
        AddTabDemoTabControl.Items.Add(new TabItem
        {
            Header     = $"new tab {index}",
            Content    = $"new tab content {index}",
            IsClosable = true
        });
    }
}
