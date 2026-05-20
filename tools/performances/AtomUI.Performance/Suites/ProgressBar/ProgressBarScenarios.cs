using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AtomButton = AtomUI.Desktop.Controls.Button;
using AtomProgressBar = AtomUI.Desktop.Controls.ProgressBar;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateProgressBarScenarios()
    {
        return
        [
            new PerfScenario("ProgressBar.Line.Basic", _ => CreateLineProgressBar(value: 55)),
            new PerfScenario("ProgressBar.Line.NoInfo", _ => CreateLineProgressBar(value: 55, isProgressInfoVisible: false)),
            new PerfScenario("ProgressBar.Line.Completed", _ => CreateLineProgressBar(value: 100)),
            new PerfScenario("ProgressBar.Line.Exception", _ => CreateLineProgressBar(value: 70, status: ProgressStatus.Exception)),
            new PerfScenario("ProgressBar.Line.InnerPositions", _ => CreateLineProgressInnerPositions()),
            new PerfScenario("ProgressBar.Steps.Basic", _ => CreateStepsProgressBar(value: 60, steps: 8)),
            new PerfScenario("ProgressBar.Steps.Vertical", _ => CreateStepsProgressBar(value: 55, steps: 10, orientation: Orientation.Vertical)),
            new PerfScenario("ProgressBar.Circle.Basic", _ => CreateCircleProgress(value: 75)),
            new PerfScenario("ProgressBar.Circle.Steps", _ => CreateCircleProgress(value: 77, stepCount: 8, stepGap: 10, indicatorThickness: 20)),
            new PerfScenario("ProgressBar.Dashboard.Basic", _ => CreateDashboardProgress(value: 75)),
            new PerfScenario("ProgressBar.Dashboard.Steps", _ => CreateDashboardProgress(value: 77, stepCount: 8, stepGap: 10, indicatorThickness: 20)),
            new PerfScenario("ProgressBar.GalleryShape.ProgressBarShowCase", _ => CreateProgressBarShowCaseShape())
        ];
    }

    private static AtomProgressBar CreateLineProgressBar(double value,
                                                         ProgressStatus status = ProgressStatus.Normal,
                                                         bool isProgressInfoVisible = true,
                                                         SizeType sizeType = SizeType.Large,
                                                         Orientation orientation = Orientation.Horizontal,
                                                         PercentPosition? percentPosition = null)
    {
        var progressBar = new AtomProgressBar
        {
            Minimum               = 0,
            Maximum               = 100,
            Value                 = value,
            Status                = status,
            IsProgressInfoVisible = isProgressInfoVisible,
            SizeType              = sizeType,
            Orientation           = orientation
        };

        if (percentPosition.HasValue)
        {
            progressBar.PercentPosition = percentPosition.Value;
        }

        return progressBar;
    }

    private static StepsProgressBar CreateStepsProgressBar(double value,
                                                           int steps,
                                                           ProgressStatus status = ProgressStatus.Normal,
                                                           SizeType sizeType = SizeType.Large,
                                                           Orientation orientation = Orientation.Horizontal,
                                                           LinePercentAlignment percentPosition = LinePercentAlignment.End,
                                                           double chunkHeight = double.NaN,
                                                           double chunkWidth = double.NaN,
                                                           List<IBrush>? stepsStrokeBrush = null)
    {
        return new StepsProgressBar
        {
            Minimum          = 0,
            Maximum          = 100,
            Value            = value,
            Steps            = steps,
            Status           = status,
            SizeType         = sizeType,
            Orientation      = orientation,
            PercentPosition  = percentPosition,
            ChunkHeight      = chunkHeight,
            ChunkWidth       = chunkWidth,
            StepsStrokeBrush = stepsStrokeBrush
        };
    }

    private static CircleProgress CreateCircleProgress(double value,
                                                       ProgressStatus status = ProgressStatus.Normal,
                                                       SizeType sizeType = SizeType.Large,
                                                       int stepCount = 0,
                                                       double stepGap = 2,
                                                       double indicatorThickness = double.NaN,
                                                       double successThreshold = double.NaN,
                                                       IBrush? strokeBrush = null,
                                                       PenLineCap strokeLineCap = PenLineCap.Round,
                                                       string? progressTextFormat = null,
                                                       double width = double.NaN,
                                                       double height = double.NaN)
    {
        var progress = new CircleProgress
        {
            Minimum            = 0,
            Maximum            = 100,
            Value              = value,
            Status             = status,
            SizeType           = sizeType,
            StepCount          = stepCount,
            StepGap            = stepGap,
            IndicatorThickness = indicatorThickness,
            SuccessThreshold   = successThreshold,
            StrokeBrush        = strokeBrush,
            StrokeLineCap      = strokeLineCap,
            Width              = width,
            Height             = height
        };

        if (progressTextFormat is not null)
        {
            progress.ProgressTextFormat = progressTextFormat;
        }

        return progress;
    }

    private static DashboardProgress CreateDashboardProgress(double value,
                                                             ProgressStatus status = ProgressStatus.Normal,
                                                             SizeType sizeType = SizeType.Large,
                                                             DashboardGapPosition dashboardGapPosition = DashboardGapPosition.Bottom,
                                                             double gapDegree = DashboardProgress.DEFAULT_GAP_DEGREE,
                                                             int stepCount = 0,
                                                             double stepGap = 2,
                                                             double indicatorThickness = double.NaN,
                                                             double successThreshold = double.NaN,
                                                             IBrush? successStrokeBrush = null,
                                                             IBrush? strokeBrush = null,
                                                             PenLineCap strokeLineCap = PenLineCap.Round,
                                                             double width = double.NaN,
                                                             double height = double.NaN)
    {
        return new DashboardProgress
        {
            Minimum              = 0,
            Maximum              = 100,
            Value                = value,
            Status               = status,
            SizeType             = sizeType,
            DashboardGapPosition = dashboardGapPosition,
            GapDegree            = gapDegree,
            StepCount            = stepCount,
            StepGap              = stepGap,
            IndicatorThickness   = indicatorThickness,
            SuccessThreshold     = successThreshold,
            SuccessStrokeBrush   = successStrokeBrush,
            StrokeBrush          = strokeBrush,
            StrokeLineCap        = strokeLineCap,
            Width                = width,
            Height               = height
        };
    }

    private static Control CreateLineProgressInnerPositions()
    {
        return CreateProgressVerticalPanel(
            CreateLineProgressBar(30, percentPosition: new PercentPosition { IsInner = true, Alignment = LinePercentAlignment.Start }),
            CreateLineProgressBar(60, percentPosition: new PercentPosition { IsInner = true, Alignment = LinePercentAlignment.Center }),
            CreateLineProgressBar(50, percentPosition: new PercentPosition { IsInner = true, Alignment = LinePercentAlignment.End }));
    }

    private static Control CreateProgressBarShowCaseShape()
    {
        var twoStopsGradient = new LinearGradientBrush
        {
            GradientStops =
            {
                new GradientStop(Color.Parse("#108ee9"), 0),
                new GradientStop(Color.Parse("#87d068"), 1)
            }
        };
        var threeStopsGradient = new LinearGradientBrush
        {
            GradientStops =
            {
                new GradientStop(Color.Parse("#87d068"), 0),
                new GradientStop(Color.Parse("#ffe58f"), 0.5),
                new GradientStop(Color.Parse("#ffccc7"), 1)
            }
        };
        var stepsChunkBrushes = new List<IBrush>
        {
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Red)
        };
        var innerStart = new PercentPosition { IsInner = true, Alignment = LinePercentAlignment.Start };
        var innerCenter = new PercentPosition { IsInner = true, Alignment = LinePercentAlignment.Center };
        var innerEnd = new PercentPosition { IsInner = true, Alignment = LinePercentAlignment.End };
        var outerStart = new PercentPosition { IsInner = false, Alignment = LinePercentAlignment.Start };
        var outerCenter = new PercentPosition { IsInner = false, Alignment = LinePercentAlignment.Center };

        return CreateProgressVerticalPanel(
            CreateProgressVerticalPanel(
                CreateLineProgressBar(30),
                CreateLineProgressBar(50),
                CreateLineProgressBar(70, status: ProgressStatus.Exception),
                CreateLineProgressBar(100),
                CreateLineProgressBar(50, isProgressInfoVisible: false)),

            CreateProgressWrapPanel(
                CreateCircleProgress(75),
                CreateCircleProgress(70, status: ProgressStatus.Exception),
                CreateCircleProgress(100)),

            CreateProgressWrapPanel(
                CreateLineProgressBar(30, sizeType: SizeType.Middle),
                CreateLineProgressBar(50, sizeType: SizeType.Middle),
                CreateLineProgressBar(70, status: ProgressStatus.Exception, sizeType: SizeType.Middle),
                CreateLineProgressBar(100, sizeType: SizeType.Middle),
                CreateLineProgressBar(50, isProgressInfoVisible: false, sizeType: SizeType.Middle)),

            CreateProgressWrapPanel(
                CreateCircleProgress(75, sizeType: SizeType.Middle),
                CreateCircleProgress(70, status: ProgressStatus.Exception, sizeType: SizeType.Middle),
                CreateCircleProgress(100, sizeType: SizeType.Middle)),

            CreateProgressVerticalPanel(
                CreateLineProgressBar(30, isProgressInfoVisible: false),
                CreateCircleProgress(30),
                new AtomButton { Content = "Sub" },
                new AtomButton { Content = "Add" }),

            CreateProgressWrapPanel(
                CreateCircleProgress(75, progressTextFormat: "{0} Days"),
                CreateCircleProgress(100)),

            CreateProgressWrapPanel(
                CreateDashboardProgress(75, dashboardGapPosition: DashboardGapPosition.Left),
                CreateDashboardProgress(60, dashboardGapPosition: DashboardGapPosition.Top),
                CreateDashboardProgress(75, dashboardGapPosition: DashboardGapPosition.Right, gapDegree: 40),
                CreateDashboardProgress(100, dashboardGapPosition: DashboardGapPosition.Bottom, gapDegree: 40)),

            CreateProgressVerticalPanel(
                ConfigureProgressControl(CreateLineProgressBar(60), progress => progress.SuccessThreshold = 30),
                CreateProgressWrapPanel(
                    CreateCircleProgress(60, successThreshold: 30),
                    CreateDashboardProgress(60, successThreshold: 30, successStrokeBrush: new SolidColorBrush(Colors.Chocolate)))),

            CreateProgressVerticalPanel(
                ConfigureProgressControl(CreateLineProgressBar(75), progress => progress.StrokeLineCap = PenLineCap.Square),
                CreateProgressWrapPanel(
                    CreateCircleProgress(75, strokeLineCap: PenLineCap.Square),
                    CreateDashboardProgress(75, strokeLineCap: PenLineCap.Square))),

            CreateProgressVerticalPanel(
                ConfigureProgressControl(CreateLineProgressBar(99), progress => progress.StrokeBrush = twoStopsGradient),
                ConfigureProgressControl(CreateLineProgressBar(50, status: ProgressStatus.Active), progress => progress.StrokeBrush = twoStopsGradient),
                CreateProgressWrapPanel(
                    CreateCircleProgress(90, strokeBrush: twoStopsGradient),
                    CreateCircleProgress(100, strokeBrush: twoStopsGradient),
                    CreateCircleProgress(93, strokeBrush: threeStopsGradient)),
                CreateProgressWrapPanel(
                    CreateDashboardProgress(90, strokeBrush: twoStopsGradient, strokeLineCap: PenLineCap.Square),
                    CreateDashboardProgress(100, strokeBrush: twoStopsGradient, strokeLineCap: PenLineCap.Square),
                    CreateDashboardProgress(93, strokeBrush: threeStopsGradient, strokeLineCap: PenLineCap.Square))),

            CreateProgressVerticalPanel(
                CreateStepsProgressBar(50, 3),
                CreateStepsProgressBar(30, 5),
                CreateStepsProgressBar(100, 5, sizeType: SizeType.Middle),
                CreateStepsProgressBar(80, 8, sizeType: SizeType.Small),
                CreateStepsProgressBar(60, 5, stepsStrokeBrush: stepsChunkBrushes)),

            CreateProgressVerticalPanel(
                CreateProgressWrapPanel(
                    CreateCircleProgress(50, stepCount: 4, stepGap: 8, indicatorThickness: 20),
                    CreateCircleProgress(100, stepCount: 10, stepGap: 8, indicatorThickness: 20),
                    CreateCircleProgress(77, status: ProgressStatus.Exception, stepCount: 8, stepGap: 10, indicatorThickness: 20),
                    CreateCircleProgress(77, stepCount: 8, stepGap: 10, indicatorThickness: 20, successThreshold: 30)),
                CreateProgressWrapPanel(
                    CreateDashboardProgress(50, stepCount: 4, stepGap: 8, indicatorThickness: 20),
                    CreateDashboardProgress(70, stepCount: 10, stepGap: 8, indicatorThickness: 20),
                    CreateDashboardProgress(77, status: ProgressStatus.Exception, stepCount: 8, stepGap: 10, indicatorThickness: 20),
                    CreateDashboardProgress(77, stepCount: 8, stepGap: 10, indicatorThickness: 20, successThreshold: 30))),

            CreateProgressVerticalPanel(
                CreateProgressVerticalPanel(
                    CreateLineProgressBar(50, sizeType: SizeType.Large),
                    CreateLineProgressBar(50, sizeType: SizeType.Middle),
                    CreateLineProgressBar(50, sizeType: SizeType.Small),
                    ConfigureProgressControl(CreateLineProgressBar(50), progress => progress.IndicatorThickness = 20)),
                CreateProgressWrapPanel(
                    CreateCircleProgress(50, sizeType: SizeType.Large),
                    CreateCircleProgress(50, sizeType: SizeType.Middle),
                    CreateCircleProgress(50, sizeType: SizeType.Small),
                    CreateCircleProgress(50, width: 20, height: 20)),
                CreateProgressWrapPanel(
                    CreateDashboardProgress(50, sizeType: SizeType.Large),
                    CreateDashboardProgress(50, sizeType: SizeType.Middle),
                    CreateDashboardProgress(50, sizeType: SizeType.Small),
                    CreateDashboardProgress(50, width: 20, height: 20)),
                CreateProgressWrapPanel(
                    CreateStepsProgressBar(50, 3, sizeType: SizeType.Large),
                    CreateStepsProgressBar(50, 3, sizeType: SizeType.Middle),
                    CreateStepsProgressBar(50, 3, sizeType: SizeType.Small),
                    CreateStepsProgressBar(50, 3, chunkHeight: 20, chunkWidth: 20),
                    CreateStepsProgressBar(50, 3, chunkHeight: 30, chunkWidth: 20))),

            CreateProgressVerticalPanel(
                ConfigureProgressControl(CreateLineProgressBar(30, percentPosition: innerStart), progress => progress.Width = 300),
                ConfigureProgressControl(CreateLineProgressBar(60, percentPosition: innerCenter), progress => progress.Width = 300),
                ConfigureProgressControl(CreateLineProgressBar(50, percentPosition: innerEnd), progress => progress.Width = 300),
                ConfigureProgressControl(CreateLineProgressBar(70, percentPosition: innerEnd), progress =>
                {
                    progress.Width       = 300;
                    progress.StrokeBrush = new SolidColorBrush(Color.Parse("#001342"));
                }),
                ConfigureProgressControl(CreateLineProgressBar(100, percentPosition: innerCenter), progress => progress.Width = 400),
                CreateLineProgressBar(100, percentPosition: outerStart),
                CreateLineProgressBar(60, sizeType: SizeType.Small, percentPosition: outerCenter),
                CreateLineProgressBar(100, percentPosition: outerCenter),
                CreateLineProgressBar(55, percentPosition: outerStart)),

            CreateProgressVerticalPanel(
                CreateStepsProgressBar(100, 8, percentPosition: LinePercentAlignment.Start),
                CreateStepsProgressBar(100, 8, percentPosition: LinePercentAlignment.Center),
                CreateStepsProgressBar(60, 8, sizeType: SizeType.Middle, percentPosition: LinePercentAlignment.Center),
                CreateStepsProgressBar(60, 8, sizeType: SizeType.Small, percentPosition: LinePercentAlignment.Center),
                CreateStepsProgressBar(55, 8, percentPosition: LinePercentAlignment.Center),
                CreateStepsProgressBar(100, 8, percentPosition: LinePercentAlignment.End),
                CreateStepsProgressBar(55, 8, status: ProgressStatus.Exception, percentPosition: LinePercentAlignment.End),
                CreateStepsProgressBar(99, 8, percentPosition: LinePercentAlignment.Start)),

            ConfigureProgressControl(CreateProgressHorizontalPanel(
                CreateLineProgressBar(100, orientation: Orientation.Vertical),
                CreateLineProgressBar(55, orientation: Orientation.Vertical),
                CreateLineProgressBar(55, sizeType: SizeType.Small, orientation: Orientation.Vertical),
                CreateLineProgressBar(55, orientation: Orientation.Vertical, percentPosition: outerStart),
                CreateLineProgressBar(55, orientation: Orientation.Vertical, percentPosition: outerCenter),
                CreateLineProgressBar(100, orientation: Orientation.Vertical, percentPosition: outerStart),
                CreateLineProgressBar(55, orientation: Orientation.Vertical, percentPosition: innerStart),
                CreateLineProgressBar(55, orientation: Orientation.Vertical, percentPosition: innerCenter),
                CreateLineProgressBar(100, orientation: Orientation.Vertical, percentPosition: innerStart),
                CreateLineProgressBar(70, orientation: Orientation.Vertical, percentPosition: innerEnd)), panel => panel.Height = 300),

            ConfigureProgressControl(CreateProgressHorizontalPanel(
                CreateStepsProgressBar(100, 10, orientation: Orientation.Vertical, percentPosition: LinePercentAlignment.End),
                CreateStepsProgressBar(55, 5, orientation: Orientation.Vertical),
                CreateStepsProgressBar(55, 10, sizeType: SizeType.Small, orientation: Orientation.Vertical),
                CreateStepsProgressBar(55, 6, orientation: Orientation.Vertical, percentPosition: LinePercentAlignment.Start),
                CreateStepsProgressBar(55, 6, orientation: Orientation.Vertical, percentPosition: LinePercentAlignment.Center),
                CreateStepsProgressBar(100, 6, orientation: Orientation.Vertical, percentPosition: LinePercentAlignment.Start)), panel => panel.Height = 300),

            ConfigureProgressControl(CreateProgressHorizontalPanel(
                CreateStepsProgressBar(100, 10, orientation: Orientation.Vertical, percentPosition: LinePercentAlignment.End),
                CreateStepsProgressBar(55, 5, orientation: Orientation.Vertical),
                CreateStepsProgressBar(55, 10, sizeType: SizeType.Small, orientation: Orientation.Vertical),
                CreateStepsProgressBar(55, 6, orientation: Orientation.Vertical, percentPosition: LinePercentAlignment.Start),
                CreateStepsProgressBar(55, 6, orientation: Orientation.Vertical, percentPosition: LinePercentAlignment.Center),
                CreateStepsProgressBar(100, 6, orientation: Orientation.Vertical, percentPosition: LinePercentAlignment.Start)), panel => panel.Height = 300),

            CreateProgressVerticalPanel(
                ConfigureProgressControl(CreateLineProgressBar(30), progress => progress.IsEnabled = true),
                ConfigureProgressControl(CreateLineProgressBar(50), progress => progress.IsEnabled = true),
                ConfigureProgressControl(CreateLineProgressBar(70, status: ProgressStatus.Exception), progress => progress.IsEnabled = true),
                ConfigureProgressControl(CreateLineProgressBar(100), progress => progress.IsEnabled = true),
                ConfigureProgressControl(CreateStepsProgressBar(30, 10), progress => progress.IsEnabled = true),
                ConfigureProgressControl(CreateStepsProgressBar(50, 10), progress => progress.IsEnabled = true),
                ConfigureProgressControl(CreateStepsProgressBar(70, 10, status: ProgressStatus.Exception), progress => progress.IsEnabled = true),
                ConfigureProgressControl(CreateStepsProgressBar(100, 10), progress => progress.IsEnabled = true),
                CreateProgressWrapPanel(
                    ConfigureProgressControl(CreateCircleProgress(75, sizeType: SizeType.Middle), progress => progress.IsEnabled = true),
                    ConfigureProgressControl(CreateCircleProgress(70, status: ProgressStatus.Exception, sizeType: SizeType.Middle), progress => progress.IsEnabled = true),
                    ConfigureProgressControl(CreateCircleProgress(100, sizeType: SizeType.Middle), progress => progress.IsEnabled = true)),
                CreateProgressWrapPanel(
                    ConfigureProgressControl(CreateDashboardProgress(75, sizeType: SizeType.Middle), progress => progress.IsEnabled = true),
                    ConfigureProgressControl(CreateDashboardProgress(70, status: ProgressStatus.Exception, sizeType: SizeType.Middle), progress => progress.IsEnabled = true),
                    ConfigureProgressControl(CreateDashboardProgress(100, sizeType: SizeType.Middle), progress => progress.IsEnabled = true)),
                new AtomButton { Content = "Disable" }));
    }

    private static StackPanel CreateProgressVerticalPanel(params Control[] children)
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

    private static T ConfigureProgressControl<T>(T control, Action<T> configure)
        where T : Control
    {
        configure(control);
        return control;
    }

    private static StackPanel CreateProgressHorizontalPanel(params Control[] children)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing     = 10
        };

        foreach (var child in children)
        {
            panel.Children.Add(child);
        }

        return panel;
    }

    private static WrapPanel CreateProgressWrapPanel(params Control[] children)
    {
        var panel = new WrapPanel
        {
            Orientation = Orientation.Horizontal
        };

        foreach (var child in children)
        {
            panel.Children.Add(child);
        }

        return panel;
    }
}
