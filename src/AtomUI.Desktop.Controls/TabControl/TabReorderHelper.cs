using System.Collections;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal static class TabReorderHelper
{
    internal const int InvalidIndex = -1;

    internal static bool TryResolveItemsList(ItemsControl owner, out IList list)
    {
        if (owner.ItemsSource is IList sourceList)
        {
            list = sourceList;
            return true;
        }

        if (owner.ItemsSource is null)
        {
            list = owner.Items;
            return true;
        }

        list = Array.Empty<object>();
        return false;
    }

    internal static bool CanMoveItems(IList list)
    {
        return !list.IsReadOnly && !list.IsFixedSize;
    }

    internal static BaseTabScrollViewer? FindTabScrollViewer(INameScope nameScope)
    {
        if (nameScope.Find<Control>("PART_CardTabStripScrollViewer") is BaseTabScrollViewer cardScrollViewer)
        {
            return cardScrollViewer;
        }

        return nameScope.Find<Control>("PART_TabsContainer") as BaseTabScrollViewer;
    }

    internal static bool IsValidIndex(int index, int count)
    {
        return index >= 0 && index < count;
    }

    internal static Point GetPointerRootPosition(Control owner, PointerEventArgs args)
    {
        var root = TopLevel.GetTopLevel(owner);
        return root is null ? args.GetPosition(owner) : args.GetPosition(root);
    }

    internal static Point? TranslateToRoot(Control owner, Visual visual, Point point)
    {
        var root = TopLevel.GetTopLevel(owner);
        return root is null
            ? visual.TranslatePoint(point, owner)
            : visual.TranslatePoint(point, root);
    }

    internal static bool TryGetPointerAnchorPrimary(
        ItemsControl owner,
        Dock placement,
        Control draggedContainer,
        Point pointerRootPosition,
        out double pointerAnchorPrimary)
    {
        var draggedBounds = GetContainerLayoutBounds(owner, draggedContainer);
        if (draggedBounds is null)
        {
            pointerAnchorPrimary = 0;
            return false;
        }

        var isHorizontal = IsHorizontal(placement);
        pointerAnchorPrimary = GetPrimary(pointerRootPosition, isHorizontal) -
                               GetPrimaryStart(draggedBounds.Value, isHorizontal);
        return true;
    }

    internal static int FindItemIndex(IList list, object? item, int preferredIndex)
    {
        if (IsValidIndex(preferredIndex, list.Count) && Equals(list[preferredIndex], item))
        {
            return preferredIndex;
        }

        for (var i = 0; i < list.Count; i++)
        {
            if (ReferenceEquals(list[i], item))
            {
                return i;
            }
        }

        return list.IndexOf(item);
    }

    internal static int NormalizeTargetIndex(int oldIndex, int insertionIndex, int itemCount)
    {
        var targetIndex = Math.Clamp(insertionIndex, 0, itemCount);
        if (targetIndex > oldIndex)
        {
            targetIndex--;
        }

        return targetIndex;
    }

    internal static void MoveItem(IList list, int oldIndex, int newIndex)
    {
        var item = list[oldIndex];
        list.RemoveAt(oldIndex);
        list.Insert(newIndex, item);
    }

    internal static void ApplyLivePreview(
        ItemsControl owner,
        Dock placement,
        Control draggedContainer,
        Point pointerRootPosition,
        double pointerAnchorPrimary,
        int oldIndex,
        int insertionIndex,
        IDictionary<Control, ITransform?> originalTransforms,
        IDictionary<Control, int> originalZIndexes,
        IDictionary<Control, Vector> previewOffsets)
    {
        var itemCount = owner.ItemCount;
        if (!IsValidIndex(oldIndex, itemCount))
        {
            ClearLivePreview(originalTransforms, originalZIndexes, previewOffsets);
            return;
        }

        var activeContainers = new HashSet<Control>();
        var isHorizontal     = IsHorizontal(placement);
        var primaryOffset    = GetDraggedPreviewPrimaryOffset(
            owner,
            placement,
            draggedContainer,
            pointerRootPosition,
            pointerAnchorPrimary);
        if (primaryOffset is null)
        {
            ClearLivePreview(originalTransforms, originalZIndexes, previewOffsets);
            return;
        }

        SetPreviewTransform(
            draggedContainer,
            isHorizontal ? primaryOffset.Value : 0,
            isHorizontal ? 0 : primaryOffset.Value,
            GetTopZIndex(owner, originalZIndexes) + 1,
            activeContainers,
            originalTransforms,
            originalZIndexes,
            previewOffsets,
            animateTransform: false);

        ApplySiblingPreviewByInsertionIndex(
            owner,
            placement,
            oldIndex,
            insertionIndex,
            activeContainers,
            originalTransforms,
            originalZIndexes,
            previewOffsets);

        ResetInactivePreviewContainers(activeContainers, originalTransforms, originalZIndexes, previewOffsets);
    }

    private static void ApplySiblingPreviewByInsertionIndex(
        ItemsControl owner,
        Dock placement,
        int oldIndex,
        int insertionIndex,
        HashSet<Control> activeContainers,
        IDictionary<Control, ITransform?> originalTransforms,
        IDictionary<Control, int> originalZIndexes,
        IDictionary<Control, Vector> previewOffsets)
    {
        if (insertionIndex == oldIndex)
        {
            return;
        }

        var isHorizontal = IsHorizontal(placement);

        if (insertionIndex > oldIndex)
        {
            for (var i = oldIndex + 1; i < insertionIndex && i < owner.ItemCount; i++)
            {
                var displacement = GetAdjacentSlotDisplacement(owner, i, -1, isHorizontal);
                if (displacement is not null)
                {
                    ApplySiblingPreview(owner, i, isHorizontal, displacement.Value, activeContainers, originalTransforms, originalZIndexes, previewOffsets);
                }
            }
            return;
        }

        for (var i = insertionIndex; i < oldIndex && i < owner.ItemCount; i++)
        {
            var displacement = GetAdjacentSlotDisplacement(owner, i, 1, isHorizontal);
            if (displacement is not null)
            {
                ApplySiblingPreview(owner, i, isHorizontal, displacement.Value, activeContainers, originalTransforms, originalZIndexes, previewOffsets);
            }
        }
    }

    internal static void ClearLivePreview(
        IDictionary<Control, ITransform?> originalTransforms,
        IDictionary<Control, int> originalZIndexes,
        IDictionary<Control, Vector> previewOffsets)
    {
        var controls = new List<Control>(originalTransforms.Keys);
        foreach (var control in controls)
        {
            RestorePreviewState(control, originalTransforms, originalZIndexes, previewOffsets);
        }
    }

    internal static int GetInsertionIndex(
        ItemsControl owner,
        Dock placement,
        Control draggedContainer,
        Point pointerRootPosition,
        double pointerAnchorPrimary,
        int oldIndex)
    {
        var draggedBounds = GetContainerLayoutBounds(owner, draggedContainer);
        if (draggedBounds is null)
        {
            return oldIndex;
        }

        var isHorizontal = IsHorizontal(placement);
        var draggedPrimaryStart = GetPrimary(pointerRootPosition, isHorizontal) - pointerAnchorPrimary;
        var primaryOffset       = draggedPrimaryStart - GetPrimaryStart(draggedBounds.Value, isHorizontal);
        if (MathUtils.AreClose(primaryOffset, 0))
        {
            return oldIndex;
        }

        if (MathUtils.GreaterThan(primaryOffset, 0))
        {
            var draggedTrailingEdge = draggedPrimaryStart + GetPrimarySize(draggedBounds.Value, isHorizontal);
            var insertionIndex = oldIndex;
            for (var i = oldIndex + 1; i < owner.ItemCount; i++)
            {
                if (owner.ContainerFromIndex(i) is not Control container || !container.IsVisible)
                {
                    continue;
                }

                var layoutBounds = GetContainerLayoutBounds(owner, container);
                if (layoutBounds is null)
                {
                    continue;
                }

                var midpoint = isHorizontal
                    ? layoutBounds.Value.X + layoutBounds.Value.Width / 2
                    : layoutBounds.Value.Y + layoutBounds.Value.Height / 2;
                if (MathUtils.GreaterThanOrClose(draggedTrailingEdge, midpoint))
                {
                    insertionIndex = i + 1;
                    continue;
                }

                break;
            }

            return insertionIndex;
        }

        var draggedLeadingEdge = draggedPrimaryStart;
        var targetIndex = oldIndex;
        for (var i = oldIndex - 1; i >= 0; i--)
        {
            if (owner.ContainerFromIndex(i) is not Control container || !container.IsVisible)
            {
                continue;
            }

            var layoutBounds = GetContainerLayoutBounds(owner, container);
            if (layoutBounds is null)
            {
                continue;
            }

            var midpoint = isHorizontal
                ? layoutBounds.Value.X + layoutBounds.Value.Width / 2
                : layoutBounds.Value.Y + layoutBounds.Value.Height / 2;
            if (MathUtils.LessThanOrClose(draggedLeadingEdge, midpoint))
            {
                targetIndex = i;
                continue;
            }

            break;
        }

        return targetIndex;
    }

    internal static bool IsHorizontal(Dock placement)
    {
        return placement is Dock.Top or Dock.Bottom;
    }

    private static double? GetDraggedPreviewPrimaryOffset(
        ItemsControl owner,
        Dock placement,
        Control draggedContainer,
        Point pointerRootPosition,
        double pointerAnchorPrimary)
    {
        var draggedBounds = GetContainerLayoutBounds(owner, draggedContainer);
        if (draggedBounds is null)
        {
            return null;
        }

        var isHorizontal = IsHorizontal(placement);
        var draggedPrimaryStart = GetPrimary(pointerRootPosition, isHorizontal) - pointerAnchorPrimary;
        return draggedPrimaryStart - GetPrimaryStart(draggedBounds.Value, isHorizontal);
    }

    private static void ApplySiblingPreview(
        ItemsControl owner,
        int index,
        bool isHorizontal,
        double displacement,
        HashSet<Control> activeContainers,
        IDictionary<Control, ITransform?> originalTransforms,
        IDictionary<Control, int> originalZIndexes,
        IDictionary<Control, Vector> previewOffsets)
    {
        if (owner.ContainerFromIndex(index) is not Control container || !container.IsVisible)
        {
            return;
        }

        SetPreviewTransform(
            container,
            isHorizontal ? displacement : 0,
            isHorizontal ? 0 : displacement,
            null,
            activeContainers,
            originalTransforms,
            originalZIndexes,
            previewOffsets,
            animateTransform: true);
    }

    private static double? GetAdjacentSlotDisplacement(ItemsControl owner, int index, int adjacentStep, bool isHorizontal)
    {
        var currentBounds = GetContainerLayoutBounds(owner, owner.ContainerFromIndex(index) as Control);
        if (currentBounds is null)
        {
            return null;
        }

        for (var adjacentIndex = index + adjacentStep;
             IsValidIndex(adjacentIndex, owner.ItemCount);
             adjacentIndex += adjacentStep)
        {
            var adjacentBounds = GetContainerLayoutBounds(owner, owner.ContainerFromIndex(adjacentIndex) as Control);
            if (adjacentBounds is null)
            {
                continue;
            }

            var displacement = GetPrimaryStart(adjacentBounds.Value, isHorizontal) -
                               GetPrimaryStart(currentBounds.Value, isHorizontal);
            return MathUtils.AreClose(displacement, 0)
                ? null
                : displacement;
        }

        return null;
    }

    private static double GetPrimaryStart(Rect bounds, bool isHorizontal)
    {
        return isHorizontal ? bounds.X : bounds.Y;
    }

    private static double GetPrimarySize(Rect bounds, bool isHorizontal)
    {
        return isHorizontal ? bounds.Width : bounds.Height;
    }

    private static double GetPrimary(Point point, bool isHorizontal)
    {
        return isHorizontal ? point.X : point.Y;
    }

    private static void SetPreviewTransform(
        Control container,
        double x,
        double y,
        int? zIndex,
        HashSet<Control> activeContainers,
        IDictionary<Control, ITransform?> originalTransforms,
        IDictionary<Control, int> originalZIndexes,
        IDictionary<Control, Vector> previewOffsets,
        bool animateTransform)
    {
        if (!originalTransforms.ContainsKey(container))
        {
            originalTransforms.Add(container, container.RenderTransform);
        }

        if (!originalZIndexes.ContainsKey(container))
        {
            originalZIndexes.Add(container, container.ZIndex);
        }

        var currentTransform = container.RenderTransform;
        var isPreviewTransform = currentTransform is TranslateTransform &&
                                 originalTransforms.TryGetValue(container, out var originalTransform) &&
                                 !ReferenceEquals(currentTransform, originalTransform);
        if (isPreviewTransform && currentTransform is TranslateTransform translateTransform)
        {
            if (animateTransform)
            {
                EnsurePreviewTransformTransitions(translateTransform);
            }
            translateTransform.X = x;
            translateTransform.Y = y;
        }
        else
        {
            container.RenderTransform = CreatePreviewTranslateTransform(container, x, y, animateTransform);
        }

        if (zIndex is not null)
        {
            container.ZIndex = zIndex.Value;
        }
        previewOffsets[container] = new Vector(x, y);
        activeContainers.Add(container);
    }

    private static void ResetInactivePreviewContainers(
        HashSet<Control> activeContainers,
        IDictionary<Control, ITransform?> originalTransforms,
        IDictionary<Control, int> originalZIndexes,
        IDictionary<Control, Vector> previewOffsets)
    {
        var inactiveContainers = new List<Control>();
        foreach (var container in originalTransforms.Keys)
        {
            if (!activeContainers.Contains(container))
            {
                inactiveContainers.Add(container);
            }
        }

        foreach (var container in inactiveContainers)
        {
            ResetPreviewState(container, activeContainers, originalTransforms, originalZIndexes, previewOffsets);
        }
    }

    private static void ResetPreviewState(
        Control container,
        HashSet<Control> activeContainers,
        IDictionary<Control, ITransform?> originalTransforms,
        IDictionary<Control, int> originalZIndexes,
        IDictionary<Control, Vector> previewOffsets)
    {
        if (!originalTransforms.TryGetValue(container, out var originalTransform))
        {
            return;
        }

        var originalOffset = GetTransformOffset(originalTransform);
        var zIndex = originalZIndexes.TryGetValue(container, out var originalZIndex)
            ? originalZIndex
            : (int?)null;
        SetPreviewTransform(
            container,
            originalOffset.X,
            originalOffset.Y,
            zIndex,
            activeContainers,
            originalTransforms,
            originalZIndexes,
            previewOffsets,
            animateTransform: true);
    }

    private static void RestorePreviewState(
        Control container,
        IDictionary<Control, ITransform?> originalTransforms,
        IDictionary<Control, int> originalZIndexes,
        IDictionary<Control, Vector> previewOffsets)
    {
        previewOffsets.Remove(container);
        if (originalTransforms.Remove(container, out var originalTransform))
        {
            container.RenderTransform = originalTransform;
        }

        if (originalZIndexes.Remove(container, out var originalZIndex))
        {
            container.ZIndex = originalZIndex;
        }
    }

    private static TranslateTransform CreatePreviewTranslateTransform(
        Control container,
        double x,
        double y,
        bool animateTransform)
    {
        var currentOffset = GetTransformOffset(container.RenderTransform);
        var translateTransform = new TranslateTransform(currentOffset.X, currentOffset.Y);
        if (animateTransform)
        {
            translateTransform.Transitions = CreatePreviewTransformTransitions();
            translateTransform.X           = x;
            translateTransform.Y           = y;
        }
        else
        {
            translateTransform.X = x;
            translateTransform.Y = y;
        }

        return translateTransform;
    }

    private static void EnsurePreviewTransformTransitions(TranslateTransform translateTransform)
    {
        if (translateTransform.Transitions is not null)
        {
            return;
        }

        translateTransform.Transitions = CreatePreviewTransformTransitions();
    }

    private static Vector GetTransformOffset(ITransform? transform)
    {
        var matrix = transform?.Value ?? Matrix.Identity;
        return new Vector(matrix.M31, matrix.M32);
    }

    private static Transitions CreatePreviewTransformTransitions()
    {
        return
        [
            new DoubleTransition
            {
                Property = TranslateTransform.XProperty,
                Duration = TimeSpan.FromMilliseconds(160),
                Easing   = new CubicEaseOut()
            },
            new DoubleTransition
            {
                Property = TranslateTransform.YProperty,
                Duration = TimeSpan.FromMilliseconds(160),
                Easing   = new CubicEaseOut()
            }
        ];
    }

    private static Rect? GetContainerLayoutBounds(ItemsControl owner, Control? container)
    {
        if (container is null || !container.IsVisible || container.GetVisualParent() is not Visual parent)
        {
            return null;
        }

        var parentOffset = TranslateToRoot(owner, parent, default);
        if (parentOffset is null)
        {
            return null;
        }

        return new Rect(
            new Point(
                parentOffset.Value.X + container.Bounds.X,
                parentOffset.Value.Y + container.Bounds.Y),
            container.Bounds.Size);
    }

    private static int GetTopZIndex(ItemsControl owner, IDictionary<Control, int> originalZIndexes)
    {
        var topZIndex = 0;
        for (var i = 0; i < owner.ItemCount; i++)
        {
            if (owner.ContainerFromIndex(i) is Control container)
            {
                var zIndex = originalZIndexes.TryGetValue(container, out var originalZIndex)
                    ? originalZIndex
                    : container.ZIndex;
                topZIndex = Math.Max(topZIndex, zIndex);
            }
        }

        return topZIndex;
    }
}
