using System;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIComboBox = AtomUI.Desktop.Controls.ComboBox;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Pagination;

public class PaginationPageSizeTests
{
    static PaginationPageSizeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void SimplePagination_Allows_Custom_PageSize()
    {
        var pagination = new SimplePagination
        {
            Total = 10
        };

        var exception = Record.Exception(() => pagination.PageSize = 3);

        exception.ShouldBeNull();
        pagination.PageSize.ShouldBe(3);
        pagination.PageCount.ShouldBe(4);
    }

    [Fact]
    public void Pagination_Allows_Custom_PageSize()
    {
        var pagination = new AtomUI.Desktop.Controls.Pagination
        {
            Total = 10
        };

        var exception = Record.Exception(() => pagination.PageSize = 3);

        exception.ShouldBeNull();
        pagination.PageSize.ShouldBe(3);
        pagination.PageCount.ShouldBe(4);
    }

    [Fact]
    public void Pagination_Rejects_Negative_PageSize()
    {
        var pagination = new AtomUI.Desktop.Controls.Pagination();

        Should.Throw<ArgumentException>(() => pagination.PageSize = -1);
    }

    [Fact]
    public void Pagination_SizeChanger_Includes_And_Selects_Custom_PageSize()
    {
        var pagination = new AtomUI.Desktop.Controls.Pagination
        {
            Total             = 10,
            PageSize          = 3,
            IsShowSizeChanger = true,
            IsMotionEnabled   = false
        };

        ShowInWindow(pagination, () =>
        {
            var sizeChanger = pagination.GetVisualDescendants()
                                        .OfType<AtomUIComboBox>()
                                        .Single();
            var pageSizes = sizeChanger.Items
                                       .Select(x => GetPageSize(x!))
                                       .ToArray();

            pageSizes.ShouldBe([3, 10, 20, 50, 100]);
            sizeChanger.SelectedIndex.ShouldBe(0);
            GetPageSize(sizeChanger.SelectedItem!).ShouldBe(3);
        });
    }

    [Fact]
    public void Pagination_SizeChanger_Uses_Configured_PageSizeOptions()
    {
        var pagination = new AtomUI.Desktop.Controls.Pagination
        {
            Total             = 100,
            PageSize          = 5,
            PageSizeOptions   = [5, 15, 30],
            IsShowSizeChanger = true,
            IsMotionEnabled   = false
        };

        ShowInWindow(pagination, () =>
        {
            var sizeChanger = pagination.GetVisualDescendants()
                                        .OfType<AtomUIComboBox>()
                                        .Single();
            var pageSizes = sizeChanger.Items
                                       .Select(x => GetPageSize(x!))
                                       .ToArray();

            pageSizes.ShouldBe([5, 15, 30]);
            GetPageSize(sizeChanger.SelectedItem!).ShouldBe(5);
        });
    }

    [Fact]
    public void Pagination_SizeChanger_Inserts_Current_PageSize_When_Not_In_Configured_Options()
    {
        var pagination = new AtomUI.Desktop.Controls.Pagination
        {
            Total             = 100,
            PageSize          = 3,
            PageSizeOptions   = [5, 10],
            IsShowSizeChanger = true,
            IsMotionEnabled   = false
        };

        ShowInWindow(pagination, () =>
        {
            var sizeChanger = pagination.GetVisualDescendants()
                                        .OfType<AtomUIComboBox>()
                                        .Single();
            var pageSizes = sizeChanger.Items
                                       .Select(x => GetPageSize(x!))
                                       .ToArray();

            pageSizes.ShouldBe([3, 5, 10]);
            GetPageSize(sizeChanger.SelectedItem!).ShouldBe(3);
        });
    }

    [Fact]
    public void Pagination_SizeChanger_Rebuilds_When_PageSizeOptions_Changes()
    {
        var pagination = new AtomUI.Desktop.Controls.Pagination
        {
            Total             = 100,
            PageSize          = 6,
            PageSizeOptions   = [6, 12],
            IsShowSizeChanger = true,
            IsMotionEnabled   = false
        };

        ShowInWindow(pagination, () =>
        {
            var sizeChanger = pagination.GetVisualDescendants()
                                        .OfType<AtomUIComboBox>()
                                        .Single();

            pagination.PageSizeOptions = [3, 9];
            Dispatcher.UIThread.RunJobs();

            var pageSizes = sizeChanger.Items
                                       .Select(x => GetPageSize(x!))
                                       .ToArray();

            pageSizes.ShouldBe([6, 3, 9]);
            GetPageSize(sizeChanger.SelectedItem!).ShouldBe(6);
        });
    }

