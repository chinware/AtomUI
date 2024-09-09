using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Button = AtomUI.Controls.Button;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class ButtonShowCase : UserControl
{
    public static readonly StyledProperty<SizeType> ButtonSizeTypeProperty =
        AvaloniaProperty.Register<ButtonShowCase, SizeType>(nameof(ButtonSizeType));

    public ButtonShowCase()
    {
        InitializeComponent();
        DataContext = this;

        ButtonSizeTypeOptionGroup.OptionCheckedChanged += HandleButtonSizeTypeOptionCheckedChanged;
        LoadingBtn1.Click                              += HandleLoadingBtnClick;
        LoadingBtn2.Click                              += HandleLoadingBtnClick;
        LoadingBtn3.Click                              += HandleLoadingBtnClick;
    }

    public SizeType ButtonSizeType
    {
        get => GetValue(ButtonSizeTypeProperty);
        set => SetValue(ButtonSizeTypeProperty, value);
    }

    private void HandleButtonSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
            ButtonSizeType = SizeType.Large;
        else if (args.Index == 1)
            ButtonSizeType = SizeType.Middle;
        else
            ButtonSizeType = SizeType.Small;
    }

    private void HandleLoadingBtnClick(object? sender, RoutedEventArgs args)
    {
        if (sender is Button button)
        {
            button.IsLoading = true;
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                button.IsLoading = false;
            });
        }
    }
}