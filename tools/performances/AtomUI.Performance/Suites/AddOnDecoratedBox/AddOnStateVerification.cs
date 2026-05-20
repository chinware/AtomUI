using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.Primitives.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using AtomTextBox = AtomUI.Desktop.Controls.TextBox;

namespace AtomUI.Performance;

internal static partial class Program
{
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
}
