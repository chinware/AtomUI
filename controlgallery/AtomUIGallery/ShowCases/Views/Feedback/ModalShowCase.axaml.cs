using System.Reactive.Disposables;
using AtomUI;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Avalonia;
using TextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUIGallery.ShowCases.Views;

public partial class ModalShowCase : ReactiveUserControl<ModalViewModel>
{
    public ModalShowCase()
    {
        this.WhenActivated(disposables =>
        {
            BasicOpenModalButton.Click       += HandleBasicModalButtonClick;
            BasicWindowOpenModalButton.Click += HandleBasicWindowModalButtonClick;
            
            LoadingDialogOpenModalButton.Click       += HandleLoadingDialogOpenModalButtonClick;
            AsyncDialogOpenModalButton.Click         += HandleAsyncDialogOpenModalButtonClick;
            CustomFooterDialogOpenButton.Click       += HandleCustomFooterDialogOpenButtonClick;
            DraggableDialogOpenButton.Click          += HandleDraggableMsgBoxOpenButtonClick;
            ConfigureButtonsDialogOpenButton.Click   += HandleConfigureButtonsDialogButtonClick;

            disposables.Add(Disposable.Create(() => BasicOpenModalButton.Click -= HandleBasicModalButtonClick));
            disposables.Add(Disposable.Create(() => BasicWindowOpenModalButton.Click -= HandleBasicWindowModalButtonClick));
            disposables.Add(Disposable.Create(() => LoadingDialogOpenModalButton.Click -= HandleLoadingDialogOpenModalButtonClick));
            disposables.Add(Disposable.Create(() => CustomFooterDialogOpenButton.Click -= HandleCustomFooterDialogOpenButtonClick));
            disposables.Add(Disposable.Create(() => DraggableDialogOpenButton.Click -= HandleDraggableMsgBoxOpenButtonClick));
            disposables.Add(Disposable.Create(() => ConfigureButtonsDialogOpenButton.Click -= HandleConfigureButtonsDialogButtonClick));
            
            ConfigureButtonPropertiesDialog.ButtonsConfigure = ConfigureButtonProperties;
            
            if (DataContext is ModalViewModel viewModel)
            {
                viewModel.CountdownSeconds = 5;
            }
        });
        InitializeComponent();
    }

