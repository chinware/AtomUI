using System.Collections;
using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Transfer;

public class TransferItemDecoratorTests
{
    static TransferItemDecoratorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void SelectionsIconTemplate_Change_Propagates_To_Current_TransferView()
    {
        var decorator    = new TransferItemDecorator();
        var transferView = new RecordingTransferView();
        var icon         = new PathIcon();

        ShowInWindow(decorator, () =>
        {
            decorator.Content = transferView;
            Dispatcher.UIThread.RunJobs();

            decorator.SelectionsIconTemplate = new IconFuncTemplate(() => icon);
            Dispatcher.UIThread.RunJobs();

            decorator.SelectionsIcon.ShouldBeSameAs(icon);
            transferView.SelectionsIcon.ShouldBeSameAs(icon);
        });
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 240,
            Content = content
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

#pragma warning disable CS0067
    private sealed class RecordingTransferView : Control, ITransferView
    {
        public IList<EntityKey>? SelectedKeys { get; set; }
        public int ItemCount => 0;
        public bool IsSupportItemTemplate => true;
        public bool IsSupportPagination => true;
        public TransferViewType ViewType { get; set; }
        public PathIcon? SelectionsIcon { get; private set; }

        public event EventHandler<TransferItemsRemovedEventArgs>? ItemsRemoved;
        public event EventHandler<ItemCountChangedEventArgs>? ItemCountChanged;
        public event EventHandler<SelectionCountChangedEventArgs>? SelectionCountChanged;
        public event EventHandler? SelectedKeyChanged;

        public void SelectAll()
        {
        }

        public void DeselectAll()
        {
        }

        public void NotifyAboutToTransfer(TransferDirection transferDirection)
        {
        }

        public void NotifyTransferCompleted(TransferDirection transferDirection)
        {
        }

        public void NotifySelectAction(TransferSelectAction selectAction)
        {
        }

        public void NotifyIsOneWay(bool isOneWay)
        {
        }

        public void SetSelectionEnabled(bool enabled)
        {
        }

        public void SetSelectionsIcon(PathIcon? icon)
        {
            SelectionsIcon = icon;
        }

        public void SetItemsSource(IEnumerable? itemsSource)
        {
        }

        public void SetItemTemplate(IDataTemplate? itemTemplate)
        {
        }

        public void SetPaginationEnabled(bool enabled)
        {
        }

        public void SetPageSize(int pageSize)
        {
        }
    }
#pragma warning restore CS0067
}
