using AtomUI.Controls;
using AtomUI.Desktop.Controls.Data;
using AtomUI.Desktop.Controls.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AtomUI.Desktop.Controls;

public partial class DataGrid
{
    private enum RowReorderSessionState
    {
        Pressed,
        Dragging,
        Completed,
        Cancelled
    }

    private sealed class RowReorderSession
    {
        public RowReorderSession(
            IPointer pointer,
            DataGridRowReorderHandle handle,
            DataGridRow row,
            object? item,
            IDataGridCollectionView view,
            IDataGridCollectionViewMoveSupport moveSupport,
            DataGridRowsPresenter rowsPresenter,
            Point startPosition)
        {
            Pointer       = pointer;
            Handle        = handle;
            Row           = row;
            Item          = item;
            View          = view;
            MoveSupport   = moveSupport;
            RowsPresenter = rowsPresenter;
            StartPosition = startPosition;
            SourceIndex   = row.Index;
            SourceBounds  = row.Bounds;
        }

        public IPointer Pointer { get; }
        public DataGridRowReorderHandle Handle { get; }
        public DataGridRow Row { get; }
        public object? Item { get; }
        public IDataGridCollectionView View { get; }
        public IDataGridCollectionViewMoveSupport MoveSupport { get; }
        public DataGridRowsPresenter RowsPresenter { get; }
        public Point StartPosition { get; }
        public int SourceIndex { get; }
        public Rect SourceBounds { get; }
        public int? TargetIndex { get; set; }
        public RowReorderSessionState State { get; set; } = RowReorderSessionState.Pressed;
    }

    private RowReorderSession? _rowReorderSession;

    internal bool TryBeginRowReorder(
        DataGridRowReorderHandle handle,
        DataGridRow row,
        IPointer pointer,
        Point startPosition)
    {
        CancelRowReorder();

        if (!IsEnabled ||
            !CanUserReorderRows ||
            !ReferenceEquals(handle.OwningGrid, this) ||
            !ReferenceEquals(handle.OwningRow, row) ||
            !ReferenceEquals(row.OwningGrid, this) ||
            row.Index < 0 ||
            RowsPresenter is not { } rowsPresenter ||
            CollectionView is not { } view ||
            view is not IDataGridCollectionViewMoveSupport { CanMove: true } moveSupport)
        {
            return false;
        }

        var item = row.DataContext;
        if (!RowReorderItemsMatch(DataConnection.GetDataItem(row.Index), item))
        {
            return false;
        }

        var session = new RowReorderSession(
            pointer,
            handle,
            row,
            item,
            view,
            moveSupport,
            rowsPresenter,
            startPosition);
        _rowReorderSession = session;
        pointer.Capture(handle);
        if (!ReferenceEquals(pointer.Captured, handle))
        {
            CancelRowReorder(session);
            return false;
        }

        return true;
    }

    internal void HandleRowReorderPointerMoved(
        DataGridRowReorderHandle handle,
        IPointer pointer,
        Point pointerPosition,
        bool isLeftButtonPressed)
    {
        if (!TryGetRowReorderSession(handle, pointer, out var session))
        {
            return;
        }
        if (!isLeftButtonPressed || !IsRowReorderSessionValid(session))
        {
            CancelRowReorder(session);
            return;
        }

        if (session.State == RowReorderSessionState.Pressed)
        {
            var distance = new Vector(
                pointerPosition.X - session.StartPosition.X,
                pointerPosition.Y - session.StartPosition.Y);
            if (distance.Length <= Constants.DragThreshold)
            {
                return;
            }

            session.State = RowReorderSessionState.Dragging;
            var eventArgs = new DataGridRowReorderingEventArgs(session.Row);
            NotifyRowReordering(eventArgs);
            if (eventArgs.Cancel || !IsRowReorderSessionValid(session))
            {
                CancelRowReorder(session);
                return;
            }

            session.RowsPresenter.DraggedRowIndex = session.SourceIndex;
            session.RowsPresenter.NotifyAboutToDragging(session.Item, session.SourceIndex);
        }

        if (session.State != RowReorderSessionState.Dragging ||
            session.RowsPresenter.DraggedRowIndex is null)
        {
            return;
        }

        var targetIndex = GetRowReorderTarget(pointerPosition, scroll: true, out var scrollAmount);
        if (!ReferenceEquals(_rowReorderSession, session))
        {
            return;
        }
        if (!IsRowReorderSessionValid(session))
        {
            CancelRowReorder(session);
            return;
        }

        session.TargetIndex                    = targetIndex;
        session.RowsPresenter.DragRowOffset   = session.SourceBounds.Y +
                                                 pointerPosition.Y -
                                                 session.StartPosition.Y +
                                                 scrollAmount;
        session.RowsPresenter.DraggedRowIndex = session.SourceIndex;
        session.RowsPresenter.InvalidateArrange();
    }

