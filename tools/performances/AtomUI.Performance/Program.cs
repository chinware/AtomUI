using System.Diagnostics;
using System.Globalization;
using System.Text;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.Primitives.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

using AtomTextBox = AtomUI.Desktop.Controls.TextBox;

internal static class Program
{
    internal const int DefaultCount = 60;
    private static readonly Size MeasureSize = new(1280, 4096);
    private static readonly Rect ArrangeRect = new(0, 0, 1280, 4096);

    [STAThread]
    public static int Main(string[] args)
    {
        var options = PerfOptions.Parse(args);
        SetupAvalonia();

        AddOnDecoratedBoxPerfProbe.IsEnabled = true;
        if (options.VerifyAccessories || options.VerifyEffectiveBrushes || options.VerifyAddonStates)
        {
            var verified = true;
            if (options.VerifyAccessories)
            {
                verified &= RunAccessoryLifecycleVerification();
            }
            if (options.VerifyEffectiveBrushes)
            {
                verified &= RunEffectiveBrushVerification();
            }
            if (options.VerifyAddonStates)
            {
                verified &= RunAddonStateVerification();
            }
            return verified ? 0 : 1;
        }

        var scenarios = CreateScenarios();
        foreach (var scenario in scenarios)
        {
            RunWarmup(scenario, Math.Min(5, options.Count));
        }

        var results = scenarios
            .Select(scenario => MeasureScenario(scenario, options.Count))
            .ToList();

        Console.WriteLine(RenderTable(results));

        if (!string.IsNullOrWhiteSpace(options.MarkdownOutputPath))
        {
            var markdown = RenderMarkdown(results, options.Count);
            var fullPath = Path.GetFullPath(options.MarkdownOutputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            File.WriteAllText(fullPath, markdown, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            Console.WriteLine();
            Console.WriteLine($"Wrote markdown baseline: {fullPath}");
        }

        return 0;
    }

    private static void SetupAvalonia()
    {
        AppBuilder.Configure<PerfApplication>()
                  .UseHeadless(new AvaloniaHeadlessPlatformOptions())
                  .SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime());
    }

    private static IReadOnlyList<PerfScenario> CreateScenarios()
    {
        return
        [
            new PerfScenario("LineEdit.Default", _ => CreateLineEdit()),
            new PerfScenario("LineEdit.AllowClear", _ => CreateLineEdit(text: "clear text", isAllowClear: true)),
            new PerfScenario("LineEdit.Reveal", _ => CreateLineEdit(text: "secret", isEnableRevealButton: true, passwordChar: '*')),
            new PerfScenario("LineEdit.Count", _ => CreateLineEdit(text: "count text", isShowCount: true, maxLength: 100)),
            new PerfScenario("LineEdit.InnerRight", _ => CreateLineEdit(innerRightContent: new Avalonia.Controls.TextBlock { Text = "kg" })),
            new PerfScenario("LineEdit.FormFeedback", _ => CreateLineEdit(formFeedback: new FormValidateFeedback
            {
                ValidateStatus = FormValidateStatus.Error,
                ErrorFeedback  = new Avalonia.Controls.TextBlock { Text = "!" }
            })),
            new PerfScenario("LineEdit.OuterAddOns", _ => CreateLineEdit(
                leftAddOn: new Avalonia.Controls.TextBlock { Text = "http://" },
                rightAddOn: new Avalonia.Controls.TextBlock { Text = ".com" })),
            new PerfScenario("SearchEdit.Default", _ => new SearchEdit
            {
                Width = 260,
                Text = "search"
            }),
            new PerfScenario("DatePicker.Default", _ => new AtomUI.Desktop.Controls.DatePicker
            {
                Width = 260,
                PlaceholderText = "Select date"
            }),
            new PerfScenario("RangeDatePicker.Default", _ => new AtomUI.Desktop.Controls.RangeDatePicker
            {
                Width = 320,
                PlaceholderText = "Select range"
            }),
            new PerfScenario("Select.Default", _ => new Select
            {
                Width = 260,
                PlaceholderText = "Select"
            }),
            new PerfScenario("TreeSelect.Default", _ => new TreeSelect
            {
                Width = 260,
                PlaceholderText = "Tree select"
            }),
            new PerfScenario("Cascader.Default", _ => new Cascader
            {
                Width = 260,
                PlaceholderText = "Cascader"
            }),
            new PerfScenario("ButtonSpinner.Default", _ => new AtomUI.Desktop.Controls.ButtonSpinner
            {
                Width   = 160,
                Content = new Avalonia.Controls.TextBlock { Text = "100" }
            }),
            new PerfScenario("CompactSpace.LineEdit.Horizontal", _ => CreateCompactSpace(Orientation.Horizontal)),
            new PerfScenario("CompactSpace.LineEdit.Vertical", _ => CreateCompactSpace(Orientation.Vertical))
        ];
    }

    private static LineEdit CreateLineEdit(
        string? text = null,
        bool isAllowClear = false,
        bool isEnableRevealButton = false,
        bool isShowCount = false,
        int maxLength = 0,
        char passwordChar = '\0',
        object? innerRightContent = null,
        FormValidateFeedback? formFeedback = null,
        object? leftAddOn = null,
        object? rightAddOn = null)
    {
        var lineEdit = new LineEdit
        {
            Width                = 260,
            Text                 = text,
            IsAllowClear         = isAllowClear,
            IsEnableRevealButton = isEnableRevealButton,
            IsShowCount          = isShowCount,
            PasswordChar         = passwordChar,
            InnerRightContent    = innerRightContent,
            LeftAddOn            = leftAddOn,
            RightAddOn           = rightAddOn
        };
        lineEdit.FormFeedback = formFeedback;

        if (maxLength > 0)
        {
            lineEdit.MaxLength = maxLength;
        }

        return lineEdit;
    }

    private static CompactSpace CreateCompactSpace(Orientation orientation)
    {
        var compactSpace = new CompactSpace
        {
            Orientation = orientation
        };
        compactSpace.Children.Add(CreateLineEdit(text: "first", isAllowClear: true));
        compactSpace.Children.Add(CreateLineEdit(text: "middle"));
        compactSpace.Children.Add(CreateLineEdit(text: "last", isShowCount: true, maxLength: 50));
        return compactSpace;
    }

    private static void RunWarmup(PerfScenario scenario, int count)
    {
        if (count <= 0)
        {
            return;
        }

        using var _ = RealizeScenario(scenario, count);
    }

    private static PerfResult MeasureScenario(PerfScenario scenario, int count)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        AddOnDecoratedBoxPerfProbe.Reset();
        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var stopwatch       = Stopwatch.StartNew();

        using var realized = RealizeScenario(scenario, count);

        stopwatch.Stop();
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        var probeSnapshot  = AddOnDecoratedBoxPerfProbe.Snapshot();
        var treeStats      = TreeStats.Collect(realized.RootControls);

        return new PerfResult(
            scenario.Name,
            count,
            stopwatch.Elapsed,
            allocatedBytes,
            treeStats,
            probeSnapshot);
    }

