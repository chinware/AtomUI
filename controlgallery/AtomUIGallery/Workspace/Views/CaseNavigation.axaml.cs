using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using AtomUIGallery.Workspace.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using ReactiveUI;
using Window = Avalonia.Controls.Window;

namespace AtomUIGallery.Workspace.Views;

public partial class CaseNavigation : UserControl
{
    public const string LanguageId = nameof(CaseNavigation);
    
    public CaseNavigation()
    {
        InitializeComponent();
        ShowCaseNavMenu.NavMenuItemClick += HandleNavMenuItemClick;
    }

    private void HandleNavMenuItemClick(object? sender, NavMenuItemClickEventArgs args)
    {
        if (DataContext is CaseNavigationViewModel caseNavigationViewModel)
        {
            var showCaseId = args.NavMenuItem.ItemKey;
            if (showCaseId is null)
            {
                // TODO 是不是可以跳转到默认的 ShowCase 页面
                return;
            }

            caseNavigationViewModel.NavigateTo(showCaseId.Value.ToString());
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        var current = Parent;
        while (current is not null)
        {
            if (current.DataContext is IScreen screen)
            {
                DataContext = new CaseNavigationViewModel(screen);
            }
            current = current.Parent;
        }
        if (DataContext is CaseNavigationViewModel caseNavigationViewModel)
        {
            caseNavigationViewModel.NavigateTo(AboutUsViewModel.ID);
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is Window window)
        {
            window.AddHandler(InputElement.KeyDownEvent, OnGlobalKeyDown, RoutingStrategies.Tunnel);
        }
    }
    
    private void OnGlobalKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is CaseNavigationViewModel caseNavigationViewModel)
        {
            if (e.Key == Key.F5)
            {
                caseNavigationViewModel.TestNavigatePages(TimeSpan.FromMilliseconds(300));
                e.Handled = true;
            }
            else if (e.Key == Key.F6)
            {
                caseNavigationViewModel.StopTestNavigatePages();
            }
        }
    }
}