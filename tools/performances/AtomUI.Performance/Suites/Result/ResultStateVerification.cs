using AtomUI.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;
using AtomResult = AtomUI.Desktop.Controls.Result;
using SvgControl = Avalonia.Svg.Svg;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunResultStateVerification()
    {
        var failures = new List<string>();
        VerifyResultStatusVisualLifecycle(failures);
        VerifyResultErrorImageLifecycle(failures);
        VerifyResultRuntimeStatusSwitching(failures);
        VerifyResultHeaderLineHeightUpdates(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Result state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Result state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyResultStatusVisualLifecycle(ICollection<string> failures)
    {
        var result = CreateResult(ResultStatus.Success, hasSubHeader: true, hasExtra: true);
        using var realized = RealizeControl(result);

        Expect(FindVisualByName<StackPanel>(result, "PART_RootLayout") != null,
            "Result template should expose PART_RootLayout.",
            failures);

        var presenter = FindVisualByName<ContentPresenter>(result, "PART_StatusIconPresenter");
        Expect(presenter is { IsVisible: true },
            "Success Result should create a visible status icon presenter.",
            failures);
        Expect(presenter?.Child is CheckCircleFilled,
            "Success Result should create the success status icon.",
            failures);
        if (presenter?.Child is Control statusIcon)
        {
            Expect(!double.IsNaN(statusIcon.Width) && statusIcon.Width > 0 &&
                   MathUtils.AreClose(statusIcon.Width, statusIcon.Height, 0.1),
                "Success Result status icon should keep themed square sizing.",
                failures);
        }
        Expect(FindVisualByName<SvgControl>(result, "PART_ErrorCodeImage") == null,
            "Success Result should not create the error-code Svg image.",
            failures);

        _ = realized;
    }

    private static void VerifyResultErrorImageLifecycle(ICollection<string> failures)
    {
        var result = CreateResult(ResultStatus.ErrorCode404, hasSubHeader: true, hasExtra: true);
        using var _ = RealizeControl(result);

        var image = FindVisualByName<SvgControl>(result, "PART_ErrorCodeImage");
        Expect(image is { IsVisible: true, Source: not null },
            "ErrorCode404 Result should create a visible Svg image with a source.",
            failures);
        Expect(image?.Source?.Contains("<svg", StringComparison.OrdinalIgnoreCase) == true,
            "ErrorCode404 Result Svg source should contain SVG markup.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(result, "PART_StatusIconPresenter") == null,
            "ErrorCode404 Result should not create the normal status icon presenter.",
            failures);
    }

    private static void VerifyResultRuntimeStatusSwitching(ICollection<string> failures)
    {
        var result = CreateResult(ResultStatus.Success, hasSubHeader: true, hasExtra: true);
        using var realized = RealizeControl(result);

        var firstPresenter = FindVisualByName<ContentPresenter>(result, "PART_StatusIconPresenter");
        Expect(firstPresenter != null,
            "Initial Success Result should create a status icon presenter.",
            failures);

        result.SetCurrentValue(AtomResult.StatusProperty, ResultStatus.ErrorCode403);
        RefreshLayout(realized.Window);
        Expect(firstPresenter == null ||
               firstPresenter.GetVisualParent() == null && firstPresenter.TemplatedParent == null,
            "Switching Result to ErrorCode403 should detach the old status icon presenter.",
            failures);
        var firstImage = FindVisualByName<SvgControl>(result, "PART_ErrorCodeImage");
        Expect(firstImage is { Source: not null },
            "Switching Result to ErrorCode403 should create an error image.",
            failures);

        result.SetCurrentValue(AtomResult.StatusProperty, ResultStatus.Warning);
        RefreshLayout(realized.Window);
        Expect(firstImage == null ||
               firstImage.GetVisualParent() == null && firstImage.TemplatedParent == null && firstImage.Source == null,
            "Switching Result away from an error code should detach and clear the old Svg image.",
            failures);
        var warningPresenter = FindVisualByName<ContentPresenter>(result, "PART_StatusIconPresenter");
        Expect(warningPresenter?.Child is WarningFilled,
            "Switching Result to Warning should create the warning status icon.",
            failures);
        Expect(FindVisualByName<SvgControl>(result, "PART_ErrorCodeImage") == null,
            "Warning Result should not keep an error-code Svg image attached.",
            failures);

        var smile = new SmileOutlined();
        result.SetCurrentValue(AtomResult.StatusProperty, ResultStatus.Info);
        result.SetCurrentValue(AtomResult.IconProperty, smile);
        RefreshLayout(realized.Window);
        var customPresenter = FindVisualByName<ContentPresenter>(result, "PART_StatusIconPresenter");
        Expect(ReferenceEquals(customPresenter?.Child, smile),
            "Custom Result icon should be used as the status icon content.",
            failures);
    }

    private static void VerifyResultHeaderLineHeightUpdates(ICollection<string> failures)
    {
        var result = CreateResult(ResultStatus.Success, hasSubHeader: true);
        using var realized = RealizeControl(result);

        var header = FindVisualByName<ContentPresenter>(result, "Header");
        var headerRatio = header is null || result.HeaderFontSize <= 0
            ? 0
            : header.LineHeight / result.HeaderFontSize;
        result.HeaderFontSize = 20;
        RefreshLayout(realized.Window);
        header = FindVisualByName<ContentPresenter>(result, "Header");
        Expect(header != null && MathUtils.AreClose(header.LineHeight, headerRatio * 20, 0.1),
            "Result Header line height should update when HeaderFontSize changes.",
            failures);

        var subHeader = FindVisualByName<ContentPresenter>(result, "SubHeader");
        var subHeaderRatio = subHeader is null || result.SubHeaderFontSize <= 0
            ? 0
            : subHeader.LineHeight / result.SubHeaderFontSize;
        result.SubHeaderFontSize = 18;
        RefreshLayout(realized.Window);
        subHeader = FindVisualByName<ContentPresenter>(result, "SubHeader");
        Expect(subHeader != null && MathUtils.AreClose(subHeader.LineHeight, subHeaderRatio * 18, 0.1),
            "Result SubHeader line height should update when SubHeaderFontSize changes.",
            failures);
    }
}