    private static RealizedScenario RealizeScenario(PerfScenario scenario, int count)
    {
        var rootControls = new List<Control>(count);
        var panel        = new StackPanel
        {
            Spacing = 8
        };

        for (var i = 0; i < count; i++)
        {
            var control = scenario.Create(i);
            rootControls.Add(control);
            panel.Children.Add(control);
        }

        var window = new Avalonia.Controls.Window
        {
            Width       = MeasureSize.Width,
            Height      = 900,
            Content     = panel,
            ShowInTaskbar = false
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        window.Measure(MeasureSize);
        window.Arrange(ArrangeRect);
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();

        return new RealizedScenario(window, rootControls);
    }

    private static bool RunAccessoryLifecycleVerification()
    {
        var failures = new List<string>();
        VerifyLineEditAccessoryLifecycle(failures);
        VerifyTextAreaAccessoryLifecycle(failures);
        VerifySearchEditAccessoryLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Accessory lifecycle verification passed.");
            return true;
        }

        Console.Error.WriteLine("Accessory lifecycle verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static bool RunEffectiveBrushVerification()
    {
        var failures = new List<string>();
        VerifyAddOnDecoratedBoxEffectiveBrushes(failures);
        VerifyFocusedFilledBackground(failures);
        VerifyDropdownEffectiveBrushes(new SelectAddOnDecoratedBox(), box => box.IsDropDownOpen = true, "SelectAddOnDecoratedBox", failures);
        VerifyDropdownEffectiveBrushes(new TreeSelectAddOnDecoratedBox(), box => box.IsDropDownOpen = true, "TreeSelectAddOnDecoratedBox", failures);
        VerifyDropdownEffectiveBrushes(new CascaderAddOnDecoratedBox(), box => box.IsDropDownOpen = true, "CascaderAddOnDecoratedBox", failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Effective brush verification passed.");
            return true;
        }

        Console.Error.WriteLine("Effective brush verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static bool RunAddonStateVerification()
    {
        var failures = new List<string>();
        VerifyAddOnStatusAndRuntimeChanges(failures);
        VerifyEmptyAddOnSlotsSkipIconScan(failures);
        VerifyAddOnGeometry(failures);
        VerifyCompactSpaceGeometry(failures);
        VerifyRightAddOnTemplateBindings(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Addon state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Addon state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyAddOnDecoratedBoxEffectiveBrushes(ICollection<string> failures)
    {
        var box     = new AddOnDecoratedBox();
        var brushes = ApplyTestBrushes(box);

        ResetEffectiveBrushState(box, InputControlStyleVariant.Outlined);
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.DefaultBorder, "Outlined default border", failures);
        ExpectBrush(box.EffectiveInnerBoxBackground, Brushes.Transparent, "Outlined default background", failures);

        box.IsInnerBoxHover = true;
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.HoverBorder, "Outlined hover border", failures);

        box.IsInnerBoxPressed = true;
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.ActiveBorder, "Outlined pressed border", failures);

        box.IsInnerBoxPressed = false;
        box.SetCurrentValue(AddOnDecoratedBox.StatusProperty, InputControlStatus.Error);
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.ErrorHoverBorder, "Outlined error hover border", failures);

        box.IsInnerBoxHover   = false;
        box.IsInnerBoxPressed = true;
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.ErrorBorder, "Outlined error pressed border", failures);

        ResetEffectiveBrushState(box, InputControlStyleVariant.Underlined);
        box.SetCurrentValue(AddOnDecoratedBox.StatusProperty, InputControlStatus.Warning);
        box.IsInnerBoxHover = true;
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.WarningHoverBorder, "Underlined warning hover border", failures);
        ExpectBrush(box.EffectiveInnerBoxBackground, Brushes.Transparent, "Underlined warning hover background", failures);

        ResetEffectiveBrushState(box, InputControlStyleVariant.Borderless);
        box.IsInnerBoxHover = true;
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, Brushes.Transparent, "Borderless hover border", failures);
        ExpectBrush(box.EffectiveInnerBoxBackground, Brushes.Transparent, "Borderless hover background", failures);

