using System.Reflection;
using AtomUI;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private const int CompactSpaceActiveZIndex = 1000;
    private const int CompactSpaceNormalZIndex = 0;

    private static bool RunSpaceStateVerification()
    {
        var failures = new List<string>();
        VerifyCompactSpaceFillerValidation(failures);
        VerifyCompactSpacePositions(failures);
        VerifyCompactSpaceZIndexState(failures);
        VerifyCompactSpaceRemoveChildCleanup(failures);
        VerifySpaceExplicitSpacing(failures);
        VerifySpaceTemplatePrioritySpacingBinding(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Space state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Space state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyCompactSpaceFillerValidation(ICollection<string> failures)
    {
        var valid = new CompactSpace
        {
            Orientation = Orientation.Horizontal
        };
        valid.Children.Add(new LineEdit { Text = "input" });
        var filler = new CompactSpaceFiller();
        valid.Children.Add(filler);
        using var validRealized = RealizeControl(valid);
        var validItems = FindCompactSpaceItems(valid);
        Expect(validItems.Count == 1,
            $"CompactSpaceFiller should not create an extra CompactSpaceItem wrapper, actual wrappers {validItems.Count}.",
            failures);
        Expect(filler.GetVisualParent() != null,
            "CompactSpaceFiller should still be realized as a layout child.",
            failures);

        var misplaced = new CompactSpace
        {
            Orientation = Orientation.Horizontal
        };
        misplaced.Children.Add(new CompactSpaceFiller());
        misplaced.Children.Add(new LineEdit { Text = "input" });
        ExpectThrows<InvalidSpaceFillerUsageException>(
            () =>
            {
                using var _ = RealizeControl(misplaced);
            },
            "CompactSpaceFiller before the last child should throw.",
            failures);

        var duplicated = new CompactSpace
        {
            Orientation = Orientation.Horizontal
        };
        duplicated.Children.Add(new LineEdit { Text = "input" });
        duplicated.Children.Add(new CompactSpaceFiller());
        duplicated.Children.Add(new CompactSpaceFiller());
        ExpectThrows<InvalidSpaceFillerUsageException>(
            () =>
            {
                using var _ = RealizeControl(duplicated);
            },
            "Multiple CompactSpaceFiller children should throw.",
            failures);
    }

    private static void VerifyCompactSpacePositions(ICollection<string> failures)
    {
        VerifyCompactSpacePositions(Orientation.Horizontal, failures);
        VerifyCompactSpacePositions(Orientation.Vertical, failures);
    }

    private static void VerifyCompactSpacePositions(Orientation orientation, ICollection<string> failures)
    {
        var compactSpace = new CompactSpace
        {
            Orientation = orientation,
            Width       = 420
        };
        compactSpace.Children.Add(new LineEdit { Text = "first" });
        compactSpace.Children.Add(new LineEdit { Text = "middle" });
        compactSpace.Children.Add(new LineEdit { Text = "last" });

        using var _ = RealizeControl(compactSpace);
        var boxes = compactSpace.GetSelfAndVisualDescendants().OfType<AddOnDecoratedBox>().ToList();
        Expect(boxes.Count == 3,
            $"CompactSpace {orientation} should create three AddOnDecoratedBox children, actual {boxes.Count}.",
            failures);
        if (boxes.Count != 3)
        {
            return;
        }

        Expect(boxes.All(box => box.IsUsedInCompactSpace),
            $"CompactSpace {orientation} should mark all boxes as compact.",
            failures);
        Expect(boxes.All(box => box.CompactSpaceOrientation == orientation),
            $"CompactSpace {orientation} should relay orientation to all boxes.",
            failures);
        Expect(boxes[0].CompactSpaceItemPosition?.HasFlag(SpaceItemPosition.First) == true,
            $"CompactSpace {orientation} first child should be First.",
            failures);
        Expect(boxes[1].CompactSpaceItemPosition?.HasFlag(SpaceItemPosition.Middle) == true,
            $"CompactSpace {orientation} middle child should be Middle.",
            failures);
        Expect(boxes[2].CompactSpaceItemPosition?.HasFlag(SpaceItemPosition.Last) == true,
            $"CompactSpace {orientation} last child should be Last.",
            failures);
    }

    private static void VerifyCompactSpaceZIndexState(ICollection<string> failures)
    {
        var first = new AtomUI.Desktop.Controls.Button
        {
            Content = "First"
        };
        var second = new AtomUI.Desktop.Controls.Button
        {
            Content = "Second"
        };
        var compactSpace = new CompactSpace
        {
            Orientation = Orientation.Horizontal
        };
        compactSpace.Children.Add(first);
        compactSpace.Children.Add(second);

        using var realized = RealizeControl(compactSpace);
        var items = FindCompactSpaceItems(compactSpace);
        Expect(items.Count == 2,
            $"CompactSpace Button group should create two CompactSpaceItem wrappers, actual {items.Count}.",
            failures);
        if (items.Count != 2)
        {
            return;
        }

        InvokeCompactSpaceHandler(compactSpace, "HandlePointerEntered", first, failures);
        RefreshLayout(realized.Window);
        Expect(items[0].ZIndex == CompactSpaceActiveZIndex,
            $"Pointer enter should activate first compact item, actual z-index {items[0].ZIndex}.",
            failures);
        Expect(items[1].ZIndex == CompactSpaceNormalZIndex,
            $"Pointer enter should keep second compact item normal, actual z-index {items[1].ZIndex}.",
            failures);

        InvokeCompactSpaceHandler(compactSpace, "HandlePointerExited", first, failures);
        RefreshLayout(realized.Window);
        Expect(items[0].ZIndex == CompactSpaceNormalZIndex,
            $"Pointer exit should restore first compact item, actual z-index {items[0].ZIndex}.",
            failures);

        InvokeCompactSpaceHandler(compactSpace, "HandleGotFocus", second, failures);
        RefreshLayout(realized.Window);
        Expect(items[1].ZIndex == CompactSpaceActiveZIndex,
            $"Focus should activate second compact item, actual z-index {items[1].ZIndex}.",
            failures);
    }

    private static void VerifyCompactSpaceRemoveChildCleanup(ICollection<string> failures)
    {
        var first = new LineEdit { Text = "first" };
        var second = new LineEdit { Text = "second" };
        var compactSpace = new CompactSpace
        {
            Orientation = Orientation.Horizontal
        };
        compactSpace.Children.Add(first);
        compactSpace.Children.Add(second);

        using var realized = RealizeControl(compactSpace);
        var initialItems = FindCompactSpaceItems(compactSpace);
        Expect(initialItems.Count == 2,
            $"CompactSpace should start with two wrappers, actual {initialItems.Count}.",
            failures);

        compactSpace.Children.Remove(first);
        RefreshLayout(realized.Window);

        var remainingItems = FindCompactSpaceItems(compactSpace);
        Expect(remainingItems.Count == 1,
            $"CompactSpace should remove wrapper when child is removed, actual wrappers {remainingItems.Count}.",
            failures);
        Expect(first.GetVisualParent() == null,
            "Removed CompactSpace child should not keep a visual parent.",
            failures);
    }

    private static void VerifySpaceExplicitSpacing(ICollection<string> failures)
    {
        var space = new Space
        {
            ItemSpacing = 12,
            LineSpacing = 14
        };
        space.Children.Add(new AtomUI.Desktop.Controls.Button { Content = "One" });
        space.Children.Add(new AtomUI.Desktop.Controls.Button { Content = "Two" });

        using var realized = RealizeControl(space);
        space.SizeType = CustomizableSizeType.Large;
        RefreshLayout(realized.Window);

        Expect(Math.Abs(space.ItemSpacing - 12) < 0.001,
            $"Explicit Space.ItemSpacing should survive SizeType changes, actual {space.ItemSpacing}.",
            failures);
        Expect(Math.Abs(space.LineSpacing - 14) < 0.001,
            $"Explicit Space.LineSpacing should survive SizeType changes, actual {space.LineSpacing}.",
            failures);

        space.ClearValue(Space.ItemSpacingProperty);
        space.ClearValue(Space.LineSpacingProperty);
        RefreshLayout(realized.Window);

        Expect(space.ItemSpacing > 0,
            $"Cleared Space.ItemSpacing should restore token value, actual {space.ItemSpacing}.",
            failures);
        Expect(space.LineSpacing > 0,
            $"Cleared Space.LineSpacing should restore token value, actual {space.LineSpacing}.",
            failures);
    }

    private static void VerifySpaceTemplatePrioritySpacingBinding(ICollection<string> failures)
    {
        var source = new SpaceTemplateSpacingSource
        {
            Value = 37
        };
        var space = new Space
        {
            SizeType = CustomizableSizeType.Small
        };
        space.Children.Add(new AtomUI.Desktop.Controls.Button { Content = "One" });
        space.Children.Add(new AtomUI.Desktop.Controls.Button { Content = "Two" });
        using var itemBinding = space.Bind(
            Space.ItemSpacingProperty,
            source.GetObservable(SpaceTemplateSpacingSource.ValueProperty),
            BindingPriority.Template);
        using var lineBinding = space.Bind(
            Space.LineSpacingProperty,
            source.GetObservable(SpaceTemplateSpacingSource.ValueProperty),
            BindingPriority.Template);

        using var realized = RealizeControl(space);
        RefreshLayout(realized.Window);
        var smallItemSpacing = space.ItemSpacing;
        var smallLineSpacing = space.LineSpacing;
        ExpectDifferent(space.ItemSpacing, source.Value,
            $"Small Space.ItemSpacing should use token spacing instead of custom slider binding, actual {space.ItemSpacing}.",
            failures);
        ExpectDifferent(space.LineSpacing, source.Value,
            $"Small Space.LineSpacing should use token spacing instead of custom slider binding, actual {space.LineSpacing}.",
            failures);

        space.SizeType = CustomizableSizeType.Middle;
        RefreshLayout(realized.Window);
        ExpectDifferent(space.ItemSpacing, source.Value,
            $"Middle Space.ItemSpacing should use token spacing instead of custom slider binding, actual {space.ItemSpacing}.",
            failures);
        ExpectDifferent(space.LineSpacing, source.Value,
            $"Middle Space.LineSpacing should use token spacing instead of custom slider binding, actual {space.LineSpacing}.",
            failures);
        ExpectDifferent(space.ItemSpacing, smallItemSpacing,
            $"Middle Space.ItemSpacing should differ from Small, actual {space.ItemSpacing}.",
            failures);
        ExpectDifferent(space.LineSpacing, smallLineSpacing,
            $"Middle Space.LineSpacing should differ from Small, actual {space.LineSpacing}.",
            failures);

        space.SizeType = CustomizableSizeType.Custom;
        RefreshLayout(realized.Window);
        ExpectClose(space.ItemSpacing, source.Value,
            $"Custom Space.ItemSpacing should use custom slider binding, actual {space.ItemSpacing}.",
            failures);
        ExpectClose(space.LineSpacing, source.Value,
            $"Custom Space.LineSpacing should use custom slider binding, actual {space.LineSpacing}.",
            failures);

        source.Value = 43;
        RefreshLayout(realized.Window);
        ExpectClose(space.ItemSpacing, source.Value,
            $"Custom Space.ItemSpacing should keep receiving source updates, actual {space.ItemSpacing}.",
            failures);
        ExpectClose(space.LineSpacing, source.Value,
            $"Custom Space.LineSpacing should keep receiving source updates, actual {space.LineSpacing}.",
            failures);

        space.SizeType = CustomizableSizeType.Large;
        RefreshLayout(realized.Window);
        ExpectDifferent(space.ItemSpacing, source.Value,
            $"Large Space.ItemSpacing should return to token spacing instead of custom slider binding, actual {space.ItemSpacing}.",
            failures);
        ExpectDifferent(space.LineSpacing, source.Value,
            $"Large Space.LineSpacing should return to token spacing instead of custom slider binding, actual {space.LineSpacing}.",
            failures);
        ExpectDifferent(space.ItemSpacing, smallItemSpacing,
            $"Large Space.ItemSpacing should differ from Small, actual {space.ItemSpacing}.",
            failures);
        ExpectDifferent(space.LineSpacing, smallLineSpacing,
            $"Large Space.LineSpacing should differ from Small, actual {space.LineSpacing}.",
            failures);
    }

    private static List<Control> FindCompactSpaceItems(CompactSpace compactSpace)
    {
        return compactSpace.GetSelfAndVisualDescendants()
                           .OfType<Control>()
                           .Where(control => control.GetType().FullName == "AtomUI.Desktop.Controls.CompactSpaceItem")
                           .ToList();
    }

    private static void InvokeCompactSpaceHandler(CompactSpace compactSpace,
                                                  string handlerName,
                                                  Control sender,
                                                  ICollection<string> failures)
    {
        var method = typeof(CompactSpace).GetMethod(handlerName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (method is null)
        {
            failures.Add($"CompactSpace private handler {handlerName} was not found.");
            return;
        }

        method.Invoke(compactSpace, [sender, null]);
    }

    private static void ExpectThrows<TException>(Action action, string message, ICollection<string> failures)
        where TException : Exception
    {
        try
        {
            action();
            failures.Add(message);
        }
        catch (TargetInvocationException ex) when (ex.InnerException is TException)
        {
        }
        catch (TException)
        {
        }
    }

    private static void ExpectClose(double actual, double expected, string message, ICollection<string> failures)
    {
        Expect(Math.Abs(actual - expected) < 0.001, message, failures);
    }

    private static void ExpectDifferent(double actual, double unexpected, string message, ICollection<string> failures)
    {
        Expect(Math.Abs(actual - unexpected) >= 0.001, message, failures);
    }

    private sealed class SpaceTemplateSpacingSource : AvaloniaObject
    {
        public static readonly StyledProperty<double> ValueProperty =
            AvaloniaProperty.Register<SpaceTemplateSpacingSource, double>(nameof(Value));

        public double Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
