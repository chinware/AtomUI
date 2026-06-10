using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Breadcrumb;

public partial class BreadcrumbShowCase : GalleryReactiveUserControl<BreadcrumbViewModel>
{
    public const string LanguageId = nameof(BreadcrumbShowCase);

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

                GalleryBindingUtils.OneWay(viewModel, nameof(BreadcrumbViewModel.BreadcrumbItems),
                                           vm => vm.BreadcrumbItems, TplBreadcrumb,
                                           ItemsControl.ItemsSourceProperty)
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
        GetMessageManager()?.Show(new AtomUIMessage(
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
