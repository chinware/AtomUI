// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Rendering;

namespace AtomUI.Controls;

[TemplatePart(DataGridCellTheme.RightGridLinePart, typeof(Rectangle))]
[PseudoClasses(StdPseudoClass.Selected, StdPseudoClass.Current, StdPseudoClass.Edited, StdPseudoClass.Invalid, StdPseudoClass.Focus)]
public class DataGridCell : ContentControl, ICustomHitTest
{
    #region 公共属性定义

    public static readonly DirectProperty<DataGridCell, bool> IsValidProperty =
        AvaloniaProperty.RegisterDirect<DataGridCell, bool>(
            nameof(IsValid),
            o => o.IsValid);
    
    bool _isValid = true;
    public bool IsValid
    {
        get => _isValid;
        internal set => SetAndRaise(IsValidProperty, ref _isValid, value);
    }

    #endregion

    #region 内部属性定义
    
    internal static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<DataGridCell>();
    
    internal SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    internal DataGridRow? OwningRow { get; set; }
    
    private DataGridColumn? _owningColumn;
    internal DataGridColumn? OwningColumn
    {
        get => _owningColumn;
        set
        {
            if (_owningColumn != value)
            {
                _owningColumn = value;
                OnOwningColumnSet(value);
            }
        }
    }
    
    internal DataGrid? OwningGrid => OwningRow?.OwningGrid ?? OwningColumn?.OwningGrid;
    
    internal double ActualRightGridLineWidth => _rightGridLine?.Bounds.Width ?? 0;
    
    internal int ColumnIndex => OwningColumn?.Index ?? -1;
    internal int RowIndex => OwningRow?.Index ?? -1;
    
    internal bool IsCurrent => OwningGrid != null && 
                               OwningColumn != null &&
                               OwningRow != null &&
                               OwningGrid.CurrentColumnIndex == OwningColumn.Index &&
                               OwningGrid.CurrentSlot == OwningRow.Slot;
    
    private bool IsEdited => OwningGrid != null && 
                             OwningGrid.EditingRow == OwningRow &&
                             OwningGrid.EditingColumnIndex == ColumnIndex;
    
    private bool IsMouseOver
    {
        get => OwningRow != null && OwningRow.MouseOverColumnIndex == ColumnIndex;
        set
        {
            Debug.Assert(OwningRow != null);
            if (value != IsMouseOver)
            {
                if (value)
                {
                    OwningRow.MouseOverColumnIndex = ColumnIndex;
                }
                else
                {
                    OwningRow.MouseOverColumnIndex = null;
                }
            }
        }
    }
    
    #endregion
    
    private Rectangle? _rightGridLine;

    static DataGridCell()
    {
        HorizontalContentAlignmentProperty.OverrideDefaultValue<DataGridCell>(HorizontalAlignment.Left);
        VerticalContentAlignmentProperty.OverrideDefaultValue<DataGridCell>(VerticalAlignment.Center);
        
        PointerPressedEvent.AddClassHandler<DataGridCell>(
            (x,e) => x.HandlePointerPressed(e), handledEventsToo: true);
        FocusableProperty.OverrideDefaultValue<DataGridCell>(true);
        IsTabStopProperty.OverrideDefaultValue<DataGridCell>(false);
        AutomationProperties.IsOffscreenBehaviorProperty.OverrideDefaultValue<DataGridCell>(IsOffscreenBehavior.FromClip);
    }

    /// <summary>
    /// Builds the visual tree for the cell control when a new template is applied.
    /// </summary>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        UpdatePseudoClasses();
        _rightGridLine = e.NameScope.Find<Rectangle>(DataGridCellTheme.RightGridLinePart);
        if (_rightGridLine != null && OwningColumn == null)
        {
            // Turn off the right GridLine for filler cells
            _rightGridLine.IsVisible = false;
        }
        else
        {
            EnsureGridLine(null);
        }
    }
    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);

        if (OwningRow != null)
        {
            IsMouseOver = true;
        }
    }
    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);

        if (OwningRow != null)
        {
            IsMouseOver = false;
        }
    }

    //TODO TabStop
    private void HandlePointerPressed(PointerPressedEventArgs e)
    {
        // OwningGrid is null for TopLeftHeaderCell and TopRightHeaderCell because they have no OwningRow
        if (OwningGrid == null || OwningColumn == null || OwningRow == null)
        {
            return;
        }
        OwningGrid.NotifyCellPointerPressed(new DataGridCellPointerPressedEventArgs(this, OwningRow, OwningColumn, e));
        if (e.Handled)
        {
            return;
        }
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            if (OwningGrid.IsTabStop)
            {
                OwningGrid.Focus();
            }
            if (OwningRow != null)
            {
                var handled = OwningGrid.UpdateStateOnMouseLeftButtonDown(e, ColumnIndex, OwningRow.Slot, !e.Handled);

                // Do not handle PointerPressed with touch or pen,
                // so we can start scroll gesture on the same event.
                if (e.Pointer.Type != PointerType.Touch && e.Pointer.Type != PointerType.Pen)
                {
                    e.Handled = handled;
                }
            }
        }
        else if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            if (OwningGrid.IsTabStop)
            {
                OwningGrid.Focus();
            }
            if (OwningRow != null)
            {
                e.Handled = OwningGrid.UpdateStateOnMouseRightButtonDown(e, ColumnIndex, OwningRow.Slot, !e.Handled);
            }
        }
    }

    internal void UpdatePseudoClasses()
    {
        if (OwningGrid == null || OwningColumn == null || OwningRow == null || !OwningRow.IsVisible || OwningRow.Slot == -1)
        {
            return;
        }

        PseudoClasses.Set(StdPseudoClass.Selected, OwningRow.IsSelected);
        PseudoClasses.Set(StdPseudoClass.Current, IsCurrent);
        PseudoClasses.Set(StdPseudoClass.Edited, IsEdited);
        PseudoClasses.Set(StdPseudoClass.Invalid, !IsValid);
        PseudoClasses.Set(StdPseudoClass.Focus, OwningGrid.IsFocused && IsCurrent);
    }

    // Makes sure the right gridline has the proper stroke and visibility. If lastVisibleColumn is specified, the 
    // right gridline will be collapsed if this cell belongs to the lastVisibleColumn and there is no filler column
    internal void EnsureGridLine(DataGridColumn? lastVisibleColumn)
    {
        if (OwningGrid != null && _rightGridLine != null)
        {
            bool newVisibility =
                (OwningGrid.GridLinesVisibility == DataGridGridLinesVisibility.Vertical || OwningGrid.GridLinesVisibility == DataGridGridLinesVisibility.All)
                && ((OwningGrid.ColumnsInternal.FillerColumn != null && OwningGrid.ColumnsInternal.FillerColumn.IsActive) || OwningColumn != lastVisibleColumn);

            if (newVisibility != _rightGridLine.IsVisible)
            {
                _rightGridLine.IsVisible = newVisibility;
            }
            _rightGridLine.Width = OwningGrid.BorderThickness.Left;
        }
    }

    private void OnOwningColumnSet(DataGridColumn? column)
    {
        if (column == null)
        {
            Classes.Clear();
            ClearValue(ThemeProperty);
        }
        else
        {
            if (Theme != column.CellTheme)
            {
                Theme = column.CellTheme;
            }
                
            Classes.Replace(column.CellStyleClasses);
        }
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}