    internal void HandleRowReorderPointerReleased(DataGridRowReorderHandle handle, IPointer pointer)
    {
        if (!TryGetRowReorderSession(handle, pointer, out var session))
        {
            return;
        }

        var targetIndex = session.TargetIndex;
        var shouldCommit = session.State == RowReorderSessionState.Dragging &&
                           targetIndex.HasValue &&
                           targetIndex.Value != session.SourceIndex &&
                           IsRowReorderSessionValid(session);
        var moved = false;

        _rowReorderSession = null;
        ReleaseRowReorderPointerCapture(session);
        try
        {
            if (shouldCommit)
            {
                moved = session.MoveSupport.TryMove(session.SourceIndex, targetIndex!.Value);
            }
            session.State = moved
                ? RowReorderSessionState.Completed
                : RowReorderSessionState.Cancelled;
        }
        catch
        {
            session.State = RowReorderSessionState.Cancelled;
            throw;
        }
        finally
        {
            CleanupRowReorderSession(session, releasePointerCapture: false);
        }

        if (moved && ReferenceEquals(CollectionView, session.View))
        {
            var eventRow = DisplayData.GetDisplayedRow(targetIndex!.Value);
            if (eventRow == null || !RowReorderItemsMatch(eventRow.DataContext, session.Item))
            {
                eventRow = session.Row;
            }
            NotifyRowReordered(new DataGridRowEventArgs(eventRow));
        }
    }

    internal void HandleRowReorderPointerCaptureLost(DataGridRowReorderHandle handle, IPointer pointer)
    {
        if (TryGetRowReorderSession(handle, pointer, out var session))
        {
            CancelRowReorder(session, releasePointerCapture: false);
        }
    }

    internal void CancelRowReorder(DataGridRowReorderHandle handle, DataGridRow? row = null)
    {
        if (_rowReorderSession is { } session &&
            ReferenceEquals(session.Handle, handle) &&
            (row is null || ReferenceEquals(session.Row, row)))
        {
            CancelRowReorder(session);
        }
    }

    internal void CancelRowReorder()
    {
        if (_rowReorderSession is { } session)
        {
            CancelRowReorder(session);
        }
    }

    private bool TryGetRowReorderSession(
        DataGridRowReorderHandle handle,
        IPointer pointer,
        out RowReorderSession session)
    {
        session = _rowReorderSession!;
        return session != null &&
               ReferenceEquals(session.Handle, handle) &&
               ReferenceEquals(session.Pointer, pointer) &&
               session.State is RowReorderSessionState.Pressed or RowReorderSessionState.Dragging;
    }

    private bool IsRowReorderSessionValid(RowReorderSession session)
    {
        return ReferenceEquals(_rowReorderSession, session) &&
               IsEnabled &&
               CanUserReorderRows &&
               ReferenceEquals(RowsPresenter, session.RowsPresenter) &&
               ReferenceEquals(CollectionView, session.View) &&
               session.MoveSupport.CanMove &&
               ReferenceEquals(session.Handle.OwningGrid, this) &&
               ReferenceEquals(session.Handle.OwningRow, session.Row) &&
               ReferenceEquals(session.Row.OwningGrid, this) &&
               session.Row.Index == session.SourceIndex &&
               IsSlotVisible(SlotFromRowIndex(session.SourceIndex)) &&
               ReferenceEquals(DisplayData.GetDisplayedRow(session.SourceIndex), session.Row) &&
               RowReorderItemsMatch(session.Row.DataContext, session.Item) &&
               RowReorderItemsMatch(DataConnection.GetDataItem(session.SourceIndex), session.Item);
    }

