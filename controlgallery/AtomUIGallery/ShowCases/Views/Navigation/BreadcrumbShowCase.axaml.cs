using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class BreadcrumbShowCase : ReactiveUserControl<BreadcrumbViewModel>
{
    private WindowMessageManager? _messageManager;

    public BreadcrumbShowCase()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            if (DataContext is BreadcrumbViewModel viewModel)
            {
                viewModel.BreadcrumbItems = [
                    new BreadcrumbItemData()
                    {
                        Separator = ":",
                        Content = "Location"
                    },
                    new BreadcrumbItemData()
                    {
                        NavigateContext = "#",
                        Content = "Application Center"
                    },
                    new BreadcrumbItemData()
                    {
                        NavigateContext = "#",
                        Content         = "Application List"
                    },
                    new BreadcrumbItemData()
                    {
                        Content         = "An Application"
                    }
                ];

                this.OneWayBind(viewModel, vm => vm.BreadcrumbItems, v => v.TplBreadcrumb.ItemsSource)
                    .DisposeWith(disposables);

                Disposable.Create(() =>
                {
                    viewModel.BreadcrumbItems = null;
                }).DisposeWith(disposables);
            }
        });
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _messageManager?.Dispose();
        _messageManager = null;
    }

    private void HandleNavigateRequest(object? sender, BreadcrumbNavigateEventArgs eventArgs)
    {
        GetMessageManager()?.Show(new Message(
            $"Navigate context: {eventArgs.BreadcrumbItem.NavigateContext}"
        ));
    }

    private WindowMessageManager? GetMessageManager()
    {
        if (_messageManager is not null)
        {
            return _messageManager;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return null;
        }

        _messageManager = new WindowMessageManager(topLevel)
        {
            MaxItems = 10
        };
        return _messageManager;
    }
}