        ResetEffectiveBrushState(box, InputControlStyleVariant.Filled);
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.FilledBorder, "Filled default border", failures);
        ExpectBrush(box.EffectiveInnerBoxBackground, brushes.FilledBackground, "Filled default background", failures);

        box.IsInnerBoxHover = true;
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.FilledBorder, "Filled hover border", failures);
        ExpectBrush(box.EffectiveInnerBoxBackground, brushes.FilledHoverBackground, "Filled hover background", failures);

        box.IsInnerBoxPressed = true;
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.ActiveBorder, "Filled pressed border", failures);
        ExpectBrush(box.EffectiveInnerBoxBackground, brushes.FilledHoverBackground, "Filled pressed keeps hover background", failures);

        box.IsInnerBoxHover   = false;
        box.IsInnerBoxPressed = false;
        box.SetCurrentValue(AddOnDecoratedBox.StatusProperty, InputControlStatus.Error);
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.ErrorFilledBorder, "Filled error border", failures);
        ExpectBrush(box.EffectiveInnerBoxBackground, brushes.ErrorBackground, "Filled error background", failures);

        box.IsInnerBoxHover = true;
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.ErrorFilledBorder, "Filled error hover border", failures);
        ExpectBrush(box.EffectiveInnerBoxBackground, brushes.ErrorHoverBackground, "Filled error hover background", failures);

        box.IsInnerBoxHover   = false;
        box.IsInnerBoxPressed = true;
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.ErrorBorder, "Filled error pressed border", failures);
        ExpectBrush(box.EffectiveInnerBoxBackground, brushes.ErrorBackground, "Filled error pressed background", failures);

        ResetEffectiveBrushState(box, InputControlStyleVariant.Filled);
        box.SetCurrentValue(InputElement.IsEnabledProperty, false);
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.DefaultBorder, "Filled disabled border", failures);
        ExpectBrush(box.EffectiveInnerBoxBackground, brushes.DisabledBackground, "Filled disabled background", failures);

        ResetEffectiveBrushState(box, InputControlStyleVariant.Outlined);
        box.IsInnerBoxHover = true;
        var replacementHoverBorder = Brush(Color.FromRgb(240, 20, 210));
        box.InnerBoxHoverBorderBrush = replacementHoverBorder;
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, replacementHoverBorder, "Source brush replacement refreshes effective border", failures);
    }

    private static void VerifyFocusedFilledBackground(ICollection<string> failures)
    {
        var editor = new Avalonia.Controls.TextBox { Text = "focus" };
        var box = new AddOnDecoratedBox
        {
            Width        = 260,
            Content      = editor,
            StyleVariant = InputControlStyleVariant.Filled,
            IsMotionEnabled = false
        };
        var brushes = ApplyTestBrushes(box);

        using var realized = RealizeControl(box);
        editor.Focus();
        RefreshLayout(realized.Window);

        Expect(box.IsKeyboardFocusWithin, "Focused filled verification should set IsKeyboardFocusWithin.", failures);
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.ActiveBorder, "Filled focused border", failures);
        ExpectBrush(box.EffectiveInnerBoxBackground, brushes.ActiveBackground, "Filled focused background", failures);
    }

    private static void VerifyDropdownEffectiveBrushes<T>(T box,
                                                          Action<T> openDropDown,
                                                          string label,
                                                          ICollection<string> failures)
        where T : AddOnDecoratedBox
    {
        var brushes = ApplyTestBrushes(box);
        box.InnerBoxFilledBorderBrush        = Brushes.Transparent;
        box.InnerBoxErrorFilledBorderBrush   = Brushes.Transparent;
        box.InnerBoxWarningFilledBorderBrush = Brushes.Transparent;
        box.SetCurrentValue(AddOnDecoratedBox.StyleVariantProperty, InputControlStyleVariant.Filled);

        ExpectBrush(box.EffectiveInnerBoxBorderBrush, Brushes.Transparent, $"{label} filled closed border", failures);
        ExpectBrush(box.EffectiveInnerBoxBackground, brushes.FilledBackground, $"{label} filled closed background", failures);

        openDropDown(box);
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.ActiveBorder, $"{label} opened border", failures);
        ExpectBrush(box.EffectiveInnerBoxBackground, brushes.FilledBackground, $"{label} opened background", failures);

        box.SetCurrentValue(AddOnDecoratedBox.StatusProperty, InputControlStatus.Warning);
        ExpectBrush(box.EffectiveInnerBoxBorderBrush, brushes.WarningBorder, $"{label} opened warning border", failures);
        ExpectBrush(box.EffectiveInnerBoxBackground, brushes.WarningBackground, $"{label} opened warning background", failures);
    }

    private static void VerifyAddOnStatusAndRuntimeChanges(ICollection<string> failures)
    {
        var leftText  = new Avalonia.Controls.TextBlock { Text = "left" };
        var rightIcon = new ProbeIcon();
        var box = new AddOnDecoratedBox
        {
            Width             = 260,
            Content           = new Avalonia.Controls.TextBlock { Text = "content" },
            ContentLeftAddOn  = leftText,
            ContentRightAddOn = rightIcon,
            Status            = InputControlStatus.Error,
            IsMotionEnabled   = false
        };

        using var realized = RealizeControl(box);
        var leftPresenter = FindVisualByName<ContentPresenter>(box, AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart);
        Expect(leftPresenter != null, "Content left addon presenter should exist.", failures);
        ExpectBrush(rightIcon.Foreground, box.AddOnStatusIconBrush, "Error status should apply icon brush.", failures);
        ExpectBrush(leftPresenter?.Foreground, box.AddOnStatusForeground, "Error status should apply addon foreground.", failures);

        box.SetCurrentValue(AddOnDecoratedBox.StatusProperty, InputControlStatus.Warning);
        RefreshLayout(realized.Window);
        ExpectBrush(rightIcon.Foreground, box.AddOnStatusIconBrush, "Warning status should refresh icon brush.", failures);
        ExpectBrush(leftPresenter?.Foreground, box.AddOnStatusForeground, "Warning status should refresh addon foreground.", failures);

        var sourceForeground = Brush(Color.FromRgb(34, 120, 222));
        var sourceIconBrush  = Brush(Color.FromRgb(222, 80, 34));
        box.SetCurrentValue(AddOnDecoratedBox.AddOnStatusForegroundProperty, sourceForeground);
        box.SetCurrentValue(AddOnDecoratedBox.AddOnStatusIconBrushProperty, sourceIconBrush);
        RefreshLayout(realized.Window);
        ExpectBrush(rightIcon.Foreground, sourceIconBrush, "Source icon brush update should refresh existing addon icon.", failures);
        ExpectBrush(leftPresenter?.Foreground, sourceForeground, "Source foreground update should refresh existing addon presenter.", failures);

        var runtimeIcon = new ProbeIcon();
        box.SetCurrentValue(AddOnDecoratedBox.ContentRightAddOnProperty, runtimeIcon);
        RefreshLayout(realized.Window);
        ExpectBrush(runtimeIcon.Foreground, sourceIconBrush, "Runtime content-right icon should receive current status brush.", failures);

        var runtimeText = new Avalonia.Controls.TextBlock { Text = "runtime-left" };
        box.SetCurrentValue(AddOnDecoratedBox.ContentLeftAddOnProperty, runtimeText);
        RefreshLayout(realized.Window);
        leftPresenter = FindVisualByName<ContentPresenter>(box, AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart);
        ExpectBrush(leftPresenter?.Foreground, sourceForeground, "Runtime content-left presenter should receive current foreground.", failures);

        box.ClearValue(AddOnDecoratedBox.AddOnStatusForegroundProperty);
        box.ClearValue(AddOnDecoratedBox.AddOnStatusIconBrushProperty);
        box.SetCurrentValue(InputElement.IsEnabledProperty, false);
        RefreshLayout(realized.Window);
        ExpectBrush(runtimeIcon.Foreground, box.AddOnStatusIconBrush, "Disabled state should refresh runtime addon icon.", failures);
        ExpectBrush(leftPresenter?.Foreground, box.AddOnStatusForeground, "Disabled state should refresh runtime addon foreground.", failures);
    }

    private static void VerifyEmptyAddOnSlotsSkipIconScan(ICollection<string> failures)
    {
        var box = new AddOnDecoratedBox
        {
            Width           = 260,
            Content         = new Avalonia.Controls.TextBlock { Text = "content" },
            IsMotionEnabled = false
        };
        using var realized = RealizeControl(box);

        AddOnDecoratedBoxPerfProbe.Reset();
        box.SetCurrentValue(AddOnDecoratedBox.StatusProperty, InputControlStatus.Error);
        RefreshLayout(realized.Window);
        var snapshot = AddOnDecoratedBoxPerfProbe.Snapshot();
        Expect(snapshot.UpdateIconStatusColorsCalls > 0,
            "Empty addon status change should still record status update for verification.", failures);
        Expect(snapshot.ApplyIconBrushCalls == 0,
            $"Empty addon status change should not call ApplyIconBrush, actual {snapshot.ApplyIconBrushCalls}.", failures);
        Expect(snapshot.ApplyIconBrushScannedVisuals == 0,
            $"Empty addon status change should not scan visuals, actual {snapshot.ApplyIconBrushScannedVisuals}.", failures);
    }

    private static void VerifyAddOnGeometry(ICollection<string> failures)
    {
        var box = new AddOnDecoratedBox
        {
            Width             = 260,
            Content           = new Avalonia.Controls.TextBlock { Text = "content" },
            LeftAddOn         = new Avalonia.Controls.TextBlock { Text = "left" },
            RightAddOn        = new Avalonia.Controls.TextBlock { Text = "right" },
            ContentLeftAddOn  = new Avalonia.Controls.TextBlock { Text = "inner-left" },
            ContentRightAddOn = new Avalonia.Controls.TextBlock { Text = "inner-right" },
            CornerRadius      = new CornerRadius(10),
            BorderThickness   = new Thickness(2),
            StyleVariant      = InputControlStyleVariant.Outlined,
            IsMotionEnabled   = false
        };

        using var _ = RealizeControl(box);

        Expect(box.InnerBoxCornerRadius.TopLeft == 0 &&
               box.InnerBoxCornerRadius.BottomLeft == 0 &&
               box.InnerBoxCornerRadius.TopRight == 0 &&
               box.InnerBoxCornerRadius.BottomRight == 0,
            $"Inner box should cut both sides when outer addons exist, actual {box.InnerBoxCornerRadius}.", failures);
        Expect(box.LeftAddOnCornerRadius.TopLeft == 10 &&
               box.LeftAddOnCornerRadius.BottomLeft == 10 &&
               box.LeftAddOnCornerRadius.TopRight == 0 &&
               box.LeftAddOnCornerRadius.BottomRight == 0,
            $"Left addon corner radius should keep only outer corners, actual {box.LeftAddOnCornerRadius}.", failures);
        Expect(box.RightAddOnCornerRadius.TopLeft == 0 &&
               box.RightAddOnCornerRadius.BottomLeft == 0 &&
               box.RightAddOnCornerRadius.TopRight == 10 &&
               box.RightAddOnCornerRadius.BottomRight == 10,
            $"Right addon corner radius should keep only outer corners, actual {box.RightAddOnCornerRadius}.", failures);
        Expect(box.LeftAddOnBorderThickness == new Thickness(2, 2, 0, 2),
            $"Left addon border thickness should cut the shared edge, actual {box.LeftAddOnBorderThickness}.", failures);
        Expect(box.RightAddOnBorderThickness == new Thickness(0, 2, 2, 2),
            $"Right addon border thickness should cut the shared edge, actual {box.RightAddOnBorderThickness}.", failures);
        Expect(FindVisualByName<ContentPresenter>(box, AddOnDecoratedBoxThemeConstants.ContentLeftAddOnPart)?.IsVisible == true,
            "Content-left addon presenter should be visible when inner addon exists.", failures);
        Expect(FindVisualByName<ContentPresenter>(box, AddOnDecoratedBoxThemeConstants.ContentRightAddOnPart)?.IsVisible == true,
            "Content-right addon presenter should be visible when inner addon exists.", failures);
    }

    private static void VerifyCompactSpaceGeometry(ICollection<string> failures)
    {
        VerifyCompactSpaceGeometry(Orientation.Horizontal, failures);
        VerifyCompactSpaceGeometry(Orientation.Vertical, failures);
    }

    private static void VerifyCompactSpaceGeometry(Orientation orientation, ICollection<string> failures)
    {
        var compactSpace = CreateCompactSpace(orientation);
        using var _ = RealizeControl(compactSpace);
        var boxes = compactSpace.GetSelfAndVisualDescendants().OfType<AddOnDecoratedBox>().ToList();

        Expect(boxes.Count == 3, $"CompactSpace {orientation} should have three AddOnDecoratedBox instances, actual {boxes.Count}.", failures);
        if (boxes.Count != 3)
        {
            return;
        }

        Expect(boxes.All(box => box.IsUsedInCompactSpace), $"CompactSpace {orientation} should mark all boxes as used in compact space.", failures);
        Expect(boxes.All(box => box.CompactSpaceOrientation == orientation), $"CompactSpace {orientation} should relay orientation to all boxes.", failures);
        Expect(boxes[0].CompactSpaceItemPosition?.HasFlag(SpaceItemPosition.First) == true,
            $"CompactSpace {orientation} first item should be marked First.", failures);
        Expect(boxes[1].CompactSpaceItemPosition?.HasFlag(SpaceItemPosition.Middle) == true,
            $"CompactSpace {orientation} middle item should be marked Middle.", failures);
        Expect(boxes[2].CompactSpaceItemPosition?.HasFlag(SpaceItemPosition.Last) == true,
            $"CompactSpace {orientation} last item should be marked Last.", failures);

        if (orientation == Orientation.Horizontal)
        {
            Expect(boxes[0].InnerBoxCornerRadius.TopRight == 0 && boxes[0].InnerBoxCornerRadius.BottomRight == 0,
                $"Horizontal first box should cut right corners, actual {boxes[0].InnerBoxCornerRadius}.", failures);
            Expect(IsZeroCornerRadius(boxes[1].InnerBoxCornerRadius),
                $"Horizontal middle box should cut all corners, actual {boxes[1].InnerBoxCornerRadius}.", failures);
            Expect(boxes[2].InnerBoxCornerRadius.TopLeft == 0 && boxes[2].InnerBoxCornerRadius.BottomLeft == 0,
                $"Horizontal last box should cut left corners, actual {boxes[2].InnerBoxCornerRadius}.", failures);
        }
        else
        {
            Expect(boxes[0].InnerBoxCornerRadius.BottomLeft == 0 && boxes[0].InnerBoxCornerRadius.BottomRight == 0,
                $"Vertical first box should cut bottom corners, actual {boxes[0].InnerBoxCornerRadius}.", failures);
            Expect(IsZeroCornerRadius(boxes[1].InnerBoxCornerRadius),
                $"Vertical middle box should cut all corners, actual {boxes[1].InnerBoxCornerRadius}.", failures);
            Expect(boxes[2].InnerBoxCornerRadius.TopLeft == 0 && boxes[2].InnerBoxCornerRadius.TopRight == 0,
                $"Vertical last box should cut top corners, actual {boxes[2].InnerBoxCornerRadius}.", failures);
        }
    }

    private static void VerifyRightAddOnTemplateBindings(ICollection<string> failures)
    {
        VerifyRightAddOnTemplateBinding(new Select { Width = 260 }, "Select", failures);
        VerifyRightAddOnTemplateBinding(new TreeSelect { Width = 260 }, "TreeSelect", failures);
        VerifyRightAddOnTemplateBinding(new Cascader { Width = 260 }, "Cascader", failures);
    }

    private static void VerifyRightAddOnTemplateBinding(AbstractSelect select, string label, ICollection<string> failures)
    {
        var leftTemplate  = new MarkerDataTemplate($"{label}-left");
        var rightTemplate = new MarkerDataTemplate($"{label}-right");
        select.SetCurrentValue(AbstractSelect.ContentLeftAddOnTemplateProperty, leftTemplate);
        select.SetCurrentValue(AbstractSelect.ContentRightAddOnProperty, $"{label}-content");
        select.SetCurrentValue(AbstractSelect.ContentRightAddOnTemplateProperty, rightTemplate);

        using var realized = RealizeControl(select);
        var presenter = FindVisualByName<ContentPresenter>(select, "PART_ContentRightAddOnPresenter");
        Expect(presenter != null, $"{label} should create PART_ContentRightAddOnPresenter.", failures);
        Expect(ReferenceEquals(presenter?.ContentTemplate, rightTemplate),
            $"{label} right addon presenter should bind ContentRightAddOnTemplate.", failures);

        var updatedTemplate = new MarkerDataTemplate($"{label}-right-updated");
        select.SetCurrentValue(AbstractSelect.ContentRightAddOnTemplateProperty, updatedTemplate);
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(presenter?.ContentTemplate, updatedTemplate),
            $"{label} right addon presenter should update when ContentRightAddOnTemplate changes.", failures);
    }

    private static void VerifyLineEditAccessoryLifecycle(ICollection<string> failures)
    {
        var lineEdit = CreateLineEdit(text: "abc");
        using var realized = RealizeControl(lineEdit);
        var decoratedBox = GetAddOnDecoratedBox(lineEdit, failures, "LineEdit");
        if (decoratedBox == null)
        {
            return;
        }

        Expect(decoratedBox.ContentRightAddOn == null, "LineEdit default should not materialize ContentRightAddOn.", failures);

        lineEdit.SetCurrentValue(AtomTextBox.IsAllowClearProperty, true);
        RefreshLayout(realized.Window);
        var clearHost = ExpectHost(decoratedBox, failures, "LineEdit clear");
        var clearButton = clearHost?.Children.OfType<InputClearIconButton>().SingleOrDefault();
        Expect(clearButton != null, "LineEdit clear should create InputClearIconButton.", failures);
        clearButton?.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent, clearButton));
        RefreshLayout(realized.Window);
        Expect(string.IsNullOrEmpty(lineEdit.Text), "LineEdit clear button should clear Text.", failures);
        Expect(decoratedBox.ContentRightAddOn == null, "LineEdit clear host should be removed after text is cleared.", failures);
        Expect(clearHost?.Children.Count == 0, "LineEdit clear host should clear children after detach.", failures);
        Expect(clearHost?.GetVisualParent() == null, "LineEdit detached clear host should leave the visual tree.", failures);
        lineEdit.SetCurrentValue(AtomTextBox.IsAllowClearProperty, false);
        RefreshLayout(realized.Window);

        lineEdit.SetCurrentValue(Avalonia.Controls.TextBox.TextProperty, "secret");
        lineEdit.SetCurrentValue(Avalonia.Controls.TextBox.PasswordCharProperty, '*');
        lineEdit.SetCurrentValue(AtomTextBox.IsEnableRevealButtonProperty, true);
        RefreshLayout(realized.Window);
        var revealHost = ExpectHost(decoratedBox, failures, "LineEdit reveal");
        var revealButton = revealHost?.Children.OfType<RevealButton>().SingleOrDefault();
        Expect(revealButton != null, "LineEdit reveal should create RevealButton.", failures);
        lineEdit.SetCurrentValue(Avalonia.Controls.TextBox.RevealPasswordProperty, true);
        RefreshLayout(realized.Window);
        Expect(revealButton?.IsChecked == true, "LineEdit RevealPassword should sync to RevealButton.", failures);
        revealButton?.SetCurrentValue(ToggleButton.IsCheckedProperty, false);
        RefreshLayout(realized.Window);
        Expect(!lineEdit.RevealPassword, "RevealButton should sync IsChecked back to LineEdit.RevealPassword.", failures);
        lineEdit.SetCurrentValue(AtomTextBox.IsEnableRevealButtonProperty, false);
        RefreshLayout(realized.Window);
        Expect(decoratedBox.ContentRightAddOn == null, "LineEdit reveal host should be removed after disabling reveal.", failures);
        Expect(revealHost?.Children.Count == 0, "LineEdit reveal host should clear children after detach.", failures);

        lineEdit.SetCurrentValue(Avalonia.Controls.TextBox.TextProperty, "abc");
        lineEdit.SetCurrentValue(Avalonia.Controls.TextBox.MaxLengthProperty, 10);
        lineEdit.SetCurrentValue(AtomTextBox.IsShowCountProperty, true);
        RefreshLayout(realized.Window);
        var countHost = ExpectHost(decoratedBox, failures, "LineEdit count");
        var countText = countHost?.Children.OfType<Avalonia.Controls.TextBlock>()
                                 .SingleOrDefault(textBlock => textBlock.Name == "TextCountIndicator");
        Expect(countText?.Text == "3 / 10", $"LineEdit count text should be '3 / 10', actual '{countText?.Text}'.", failures);
        lineEdit.SetCurrentValue(AtomTextBox.IsShowCountProperty, false);
        RefreshLayout(realized.Window);
        Expect(decoratedBox.ContentRightAddOn == null, "LineEdit count host should be removed after hiding count.", failures);
        Expect(countHost?.Children.Count == 0, "LineEdit count host should clear children after detach.", failures);

        var innerRightContent = new Avalonia.Controls.TextBlock { Text = "kg" };
        lineEdit.SetCurrentValue(Avalonia.Controls.TextBox.InnerRightContentProperty, innerRightContent);
        RefreshLayout(realized.Window);
        var innerRightHost = ExpectHost(decoratedBox, failures, "LineEdit inner right");
        var innerRightPresenter = innerRightHost?.Children.OfType<ContentPresenter>()
                                               .SingleOrDefault(presenter => presenter.Name == "InnerRightContentPresenter");
        Expect(ReferenceEquals(innerRightPresenter?.Content, innerRightContent), "LineEdit inner right presenter should hold InnerRightContent.", failures);
        lineEdit.SetCurrentValue(Avalonia.Controls.TextBox.InnerRightContentProperty, null);
        RefreshLayout(realized.Window);
        Expect(decoratedBox.ContentRightAddOn == null, "LineEdit inner right host should be removed after clearing content.", failures);
        Expect(innerRightHost?.Children.Count == 0, "LineEdit inner right host should clear children after detach.", failures);
        Expect(innerRightPresenter?.Content == null, "LineEdit inner right presenter should clear Content after detach.", failures);

        var feedback = new FormValidateFeedback
        {
            ErrorFeedback = new Avalonia.Controls.TextBlock { Text = "!" }
        };
        lineEdit.FormFeedback = feedback;
        feedback.SetCurrentValue(FormValidateFeedback.ValidateStatusProperty, FormValidateStatus.Error);
        RefreshLayout(realized.Window);
        var feedbackHost = ExpectHost(decoratedBox, failures, "LineEdit form feedback");
        var feedbackPresenter = feedbackHost?.Children.OfType<ContentPresenter>()
                                           .SingleOrDefault(presenter => presenter.Name == "FormFeedBack");
        Expect(ReferenceEquals(feedbackPresenter?.Content, feedback), "LineEdit form feedback presenter should hold FormFeedback.", failures);
        feedback.SetCurrentValue(FormValidateFeedback.ValidateStatusProperty, FormValidateStatus.Default);
        RefreshLayout(realized.Window);
        Expect(decoratedBox.ContentRightAddOn == null, "LineEdit form feedback host should be removed when feedback status returns to default.", failures);
        Expect(feedbackHost?.Children.Count == 0, "LineEdit feedback host should clear children after detach.", failures);
        Expect(feedbackPresenter?.Content == null, "LineEdit feedback presenter should clear Content after detach.", failures);
    }

    private static void VerifyTextAreaAccessoryLifecycle(ICollection<string> failures)
    {
        var textArea = new TextArea
        {
            Text  = "abc",
            Width = 260
        };
        using var realized = RealizeControl(textArea);
        var decoratedBox = GetAddOnDecoratedBox(textArea, failures, "TextArea");
        if (decoratedBox == null)
        {
            return;
        }

        Expect(decoratedBox.ContentRightAddOn == null, "TextArea default should not materialize ContentRightAddOn.", failures);

        textArea.SetCurrentValue(TextArea.IsAllowClearProperty, true);
        RefreshLayout(realized.Window);
        var clearHost = ExpectTextAreaHost(decoratedBox, failures, "TextArea clear");
        var clearButton = clearHost?.Children.OfType<InputClearIconButton>().SingleOrDefault();
        Expect(clearButton != null, "TextArea clear should create InputClearIconButton.", failures);
        clearButton?.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent, clearButton));
        RefreshLayout(realized.Window);
        Expect(string.IsNullOrEmpty(textArea.Text), "TextArea clear button should clear Text.", failures);
        Expect(decoratedBox.ContentRightAddOn == null, "TextArea clear host should be removed after text is cleared.", failures);
        Expect(clearHost?.Children.Count == 0, "TextArea clear host should clear children after detach.", failures);
        Expect(clearHost?.GetVisualParent() == null, "TextArea detached clear host should leave the visual tree.", failures);

        var innerRightContent = new Avalonia.Controls.TextBlock { Text = "rows" };
        textArea.SetCurrentValue(Avalonia.Controls.TextBox.InnerRightContentProperty, innerRightContent);
        RefreshLayout(realized.Window);
        var innerRightHost = ExpectTextAreaHost(decoratedBox, failures, "TextArea inner right");
        var innerRightPresenter = innerRightHost?.Children.OfType<ContentPresenter>()
                                               .SingleOrDefault(presenter => presenter.Name == "PART_InnerRightContentPresenter");
        Expect(ReferenceEquals(innerRightPresenter?.Content, innerRightContent), "TextArea inner right presenter should hold InnerRightContent.", failures);
        textArea.SetCurrentValue(Avalonia.Controls.TextBox.InnerRightContentProperty, null);
        RefreshLayout(realized.Window);
        Expect(decoratedBox.ContentRightAddOn == null, "TextArea inner right host should be removed after clearing content.", failures);
        Expect(innerRightHost?.Children.Count == 0, "TextArea inner right host should clear children after detach.", failures);
        Expect(innerRightPresenter?.Content == null, "TextArea inner right presenter should clear Content after detach.", failures);
    }

    private static void VerifySearchEditAccessoryLifecycle(ICollection<string> failures)
    {
        var searchEdit = new SearchEdit
        {
            Text        = "search",
            Width       = 260,
            IsShowCount = true,
            MaxLength   = 30
        };
        using var realized = RealizeControl(searchEdit);
        var decoratedBox = GetAddOnDecoratedBox(searchEdit, failures, "SearchEdit");
        if (decoratedBox == null)
        {
            return;
        }

        Expect(decoratedBox.ContentRightAddOn == null, "SearchEdit count should not materialize LineEdit count accessory.", failures);

        searchEdit.SetCurrentValue(AtomTextBox.IsAllowClearProperty, true);
        RefreshLayout(realized.Window);
        var clearHost = ExpectHost(decoratedBox, failures, "SearchEdit clear");
        Expect(clearHost?.Children.OfType<InputClearIconButton>().SingleOrDefault() != null,
            "SearchEdit clear should still create InputClearIconButton.", failures);

        searchEdit.SetCurrentValue(AtomTextBox.IsAllowClearProperty, false);
        RefreshLayout(realized.Window);
        Expect(decoratedBox.ContentRightAddOn == null, "SearchEdit clear host should be removed after disabling clear.", failures);
        Expect(clearHost?.Children.Count == 0, "SearchEdit clear host should clear children after detach.", failures);
    }

    private static RealizedScenario RealizeControl(Control control)
    {
        return RealizeScenario(new PerfScenario("Accessory.Verify", _ => control), 1);
    }

    private static void RefreshLayout(Avalonia.Controls.Window window)
    {
        Dispatcher.UIThread.RunJobs();
        window.Measure(MeasureSize);
        window.Arrange(ArrangeRect);
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
    }

    private static AddOnDecoratedBox? GetAddOnDecoratedBox(Control root, ICollection<string> failures, string label)
    {
        var decoratedBoxes = root.GetSelfAndVisualDescendants()
                                 .OfType<AddOnDecoratedBox>()
                                 .ToList();
        if (decoratedBoxes.Count != 1)
        {
            failures.Add($"{label} should have exactly one AddOnDecoratedBox, actual {decoratedBoxes.Count}.");
            return null;
        }

        return decoratedBoxes[0];
    }

    private static LineEditAccessoryHost? ExpectHost(AddOnDecoratedBox decoratedBox,
                                                     ICollection<string> failures,
                                                     string label)
    {
        if (decoratedBox.ContentRightAddOn is LineEditAccessoryHost host)
        {
            return host;
        }

        failures.Add($"{label} should materialize LineEditAccessoryHost.");
        return null;
    }

    private static TextAreaAccessoryHost? ExpectTextAreaHost(AddOnDecoratedBox decoratedBox,
                                                             ICollection<string> failures,
                                                             string label)
    {
        if (decoratedBox.ContentRightAddOn is TextAreaAccessoryHost host)
        {
            return host;
        }

        failures.Add($"{label} should materialize TextAreaAccessoryHost.");
        return null;
    }

    private static void Expect(bool condition, string message, ICollection<string> failures)
    {
        if (!condition)
        {
            failures.Add(message);
        }
    }

    private static T? FindVisualByName<T>(Control root, string name)
        where T : Control
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<T>()
                   .FirstOrDefault(control => control.Name == name);
    }

    private static bool IsZeroCornerRadius(CornerRadius cornerRadius)
    {
        return cornerRadius.TopLeft == 0 &&
               cornerRadius.TopRight == 0 &&
               cornerRadius.BottomLeft == 0 &&
               cornerRadius.BottomRight == 0;
    }

    private static void ExpectBrush(IBrush? actual, IBrush? expected, string label, ICollection<string> failures)
    {
        if (BrushEquals(actual, expected))
        {
            return;
        }

        failures.Add($"{label}: expected {DescribeBrush(expected)}, actual {DescribeBrush(actual)}.");
    }

    private static bool BrushEquals(IBrush? actual, IBrush? expected)
    {
        if (ReferenceEquals(actual, expected))
        {
            return true;
        }
        if (actual is ISolidColorBrush actualSolid && expected is ISolidColorBrush expectedSolid)
        {
            return actualSolid.Color == expectedSolid.Color &&
                   Math.Abs(actualSolid.Opacity - expectedSolid.Opacity) < 0.001;
        }
        return Equals(actual, expected);
    }

    private static string DescribeBrush(IBrush? brush)
    {
        return brush switch
        {
            null => "<null>",
            ISolidColorBrush solid => $"{solid.Color} opacity {solid.Opacity:0.###}",
            _ => brush.ToString() ?? brush.GetType().Name
        };
    }

    private static void ResetEffectiveBrushState(AddOnDecoratedBox box, InputControlStyleVariant styleVariant)
    {
        box.SetCurrentValue(InputElement.IsEnabledProperty, true);
        box.SetCurrentValue(AddOnDecoratedBox.StyleVariantProperty, styleVariant);
        box.SetCurrentValue(AddOnDecoratedBox.StatusProperty, InputControlStatus.Default);
        box.IsInnerBoxHover   = false;
        box.IsInnerBoxPressed = false;
    }

    private static EffectiveBrushSet ApplyTestBrushes(AddOnDecoratedBox box)
    {
        var brushes = new EffectiveBrushSet();
        box.InnerBoxDefaultBorderBrush        = brushes.DefaultBorder;
        box.InnerBoxHoverBorderBrush          = brushes.HoverBorder;
        box.InnerBoxActiveBorderBrush         = brushes.ActiveBorder;
        box.InnerBoxFilledBackground          = brushes.FilledBackground;
        box.InnerBoxFilledBorderBrush         = brushes.FilledBorder;
        box.InnerBoxFilledHoverBackground     = brushes.FilledHoverBackground;
        box.InnerBoxActiveBackground          = brushes.ActiveBackground;
        box.InnerBoxDisabledBackground        = brushes.DisabledBackground;
        box.InnerBoxErrorBorderBrush          = brushes.ErrorBorder;
        box.InnerBoxErrorHoverBorderBrush     = brushes.ErrorHoverBorder;
        box.InnerBoxErrorBackground           = brushes.ErrorBackground;
        box.InnerBoxErrorFilledBorderBrush    = brushes.ErrorFilledBorder;
        box.InnerBoxErrorHoverBackground      = brushes.ErrorHoverBackground;
        box.InnerBoxWarningBorderBrush        = brushes.WarningBorder;
        box.InnerBoxWarningHoverBorderBrush   = brushes.WarningHoverBorder;
        box.InnerBoxWarningBackground         = brushes.WarningBackground;
        box.InnerBoxWarningFilledBorderBrush  = brushes.WarningFilledBorder;
        box.InnerBoxWarningHoverBackground    = brushes.WarningHoverBackground;
        return brushes;
    }

    private static IBrush Brush(Color color)
    {
        return new SolidColorBrush(color);
    }

    private static string RenderTable(IReadOnlyList<PerfResult> results)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Scenario                                Count  Total ms  ms/item  KB/item  Visual  Logical  CP  Button  TB  Icon  Stack  AODB  IconUpdates  BrushCalls  Scanned");
        builder.AppendLine("----------------------------------------------------------------------------------------------------------------------------------------------------------------");

        foreach (var result in results)
        {
            builder.Append(CultureInfo.InvariantCulture,
                $"{result.Name,-39}{result.Count,5}{result.Elapsed.TotalMilliseconds,10:0.00}{result.MillisecondsPerItem,9:0.000}{result.KilobytesPerItem,9:0.0}");
            builder.Append(CultureInfo.InvariantCulture,
                $"{result.TreeStats.VisualPerRoot,8:0.0}{result.TreeStats.LogicalPerRoot,9:0.0}");
            builder.Append(CultureInfo.InvariantCulture,
                $"{result.TreeStats.ContentPresenterPerRoot,4:0.0}{result.TreeStats.ButtonPerRoot,8:0.0}{result.TreeStats.TextBlockPerRoot,5:0.0}{result.TreeStats.IconPerRoot,6:0.0}");
            builder.Append(CultureInfo.InvariantCulture,
                $"{result.TreeStats.StackPanelPerRoot,7:0.0}{result.TreeStats.AddOnDecoratedBoxPerRoot,6:0.0}");
            builder.Append(CultureInfo.InvariantCulture,
                $"{result.ProbeSnapshot.UpdateIconStatusColorsCalls,13}{result.ProbeSnapshot.ApplyIconBrushCalls,12}{result.ProbeSnapshot.ApplyIconBrushScannedVisuals,9}");
            builder.AppendLine();
        }

        return builder.ToString();
    }

    private static string RenderMarkdown(IReadOnlyList<PerfResult> results, int count)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# AddOnDecoratedBox / LineEdit Baseline");
        builder.AppendLine();
        builder.AppendLine($"- Date: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}");
        builder.AppendLine($"- Configuration: Debug");
        builder.AppendLine($"- Count per scenario: {count}");
        builder.AppendLine($"- Runner: `tools/performances/AtomUI.Performance`");
        builder.AppendLine();
        builder.AppendLine("| Scenario | Count | Total ms | ms/item | KB/item | Visual/root | Logical/root | ContentPresenter/root | Button/root | TextBlock/root | Icon/root | StackPanel/root | AddOnDecoratedBox/root | Icon status calls | Icon brush calls | Icon scan visuals | Icon matches |");
        builder.AppendLine("| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |");

        foreach (var result in results)
        {
            builder.Append(CultureInfo.InvariantCulture,
                $"| {result.Name} | {result.Count} | {result.Elapsed.TotalMilliseconds:0.00} | {result.MillisecondsPerItem:0.000} | {result.KilobytesPerItem:0.0} | ");
            builder.Append(CultureInfo.InvariantCulture,
                $"{result.TreeStats.VisualPerRoot:0.0} | {result.TreeStats.LogicalPerRoot:0.0} | {result.TreeStats.ContentPresenterPerRoot:0.0} | ");
            builder.Append(CultureInfo.InvariantCulture,
                $"{result.TreeStats.ButtonPerRoot:0.0} | {result.TreeStats.TextBlockPerRoot:0.0} | {result.TreeStats.IconPerRoot:0.0} | ");
            builder.Append(CultureInfo.InvariantCulture,
                $"{result.TreeStats.StackPanelPerRoot:0.0} | {result.TreeStats.AddOnDecoratedBoxPerRoot:0.0} | ");
            builder.Append(CultureInfo.InvariantCulture,
                $"{result.ProbeSnapshot.UpdateIconStatusColorsCalls} | {result.ProbeSnapshot.ApplyIconBrushCalls} | ");
            builder.Append(CultureInfo.InvariantCulture,
                $"{result.ProbeSnapshot.ApplyIconBrushScannedVisuals} | {result.ProbeSnapshot.ApplyIconBrushMatchedIcons} |");
            builder.AppendLine();
        }

        builder.AppendLine();
        builder.AppendLine("Notes:");
        builder.AppendLine();
        builder.AppendLine("- `Visual/root` and `Logical/root` include the scenario root control itself.");
        builder.AppendLine("- CompactSpace scenarios use three `LineEdit` children per root.");
        builder.AppendLine("- Icon probe data is Debug-only and records `AddOnDecoratedBox.UpdateIconStatusColors()` plus `ApplyIconBrush()` scans.");
        builder.AppendLine("- Binding expression count is not directly measured yet; current baseline uses node counts, allocation, timing, and AddOnDecoratedBox probe counters.");
        builder.AppendLine("- This measures control-level template/style/materialization cost, not Gallery navigation.");
        return builder.ToString();
    }
}