    [Fact]
    public void Pagination_SizeChanger_Does_Not_Rebuild_Items_During_PageSize_Selection()
    {
        var pagination = new AtomUI.Desktop.Controls.Pagination
        {
            Total             = 100,
            PageSize          = 5,
            PageSizeOptions   = [5, 10, 20],
            IsShowSizeChanger = true,
            IsMotionEnabled   = false
        };

        ShowInWindow(pagination, () =>
        {
            var sizeChanger = pagination.GetVisualDescendants()
                                        .OfType<AtomUIComboBox>()
                                        .Single();

            var exception = Record.Exception(() =>
            {
                sizeChanger.SelectedIndex = 1;
                Dispatcher.UIThread.RunJobs();
            });

            exception.ShouldBeNull();
            pagination.PageSize.ShouldBe(10);
            GetPageSize(sizeChanger.SelectedItem!).ShouldBe(10);
        });
    }

    [Fact]
    public void Pagination_Rejects_Invalid_PageSizeOptions()
    {
        var pagination = new AtomUI.Desktop.Controls.Pagination();

        Should.Throw<ArgumentException>(() => pagination.PageSizeOptions = [0, 10]);
        Should.Throw<ArgumentException>(() => pagination.PageSizeOptions = [10, -20]);
    }

    [Fact]
    public void SimplePagination_Template_Assigns_Next_Item_Type()
    {
        var pagination = new SimplePagination
        {
            Total           = 100,
            CurrentPage     = 2,
            IsMotionEnabled = false
        };

        ShowInWindow(pagination, () =>
        {
            var previousItem = pagination.GetVisualDescendants()
                                         .OfType<PaginationNavItem>()
                                         .Single(item => item.Name == "PART_PreviousNavItem");
            var nextItem = pagination.GetVisualDescendants()
                                     .OfType<PaginationNavItem>()
                                     .Single(item => item.Name == "PART_NextNavItem");

            previousItem.PaginationItemType.ShouldBe(PaginationItemType.Previous);
            nextItem.PaginationItemType.ShouldBe(PaginationItemType.Next);
        });
    }

    [Fact]
    public void SimplePagination_Editable_Jump_Uses_Default_PageSize_When_PageSize_Is_Zero()
    {
        var pagination = new SimplePagination
        {
            Total           = 100,
            CurrentPage     = 1,
            PageSize        = 0,
            IsReadOnly      = false,
            IsMotionEnabled = false
        };

        ShowInWindow(pagination, () =>
        {
            var quickJumper = pagination.GetVisualDescendants()
                                        .OfType<QuickJumpEdit>()
                                        .Single(item => item.Name == "PART_QuickJumper");

            quickJumper.Text = "3";

            var exception = Record.Exception(() =>
            {
                quickJumper.RaiseEvent(new KeyEventArgs
                {
                    RoutedEvent  = InputElement.KeyUpEvent,
                    Source       = quickJumper,
                    Key          = Key.Enter,
                    PhysicalKey  = PhysicalKey.Enter,
                    KeyModifiers = KeyModifiers.None
                });
                Dispatcher.UIThread.RunJobs();
            });

            exception.ShouldBeNull();
            pagination.CurrentPage.ShouldBe(3);
        });
    }

    private static int GetPageSize(object item)
    {
        var pageSizeProperty = item.GetType()
                                   .GetProperty(
                                       "PageSize",
                                       BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        pageSizeProperty.ShouldNotBeNull($"Expected {item.GetType().Name} to expose a PageSize property.");
        return (int)pageSizeProperty.GetValue(item)!;
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        ShowInWindow(content, _ => assertion());
    }

    private static void ShowInWindow(Control content, Action<AvaloniaWindow> assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 220,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion(window);
        }
        finally
        {
            window.Close();
        }
    }

}
