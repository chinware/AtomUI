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
using ToggleSwitch = AtomUI.Desktop.Controls.ToggleSwitch;

namespace AtomUIGallery.ShowCases.Views;

public partial class MessageBoxShowCase : ReactiveUserControl<MessageBoxViewModel>
{
    public MessageBoxShowCase()
    {
        this.WhenActivated(disposables =>
        {
            ConfirmMsgBoxBtn.Click                   += HandleConfirmMsgBoxBtnClick;
            InformationMsgBoxBtn.Click               += HandleInformationMsgBoxBtnClick;
            SuccessMsgBoxBtn.Click                   += HandleSuccessMsgBoxBtnClick;
            ErrorMsgBoxBtn.Click                     += HandleErrorMsgBoxBtnClick;
            WarningMsgBoxBtn.Click                   += HandleWarningMsgBoxBtnClick;
            StyleCaseHostTypeSwitch.IsCheckedChanged += HandleStyleCaseHostTypeSwitchChanged;
            CustomFooterMsgBoxOpenButton.Click       += HandleCustomFooterMsgBoxOpenButtonClick;
            DelayedCloseMsgBoxOpenButton.Click       += HandleDelayedCloseMsgBoxOpenButtonClick;

            disposables.Add(Disposable.Create(() => ConfirmMsgBoxBtn.Click -= HandleConfirmMsgBoxBtnClick));
            disposables.Add(Disposable.Create(() => InformationMsgBoxBtn.Click -= HandleInformationMsgBoxBtnClick));
            disposables.Add(Disposable.Create(() => SuccessMsgBoxBtn.Click -= HandleSuccessMsgBoxBtnClick));
            disposables.Add(Disposable.Create(() => ErrorMsgBoxBtn.Click -= HandleErrorMsgBoxBtnClick));
            disposables.Add(Disposable.Create(() => WarningMsgBoxBtn.Click -= HandleWarningMsgBoxBtnClick));
            disposables.Add(Disposable.Create(() => StyleCaseHostTypeSwitch.IsCheckedChanged -= HandleStyleCaseHostTypeSwitchChanged));
            disposables.Add(Disposable.Create(() => CustomFooterMsgBoxOpenButton.Click -= HandleCustomFooterMsgBoxOpenButtonClick));
            disposables.Add(Disposable.Create(() => DelayedCloseMsgBoxOpenButton.Click -= HandleDelayedCloseMsgBoxOpenButtonClick));

            if (DataContext is MessageBoxViewModel viewModel)
            {
                viewModel.MessageBoxStyleCaseHostType = DialogHostType.Overlay;
                viewModel.CountdownSeconds            = 5;
            }
        });
        InitializeComponent();
    }

    private void HandleConfirmMsgBoxBtnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MessageBoxViewModel viewModel)
        {
            viewModel.IsConfirmMsgBoxOpened = true;
        }
    }

    private void HandleInformationMsgBoxBtnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MessageBoxViewModel viewModel)
        {
            viewModel.IsInformationMsgBoxOpened = true;
        }
    }

    private void HandleSuccessMsgBoxBtnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MessageBoxViewModel viewModel)
        {
            viewModel.IsSuccessMsgBoxOpened = true;
        }
    }

    private void HandleErrorMsgBoxBtnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MessageBoxViewModel viewModel)
        {
            viewModel.IsErrorMsgBoxOpened = true;
        }
    }

    private void HandleWarningMsgBoxBtnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MessageBoxViewModel viewModel)
        {
            viewModel.IsWarningMsgBoxOpened = true;
        }
    }

    private void HandleStyleCaseHostTypeSwitchChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            if (DataContext is MessageBoxViewModel viewModel)
            {
                viewModel.MessageBoxStyleCaseHostType = toggleSwitch.IsChecked == true ? DialogHostType.Window : DialogHostType.Overlay;
            }
        }
    }

    private void HandleCustomFooterMsgBoxOpenButtonClick(object? sender, EventArgs e)
    {
        if (DataContext is MessageBoxViewModel viewModel)
        {
            viewModel.IsCustomFooterMsgBoxOpened = true;
        }
    }

    private void HandleDelayedCloseMsgBoxOpenButtonClick(object? sender, EventArgs e)
    {
        if (DataContext is MessageBoxViewModel viewModel)
        {
            viewModel.IsDelayedCloseMsgBoxOpened = true;
        }
    }

    private IDisposable? _delayedCloseDialogDisposal;

    private void HandleDelayedCloseMsgBoxOpened(object? sender, EventArgs e)
    {
        if (sender is MessageBox messageBox)
        {
            if (DataContext is MessageBoxViewModel viewModel)
            {
                viewModel.CountdownSeconds = 5;
                _delayedCloseDialogDisposal?.Dispose();
                _delayedCloseDialogDisposal = DispatcherTimer.Run(() =>
                {
                    if (viewModel.CountdownSeconds == 0)
                    {
                        messageBox.Confirm();
                        return false;
                    }
                    viewModel.CountdownSeconds--;
                    return true;
                }, TimeSpan.FromMilliseconds(1000));
            }
        }
    }

    private async void HandleCreateConfirmMessageBox(object? sender, RoutedEventArgs e)
    {
        try
        {
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
            var content = BuildMessageBoxContent();
            var options = new MessageBoxOptions
            {
                Title             = "Do you want to delete these items?",
                IsDragMovable     = true,
                IsCenterOnStartup = true,
                Style             = MessageBoxStyle.Confirm
            };
            await MessageBox.ShowMessageModalAsync(content, null, options);
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Modal demo failed: {ex}"); }
    }

    private async void HandleCreateInformationMessageBox(object? sender, RoutedEventArgs e)
    {
        try
        {
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
            var content = BuildMessageBoxContent();
            var options = new MessageBoxOptions
            {
                Title             = "This is a notification message",
                IsDragMovable     = true,
                IsCenterOnStartup = true,
                Style             = MessageBoxStyle.Information
            };
            await MessageBox.ShowMessageModalAsync(content, null, options);
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Modal demo failed: {ex}"); }
    }

    private async void HandleCreateSuccessMessageBox(object? sender, RoutedEventArgs e)
    {
        try
        {
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
            var content = BuildMessageBoxContent();
            var options = new MessageBoxOptions
            {
                Title             = "Operation successful",
                IsDragMovable     = true,
                IsCenterOnStartup = true,
                Style             = MessageBoxStyle.Success
            };
            await MessageBox.ShowMessageModalAsync(content, null, options);
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Modal demo failed: {ex}"); }
    }

    private async void HandleCreateErrorMessageBox(object? sender, RoutedEventArgs e)
    {
        try
        {
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
            var content = BuildMessageBoxContent();
            var options = new MessageBoxOptions
            {
                Title             = "This is an error message",
                IsDragMovable     = true,
                IsCenterOnStartup = true,
                Style             = MessageBoxStyle.Error
            };
            await MessageBox.ShowMessageModalAsync(content, null, options);
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Modal demo failed: {ex}"); }
    }

    private async void HandleCreateWarningMessageBox(object? sender, RoutedEventArgs e)
    {
        try
        {
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
            var content = BuildMessageBoxContent();
            var options = new MessageBoxOptions
            {
                Title             = "This is a warning message",
                IsDragMovable     = true,
                IsCenterOnStartup = true,
                Style             = MessageBoxStyle.Warning
            };
            await MessageBox.ShowMessageModalAsync(content, null, options);
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Modal demo failed: {ex}"); }
    }

    private Control BuildMessageBoxContent()
    {
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 5
        };
        stackPanel.Children.Add(new TextBlock
        {
            Text = "some messages...some messages..."
        });
        stackPanel.Children.Add(new TextBlock
        {
            Text = "some messages...some messages..."
        });
        return stackPanel;
    }
}