internal sealed record PerfOptions(
    int Count,
    string? MarkdownOutputPath,
    bool VerifyAccessories,
    bool VerifyEffectiveBrushes,
    bool VerifyAddonStates)
{
    public static PerfOptions Parse(string[] args)
    {
        var count                  = Program.DefaultCount;
        string? markdownOutput     = null;
        var verifyAccessories      = false;
        var verifyEffectiveBrushes = false;
        var verifyAddonStates      = false;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--count" when i + 1 < args.Length && int.TryParse(args[i + 1], out var parsedCount):
                    count = parsedCount;
                    i++;
                    break;
                case "--markdown" when i + 1 < args.Length:
                    markdownOutput = args[i + 1];
                    i++;
                    break;
                case "--verify-accessories":
                    verifyAccessories = true;
                    break;
                case "--verify-effective-brushes":
                    verifyEffectiveBrushes = true;
                    break;
                case "--verify-addon-states":
                    verifyAddonStates = true;
                    break;
            }
        }

        return new PerfOptions(
            Math.Max(1, count),
            markdownOutput,
            verifyAccessories,
            verifyEffectiveBrushes,
            verifyAddonStates);
    }
}

internal sealed class ProbeIcon : Icon
{
}

internal sealed class MarkerDataTemplate : IDataTemplate
{
    private readonly string _marker;