    private void CancelRowReorder(RowReorderSession session, bool releasePointerCapture = true)
    {
        if (session.State is RowReorderSessionState.Completed or RowReorderSessionState.Cancelled &&
            !ReferenceEquals(_rowReorderSession, session))
        {
            return;
        }

        session.State = RowReorderSessionState.Cancelled;
        if (ReferenceEquals(_rowReorderSession, session))
        {
            _rowReorderSession = null;
        }
        CleanupRowReorderSession(session, releasePointerCapture);
    }

    private static void CleanupRowReorderSession(RowReorderSession session, bool releasePointerCapture)
    {
        var hadDragVisual = session.RowsPresenter.DraggedRowIndex is not null;
        session.Handle.DisableIndicatorTransitions();
        if (releasePointerCapture)
        {
            ReleaseRowReorderPointerCapture(session);
        }

        session.RowsPresenter.NotifyDropped();
        session.RowsPresenter.DraggedRowIndex = null;
        session.RowsPresenter.DragRowOffset   = 0;
        if (hadDragVisual)
        {
            session.RowsPresenter.InvalidateArrange();
        }
        session.Handle.EnableIndicatorTransitions();
    }

    private static void ReleaseRowReorderPointerCapture(RowReorderSession session)
    {
        if (ReferenceEquals(session.Pointer.Captured, session.Handle))
        {
            session.Pointer.Capture(null);
        }
    }

    private int? GetRowReorderTarget(Point pointerPosition, bool scroll, out double scrollAmount)
    {
        scrollAmount = 0;
        if (RowsPresenter is not { } rowsPresenter)
        {
            return null;
        }

        var topEdge    = 0d;
        var bottomEdge = rowsPresenter.DesiredSize.Height;

        if (pointerPosition.Y < topEdge)
        {
            if (scroll &&
                VerticalScrollBar is { IsVisible: true, Value: > 0 } verticalScrollBar)
            {
                scrollAmount = Math.Max(
                    pointerPosition.Y - topEdge,
                    -verticalScrollBar.Value);
                ScrollSlotsByHeight(scrollAmount);
            }

            pointerPosition = pointerPosition.WithY(topEdge);
        }
        else if (pointerPosition.Y >= bottomEdge)
        {
            if (scroll &&
                VerticalScrollBar is { IsVisible: true } verticalScrollBar &&
                verticalScrollBar.Value < verticalScrollBar.Maximum)
            {
                scrollAmount = Math.Min(
                    pointerPosition.Y - bottomEdge,
                    verticalScrollBar.Maximum - verticalScrollBar.Value);
                if (scrollAmount > 0)
                {
                    ScrollSlotsByHeight(scrollAmount);
                }
            }

            pointerPosition = pointerPosition.WithY(Math.Max(topEdge, bottomEdge - 1));
        }

        var displayedElementCount = DisplayData.NumDisplayedScrollingElements;
        for (var displayIndex = 0; displayIndex < displayedElementCount; displayIndex++)
        {
            if (DisplayData.GetScrollingElementAtDisplayIndex(displayIndex) is DataGridRow row)
            {
                var rowPosition = rowsPresenter.Translate(row, pointerPosition);
                if (rowPosition.Y >= 0 && rowPosition.Y <= row.Bounds.Height)
                {
                    return row.Index;
                }
            }
        }

        return null;
    }

    private static bool RowReorderItemsMatch(object? actual, object? expected)
    {
        if (ReferenceEquals(actual, expected))
        {
            return true;
        }

        return actual != null &&
               expected != null &&
               actual.GetType().IsValueType &&
               actual.GetType() == expected.GetType() &&
               actual.Equals(expected);
    }
}
