using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.VisualTree;
using AtomProgressBar = AtomUI.Desktop.Controls.ProgressBar;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunProgressBarStateVerification()
    {
        var failures = new List<string>();
        VerifyProgressBarPseudoClasses(failures);
        VerifyLineProgressStatusIconLifecycle(failures);
        VerifyCircleProgressStatusIconLifecycle(failures);
        VerifyProgressBarRangeMath(failures);
        VerifyProgressBarInnerPositionRangeMath(failures);
        VerifyProgressBarReapplyTemplate(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("ProgressBar state verification passed.");
            return true;
        }

        Console.Error.WriteLine("ProgressBar state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyProgressBarPseudoClasses(ICollection<string> failures)
    {
        var progressBar = new AtomProgressBar();
        using var realized = RealizeControl(progressBar);

        progressBar.IsIndeterminate = true;
        RefreshLayout(realized.Window);
        Expect(progressBar.Classes.Contains(ProgressBarPseudoClass.Indeterminate),
            "ProgressBar should set :indeterminate when IsIndeterminate is true.",
            failures);

        progressBar.IsIndeterminate = false;
        RefreshLayout(realized.Window);
        Expect(!progressBar.Classes.Contains(ProgressBarPseudoClass.Indeterminate),
            "ProgressBar should clear :indeterminate when IsIndeterminate is false.",
            failures);

        progressBar.PercentPosition = new PercentPosition
        {
            IsInner   = true,
            Alignment = LinePercentAlignment.End
        };
        RefreshLayout(realized.Window);
        Expect(progressBar.Classes.Contains(ProgressBarPseudoClass.PercentLabelInnerEnd),
            "ProgressBar should set :labelinner-end for inner end percent position.",
            failures);
        Expect(!progressBar.Classes.Contains(ProgressBarPseudoClass.PercentLabelInnerCenter),
            "ProgressBar should not set :labelinner-center for inner end percent position.",
            failures);
    }

    private static void VerifyLineProgressStatusIconLifecycle(ICollection<string> failures)
    {
        var progressBar = new AtomProgressBar
        {
            Minimum         = 0,
            Maximum         = 100,
            Value           = 50,
            IsMotionEnabled = false
        };

        using var realized = RealizeControl(progressBar);
        Expect(FindVisualByName<IconPresenter>(progressBar, "PART_ExceptionCompletedIconPresenter") == null,
            "Normal line ProgressBar should not create exception icon presenter.",
            failures);
        Expect(FindVisualByName<IconPresenter>(progressBar, "PART_SuccessCompletedIconPresenter") == null,
            "Normal incomplete line ProgressBar should not create success icon presenter.",
            failures);

        progressBar.Status = ProgressStatus.Exception;
        RefreshLayout(realized.Window);
        var exceptionIcon = FindVisualByName<IconPresenter>(progressBar, "PART_ExceptionCompletedIconPresenter");
        Expect(exceptionIcon != null,
            "Exception line ProgressBar should create exception icon presenter.",
            failures);
        Expect(FindVisualByName<IconPresenter>(progressBar, "PART_SuccessCompletedIconPresenter") == null,
            "Exception line ProgressBar should not create success icon presenter.",
            failures);

        progressBar.Status = ProgressStatus.Normal;
        progressBar.Value  = 100;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<IconPresenter>(progressBar, "PART_SuccessCompletedIconPresenter") != null,
            "Completed line ProgressBar should create success icon presenter.",
            failures);
        Expect(exceptionIcon?.GetVisualParent() == null,
            "Released exception icon presenter should not keep a visual parent.",
            failures);

        var successIcon = FindVisualByName<IconPresenter>(progressBar, "PART_SuccessCompletedIconPresenter");
        progressBar.Value = 50;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<IconPresenter>(progressBar, "PART_SuccessCompletedIconPresenter") == null,
            "Incomplete line ProgressBar should release success icon presenter.",
            failures);
        Expect(successIcon?.GetVisualParent() == null,
            "Released success icon presenter should not keep a visual parent.",
            failures);
    }

    private static void VerifyCircleProgressStatusIconLifecycle(ICollection<string> failures)
    {
        var circle = new CircleProgress
        {
            Minimum         = 0,
            Maximum         = 100,
            Value           = 50,
            IsMotionEnabled = false
        };

        using var realized = RealizeControl(circle);
        Expect(FindVisualByName<IconPresenter>(circle, "PART_ExceptionCompletedIconPresenter") == null,
            "Normal CircleProgress should not create exception icon presenter.",
            failures);
        Expect(FindVisualByName<IconPresenter>(circle, "PART_SuccessCompletedIconPresenter") == null,
            "Normal incomplete CircleProgress should not create success icon presenter.",
            failures);

        circle.Status = ProgressStatus.Exception;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<IconPresenter>(circle, "PART_ExceptionCompletedIconPresenter") != null,
            "Exception CircleProgress should create exception icon presenter.",
            failures);

        circle.Status = ProgressStatus.Normal;
        circle.Value  = 100;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<IconPresenter>(circle, "PART_SuccessCompletedIconPresenter") != null,
            "Completed CircleProgress should create success icon presenter.",
            failures);

        circle.Value = 50;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<IconPresenter>(circle, "PART_SuccessCompletedIconPresenter") == null,
            "Incomplete CircleProgress should release success icon presenter.",
            failures);
    }

    private static void VerifyProgressBarRangeMath(ICollection<string> failures)
    {
        var progressBar = new AtomProgressBar
        {
            Minimum = 50,
            Maximum = 150,
            Value   = 100
        };

        using var _ = RealizeControl(progressBar);
        Expect(Math.Abs(progressBar.Percentage - 50) < 0.001,
            $"ProgressBar percentage should account for non-zero Minimum. Actual: {progressBar.Percentage}.",
            failures);
    }

    private static void VerifyProgressBarInnerPositionRangeMath(ICollection<string> failures)
    {
        var progressBar = new AtomProgressBar
        {
            Width           = 300,
            Height          = 24,
            Minimum         = 50,
            Maximum         = 150,
            Value           = 100,
            IsMotionEnabled = false,
            PercentPosition = new PercentPosition
            {
                IsInner   = true,
                Alignment = LinePercentAlignment.Center
            }
        };

        using var realized = RealizeControl(progressBar);
        RefreshLayout(realized.Window);
        var labelHost = FindVisualByName<LayoutTransformControl>(progressBar, "PART_LayoutTransformControl");
        var left      = labelHost is null ? double.NaN : Canvas.GetLeft(labelHost);
        Expect(!double.IsNaN(left) && left < progressBar.Bounds.Width * 0.4,
            $"Inner percent label should be positioned against calculated percentage for non-zero Minimum. Left: {left:0.###}.",
            failures);
    }

    private static void VerifyProgressBarReapplyTemplate(ICollection<string> failures)
    {
        var progressBar = new AtomProgressBar
        {
            Value           = 100,
            IsMotionEnabled = false
        };

        using var realized = RealizeControl(progressBar);
        progressBar.ApplyTemplate();
        RefreshLayout(realized.Window);
        var successIcons = progressBar.GetSelfAndVisualDescendants()
                                      .OfType<IconPresenter>()
                                      .Count(icon => icon.Name == "PART_SuccessCompletedIconPresenter");
        Expect(successIcons == 1,
            $"ProgressBar should not duplicate status icon presenters after reapplying template. Actual: {successIcons}.",
            failures);
    }
}