    public MarkerDataTemplate(string marker)
    {
        _marker = marker;
    }

    public bool Match(object? data)
    {
        return true;
    }

    public Control Build(object? param)
    {
        return new Avalonia.Controls.TextBlock
        {
            Name = _marker,
            Text = _marker
        };
    }
}

internal sealed class EffectiveBrushSet
{
    public IBrush DefaultBorder { get; } = ProgramTestBrush(10, 10, 10);
    public IBrush HoverBorder { get; } = ProgramTestBrush(20, 20, 20);
    public IBrush ActiveBorder { get; } = ProgramTestBrush(30, 30, 30);
    public IBrush FilledBackground { get; } = ProgramTestBrush(40, 40, 40);
    public IBrush FilledBorder { get; } = ProgramTestBrush(50, 50, 50);
    public IBrush FilledHoverBackground { get; } = ProgramTestBrush(60, 60, 60);
    public IBrush ActiveBackground { get; } = ProgramTestBrush(70, 70, 70);
    public IBrush DisabledBackground { get; } = ProgramTestBrush(80, 80, 80);
    public IBrush ErrorBorder { get; } = ProgramTestBrush(90, 20, 20);
    public IBrush ErrorHoverBorder { get; } = ProgramTestBrush(100, 30, 30);
    public IBrush ErrorBackground { get; } = ProgramTestBrush(110, 40, 40);
    public IBrush ErrorFilledBorder { get; } = ProgramTestBrush(120, 50, 50);
    public IBrush ErrorHoverBackground { get; } = ProgramTestBrush(130, 60, 60);
    public IBrush WarningBorder { get; } = ProgramTestBrush(90, 90, 20);
    public IBrush WarningHoverBorder { get; } = ProgramTestBrush(100, 100, 30);
    public IBrush WarningBackground { get; } = ProgramTestBrush(110, 110, 40);
    public IBrush WarningFilledBorder { get; } = ProgramTestBrush(120, 120, 50);
    public IBrush WarningHoverBackground { get; } = ProgramTestBrush(130, 130, 60);