    private void HandleBasicModalButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ModalViewModel viewModel)
        {
            viewModel.IsBasicModalOpened = true;
        }
    }
    
    private void HandleBasicWindowModalButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ModalViewModel viewModel)
        {
            viewModel.IsBasicWindowModalOpened = true;
        }
    }
    private void HandleLoadingDialogOpenModalButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ModalViewModel viewModel)
        {
            viewModel.IsLoadingMsgBoxOpened = true;
        }
    }

    private void HandleLoadingDialogOpened(object? sender, EventArgs e)
    {
        if (sender is Dialog dialog)
        {
            DispatcherTimer.RunOnce(() =>
            {
                dialog.IsLoading = false;
            }, TimeSpan.FromMilliseconds(3000));
        }
    }

    private void HandleLoadingDialogButtonClicked(object? sender, DialogButtonClickedEventArgs e)
    {
        if (sender is Dialog dialog)
        {
            dialog.IsLoading = true;
            DispatcherTimer.RunOnce(() =>
            {
                dialog.IsLoading = false;
            }, TimeSpan.FromMilliseconds(3000));
        }
    }
    
    private void HandleAsyncDialogOpenModalButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ModalViewModel viewModel)
        {
            viewModel.IsAsyncDialogOpened = true;
        }
    }
    
    private void HandleAsyncDialogButtonClicked(object? sender, DialogButtonClickedEventArgs e)
    {
        if (sender is Dialog dialog && e.SourceButton.Role == DialogButtonRole.AcceptRole)
        {
            dialog.IsConfirmLoading = true;
            e.Handled               = true;
            DispatcherTimer.RunOnce(() =>
            {
                dialog.IsConfirmLoading = false;
                dialog.Done();
            }, TimeSpan.FromMilliseconds(3000));
        }
    }

    private void HandleCustomFooterDialogOpenButtonClick(object? sender, EventArgs e)
    {
        if (DataContext is ModalViewModel viewModel)
        {
            viewModel.IsCustomFooterDialogOpened = true;
        }
    }

    private void HandleDraggableMsgBoxOpenButtonClick(object? sender, EventArgs e)
    {
        if (DataContext is ModalViewModel viewModel)
        {
            viewModel.IsDraggableMsgBoxOpened = true;
        }
    }
    
    private void HandleConfigureButtonsDialogButtonClick(object? sender, EventArgs e)
    {
        if (DataContext is ModalViewModel viewModel)
        {
            viewModel.IsConfigureButtonsDialogOpened = true;
        }
    }

    private void ConfigureButtonProperties(IReadOnlyList<DialogButton> buttons)
    {
        foreach (var button in buttons)
        {
            button.IsEnabled = false;
        }
    }
    
    private async void HandleOpenOverlayDialogButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            // 鼠标会被卡死，强制刷新一次事件循环
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
            var content = BuildDialogContent();
            var options = new DialogOptions
            {
                Title                     = "Basic Modal",
                IsResizable               = false,
                IsDragMovable             = true,
                IsMaximizable             = false,
                StandardButtons           = DialogStandardButtons.Parse("Cancel,Ok"),
                DefaultStandardButton     = DialogStandardButton.Ok,
                HorizontalStartupLocation = DialogHorizontalAnchor.Center,
                VerticalOffset            = new Dimension(30, DimensionUnitType.Percentage),
                HostMinWidth              = 400,
                PlacementTarget = OpenOverlayDialogAPIButton
            };
            await Dialog.ShowDialogModalAsync(content, null, options);
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Modal demo failed: {ex}"); }
    }

    private async void HandleOpenWindowDialogButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            // 鼠标会被卡死，强制刷新一次事件循环
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
            var content = BuildDialogContent();
            var options = new DialogOptions
            {
                Title                     = "Basic Modal",
                IsResizable               = false,
                IsDragMovable             = true,
                IsMaximizable             = false,
                DialogHostType            = DialogHostType.Window,
                StandardButtons           = DialogStandardButtons.Parse("Cancel,Ok"),
                DefaultStandardButton     = DialogStandardButton.Ok,
                HorizontalStartupLocation = DialogHorizontalAnchor.Center,
                VerticalOffset            = new Dimension(30, DimensionUnitType.Percentage),
                HostMinWidth              = 400
            };
            await Dialog.ShowDialogModalAsync(content, null, options);
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Modal demo failed: {ex}"); }
    }

    private async void HandleOpenCustomViewDialogButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            // 鼠标会被卡死，强制刷新一次事件循环
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
            var options = new DialogOptions
            {
                Title                     = "Basic Modal",
                IsResizable               = false,
                IsDragMovable             = true,
                IsMaximizable             = false,
                DialogHostType            = DialogHostType.Window,
                StandardButtons           = DialogStandardButtons.Parse("Cancel,Ok"),
                DefaultStandardButton     = DialogStandardButton.Ok,
                HorizontalStartupLocation = DialogHorizontalAnchor.Center,
                VerticalOffset            = new Dimension(30, DimensionUnitType.Percentage),
                HostMinWidth              = 400
            };
            var viewModel = new ModalUserControlViewModel()
            {
                Name = "AtomUI",
                Age  = 2
            };
            await Dialog.ShowDialogModalAsync<ModalUserControlView, ModalUserControlViewModel>(viewModel, options);
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Modal demo failed: {ex}"); }
    }

    private Control BuildDialogContent()
    {
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 5
        };
        stackPanel.Children.Add(new TextBlock
        {
            Text = "Some contents..."
        });
        stackPanel.Children.Add(new TextBlock
        {
            Text = "Some contents..."
        });
        stackPanel.Children.Add(new TextBlock
        {
            Text = "Some contents..."
        });
        return stackPanel;
    }
}
