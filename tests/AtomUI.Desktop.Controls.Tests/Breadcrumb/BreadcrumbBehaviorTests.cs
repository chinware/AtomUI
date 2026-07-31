using Avalonia.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Breadcrumb;

public class BreadcrumbBehaviorTests
{
    [Fact]
    public void BreadcrumbItemData_Content_Is_Applied_Without_ItemTemplate()
    {
        var breadcrumb = new TestBreadcrumb();
        var container  = new BreadcrumbItem();

        breadcrumb.PrepareItem(
            container,
            new BreadcrumbItemData
            {
                Content         = "Home",
                NavigateContext = "home"
            },
            0);

        container.Content.ShouldBe("Home");
        container.NavigateContext.ShouldBe("home");
        container.IsNavigateResponsive.ShouldBeTrue();
    }

    [Fact]
    public void Breadcrumb_Reused_Container_Does_Not_Retain_Previous_ItemData_State()
    {
        var breadcrumb = new TestBreadcrumb();
        var container  = new BreadcrumbItem();
        var icon       = new PathIcon();
        var uri        = new Uri("https://atomui.net/");

        breadcrumb.PrepareItem(
            container,
            new BreadcrumbItemData
            {
                Content         = "First",
                Icon            = icon,
                NavigateContext = "first",
                NavigateUri     = uri,
                Separator       = ":"
            },
            0);

        breadcrumb.ClearItem(container);
        breadcrumb.PrepareItem(
            container,
            new BreadcrumbItemData
            {
                Content = "Second"
            },
            0);

        container.Content.ShouldBe("Second");
        container.Icon.ShouldBeNull();
        container.NavigateContext.ShouldBeNull();
        container.NavigateUri.ShouldBeNull();
        container.Separator.ShouldBe(Desktop.Controls.Breadcrumb.DefaultSeparator);
        container.IsNavigateResponsive.ShouldBeFalse();
    }

    private sealed class TestBreadcrumb : Desktop.Controls.Breadcrumb
    {
        public void PrepareItem(BreadcrumbItem container, object? item, int index)
        {
            PrepareContainerForItemOverride(container, item, index);
        }

        public void ClearItem(BreadcrumbItem container)
        {
            ClearContainerForItemOverride(container);
        }
    }
}