    private static IBrush ProgramTestBrush(byte red, byte green, byte blue)
    {
        return new SolidColorBrush(Color.FromRgb(red, green, blue));
    }
}

internal sealed record PerfScenario(string Name, Func<int, Control> Create);

internal sealed record PerfResult(
    string Name,
    int Count,
    TimeSpan Elapsed,
    long AllocatedBytes,
    TreeStats TreeStats,
    AddOnDecoratedBoxPerfSnapshot ProbeSnapshot)
{
    public double MillisecondsPerItem => Elapsed.TotalMilliseconds / Count;
    public double KilobytesPerItem    => AllocatedBytes / 1024.0 / Count;
}

internal sealed class RealizedScenario : IDisposable
{
    public RealizedScenario(Avalonia.Controls.Window window, IReadOnlyList<Control> rootControls)
    {
        Window       = window;
        RootControls = rootControls;
    }

    public Avalonia.Controls.Window Window { get; }
    public IReadOnlyList<Control> RootControls { get; }

    public void Dispose()
    {
        Window.Close();
        Dispatcher.UIThread.RunJobs();
    }
}

internal sealed record TreeStats(
    double VisualPerRoot,
    double LogicalPerRoot,
    double ContentPresenterPerRoot,
    double ButtonPerRoot,
    double TextBlockPerRoot,
    double IconPerRoot,
    double StackPanelPerRoot,
    double AddOnDecoratedBoxPerRoot)
{
    public static TreeStats Collect(IReadOnlyList<Control> roots)
    {
        var visualCount              = 0;
        var logicalCount             = 0;
        var contentPresenterCount    = 0;
        var buttonCount              = 0;
        var textBlockCount           = 0;
        var iconCount                = 0;
        var stackPanelCount          = 0;
        var addOnDecoratedBoxCount   = 0;

        foreach (var root in roots)
        {
            var visuals = root.GetSelfAndVisualDescendants().ToList();
            visualCount += visuals.Count;

            foreach (var visual in visuals)
            {
                var type = visual.GetType();
                if (type.Name == "ContentPresenter")
                {
                    contentPresenterCount++;
                }
                if (visual is Avalonia.Controls.Button)
                {
                    buttonCount++;
                }
                if (type.Name == "TextBlock")
                {
                    textBlockCount++;
                }
                if (type.Name.EndsWith("Icon", StringComparison.Ordinal) || IsAtomIcon(type))
                {
                    iconCount++;
                }
                if (visual is StackPanel)
                {
                    stackPanelCount++;
                }
                if (IsAddOnDecoratedBox(type))
                {
                    addOnDecoratedBoxCount++;
                }
            }

            logicalCount += root.GetSelfAndLogicalDescendants().Count();
        }

        var rootCount = Math.Max(1, roots.Count);
        return new TreeStats(
            visualCount / (double)rootCount,
            logicalCount / (double)rootCount,
            contentPresenterCount / (double)rootCount,
            buttonCount / (double)rootCount,
            textBlockCount / (double)rootCount,
            iconCount / (double)rootCount,
            stackPanelCount / (double)rootCount,
            addOnDecoratedBoxCount / (double)rootCount);
    }

    private static bool IsAtomIcon(Type type)
    {
        while (type.BaseType != null)
        {
            if (type.BaseType.FullName == "AtomUI.Controls.Icon")
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }

    private static bool IsAddOnDecoratedBox(Type type)
    {
        if (type.FullName == "AtomUI.Desktop.Controls.AddOnDecoratedBox")
        {
            return true;
        }

        while (type.BaseType != null)
        {
            if (type.BaseType.FullName == "AtomUI.Desktop.Controls.AddOnDecoratedBox")
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }
}
