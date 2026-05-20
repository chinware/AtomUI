using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreatePaginationScenarios()
    {
        return
        [
            new PerfScenario("Pagination.Basic.Total50", _ => CreatePagination(total: 50)),
            new PerfScenario("Pagination.More.Total500", _ => CreatePagination(total: 500, currentPage: 6, showSizeChanger: true)),
            new PerfScenario("Pagination.SizeChanger", _ => CreatePagination(total: 85, pageSize: 20, showSizeChanger: true)),
            new PerfScenario("Pagination.QuickJumper", _ => CreatePagination(total: 500, currentPage: 3, showSizeChanger: true, showQuickJumper: true)),
            new PerfScenario("Pagination.TotalInfo", _ => CreatePagination(total: 85, pageSize: 20, showSizeChanger: true, showTotalInfo: true)),
            new PerfScenario("Pagination.Small", _ => CreatePagination(total: 50, sizeType: SizeType.Small)),
            new PerfScenario("SimplePagination.ReadOnly", _ => CreateSimplePagination()),
            new PerfScenario("SimplePagination.Editable", _ => CreateSimplePagination(readOnly: false)),
            new PerfScenario("Pagination.GalleryShape.PaginationShowCase", _ => CreatePaginationShowCaseShape())
        ];
    }

    private static Pagination CreatePagination(int total,
                                               int currentPage = 1,
                                               int pageSize = AbstractPagination.DefaultPageSize,
                                               bool showSizeChanger = false,
                                               bool showQuickJumper = false,
                                               bool showTotalInfo = false,
                                               bool isEnabled = true,
                                               SizeType sizeType = SizeType.Middle,
                                               PaginationAlign align = PaginationAlign.Start,
                                               string? totalInfoTemplate = null)
    {
        var pagination = new Pagination
        {
            Total             = total,
            CurrentPage       = currentPage,
            PageSize          = pageSize,
            IsShowSizeChanger = showSizeChanger,
            IsShowQuickJumper = showQuickJumper,
            IsShowTotalInfo   = showTotalInfo,
            IsEnabled         = isEnabled,
            SizeType          = sizeType,
            Align             = align
        };

        if (totalInfoTemplate is not null)
        {
            pagination.TotalInfoTemplate = totalInfoTemplate;
        }

        return pagination;
    }

    private static SimplePagination CreateSimplePagination(bool readOnly = true,
                                                           bool isEnabled = true,
                                                           SizeType sizeType = SizeType.Middle)
    {
        return new SimplePagination
        {
            Total       = 50,
            CurrentPage = 1,
            IsReadOnly  = readOnly,
            IsEnabled   = isEnabled,
            SizeType    = sizeType
        };
    }

    private static Control CreatePaginationShowCaseShape()
    {
        var panel = CreateVerticalPanel();

        panel.Children.Add(CreatePagination(total: 50));

        panel.Children.Add(CreateVerticalPanel(
            CreatePagination(total: 50, align: PaginationAlign.Start),
            CreatePagination(total: 50, align: PaginationAlign.Center),
            CreatePagination(total: 50, align: PaginationAlign.End)));

        panel.Children.Add(CreatePagination(total: 500, currentPage: 6, showSizeChanger: true));

        panel.Children.Add(CreateVerticalPanel(
            CreatePagination(total: 500, currentPage: 3, showSizeChanger: true, showQuickJumper: true),
            CreatePagination(total: 500, currentPage: 3, showSizeChanger: true, showQuickJumper: true, isEnabled: false)));

        panel.Children.Add(CreateVerticalPanel(
            CreatePagination(total: 50, sizeType: SizeType.Small),
            CreatePagination(total: 50, sizeType: SizeType.Small, showSizeChanger: true, showQuickJumper: true),
            CreatePagination(total: 50, sizeType: SizeType.Small, showTotalInfo: true),
            CreatePagination(total: 50, sizeType: SizeType.Small, showSizeChanger: true, showQuickJumper: true, showTotalInfo: true, isEnabled: false)));

        panel.Children.Add(CreateVerticalPanel(
            CreatePagination(total: 85, pageSize: 20, showSizeChanger: true, showTotalInfo: true),
            CreatePagination(total: 85,
                pageSize: 20,
                showSizeChanger: true,
                showTotalInfo: true,
                totalInfoTemplate: "${RangeStart}-${RangeEnd} of ${Total} items")));

        panel.Children.Add(CreateVerticalPanel(
            CreateSimplePagination(),
            CreateSimplePagination(readOnly: false),
            CreateSimplePagination(readOnly: false, isEnabled: false),
            CreateSimplePagination(sizeType: SizeType.Small),
            CreateSimplePagination(sizeType: SizeType.Small),
            CreateSimplePagination(readOnly: false, sizeType: SizeType.Small),
            CreateSimplePagination(readOnly: false, isEnabled: false, sizeType: SizeType.Small)));

        return panel;
    }

    private static StackPanel CreateVerticalPanel(params Control[] children)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 10
        };

        foreach (var child in children)
        {
            panel.Children.Add(child);
        }

        return panel;
    }
}
