using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AtomUIGallery.ShowCases.Button;

public partial class ButtonShowCase : GalleryReactiveUserControl<ButtonViewModel>
{
    public const string LanguageId = nameof(ButtonShowCase);

    private ButtonViewModel? _viewModel;
    public ButtonShowCase()
    {
        this.WhenActivated(disposables =>
        {
            _viewModel = DataContext as ButtonViewModel;
        });
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        _viewModel                   = DataContext as ButtonViewModel;
    }

    public void HandleButtonSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (_viewModel != null)
        {
            var sizeType = args.Index switch
            {
                0 => CustomizableSizeType.Large,
                1 => CustomizableSizeType.Middle,
                2 => CustomizableSizeType.Small,
                3 => CustomizableSizeType.Custom,
                _ => _viewModel.ButtonSizeType
            };
            _viewModel.ButtonSizeType = sizeType;
        }
    }

    public void HandleButtonIconPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (_viewModel != null)
        {
            var iconPlacement = args.Index switch
            {
                0 => ButtonIconPlacement.Start,
                1 => ButtonIconPlacement.End,
                _ => _viewModel.ButtonIconPlacement
            };
            _viewModel.ButtonIconPlacement = iconPlacement;
        }
    }

    public void HandleLoadingBtnClick(object? sender, RoutedEventArgs args)
    {
        if (sender is AtomUIButton button)
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
