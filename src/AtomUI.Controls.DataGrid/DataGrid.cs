// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using AtomUI.Controls.Data;
using AtomUI.Controls.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Utilities;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public partial class DataGrid : TemplatedControl
{
    #region 公共属性定义

    /// <summary>
    /// Identifies the CanUserReorderColumns dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> CanUserReorderColumnsProperty =
        AvaloniaProperty.Register<DataGrid, bool>(nameof(CanUserReorderColumns));
    
    /// <summary>
    /// Identifies the CanUserResizeColumns dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> CanUserResizeColumnsProperty =
        AvaloniaProperty.Register<DataGrid, bool>(nameof(CanUserResizeColumns));
    
    /// <summary>
    /// Identifies the CanUserSortColumns dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> CanUserSortColumnsProperty =
        AvaloniaProperty.Register<DataGrid, bool>(nameof(CanUserSortColumns), true);
    
    /// <summary>
    /// Identifies the ColumnHeaderHeight dependency property.
    /// </summary>
    public static readonly StyledProperty<double> ColumnHeaderHeightProperty =
        AvaloniaProperty.Register<DataGrid, double>(
            nameof(ColumnHeaderHeight),
            defaultValue: double.NaN,
            validate: IsValidColumnHeaderHeight);
    
    /// <summary>
    /// Identifies the <see cref="ColumnHeaderTheme"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<ControlTheme> ColumnHeaderThemeProperty =
        AvaloniaProperty.Register<DataGrid, ControlTheme>(nameof(ColumnHeaderTheme));
    
    /// <summary>
    /// Identifies the ColumnWidth dependency property.
    /// </summary>
    public static readonly StyledProperty<DataGridLength> ColumnWidthProperty =
        AvaloniaProperty.Register<DataGrid, DataGridLength>(nameof(ColumnWidth), defaultValue: DataGridLength.Auto);
    
    /// <summary>
    /// Identifies the <see cref="RowTheme"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<ControlTheme> RowThemeProperty =
        AvaloniaProperty.Register<DataGrid, ControlTheme>(nameof(RowTheme));
    
    /// <summary>
    /// Identifies the <see cref="CellTheme"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<ControlTheme> CellThemeProperty =
        AvaloniaProperty.Register<DataGrid, ControlTheme>(nameof(CellTheme));
    
    /// <summary>
    /// Identifies the <see cref="RowGroupTheme"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<ControlTheme> RowGroupThemeProperty =
        AvaloniaProperty.Register<DataGrid, ControlTheme>(nameof(RowGroupTheme));
    
    public static readonly StyledProperty<int> FrozenColumnCountProperty =
        AvaloniaProperty.Register<DataGrid, int>(
            nameof(FrozenColumnCount),
            validate: ValidateFrozenColumnCount);
    
    public static readonly StyledProperty<DataGridGridLinesVisibility> GridLinesVisibilityProperty =
        AvaloniaProperty.Register<DataGrid, DataGridGridLinesVisibility>(nameof(GridLinesVisibility));
    
    public static readonly StyledProperty<DataGridHeadersVisibility> HeadersVisibilityProperty =
        AvaloniaProperty.Register<DataGrid, DataGridHeadersVisibility>(nameof(HeadersVisibility));
    
    public static readonly StyledProperty<IBrush?> HorizontalGridLinesBrushProperty =
        AvaloniaProperty.Register<DataGrid, IBrush?>(nameof(HorizontalGridLinesBrush));
    
    public static readonly StyledProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<DataGrid, ScrollBarVisibility>(nameof(HorizontalScrollBarVisibility));
    
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<DataGrid, bool>(nameof(IsReadOnly));
    
    public static readonly StyledProperty<bool> AreRowGroupHeadersFrozenProperty =
        AvaloniaProperty.Register<DataGrid, bool>(
            nameof(AreRowGroupHeadersFrozen),
            defaultValue: true);
    
    public static readonly DirectProperty<DataGrid, bool> IsValidProperty =
        AvaloniaProperty.RegisterDirect<DataGrid, bool>(
            nameof(IsValid),
            o => o.IsValid);
    
    public static readonly StyledProperty<double> MaxColumnWidthProperty =
        AvaloniaProperty.Register<DataGrid, double>(
            nameof(MaxColumnWidth),
            defaultValue: DefaultMaxColumnWidth,
            validate: IsValidColumnWidth);

    public static readonly StyledProperty<double> MinColumnWidthProperty =
        AvaloniaProperty.Register<DataGrid, double>(
            nameof(MinColumnWidth),
            defaultValue: DefaultMinColumnWidth,
            validate: IsValidMinColumnWidth);
    
    /// <summary>
    /// Defines the <see cref="IsScrollInertiaEnabled"/> property.
    /// </summary>
    public static readonly AttachedProperty<bool> IsScrollInertiaEnabledProperty =
        ScrollViewer.IsScrollInertiaEnabledProperty.AddOwner<DataGrid>();
    
    public static readonly StyledProperty<IBrush> RowBackgroundProperty =
        AvaloniaProperty.Register<DataGrid, IBrush>(nameof(RowBackground));
    
    public static readonly StyledProperty<double> RowHeightProperty =
        AvaloniaProperty.Register<DataGrid, double>(
            nameof(RowHeight),
            defaultValue: double.NaN,
            validate: IsValidRowHeight);
    
    public static readonly StyledProperty<double> RowHeaderWidthProperty =
        AvaloniaProperty.Register<DataGrid, double>(
            nameof(RowHeaderWidth),
            defaultValue: double.NaN,
            validate: IsValidRowHeaderWidth);
    
    public static readonly StyledProperty<DataGridSelectionMode> SelectionModeProperty =
        AvaloniaProperty.Register<DataGrid, DataGridSelectionMode>(nameof(SelectionMode));
    
    public static readonly StyledProperty<IBrush?> VerticalGridLinesBrushProperty =
        AvaloniaProperty.Register<DataGrid, IBrush?>(nameof(VerticalGridLinesBrush));
    
    public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<DataGrid, ScrollBarVisibility>(nameof(VerticalScrollBarVisibility));
    
    public static readonly StyledProperty<ITemplate<Control>> DropLocationIndicatorTemplateProperty =
        AvaloniaProperty.Register<DataGrid, ITemplate<Control>>(nameof(DropLocationIndicatorTemplate));
    
    public static readonly DirectProperty<DataGrid, int> SelectedIndexProperty =
        AvaloniaProperty.RegisterDirect<DataGrid, int>(
            nameof(SelectedIndex),
            o => o.SelectedIndex,
            (o, v) => o.SelectedIndex = v,
            defaultBindingMode: BindingMode.TwoWay);
    
    public static readonly DirectProperty<DataGrid, object?> SelectedItemProperty =
        AvaloniaProperty.RegisterDirect<DataGrid, object?>(
            nameof(SelectedItem),
            o => o.SelectedItem,
            (o, v) => o.SelectedItem = v,
            defaultBindingMode: BindingMode.TwoWay);
    
    public static readonly StyledProperty<DataGridClipboardCopyMode> ClipboardCopyModeProperty =
        AvaloniaProperty.Register<DataGrid, DataGridClipboardCopyMode>(
            nameof(ClipboardCopyMode),
            defaultValue: DataGridClipboardCopyMode.ExcludeHeader);
    
    public static readonly StyledProperty<bool> AutoGenerateColumnsProperty =
        AvaloniaProperty.Register<DataGrid, bool>(nameof(AutoGenerateColumns));
    
    /// <summary>
    /// Identifies the ItemsSource property.
    /// </summary>
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<DataGrid, IEnumerable?>(nameof(ItemsSource));
    
    public static readonly StyledProperty<bool> AreRowDetailsFrozenProperty =
        AvaloniaProperty.Register<DataGrid, bool>(nameof(AreRowDetailsFrozen));
    
    public static readonly StyledProperty<IDataTemplate?> RowDetailsTemplateProperty =
        AvaloniaProperty.Register<DataGrid, IDataTemplate?>(nameof(RowDetailsTemplate));
    
    public static readonly StyledProperty<DataGridRowDetailsVisibilityMode> RowDetailsVisibilityModeProperty =
        AvaloniaProperty.Register<DataGrid, DataGridRowDetailsVisibilityMode>(nameof(RowDetailsVisibilityMode));
    
    // public static readonly DirectProperty<DataGrid, IDataGridCollectionView> CollectionViewProperty =
    //     AvaloniaProperty.RegisterDirect<DataGrid, IDataGridCollectionView>(nameof(CollectionView),
    //         o => o.CollectionView);
    
    /// <summary>
    /// Gets or sets a value that indicates whether the user can change
    /// the column display order by dragging column headers with the mouse.
    /// </summary>
    public bool CanUserReorderColumns
    {
        get => GetValue(CanUserReorderColumnsProperty);
        set => SetValue(CanUserReorderColumnsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the user can adjust column widths using the mouse.
    /// </summary>
    public bool CanUserResizeColumns
    {
        get => GetValue(CanUserResizeColumnsProperty);
        set => SetValue(CanUserResizeColumnsProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a value that indicates whether the user can sort columns by clicking the column header.
    /// </summary>
    public bool CanUserSortColumns
    {
        get => GetValue(CanUserSortColumnsProperty);
        set => SetValue(CanUserSortColumnsProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the height of the column headers row.
    /// </summary>
    public double ColumnHeaderHeight
    {
        get => GetValue(ColumnHeaderHeightProperty);
        set => SetValue(ColumnHeaderHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the standard width or automatic sizing mode of columns in the control.
    /// </summary>
    public DataGridLength ColumnWidth
    {
        get => GetValue(ColumnWidthProperty);
        set => SetValue(ColumnWidthProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the theme applied to all rows.
    /// </summary>
    public ControlTheme RowTheme
    {
        get => GetValue(RowThemeProperty);
        set => SetValue(RowThemeProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the theme applied to all cells.
    /// </summary>
    public ControlTheme CellTheme
    {
        get => GetValue(CellThemeProperty);
        set => SetValue(CellThemeProperty, value);
    }

    /// <summary>
    /// Gets or sets the theme applied to all column headers.
    /// </summary>
    public ControlTheme ColumnHeaderTheme
    {
        get => GetValue(ColumnHeaderThemeProperty);
        set => SetValue(ColumnHeaderThemeProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the theme applied to all row groups.
    /// </summary>
    public ControlTheme RowGroupTheme
    {
        get => GetValue(RowGroupThemeProperty);
        set => SetValue(RowGroupThemeProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the number of columns that the user cannot scroll horizontally.
    /// </summary>
    public int FrozenColumnCount
    {
        get => GetValue(FrozenColumnCountProperty);
        set => SetValue(FrozenColumnCountProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a value that indicates which grid lines separating inner cells are shown.
    /// </summary>
    public DataGridGridLinesVisibility GridLinesVisibility
    {
        get => GetValue(GridLinesVisibilityProperty);
        set => SetValue(GridLinesVisibilityProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a value that indicates the visibility of row and column headers.
    /// </summary>
    public DataGridHeadersVisibility HeadersVisibility
    {
        get => GetValue(HeadersVisibilityProperty);
        set => SetValue(HeadersVisibilityProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the <see cref="T:System.Windows.Media.Brush" /> that is used to paint grid lines separating rows.
    /// </summary>
    public IBrush? HorizontalGridLinesBrush
    {
        get => GetValue(HorizontalGridLinesBrushProperty);
        set => SetValue(HorizontalGridLinesBrushProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a value that indicates how the horizontal scroll bar is displayed.
    /// </summary>
    public ScrollBarVisibility HorizontalScrollBarVisibility
    {
        get => GetValue(HorizontalScrollBarVisibilityProperty);
        set => SetValue(HorizontalScrollBarVisibilityProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a value that indicates whether the user can edit the values in the control.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a value that indicates whether the row group header sections
    /// remain fixed at the width of the display area or can scroll horizontally.
    /// </summary>
    public bool AreRowGroupHeadersFrozen
    {
        get => GetValue(AreRowGroupHeadersFrozenProperty);
        set => SetValue(AreRowGroupHeadersFrozenProperty, value);
    }
    
    /// <summary>
    /// Gets or sets whether scroll gestures should include inertia in their behavior and value.
    /// </summary>
    public bool IsScrollInertiaEnabled
    {
        get => GetValue(IsScrollInertiaEnabledProperty);
        set => SetValue(IsScrollInertiaEnabledProperty, value);
    }
    
    private bool _isValid = true;
    public bool IsValid
    {
        get => _isValid;
        internal set
        {
            SetAndRaise(IsValidProperty, ref _isValid, value);
            PseudoClasses.Set(":invalid", !value);
        }
    }
    
    /// <summary>
    /// Gets or sets the maximum width of columns in the <see cref="T:Avalonia.Controls.DataGrid" /> .
    /// </summary>
    public double MaxColumnWidth
    {
        get => GetValue(MaxColumnWidthProperty);
        set => SetValue(MaxColumnWidthProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the minimum width of columns in the <see cref="T:Avalonia.Controls.DataGrid" />.
    /// </summary>
    public double MinColumnWidth
    {
        get => GetValue(MinColumnWidthProperty);
        set => SetValue(MinColumnWidthProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the <see cref="T:System.Windows.Media.Brush" /> that is used to paint row backgrounds.
    /// </summary>
    public IBrush RowBackground
    {
        get => GetValue(RowBackgroundProperty);
        set => SetValue(RowBackgroundProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the standard height of rows in the control.
    /// </summary>
    public double RowHeight
    {
        get => GetValue(RowHeightProperty);
        set => SetValue(RowHeightProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the width of the row header column.
    /// </summary>
    public double RowHeaderWidth
    {
        get => GetValue(RowHeaderWidthProperty);
        set => SetValue(RowHeaderWidthProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a value that indicates how the vertical scroll bar is displayed.
    /// </summary>
    public ScrollBarVisibility VerticalScrollBarVisibility
    {
        get => GetValue(VerticalScrollBarVisibilityProperty);
        set => SetValue(VerticalScrollBarVisibilityProperty, value);
    }

    /// <summary>
    /// Gets or sets the selection behavior of the data grid.
    /// </summary>
    public DataGridSelectionMode SelectionMode
    {
        get => GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="T:System.Windows.Media.Brush" /> that is used to paint grid lines separating columns.
    /// </summary>
    public IBrush? VerticalGridLinesBrush
    {
        get => GetValue(VerticalGridLinesBrushProperty);
        set => SetValue(VerticalGridLinesBrushProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the template that is used when rendering the column headers.
    /// </summary>
    public ITemplate<Control> DropLocationIndicatorTemplate
    {
        get => GetValue(DropLocationIndicatorTemplateProperty);
        set => SetValue(DropLocationIndicatorTemplateProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the index of the current selection.
    /// </summary>
    /// <returns>
    /// The index of the current selection, or -1 if the selection is empty.
    /// </returns>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set => SetAndRaise(SelectedIndexProperty, ref _selectedIndex, value);
    }
    private int _selectedIndex = -1;
    
    /// <summary>
    /// Gets or sets the data item corresponding to the selected row.
    /// </summary>
    public object? SelectedItem
    {
        get => _selectedItem;
        set => SetAndRaise(SelectedItemProperty, ref _selectedItem, value);
    }
    
    private object? _selectedItem;
    
    /// <summary>
    /// The property which determines how DataGrid content is copied to the Clipboard.
    /// </summary>
    public DataGridClipboardCopyMode ClipboardCopyMode
    {
        get => GetValue(ClipboardCopyModeProperty);
        set => SetValue(ClipboardCopyModeProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a value that indicates whether columns are created
    /// automatically when the <see cref="P:Avalonia.Controls.DataGrid.ItemsSource" /> property is set.
    /// </summary>
    public bool AutoGenerateColumns
    {
        get => GetValue(AutoGenerateColumnsProperty);
        set => SetValue(AutoGenerateColumnsProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a collection that is used to generate the content of the control.
    /// </summary>
    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a value that indicates whether the row details sections remain
    /// fixed at the width of the display area or can scroll horizontally.
    /// </summary>
    public bool AreRowDetailsFrozen
    {
        get => GetValue(AreRowDetailsFrozenProperty);
        set => SetValue(AreRowDetailsFrozenProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the template that is used to display the content of the details section of rows.
    /// </summary>
    public IDataTemplate? RowDetailsTemplate
    {
        get => GetValue(RowDetailsTemplateProperty);
        set => SetValue(RowDetailsTemplateProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a value that indicates when the details sections of rows are displayed.
    /// </summary>
    public DataGridRowDetailsVisibilityMode RowDetailsVisibilityMode
    {
        get => GetValue(RowDetailsVisibilityModeProperty);
        set => SetValue(RowDetailsVisibilityModeProperty, value);
    }
    
    /// <summary>
    /// Gets current <see cref="IDataGridCollectionView"/>.
    /// </summary>
    // public IDataGridCollectionView CollectionView =>
    //     DataConnection.CollectionView;
    
    /// <summary>
    /// Gets or sets the column that contains the current cell.
    /// </summary>
    public DataGridColumn? CurrentColumn
    {
        get
        {
            if (CurrentColumnIndex == -1)
            {
                return null;
            }
            Debug.Assert(CurrentColumnIndex < ColumnsItemsInternal.Count);
            return ColumnsItemsInternal[CurrentColumnIndex];
        }
        set
        {
            DataGridColumn? dataGridColumn = value;
            if (dataGridColumn == null)
            {
                throw DataGridError.DataGrid.ValueCannotBeSetToNull("value", "CurrentColumn");
            }
            if (CurrentColumn != dataGridColumn)
            {
                if (dataGridColumn.OwningGrid != this)
                {
                    // Provided column does not belong to this DataGrid
                    throw DataGridError.DataGrid.ColumnNotInThisDataGrid();
                }
                if (!dataGridColumn.IsVisible)
                {
                    // CurrentColumn cannot be set to an invisible column
                    throw DataGridError.DataGrid.ColumnCannotBeCollapsed();
                }
                if (CurrentSlot == -1)
                {
                    // There is no current row so the current column cannot be set
                    throw DataGridError.DataGrid.NoCurrentRow();
                }
                bool beginEdit = _editingColumnIndex != -1;

                //exitEditingMode, keepFocus, raiseEvents
                if (!EndCellEdit(DataGridEditAction.Commit, true, ContainsFocus, true))
                {
                    // Edited value couldn't be committed or aborted
                    return;
                }

                UpdateSelectionAndCurrency(dataGridColumn.Index, CurrentSlot, DataGridSelectionAction.None, false); //scrollIntoView
                Debug.Assert(_successfullyUpdatedSelection);

                if (beginEdit &&
                    _editingColumnIndex == -1 &&
                    CurrentSlot != -1 &&
                    CurrentColumnIndex != -1 &&
                    CurrentColumnIndex == dataGridColumn.Index &&
                    dataGridColumn.OwningGrid == this &&
                    !GetColumnEffectiveReadOnlyState(dataGridColumn))
                {
                    // Returning to editing mode since the grid was in that mode prior to the EndCellEdit call above.
                    BeginCellEdit(new RoutedEventArgs());
                }
            }
        }
    }

    /// <summary>
    /// Gets a collection that contains all the columns in the control.
    /// we use a backing field here because the field's type
    /// is a subclass of the property's
    /// </summary>
    public ObservableCollection<DataGridColumn> Columns => ColumnsInternal;
    
    /// <summary>
    /// Gets a list that contains the data items corresponding to the selected rows.
    /// </summary>
    public IList SelectedItems => _selectedItems as IList;
    
    #endregion

    #region 属性验证方法

    private static bool IsValidColumnHeaderHeight(double value)
    {
        return double.IsNaN(value) ||
               (value >= MinimumColumnHeaderHeight && value <= MaxHeadersThickness);
    }
    
    private static bool ValidateFrozenColumnCount(int value) => value >= 0;
    
    private static bool IsValidColumnWidth(double value)
    {
        return !double.IsNaN(value) && value > 0;
    }
    
    private static bool IsValidMinColumnWidth(double value)
    {
        return !double.IsNaN(value) && !double.IsPositiveInfinity(value) && value >= 0;
    }
    
    private static bool IsValidRowHeight(double value)
    {
        return double.IsNaN(value) ||
               (value >= DataGridRow.MinimumHeight &&
                value <= DataGridRow.MaximumHeight);
    }
    
    private static bool IsValidRowHeaderWidth(double value)
    {
        return double.IsNaN(value) ||
               (value >= MinimumRowHeaderWidth &&
                value <= MaxHeadersThickness);
    }
    #endregion

    #region 公共事件定义
    
    public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
        RoutedEvent.Register<DataGrid, SelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs one time for each public, non-static property in the bound data type when the
    /// <see cref="P:Avalonia.Controls.DataGrid.ItemsSource" /> property is changed and the
    /// <see cref="P:Avalonia.Controls.DataGrid.AutoGenerateColumns" /> property is true.
    /// </summary>
    public event EventHandler<DataGridAutoGeneratingColumnEventArgs>? AutoGeneratingColumn;

    /// <summary>
    /// Occurs before a cell or row enters editing mode.
    /// </summary>
    public event EventHandler<DataGridBeginningEditEventArgs>? BeginningEdit;

    /// <summary>
    /// Occurs after cell editing has ended.
    /// </summary>
    public event EventHandler<DataGridCellEditEndedEventArgs>? CellEditEnded;

    /// <summary>
    /// Occurs immediately before cell editing has ended.
    /// </summary>
    public event EventHandler<DataGridCellEditEndingEventArgs>? CellEditEnding;

    /// <summary>
    /// Occurs when cell is mouse-pressed.
    /// </summary>
    public event EventHandler<DataGridCellPointerPressedEventArgs>? CellPointerPressed;

    /// <summary>
    /// Occurs when the <see cref="P:Avalonia.Controls.DataGridColumn.DisplayIndex" />
    /// property of a column changes.
    /// </summary>
    public event EventHandler<DataGridColumnEventArgs>? ColumnDisplayIndexChanged;

    /// <summary>
    /// Raised when column reordering ends, to allow subscribers to clean up.
    /// </summary>
    public event EventHandler<DataGridColumnEventArgs>? ColumnReordered;

    /// <summary>
    /// Raised when starting a column reordering action.  Subscribers to this event can
    /// set tooltip and caret UIElements, constrain tooltip position, indicate that
    /// a preview should be shown, or cancel reordering.
    /// </summary>
    public event EventHandler<DataGridColumnReorderingEventArgs>? ColumnReordering;

    /// <summary>
    /// Occurs when a different cell becomes the current cell.
    /// </summary>
    public event EventHandler<EventArgs>? CurrentCellChanged;

    /// <summary>
    /// Occurs after a <see cref="T:Avalonia.Controls.DataGridRow" />
    /// is instantiated, so that you can customize it before it is used.
    /// </summary>
    public event EventHandler<DataGridRowEventArgs>? LoadingRow;

    /// <summary>
    /// Occurs when a cell in a <see cref="T:Avalonia.Controls.DataGridTemplateColumn" /> enters editing mode.
    ///
    /// </summary>
    public event EventHandler<DataGridPreparingCellForEditEventArgs>? PreparingCellForEdit;

    /// <summary>
    /// Occurs when the row has been successfully committed or cancelled.
    /// </summary>
    public event EventHandler<DataGridRowEditEndedEventArgs>? RowEditEnded;

    /// <summary>
    /// Occurs immediately before the row has been successfully committed or cancelled.
    /// </summary>
    public event EventHandler<DataGridRowEditEndingEventArgs>? RowEditEnding;

    /// <summary>
    /// Occurs when the <see cref="P:Avalonia.Controls.DataGrid.SelectedItem" /> or
    /// <see cref="P:Avalonia.Controls.DataGrid.SelectedItems" /> property value changes.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged
    {
        add => AddHandler(SelectionChangedEvent, value);
        remove => RemoveHandler(SelectionChangedEvent, value);
    }
    
    /// <summary>
    /// Occurs when the <see cref="DataGridColumn"/> sorting request is triggered.
    /// </summary>
    public event EventHandler<DataGridColumnEventArgs>? Sorting;

    /// <summary>
    /// Occurs when a <see cref="T:Avalonia.Controls.DataGridRow" />
    /// object becomes available for reuse.
    /// </summary>
    public event EventHandler<DataGridRowEventArgs>? UnloadingRow;

    /// <summary>
    /// Occurs when a new row details template is applied to a row, so that you can customize
    /// the details section before it is used.
    /// </summary>
    public event EventHandler<DataGridRowDetailsEventArgs>? LoadingRowDetails;

    /// <summary>
    /// Occurs when the <see cref="P:Avalonia.Controls.DataGrid.RowDetailsVisibilityMode" />
    /// property value changes.
    /// </summary>
    public event EventHandler<DataGridRowDetailsEventArgs>? RowDetailsVisibilityChanged;

    /// <summary>
    /// Occurs when a row details element becomes available for reuse.
    /// </summary>
    public event EventHandler<DataGridRowDetailsEventArgs>? UnloadingRowDetails;
    
    public event EventHandler<ScrollEventArgs>? HorizontalScroll;
    public event EventHandler<ScrollEventArgs>? VerticalScroll;
    
    /// <summary>
    /// Occurs before a DataGridRowGroupHeader header is used.
    /// </summary>
    public event EventHandler<DataGridRowGroupHeaderEventArgs>? LoadingRowGroup;

    /// <summary>
    /// Occurs when the DataGridRowGroupHeader is available for reuse.
    /// </summary>
    public event EventHandler<DataGridRowGroupHeaderEventArgs>? UnloadingRowGroup;
    
    /// <summary>
    /// This event is raised by OnCopyingRowClipboardContent method after the default row content is prepared.
    /// Event listeners can modify or add to the row clipboard content.
    /// </summary>
    public event EventHandler<DataGridRowClipboardEventArgs>? CopyingRowClipboardContent;

    #endregion
    
    /// <summary>
    /// Gets the data item bound to the row that contains the current cell.
    /// </summary>
    protected object? CurrentItem
    {
        get
        {
            if (CurrentSlot == -1 || ItemsSource == null || RowGroupHeadersTable.Contains(CurrentSlot))
            {
                return null;
            }
            return DataConnection.GetDataItem(RowIndexFromSlot(CurrentSlot));
        }
    }

    static DataGrid()
    {
        AffectsMeasure<DataGrid>(
            ColumnHeaderHeightProperty,
            HorizontalScrollBarVisibilityProperty,
            VerticalScrollBarVisibilityProperty);
        
        ItemsSourceProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.HandleItemsSourcePropertyChanged(e));
    }

    public DataGrid()
    {
        KeyDown += HandleKeyDown;
        KeyUp   += HandleKeyUp;

        //TODO: Check if override works
        GotFocus  += HandleGotFocus;
        LostFocus += HandleLostFocus;
        
        _loadedRows       = new List<DataGridRow>();
        _lostFocusActions = new Queue<Action>();
        _selectedItems    = new DataGridSelectedItemsCollection(this);

        RowGroupHeadersTable     = new IndexToValueTable<DataGridRowGroupInfo>();
        _bindingValidationErrors = new List<Exception>();
        DisplayData              = new DataGridDisplayData(this);
        
        ColumnsInternal                   =  CreateColumnsInstance();
        ColumnsInternal.CollectionChanged += HandleColumnsInternalCollectionChanged;

        RowHeightEstimate        = DefaultRowHeight;
        RowDetailsHeightEstimate = 0;
        _rowHeaderDesiredWidth   = 0;

        DataConnection       = new DataGridDataConnection(this);
        _showDetailsTable    = new IndexToValueTable<bool>();
        _collapsedSlotsTable = new IndexToValueTable<bool>();

        AnchorSlot             = -1;
        _lastEstimatedRow      = -1;
        _editingColumnIndex    = -1;
        _mouseOverRowIndex     = null;
        CurrentCellCoordinates = new DataGridCellCoordinates(-1, -1);

        RowGroupHeaderHeightEstimate = DefaultRowHeight;
        RowGroupSublevelIndents      = [];
        _rowGroupHeightsByLevel      = [];
        UpdatePseudoClasses();
    }

    /// <summary>
    /// Enters editing mode for the current cell and current row (if they're not already in editing mode).
    /// </summary>
    /// <param name="editingEventArgs">Provides information about the user gesture that caused the call to BeginEdit. Can be null.</param>
    /// <returns>True if operation was successful. False otherwise.</returns>
    public bool BeginEdit(RoutedEventArgs editingEventArgs)
    {
        if (CurrentColumnIndex == -1 || !GetRowSelection(CurrentSlot))
        {
            return false;
        }

        Debug.Assert(CurrentColumnIndex >= 0);
        Debug.Assert(CurrentColumnIndex < ColumnsItemsInternal.Count);
        Debug.Assert(CurrentSlot >= -1);
        Debug.Assert(CurrentSlot < SlotCount);
        Debug.Assert(EditingRow == null || EditingRow.Slot == CurrentSlot);

        if (GetColumnEffectiveReadOnlyState(CurrentColumn))
        {
            // Current column is read-only
            return false;
        }
        return BeginCellEdit(editingEventArgs);
    }

    /// <summary>
    /// Cancels editing mode for the specified DataGridEditingUnit and restores its original value.
    /// </summary>
    /// <param name="editingUnit">Specifies whether to cancel edit for a Cell or Row.</param>
    /// <returns>True if operation was successful. False otherwise.</returns>
    public bool CancelEdit(DataGridEditingUnit editingUnit = DataGridEditingUnit.Row)
    {
        return CancelEdit(editingUnit, raiseEvents: true);
    }

    /// <summary>
    /// Commits editing mode and pushes changes to the backend.
    /// </summary>
    /// <returns>True if operation was successful. False otherwise.</returns>
    public bool CommitEdit()
    {
        return CommitEdit(DataGridEditingUnit.Row, true);
    }

    /// <summary>
    /// Commits editing mode for the specified DataGridEditingUnit and pushes changes to the backend.
    /// </summary>
    /// <param name="editingUnit">Specifies whether to commit edit for a Cell or Row.</param>
    /// <param name="exitEditingMode">Editing mode is left if True.</param>
    /// <returns>True if operation was successful. False otherwise.</returns>
    public bool CommitEdit(DataGridEditingUnit editingUnit, bool exitEditingMode)
    {
        if (!EndCellEdit(
                editAction: DataGridEditAction.Commit,
                exitEditingMode: editingUnit == DataGridEditingUnit.Cell ? exitEditingMode : true,
                keepFocus: ContainsFocus,
                raiseEvents: true))
        {
            return false;
        }
        if (editingUnit == DataGridEditingUnit.Row)
        {
            return EndRowEdit(DataGridEditAction.Commit, exitEditingMode, raiseEvents: true);
        }
        return true;
    }

    /// <summary>
    /// Scrolls the specified item or RowGroupHeader and/or column into view.
    /// If item is not null: scrolls the row representing the item into view;
    /// If column is not null: scrolls the column into view;
    /// If both item and column are null, the method returns without scrolling.
    /// </summary>
    /// <param name="item">an item from the DataGrid's items source or a CollectionViewGroup from the collection view</param>
    /// <param name="column">a column from the DataGrid's columns collection</param>
    public void ScrollIntoView(object? item, DataGridColumn? column)
    {
        if ((column == null && (item == null || FirstDisplayedNonFillerColumnIndex == -1))
            || (column != null && column.OwningGrid != this))
        {
            // no-op
            return;
        }
        if (item == null && column != null)
        {
            // scroll column into view
            ScrollSlotIntoView(
                column.Index,
                DisplayData.FirstScrollingSlot,
                forCurrentCellChange: false,
                forceHorizontalScroll: true);
        }
        else
        {
            int                  slot         = -1;
            DataGridRowGroupInfo? rowGroupInfo = null;
            if (item is DataGridCollectionViewGroup collectionViewGroup)
            {
                rowGroupInfo = RowGroupInfoFromCollectionViewGroup(collectionViewGroup);
                if (rowGroupInfo == null)
                {
                    Debug.Assert(false);
                    return;
                }
                slot = rowGroupInfo.Slot;
            }
            else
            {
                // the row index will be set to -1 if the item is null or not in the list
                int rowIndex = DataConnection.IndexOf(item);
                if (rowIndex == -1)
                {
                    return;
                }
                slot = SlotFromRowIndex(rowIndex);
            }

            int columnIndex = (column == null) ? FirstDisplayedNonFillerColumnIndex : column.Index;

            if (_collapsedSlotsTable.Contains(slot))
            {
                // We need to expand all parent RowGroups so that the slot is visible
                if (rowGroupInfo != null)
                {
                    ExpandRowGroupParentChain(rowGroupInfo.Level - 1, rowGroupInfo.Slot);
                }
                else
                {
                    rowGroupInfo = RowGroupHeadersTable.GetValueAt(RowGroupHeadersTable.GetPreviousIndex(slot));
                    Debug.Assert(rowGroupInfo != null);
                    ExpandRowGroupParentChain(rowGroupInfo.Level, rowGroupInfo.Slot);
                }

                // Update Scrollbar and display information
                NegVerticalOffset = 0;
                SetVerticalOffset(0);
                ResetDisplayedRows();
                DisplayData.FirstScrollingSlot = 0;
                ComputeScrollBarsLayout();
            }

            ScrollSlotIntoView(
                columnIndex, slot,
                forCurrentCellChange: true,
                forceHorizontalScroll: true);
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataConnection.DataSource != null && !DataConnection.EventsWired)
        {
            DataConnection.WireEvents(DataConnection.DataSource);
            InitializeElements(true /*recycleRows*/);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        // When wired to INotifyCollectionChanged, the DataGrid will be cleaned up by GC
        if (DataConnection.DataSource != null && DataConnection.EventsWired)
        {
            DataConnection.UnWireEvents(DataConnection.DataSource);
        }
    }

    /// <summary>
    /// Arranges the content of the <see cref="T:Avalonia.Controls.DataGridRow" />.
    /// </summary>
    /// <param name="finalSize">
    /// The final area within the parent that this element should use to arrange itself and its children.
    /// </param>
    /// <returns>
    /// The actual size used by the <see cref="T:Avalonia.Controls.DataGridRow" />.
    /// </returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_makeFirstDisplayedCellCurrentCellPending)
        {
            MakeFirstDisplayedCellCurrentCell();
        }

        if (Bounds.Width != finalSize.Width)
        {
            // If our final width has changed, we might need to update the filler
            InvalidateColumnHeadersArrange();
            InvalidateCellsArrange();
        }

        return base.ArrangeOverride(finalSize);
    }

    /// <summary>
    /// Measures the children of a <see cref="T:Avalonia.Controls.DataGridRow" /> to prepare for
    /// arranging them during the
    /// <see cref="M:Avalonia.Controls.DataGridRow.ArrangeOverride(System.Windows.Size)" /> pass.
    /// </summary>
    /// <returns>
    /// The size that the <see cref="T:Avalonia.Controls.DataGridRow" /> determines it needs during layout, based on its calculations of child object allocated sizes.
    /// </returns>
    /// <param name="availableSize">
    /// The available size that this element can give to child elements. Indicates an upper limit that
    /// child elements should not exceed.
    /// </param>
    protected override Size MeasureOverride(Size availableSize)
    {
        // Delay layout until after the initial measure to avoid invalid calculations when the
        // DataGrid is not part of the visual tree
        if (!_measured)
        {
            _measured = true;

            // We don't need to clear the rows because it was already done when the ItemsSource changed
            RefreshRowsAndColumns(clearRows: false);

            //// Update our estimates now that the DataGrid has all of the information necessary
            UpdateRowDetailsHeightEstimate();

            // Update frozen columns to account for columns added prior to loading or autogenerated columns
            if (FrozenColumnCountWithFiller > 0)
            {
                ProcessFrozenColumnCount();
            }
        }

        Size desiredSize;
        // This is a shortcut to skip layout if we don't have any columns
        if (ColumnsInternal.VisibleEdgedColumnsWidth == 0)
        {
            if (_hScrollBar != null && _hScrollBar.IsVisible)
            {
                _hScrollBar.IsVisible = false;
            }
            if (_vScrollBar != null && _vScrollBar.IsVisible)
            {
                _vScrollBar.IsVisible = false;
            }
            desiredSize = base.MeasureOverride(availableSize);
        }
        else
        {
            if (_rowsPresenter != null)
            {
                _rowsPresenter.InvalidateMeasure();
            }

            InvalidateColumnHeadersMeasure();

            desiredSize = base.MeasureOverride(availableSize);

            ComputeScrollBarsLayout();
        }

        return desiredSize;
    }

    /// <inheritdoc/>
    protected override void OnDataContextBeginUpdate()
    {
        base.OnDataContextBeginUpdate();

        NotifyDataContextPropertyForAllRowCells(GetAllRows(), true);
    }

    /// <inheritdoc/>
    protected override void OnDataContextEndUpdate()
    {
        base.OnDataContextEndUpdate();

        NotifyDataContextPropertyForAllRowCells(GetAllRows(), false);
    }

    /// <summary>
    /// Raises the BeginningEdit event.
    /// </summary>
    protected virtual void NotifyBeginningEdit(DataGridBeginningEditEventArgs e)
    {
        BeginningEdit?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellEditEnded event.
    /// </summary>
    protected virtual void NotifyCellEditEnded(DataGridCellEditEndedEventArgs e)
    {
        CellEditEnded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellEditEnding event.
    /// </summary>
    protected virtual void NotifyCellEditEnding(DataGridCellEditEndingEventArgs e)
    {
        CellEditEnding?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CellPointerPressed event.
    /// </summary>
    internal virtual void NotifyCellPointerPressed(DataGridCellPointerPressedEventArgs e)
    {
        CellPointerPressed?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the CurrentCellChanged event.
    /// </summary>
    protected virtual void OnCurrentCellChanged(EventArgs e)
    {
        CurrentCellChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the LoadingRow event for row preparation.
    /// </summary>
    protected virtual void OnLoadingRow(DataGridRowEventArgs e)
    {
        EventHandler<DataGridRowEventArgs>? handler = LoadingRow;
        if (handler != null)
        {
            Debug.Assert(!_loadedRows.Contains(e.Row));
            _loadedRows.Add(e.Row);
            LoadingOrUnloadingRow = true;
            handler(this, e);
            LoadingOrUnloadingRow = false;
            Debug.Assert(_loadedRows.Contains(e.Row));
            _loadedRows.Remove(e.Row);
        }
    }

    /// <summary>
    /// Scrolls the DataGrid according to the direction of the delta.
    /// </summary>
    /// <param name="e">PointerWheelEventArgs</param>
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        var delta = e.Delta;
            
        // KeyModifiers.Shift should scroll in horizontal direction. This does not work on every platform. 
        // If Shift-Key is pressed and X is close to 0 we swap the Vector.
        if (e.KeyModifiers == KeyModifiers.Shift && MathUtilities.IsZero(delta.X))
        {
            delta = new Vector(delta.Y, delta.X);
        }
            
        if(UpdateScroll(delta * MouseWheelDelta))
        {
            e.Handled = true;
        }
        else
        {
            e.Handled = e.Handled || !ScrollViewer.GetIsScrollChainingEnabled(this);
        }
    }

    internal bool UpdateScroll(Vector delta)
    {
        if (IsEnabled && DisplayData.NumDisplayedScrollingElements > 0)
        {
            var handled          = false;
            var ignoreInvalidate = false;
            var scrollHeight     = 0d;

            // Vertical scroll handling
            if (delta.Y > 0)
            {
                scrollHeight = Math.Max(-_verticalOffset, -delta.Y);
            }
            else if (delta.Y < 0)
            {
                if (_vScrollBar != null && VerticalScrollBarVisibility == ScrollBarVisibility.Visible)
                {
                    scrollHeight = Math.Min(Math.Max(0, _vScrollBar.Maximum - _verticalOffset), -delta.Y);
                }
                else
                {
                    double maximum = EdgedRowsHeightCalculated - CellsEstimatedHeight;
                    scrollHeight = Math.Min(Math.Max(0, maximum - _verticalOffset), -delta.Y);
                }
            }

            if (scrollHeight != 0)
            {
                DisplayData.PendingVerticalScrollHeight = scrollHeight;
                handled                                 = true;
            }

            // Horizontal scroll handling
            if (delta.X != 0)
            {
                var horizontalOffset = HorizontalOffset - delta.X;
                var widthNotVisible  = Math.Max(0, ColumnsInternal.VisibleEdgedColumnsWidth - CellsWidth);

                if (horizontalOffset < 0)
                {
                    horizontalOffset = 0;
                }
                if (horizontalOffset > widthNotVisible)
                {
                    horizontalOffset = widthNotVisible;
                }

                if (UpdateHorizontalOffset(horizontalOffset))
                {
                    // We don't need to invalidate once again after UpdateHorizontalOffset.
                    ignoreInvalidate = true;
                    handled          = true;
                }
            }

            if (handled)
            {
                if (!ignoreInvalidate)
                {
                    InvalidateRowsMeasure(invalidateIndividualElements: false);
                }
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Raises the PreparingCellForEdit event.
    /// </summary>
    protected virtual void OnPreparingCellForEdit(DataGridPreparingCellForEditEventArgs e)
    {
        PreparingCellForEdit?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowEditEnded event.
    /// </summary>
    protected virtual void OnRowEditEnded(DataGridRowEditEndedEventArgs e)
    {
        RowEditEnded?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the RowEditEnding event.
    /// </summary>
    protected virtual void OnRowEditEnding(DataGridRowEditEndingEventArgs e)
    {
        RowEditEnding?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the SelectionChanged event and clears the _selectionChanged.
    /// This event won't get raised again until after _selectionChanged is set back to true.
    /// </summary>
    protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        RaiseEvent(e);
    }

    /// <summary>
    /// Raises the UnloadingRow event for row recycling.
    /// </summary>
    protected virtual void OnUnloadingRow(DataGridRowEventArgs e)
    {
        EventHandler<DataGridRowEventArgs>? handler = UnloadingRow;
        if (handler != null)
        {
            LoadingOrUnloadingRow = true;
            handler(this, e);
            LoadingOrUnloadingRow = false;
        }
    }

    /// <summary>
    /// Comparator class so we can sort list by the display index
    /// </summary>
    public class DisplayIndexComparer : IComparer<DataGridColumn>
    {
        int IComparer<DataGridColumn>.Compare(DataGridColumn? x, DataGridColumn? y)
        {
            if (x == null && y == null)
            {
                return 0;
            } 
            if (x == null && y != null)
            {
                return -1;
            }

            if (x != null && y == null)
            {
                return 1;
            }
            
            Debug.Assert(x != null && y != null);
            return (x.DisplayIndexWithFiller < y.DisplayIndexWithFiller) ? -1 : 1;
        }
    }

    /// <summary>
    /// Builds the visual tree for the column header when a new template is applied.
    /// </summary>
    //TODO Validation UI
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        // The template has changed, so we need to refresh the visuals
        _measured = false;

        if (_columnHeadersPresenter != null)
        {
            // If we're applying a new template, we want to remove the old column headers first
            _columnHeadersPresenter.Children.Clear();
        }

        _columnHeadersPresenter = e.NameScope.Find<DataGridColumnHeadersPresenter>(DataGridTheme.ColumnHeadersPresenterPart);

        if (_columnHeadersPresenter != null)
        {
            if (ColumnsInternal.FillerColumn != null)
            {
                ColumnsInternal.FillerColumn.IsRepresented = false;
            }
            _columnHeadersPresenter.OwningGrid = this;

            // Columns were added before our Template was applied, add the ColumnHeaders now
            List<DataGridColumn> sortedInternal = new List<DataGridColumn>(ColumnsItemsInternal);
            sortedInternal.Sort(new DisplayIndexComparer());
            foreach (DataGridColumn column in sortedInternal)
            {
                InsertDisplayedColumnHeader(column);
            }
        }

        if (_rowsPresenter != null)
        {
            // If we're applying a new template, we want to remove the old rows first
            UnloadElements(recycle: false);
        }

        _rowsPresenter = e.NameScope.Find<DataGridRowsPresenter>(DataGridTheme.RowsPresenterPart);

        if (_rowsPresenter != null)
        {
            _rowsPresenter.OwningGrid = this;
            InvalidateRowHeightEstimate();
            UpdateRowDetailsHeightEstimate();
        }

        _frozenColumnScrollBarSpacer = e.NameScope.Find<Control>(DataGridTheme.FrozenColumnScrollBarSpacerPart);

        if (_hScrollBar != null)
        {
            _hScrollBar.Scroll -= HandleHorizontalScrollBarScroll;
        }

        _hScrollBar = e.NameScope.Find<ScrollBar>(DataGridTheme.HorizontalScrollbarPart);

        if (_hScrollBar != null)
        {
            _hScrollBar.IsTabStop   =  false;
            _hScrollBar.Maximum     =  0.0;
            _hScrollBar.Orientation =  Orientation.Horizontal;
            _hScrollBar.IsVisible   =  false;
            _hScrollBar.Scroll      += HandleHorizontalScrollBarScroll;
        }

        if (_vScrollBar != null)
        {
            _vScrollBar.Scroll -= HandleVerticalScrollBarScroll;
        }

        _vScrollBar = e.NameScope.Find<ScrollBar>(DataGridTheme.VerticalScrollbarPart);

        if (_vScrollBar != null)
        {
            _vScrollBar.IsTabStop   =  false;
            _vScrollBar.Maximum     =  0.0;
            _vScrollBar.Orientation =  Orientation.Vertical;
            _vScrollBar.IsVisible   =  false;
            _vScrollBar.Scroll      += HandleVerticalScrollBarScroll;
        }

        _topLeftCornerHeader = e.NameScope.Find<ContentControl>(DataGridTheme.TopLeftCornerPart);
        EnsureTopLeftCornerHeader(); // EnsureTopLeftCornerHeader checks for a null _topLeftCornerHeader;
        _topRightCornerHeader = e.NameScope.Find<ContentControl>(DataGridTheme.TopRightCornePart);
        _bottomRightCorner    = e.NameScope.Find<Visual>(DataGridTheme.BottomRightCornerPart);
    }

    /// <summary>
    /// Cancels editing mode for the specified DataGridEditingUnit and restores its original value.
    /// </summary>
    /// <param name="editingUnit">Specifies whether to cancel edit for a Cell or Row.</param>
    /// <param name="raiseEvents">Specifies whether or not to raise editing events</param>
    /// <returns>True if operation was successful. False otherwise.</returns>
    internal bool CancelEdit(DataGridEditingUnit editingUnit, bool raiseEvents)
    {
        if (!EndCellEdit(
                DataGridEditAction.Cancel,
                exitEditingMode: true,
                keepFocus: ContainsFocus,
                raiseEvents: raiseEvents))
        {
            return false;
        }

        if (editingUnit == DataGridEditingUnit.Row)
        {
            return EndRowEdit(DataGridEditAction.Cancel, true, raiseEvents);
        }

        return true;
    }

    /// <summary>
    /// call when: selection changes or SelectedItems object changes
    /// </summary>
    internal void CoerceSelectedItem()
    {
        object? selectedItem = null;

        if (SelectionMode == DataGridSelectionMode.Extended &&
            CurrentSlot != -1 &&
            _selectedItems.ContainsSlot(CurrentSlot))
        {
            selectedItem = CurrentItem;
        }
        else if (_selectedItems.Count > 0)
        {
            selectedItem = _selectedItems[0];
        }

        SetValueNoCallback(SelectedItemProperty, selectedItem);

        // Update the SelectedIndex
        int newIndex = -1;

        if (selectedItem != null)
        {
            newIndex = DataConnection.IndexOf(selectedItem);
        }

        SetValueNoCallback(SelectedIndexProperty, newIndex);
    }

    internal IEnumerable<object?> GetSelectionInclusive(int startRowIndex, int endRowIndex)
    {
        int endSlot = SlotFromRowIndex(endRowIndex);
        foreach (int slot in _selectedItems.GetSlots(SlotFromRowIndex(startRowIndex)))
        {
            if (slot > endSlot)
            {
                break;
            }
            yield return DataConnection.GetDataItem(RowIndexFromSlot(slot));
        }
    }

    internal void InitializeElements(bool recycleRows)
    {
        try
        {
            _noCurrentCellChangeCount++;

            // The underlying collection has changed and our editing row (if there is one)
            // is no longer relevant, so we should force a cancel edit.
            CancelEdit(DataGridEditingUnit.Row, raiseEvents: false);

            // We want to persist selection throughout a reset, so store away the selected items
            List<object> selectedItemsCache = new List<object>(_selectedItems.SelectedItemsCache);

            if (recycleRows)
            {
                RefreshRows(recycleRows, clearRows: true);
            }
            else
            {
                RefreshRowsAndColumns(clearRows: true);
            }

            // Re-select the old items
            _selectedItems.SelectedItemsCache = selectedItemsCache;
            CoerceSelectedItem();
            if (RowDetailsVisibilityMode != DataGridRowDetailsVisibilityMode.Collapsed)
            {
                UpdateRowDetailsVisibilityMode(RowDetailsVisibilityMode);
            }

            // The currently displayed rows may have incorrect visual states because of the selection change
            ApplyDisplayedRowsState(DisplayData.FirstScrollingSlot, DisplayData.LastScrollingSlot);
        }
        finally
        {
            NoCurrentCellChangeCount--;
        }
    }

    internal bool IsDoubleClickRecordsClickOnCall(Control element)
    {
        if (_clickedElement == element)
        {
            _clickedElement = null;
            return true;
        }
        _clickedElement = element;
        return false;
    }

    // Returns the item or the CollectionViewGroup that is used as the DataContext for a given slot.
    // If the DataContext is an item, rowIndex is set to the index of the item within the collection
    internal object? ItemFromSlot(int slot, ref int rowIndex)
    {
        if (RowGroupHeadersTable.Contains(slot))
        {
            return RowGroupHeadersTable.GetValueAt(slot)?.CollectionViewGroup;
        }
        rowIndex = RowIndexFromSlot(slot);
        return DataConnection.GetDataItem(rowIndex);
    }

    internal bool ProcessDownKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out bool ctrl, out bool shift);
        return ProcessDownKeyInternal(shift, ctrl);
    }

    internal bool ProcessEndKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out bool ctrl, out bool shift);
        return ProcessEndKey(shift, ctrl);
    }

    internal bool ProcessEnterKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out bool ctrl, out bool shift);
        return ProcessEnterKey(shift, ctrl);
    }

    internal bool ProcessHomeKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out bool ctrl, out bool shift);
        return ProcessHomeKey(shift, ctrl);
    }

    internal void ProcessHorizontalScroll(ScrollEventType scrollEventType)
    {
        if (_horizontalScrollChangesIgnored > 0)
        {
            return;
        }

        // If the user scrolls with the buttons, we need to update the new value of the scroll bar since we delay
        // this calculation.  If they scroll in another other way, the scroll bar's correct value has already been set
        double scrollBarValueDifference = 0;
        if (scrollEventType == ScrollEventType.SmallIncrement)
        {
            scrollBarValueDifference = GetHorizontalSmallScrollIncrease();
        }
        else if (scrollEventType == ScrollEventType.SmallDecrement)
        {
            scrollBarValueDifference = -GetHorizontalSmallScrollDecrease();
        }
        _horizontalScrollChangesIgnored++;
        try
        {
            Debug.Assert(_hScrollBar != null);
            if (scrollBarValueDifference != 0)
            {
                Debug.Assert(_horizontalOffset + scrollBarValueDifference >= 0);
                _hScrollBar.Value = _horizontalOffset + scrollBarValueDifference;
            }
            UpdateHorizontalOffset(_hScrollBar.Value);
        }
        finally
        {
            _horizontalScrollChangesIgnored--;
        }
    }

    internal bool ProcessLeftKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out bool ctrl, out bool shift);
        return ProcessLeftKey(shift, ctrl);
    }

    internal bool ProcessNextKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out bool ctrl, out bool shift);
        return ProcessNextKey(shift, ctrl);
    }

    internal bool ProcessPriorKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out bool ctrl, out bool shift);
        return ProcessPriorKey(shift, ctrl);
    }

    internal bool ProcessRightKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out bool ctrl, out bool shift);
        return ProcessRightKey(shift, ctrl);
    }

    /// <summary>
    /// Selects items and updates currency based on parameters
    /// </summary>
    /// <param name="columnIndex">column index to make current</param>
    /// <param name="item">data item or CollectionViewGroup to make current</param>
    /// <param name="backupSlot">slot to use in case the item is no longer valid</param>
    /// <param name="action">selection action to perform</param>
    /// <param name="scrollIntoView">whether or not the new current item should be scrolled into view</param>
    internal void ProcessSelectionAndCurrency(int columnIndex, object item, int backupSlot, DataGridSelectionAction action, bool scrollIntoView)
    {
        _noSelectionChangeCount++;
        _noCurrentCellChangeCount++;
        try
        {
            int slot = -1;
            if (item is DataGridCollectionViewGroup group)
            {
                DataGridRowGroupInfo? groupInfo = RowGroupInfoFromCollectionViewGroup(group);
                if (groupInfo != null)
                {
                    slot = groupInfo.Slot;
                }
            }
            else
            {
                slot = SlotFromRowIndex(DataConnection.IndexOf(item));
            }
            if (slot == -1)
            {
                slot = backupSlot;
            }
            if (slot < 0 || slot > SlotCount)
            {
                return;
            }

            switch (action)
            {
                case DataGridSelectionAction.AddCurrentToSelection:
                    SetRowSelection(slot, isSelected: true, setAnchorSlot: true);
                    break;
                case DataGridSelectionAction.RemoveCurrentFromSelection:
                    SetRowSelection(slot, isSelected: false, setAnchorSlot: false);
                    break;
                case DataGridSelectionAction.SelectFromAnchorToCurrent:
                    if (SelectionMode == DataGridSelectionMode.Extended && AnchorSlot != -1)
                    {
                        int anchorSlot = AnchorSlot;
                        if (slot <= anchorSlot)
                        {
                            SetRowsSelection(slot, anchorSlot);
                        }
                        else
                        {
                            SetRowsSelection(anchorSlot, slot);
                        }
                    }
                    else
                    {
                        goto case DataGridSelectionAction.SelectCurrent;
                    }
                    break;
                case DataGridSelectionAction.SelectCurrent:
                    ClearRowSelection(slot, setAnchorSlot: true);
                    break;
                case DataGridSelectionAction.None:
                    break;
            }

            if (CurrentSlot != slot || (CurrentColumnIndex != columnIndex && columnIndex != -1))
            {
                if (columnIndex == -1)
                {
                    if (CurrentColumnIndex != -1)
                    {
                        columnIndex = CurrentColumnIndex;
                    }
                    else
                    {
                        DataGridColumn? firstVisibleColumn = ColumnsInternal.FirstVisibleNonFillerColumn;
                        if (firstVisibleColumn != null)
                        {
                            columnIndex = firstVisibleColumn.Index;
                        }
                    }
                }
                if (columnIndex != -1)
                {
                    if (!SetCurrentCellCore(
                            columnIndex, slot,
                            commitEdit: true,
                            endRowEdit: SlotFromRowIndex(SelectedIndex) != slot)
                        || (scrollIntoView &&
                            !ScrollSlotIntoView(
                                columnIndex, slot,
                                forCurrentCellChange: true,
                                forceHorizontalScroll: false)))
                    {
                        return;
                    }
                }
            }
            _successfullyUpdatedSelection = true;
        }
        finally
        {
            NoCurrentCellChangeCount--;
            NoSelectionChangeCount--;
        }
    }

    internal bool ProcessUpKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out bool ctrl, out bool shift);
        return ProcessUpKey(shift, ctrl);
    }

    //internal void ProcessVerticalScroll(double oldValue, double newValue)
    internal void ProcessVerticalScroll(ScrollEventType scrollEventType)
    {
        if (_verticalScrollChangesIgnored > 0)
        {
            return;
        }
        Debug.Assert(_vScrollBar != null);
        Debug.Assert(MathUtilities.LessThanOrClose(_vScrollBar.Value, _vScrollBar.Maximum));

        _verticalScrollChangesIgnored++;
        try
        {
            Debug.Assert(_vScrollBar != null);
            if (scrollEventType == ScrollEventType.SmallIncrement)
            {
                DisplayData.PendingVerticalScrollHeight = GetVerticalSmallScrollIncrease();
                double newVerticalOffset = _verticalOffset + DisplayData.PendingVerticalScrollHeight;
                if (newVerticalOffset > _vScrollBar.Maximum)
                {
                    DisplayData.PendingVerticalScrollHeight -= newVerticalOffset - _vScrollBar.Maximum;
                }
            }
            else if (scrollEventType == ScrollEventType.SmallDecrement)
            {
                if (MathUtilities.GreaterThan(NegVerticalOffset, 0))
                {
                    DisplayData.PendingVerticalScrollHeight -= NegVerticalOffset;
                }
                else
                {
                    int previousScrollingSlot = GetPreviousVisibleSlot(DisplayData.FirstScrollingSlot);
                    if (previousScrollingSlot >= 0)
                    {
                        ScrollSlotIntoView(previousScrollingSlot, scrolledHorizontally: false);
                    }
                    return;
                }
            }
            else
            {
                DisplayData.PendingVerticalScrollHeight = _vScrollBar.Value - _verticalOffset;
            }

            if (!MathUtilities.IsZero(DisplayData.PendingVerticalScrollHeight))
            {
                // Invalidate so the scroll happens on idle
                InvalidateRowsMeasure(invalidateIndividualElements: false);
            }
        }
        finally
        {
            _verticalScrollChangesIgnored--;
        }
    }

    internal void RefreshRowsAndColumns(bool clearRows)
    {
        if (_measured)
        {
            try
            {
                _noCurrentCellChangeCount++;

                if (clearRows)
                {
                    ClearRows(false);
                    ClearRowGroupHeadersTable();
                    PopulateRowGroupHeadersTable();
                }
                if (AutoGenerateColumns)
                {
                    //Column auto-generation refreshes the rows too
                    AutoGenerateColumnsPrivate();
                }
                foreach (DataGridColumn column in ColumnsItemsInternal)
                {
                    //We don't need to refresh the state of AutoGenerated column headers because they're up-to-date
                    if (!column.IsAutoGenerated && column.HasHeaderCell)
                    {
                        column.HeaderCell.UpdatePseudoClasses();
                    }
                }

                RefreshRows(recycleRows: false, clearRows: false);

                if (Columns.Count > 0 && CurrentColumnIndex == -1)
                {
                    MakeFirstDisplayedCellCurrentCell();
                }
                else
                {
                    _makeFirstDisplayedCellCurrentCellPending = false;
                    _desiredCurrentColumnIndex                = -1;
                    FlushCurrentCellChanged();
                }
            }
            finally
            {
                NoCurrentCellChangeCount--;
            }
        }
        else
        {
            if (clearRows)
            {
                ClearRows(recycle: false);
            }
            ClearRowGroupHeadersTable();
            PopulateRowGroupHeadersTable();
        }
    }

    internal bool ScrollSlotIntoView(int columnIndex, int slot, bool forCurrentCellChange, bool forceHorizontalScroll)
    {
        Debug.Assert(columnIndex >= 0 && columnIndex < ColumnsItemsInternal.Count);
        Debug.Assert(DisplayData.FirstDisplayedScrollingCol >= -1 && DisplayData.FirstDisplayedScrollingCol < ColumnsItemsInternal.Count);
        Debug.Assert(DisplayData.LastTotallyDisplayedScrollingCol >= -1 && DisplayData.LastTotallyDisplayedScrollingCol < ColumnsItemsInternal.Count);
        Debug.Assert(!IsSlotOutOfBounds(slot));
        Debug.Assert(DisplayData.FirstScrollingSlot >= -1 && DisplayData.FirstScrollingSlot < SlotCount);
        Debug.Assert(ColumnsItemsInternal[columnIndex].IsVisible);

        if (CurrentColumnIndex >= 0 &&
            (CurrentColumnIndex != columnIndex || CurrentSlot != slot))
        {
            if (!CommitEditForOperation(columnIndex, slot, forCurrentCellChange) || IsInnerCellOutOfBounds(columnIndex, slot))
            {
                return false;
            }
        }

        double oldHorizontalOffset = HorizontalOffset;

        //scroll horizontally unless we're on a RowGroupHeader and we're not forcing horizontal scrolling
        if ((forceHorizontalScroll || (slot != -1))
            && !ScrollColumnIntoView(columnIndex))
        {
            return false;
        }

        //scroll vertically
        if (!ScrollSlotIntoView(slot, scrolledHorizontally: !MathUtils.AreClose(oldHorizontalOffset, HorizontalOffset)))
        {
            return false;
        }

        return true;
    }

    // Convenient overload that commits the current edit.
    internal bool SetCurrentCellCore(int columnIndex, int slot)
    {
        return SetCurrentCellCore(columnIndex, slot, commitEdit: true, endRowEdit: true);
    }

    internal bool UpdateHorizontalOffset(double newValue)
    {
        if (!MathUtils.AreClose(HorizontalOffset, newValue))
        {
            HorizontalOffset = newValue;
            InvalidateColumnHeadersMeasure();
            InvalidateRowsMeasure(true);
            return true;
        }
        return false;
    }

    internal bool UpdateSelectionAndCurrency(int columnIndex, int slot, DataGridSelectionAction action, bool scrollIntoView)
    {
        _successfullyUpdatedSelection = false;

        _noSelectionChangeCount++;
        _noCurrentCellChangeCount++;
        try
        {
            Debug.Assert(ColumnsInternal.RowGroupSpacerColumn != null);
            if (ColumnsInternal.RowGroupSpacerColumn.IsRepresented &&
                columnIndex == ColumnsInternal.RowGroupSpacerColumn.Index)
            {
                columnIndex = -1;
            }
            if (IsSlotOutOfSelectionBounds(slot) || (columnIndex != -1 && IsColumnOutOfBounds(columnIndex)))
            {
                return false;
            }

            int    newCurrentPosition = -1;
            object? item               = ItemFromSlot(slot, ref newCurrentPosition);

            if (EditingRow != null && slot != EditingRow.Slot && !CommitEdit(DataGridEditingUnit.Row, true))
            {
                return false;
            }
            // TODO 需要评估
            if (item == null)
            {
                return false;
            }
            if (DataConnection.CollectionView != null &&
                DataConnection.CollectionView.CurrentPosition != newCurrentPosition)
            {
                DataConnection.MoveCurrentTo(item, slot, columnIndex, action, scrollIntoView);
            }
            else
            {
                ProcessSelectionAndCurrency(columnIndex, item, slot, action, scrollIntoView);
            }
        }
        finally
        {
            NoCurrentCellChangeCount--;
            NoSelectionChangeCount--;
        }

        return _successfullyUpdatedSelection;
    }

    internal void UpdateStateOnCurrentChanged(object? currentItem, int currentPosition)
    {
        if (currentItem == CurrentItem && currentItem == SelectedItem && currentPosition == SelectedIndex)
        {
            // The DataGrid's CurrentItem is already up-to-date, so we don't need to do anything
            return;
        }
        Debug.Assert(ColumnsInternal.RowGroupSpacerColumn != null);
        int columnIndex = CurrentColumnIndex;
        if (columnIndex == -1)
        {
            if (IsColumnOutOfBounds(_desiredCurrentColumnIndex) ||
                (ColumnsInternal.RowGroupSpacerColumn.IsRepresented && _desiredCurrentColumnIndex == ColumnsInternal.RowGroupSpacerColumn.Index))
            {
                columnIndex = FirstDisplayedNonFillerColumnIndex;
            }
            else
            {
                columnIndex = _desiredCurrentColumnIndex;
            }
        }
        _desiredCurrentColumnIndex = -1;

        try
        {
            _noSelectionChangeCount++;
            _noCurrentCellChangeCount++;

            if (!CommitEdit())
            {
                CancelEdit(DataGridEditingUnit.Row, false);
            }

            ClearRowSelection(true);
            if (currentItem == null)
            {
                SetCurrentCellCore(-1, -1);
            }
            else
            {
                int slot = SlotFromRowIndex(currentPosition);
                ProcessSelectionAndCurrency(columnIndex, currentItem, slot, DataGridSelectionAction.SelectCurrent, false);
            }
        }
        finally
        {
            NoCurrentCellChangeCount--;
            NoSelectionChangeCount--;
        }
    }

    //TODO: Ensure right button is checked for
    internal bool UpdateStateOnMouseRightButtonDown(PointerPressedEventArgs pointerPressedEventArgs, int columnIndex, int slot, bool allowEdit)
    {
        KeyboardHelper.GetMetaKeyState(this, pointerPressedEventArgs.KeyModifiers, out bool ctrl, out bool shift);
        return UpdateStateOnMouseRightButtonDown(pointerPressedEventArgs, columnIndex, slot, allowEdit, shift, ctrl);
    }
    
    //TODO: Ensure left button is checked for
    internal bool UpdateStateOnMouseLeftButtonDown(PointerPressedEventArgs pointerPressedEventArgs, int columnIndex, int slot, bool allowEdit)
    {
        KeyboardHelper.GetMetaKeyState(this, pointerPressedEventArgs.KeyModifiers, out bool ctrl, out bool shift);
        return UpdateStateOnMouseLeftButtonDown(pointerPressedEventArgs, columnIndex, slot, allowEdit, shift, ctrl);
    }

    internal void UpdateVerticalScrollBar()
    {
        if (_vScrollBar != null && _vScrollBar.IsVisible)
        {
            double cellsHeight               = CellsEstimatedHeight;
            double edgedRowsHeightCalculated = EdgedRowsHeightCalculated;
            UpdateVerticalScrollBar(
                needVertScrollbar: edgedRowsHeightCalculated > cellsHeight,
                forceVertScrollbar: VerticalScrollBarVisibility == ScrollBarVisibility.Visible,
                totalVisibleHeight: edgedRowsHeightCalculated,
                cellsHeight: cellsHeight);
        }
    }

    /// <summary>
    /// If the editing element has focus, this method will set focus to the DataGrid itself
    /// in order to force the element to lose focus.  It will then wait for the editing element's
    /// LostFocus event, at which point it will perform the specified action.
    ///
    /// NOTE: It is important to understand that the specified action will be performed when the editing
    /// element loses focus only if this method returns true.  If it returns false, then the action
    /// will not be performed later on, and should instead be performed by the caller, if necessary.
    /// </summary>
    /// <param name="action">Action to perform after the editing element loses focus</param>
    /// <returns>True if the editing element had focus and the action was cached away; false otherwise</returns>
    //TODO TabStop
    internal bool WaitForLostFocus(Action action)
    {
        if (EditingRow != null && EditingColumnIndex != -1 && !_executingLostFocusActions)
        {
            DataGridColumn editingColumn  = ColumnsItemsInternal[EditingColumnIndex];
            Control?        editingElement = editingColumn.GetCellContent(EditingRow);
            if (editingElement != null && editingElement.ContainsChild(_focusedObject))
            {
                Debug.Assert(_lostFocusActions != null);
                _lostFocusActions.Enqueue(action);
                editingElement.LostFocus += HandleEditingElementLostFocus;
                //IsTabStop = true;
                Focus();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Raises the LoadingRowDetails for row details preparation
    /// </summary>
    protected virtual void NotifyLoadingRowDetails(DataGridRowDetailsEventArgs e)
    {
        EventHandler<DataGridRowDetailsEventArgs>? handler = LoadingRowDetails;
        if (handler != null)
        {
            LoadingOrUnloadingRow = true;
            handler(this, e);
            LoadingOrUnloadingRow = false;
        }
    }

    /// <summary>
    /// Raises the UnloadingRowDetails event
    /// </summary>
    protected virtual void NotifyUnloadingRowDetails(DataGridRowDetailsEventArgs e)
    {
        EventHandler<DataGridRowDetailsEventArgs>? handler = UnloadingRowDetails;
        if (handler != null)
        {
            LoadingOrUnloadingRow = true;
            handler(this, e);
            LoadingOrUnloadingRow = false;
        }
    }

    internal void NotifyRowDetailsChanged()
    {
        if (!_scrollingByHeight)
        {
            // Update layout when RowDetails are expanded or collapsed, just updating the vertical scroll bar is not enough
            // since rows could be added or removed
            InvalidateMeasure();
        }
    }

    private static void NotifyDataContextPropertyForAllRowCells(IEnumerable<DataGridRow> rowSource, bool arg2)
    {
        foreach (DataGridRow row in rowSource)
        {
            foreach (DataGridCell cell in row.Cells)
            {
                if (cell.Content is StyledElement cellContent)
                {
                    // TODO Error 需要重构
                    //DataContextProperty.Notifying?.Invoke(cellContent, arg2);
                }
            }
        }
    }

    private void UpdateRowDetailsVisibilityMode(DataGridRowDetailsVisibilityMode newDetailsMode)
    {
        int itemCount = DataConnection.Count;
        if (_rowsPresenter != null && itemCount > 0)
        {
            bool newDetailsVisibility = false;
            switch (newDetailsMode)
            {
                case DataGridRowDetailsVisibilityMode.Visible:
                    newDetailsVisibility = true;
                    _showDetailsTable.AddValues(0, itemCount, true);
                    break;
                case DataGridRowDetailsVisibilityMode.Collapsed:
                    newDetailsVisibility = false;
                    _showDetailsTable.AddValues(0, itemCount, false);
                    break;
                case DataGridRowDetailsVisibilityMode.VisibleWhenSelected:
                    _showDetailsTable.Clear();
                    break;
            }

            bool updated = false;
            foreach (DataGridRow row in GetAllRows())
            {
                if (row.IsVisible)
                {
                    if (newDetailsMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
                    {
                        // For VisibleWhenSelected, we need to calculate the value for each individual row
                        newDetailsVisibility = _selectedItems.ContainsSlot(row.Slot);
                    }
                    if (row.AreDetailsVisible != newDetailsVisibility)
                    {
                        updated = true;

                        row.SetDetailsVisibilityInternal(newDetailsVisibility, raiseNotification: true, animate: false);
                    }
                }
            }
            if (updated)
            {
                UpdateDisplayedRows(DisplayData.FirstScrollingSlot, CellsEstimatedHeight);
                InvalidateRowsMeasure(invalidateIndividualElements: false);
            }
        }
    }

    private void AddNewCellPrivate(DataGridRow row, DataGridColumn column)
    {
        DataGridCell newCell = new DataGridCell();
        PopulateCellContent(
            isCellEdited: false,
            dataGridColumn: column,
            dataGridRow: row,
            dataGridCell: newCell);
        if (row.OwningGrid != null)
        {
            newCell.OwningColumn = column;
            newCell.IsVisible    = column.IsVisible;
            if (row.OwningGrid.CellTheme is {} cellTheme)
            {
                newCell.SetValue(ThemeProperty, cellTheme, BindingPriority.Template);
            }
        }
        row.Cells.Insert(column.Index, newCell);
    }

    private bool BeginCellEdit(RoutedEventArgs editingEventArgs)
    {
        if (CurrentColumnIndex == -1 || !GetRowSelection(CurrentSlot))
        {
            return false;
        }
    
        Debug.Assert(CurrentColumn != null);
        Debug.Assert(CurrentColumnIndex >= 0);
        Debug.Assert(CurrentColumnIndex < ColumnsItemsInternal.Count);
        Debug.Assert(CurrentSlot >= -1);
        Debug.Assert(CurrentSlot < SlotCount);
        Debug.Assert(EditingRow == null || EditingRow.Slot == CurrentSlot);
        Debug.Assert(!GetColumnEffectiveReadOnlyState(CurrentColumn));
        Debug.Assert(CurrentColumn.IsVisible);

        if (_editingColumnIndex != -1)
        {
            // Current cell is already in edit mode
            Debug.Assert(_editingColumnIndex == CurrentColumnIndex);
            return true;
        }

        // Get or generate the editing row if it doesn't exist
        DataGridRow? dataGridRow = EditingRow;
        if (dataGridRow == null)
        {
            if (IsSlotVisible(CurrentSlot))
            {
                dataGridRow = DisplayData.GetDisplayedElement(CurrentSlot) as DataGridRow;
                Debug.Assert(dataGridRow != null);
            }
            else
            {
                dataGridRow = GenerateRow(RowIndexFromSlot(CurrentSlot), CurrentSlot);
            }
        }
        Debug.Assert(dataGridRow != null);

        // Cache these to see if they change later
        int currentRowIndex    = CurrentSlot;
        int currentColumnIndex = CurrentColumnIndex;

        // Raise the BeginningEdit event
        DataGridCell dataGridCell = dataGridRow.Cells[CurrentColumnIndex];
        DataGridBeginningEditEventArgs e = new DataGridBeginningEditEventArgs(CurrentColumn, dataGridRow, editingEventArgs);
        NotifyBeginningEdit(e);
        if (e.Cancel
            || currentRowIndex != CurrentSlot
            || currentColumnIndex != CurrentColumnIndex
            || !GetRowSelection(CurrentSlot)
            || (EditingRow == null && !BeginRowEdit(dataGridRow)))
        {
            // If either BeginningEdit was canceled, currency/selection was changed in the event handler,
            // or we failed opening the row for edit, then we can no longer continue BeginCellEdit
            return false;
        }
        Debug.Assert(EditingRow != null);
        Debug.Assert(EditingRow.Slot == CurrentSlot);

        // Finally, we can prepare the cell for editing
        _editingColumnIndex = CurrentColumnIndex;
        _editingEventArgs   = editingEventArgs;
        EditingRow.Cells[CurrentColumnIndex].UpdatePseudoClasses();
        PopulateCellContent(
            isCellEdited: true,
            dataGridColumn: CurrentColumn,
            dataGridRow: dataGridRow,
            dataGridCell: dataGridCell);
        return true;
    }

    //TODO Validation
    private bool BeginRowEdit(DataGridRow dataGridRow)
    {
        Debug.Assert(EditingRow == null);
        Debug.Assert(dataGridRow != null);

        Debug.Assert(CurrentSlot >= -1);
        Debug.Assert(CurrentSlot < SlotCount);

        if (DataConnection.BeginEdit(dataGridRow.DataContext))
        {
            EditingRow = dataGridRow;
            GenerateEditingElements();
            return true;
        }
        return false;
    }

    private bool CancelRowEdit(bool exitEditingMode)
    {
        if (EditingRow == null)
        {
            return true;
        }
        Debug.Assert(EditingRow != null && EditingRow.Index >= -1);
        Debug.Assert(EditingRow.Slot < SlotCount);
        Debug.Assert(CurrentColumn != null);

        object? dataItem = EditingRow.DataContext;
        if (!DataConnection.CancelEdit(dataItem))
        {
            return false;
        }
        foreach (DataGridColumn column in Columns)
        {
            if (!exitEditingMode && column.Index == _editingColumnIndex && column is DataGridBoundColumn)
            {
                continue;
            }
            PopulateCellContent(
                isCellEdited: !exitEditingMode && column.Index == _editingColumnIndex,
                dataGridColumn: column,
                dataGridRow: EditingRow,
                dataGridCell: EditingRow.Cells[column.Index]);
        }
        return true;
    }

    private bool CommitEditForOperation(int columnIndex, int slot, bool forCurrentCellChange)
    {
        if (forCurrentCellChange)
        {
            if (!EndCellEdit(DataGridEditAction.Commit, exitEditingMode: true, keepFocus: true, raiseEvents: true))
            {
                return false;
            }
            if (CurrentSlot != slot &&
                !EndRowEdit(DataGridEditAction.Commit, exitEditingMode: true, raiseEvents: true))
            {
                return false;
            }
        }

        if (IsColumnOutOfBounds(columnIndex))
        {
            return false;
        }
        if (slot >= SlotCount)
        {
            // Current cell was reset because the commit deleted row(s).
            // Since the user wants to change the current cell, we don't
            // want to end up with no current cell. We pick the last row
            // in the grid which may be the 'new row'.
            int lastSlot = LastVisibleSlot;
            if (forCurrentCellChange &&
                CurrentColumnIndex == -1 &&
                lastSlot != -1)
            {
                SetAndSelectCurrentCell(columnIndex, lastSlot, forceCurrentCellSelection: false);
            }
            // Interrupt operation because it has become invalid.
            return false;
        }
        return true;
    }

    //TODO Validation
    private bool CommitRowEdit(bool exitEditingMode)
    {
        if (EditingRow == null)
        {
            return true;
        }
        Debug.Assert(EditingRow != null && EditingRow.Index >= -1);
        Debug.Assert(EditingRow.Slot < SlotCount);

        //if (!ValidateEditingRow(scrollIntoView: true, wireEvents: false))
        if (!EditingRow.IsValid)
        {
            return false;
        }

        DataConnection.EndEdit(EditingRow.DataContext);

        if (!exitEditingMode)
        {
            DataConnection.BeginEdit(EditingRow.DataContext);
        }
        return true;
    }

    private void CompleteCellsCollection(DataGridRow dataGridRow)
    {
        Debug.Assert(dataGridRow != null);
        int cellsInCollection = dataGridRow.Cells.Count;
        if (ColumnsItemsInternal.Count > cellsInCollection)
        {
            for (int columnIndex = cellsInCollection; columnIndex < ColumnsItemsInternal.Count; columnIndex++)
            {
                AddNewCellPrivate(dataGridRow, ColumnsItemsInternal[columnIndex]);
            }
        }
    }

    private void ComputeScrollBarsLayout()
    {
        if (_ignoreNextScrollBarsLayout)
        {
            _ignoreNextScrollBarsLayout = false;
        }

        bool isHorizontalScrollBarOverCells = IsHorizontalScrollBarOverCells;
        bool isVerticalScrollBarOverCells   = IsVerticalScrollBarOverCells;

        double cellsWidth  = CellsWidth;
        double cellsHeight = CellsEstimatedHeight;

        bool   allowHorizScrollbar  = false;
        bool   forceHorizScrollbar  = false;
        double horizScrollBarHeight = 0;
        if (_hScrollBar != null)
        {
            forceHorizScrollbar = HorizontalScrollBarVisibility == ScrollBarVisibility.Visible;
            allowHorizScrollbar = forceHorizScrollbar || (ColumnsInternal.VisibleColumnCount > 0 &&
                                                          HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled &&
                                                          HorizontalScrollBarVisibility != ScrollBarVisibility.Hidden);
            // Compensate if the horizontal scrollbar is already taking up space
            if (!forceHorizScrollbar && _hScrollBar.IsVisible)
            {
                if (!isHorizontalScrollBarOverCells)
                {
                    cellsHeight += _hScrollBar.DesiredSize.Height;
                }
            }
            if (!isHorizontalScrollBarOverCells)
            {
                horizScrollBarHeight = _hScrollBar.Height + _hScrollBar.Margin.Top + _hScrollBar.Margin.Bottom;
            }
        }

        bool   allowVertScrollbar = false;
        bool   forceVertScrollbar = false;
        double vertScrollBarWidth = 0;
        if (_vScrollBar != null)
        {
            forceVertScrollbar = VerticalScrollBarVisibility == ScrollBarVisibility.Visible;
            allowVertScrollbar = forceVertScrollbar || (ColumnsItemsInternal.Count > 0 &&
                                                        VerticalScrollBarVisibility != ScrollBarVisibility.Disabled &&
                                                        VerticalScrollBarVisibility != ScrollBarVisibility.Hidden);
            // Compensate if the vertical scrollbar is already taking up space
            if (!forceVertScrollbar && _vScrollBar.IsVisible)
            {
                if (!isVerticalScrollBarOverCells)
                {
                    cellsWidth += _vScrollBar.DesiredSize.Width;
                }
            }
            if (!isVerticalScrollBarOverCells)
            {
                vertScrollBarWidth = _vScrollBar.Width + _vScrollBar.Margin.Left + _vScrollBar.Margin.Right;
            }
        }

        // Now cellsWidth is the width potentially available for displaying data cells.
        // Now cellsHeight is the height potentially available for displaying data cells.

        bool needHorizScrollbar = false;
        bool needVertScrollbar  = false;

        double totalVisibleWidth       = ColumnsInternal.VisibleEdgedColumnsWidth;
        double totalVisibleFrozenWidth = ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth();

        UpdateDisplayedRows(DisplayData.FirstScrollingSlot, CellsEstimatedHeight);
        double totalVisibleHeight = EdgedRowsHeightCalculated;

        if (!forceHorizScrollbar && !forceVertScrollbar)
        {
            bool needHorizScrollbarWithoutVertScrollbar = false;

            if (allowHorizScrollbar &&
                MathUtilities.GreaterThan(totalVisibleWidth, cellsWidth) &&
                MathUtilities.LessThan(totalVisibleFrozenWidth, cellsWidth) &&
                MathUtilities.LessThanOrClose(horizScrollBarHeight, cellsHeight))
            {
                double oldDataHeight = cellsHeight;
                cellsHeight -= horizScrollBarHeight;
                Debug.Assert(cellsHeight >= 0);
                needHorizScrollbarWithoutVertScrollbar = needHorizScrollbar = true;

                if (vertScrollBarWidth > 0 &&
                    allowVertScrollbar && (MathUtilities.LessThanOrClose(totalVisibleWidth - cellsWidth, vertScrollBarWidth) ||
                                           MathUtilities.LessThanOrClose(cellsWidth - totalVisibleFrozenWidth, vertScrollBarWidth)))
                {
                    // Would we still need a horizontal scrollbar without the vertical one?
                    UpdateDisplayedRows(DisplayData.FirstScrollingSlot, cellsHeight);
                    if (DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount)
                    {
                        needHorizScrollbar = MathUtilities.LessThan(totalVisibleFrozenWidth, cellsWidth - vertScrollBarWidth);
                    }
                }

                if (!needHorizScrollbar)
                {
                    // Restore old data height because turns out a horizontal scroll bar wouldn't make sense
                    cellsHeight = oldDataHeight;
                }
            }

            // Store the current FirstScrollingSlot because removing the horizontal scrollbar could scroll
            // the DataGrid up; however, if we realize later that we need to keep the horizontal scrollbar
            // then we should use the first slot stored here which is not scrolled.
            int firstScrollingSlot = DisplayData.FirstScrollingSlot;

            UpdateDisplayedRows(firstScrollingSlot, cellsHeight);
            if (allowVertScrollbar &&
                MathUtilities.GreaterThan(cellsHeight, 0) &&
                MathUtilities.LessThanOrClose(vertScrollBarWidth, cellsWidth) &&
                DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount)
            {
                cellsWidth -= vertScrollBarWidth;
                Debug.Assert(cellsWidth >= 0);
                needVertScrollbar = true;
            }

            DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();

            // we compute the number of visible columns only after we set up the vertical scroll bar.
            ComputeDisplayedColumns();

            if ((vertScrollBarWidth > 0 || horizScrollBarHeight > 0) &&
                allowHorizScrollbar &&
                needVertScrollbar && !needHorizScrollbar &&
                MathUtilities.GreaterThan(totalVisibleWidth, cellsWidth) &&
                MathUtilities.LessThan(totalVisibleFrozenWidth, cellsWidth) &&
                MathUtilities.LessThanOrClose(horizScrollBarHeight, cellsHeight))
            {
                cellsWidth  += vertScrollBarWidth;
                cellsHeight -= horizScrollBarHeight;
                Debug.Assert(cellsHeight >= 0);
                needVertScrollbar = false;

                UpdateDisplayedRows(firstScrollingSlot, cellsHeight);
                if (cellsHeight > 0 &&
                    vertScrollBarWidth <= cellsWidth &&
                    DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount)
                {
                    cellsWidth -= vertScrollBarWidth;
                    Debug.Assert(cellsWidth >= 0);
                    needVertScrollbar = true;
                }
                if (needVertScrollbar)
                {
                    needHorizScrollbar = true;
                }
                else
                {
                    needHorizScrollbar = needHorizScrollbarWithoutVertScrollbar;
                }
            }
        }
        else if (forceHorizScrollbar && !forceVertScrollbar)
        {
            if (allowVertScrollbar)
            {
                if (cellsHeight > 0 &&
                    MathUtilities.LessThanOrClose(vertScrollBarWidth, cellsWidth) &&
                    DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount)
                {
                    cellsWidth -= vertScrollBarWidth;
                    Debug.Assert(cellsWidth >= 0);
                    needVertScrollbar = true;
                }
                DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                ComputeDisplayedColumns();
            }
            needHorizScrollbar = totalVisibleWidth > cellsWidth && totalVisibleFrozenWidth < cellsWidth;
        }
        else if (!forceHorizScrollbar && forceVertScrollbar)
        {
            if (allowHorizScrollbar)
            {
                if (cellsWidth > 0 &&
                    MathUtilities.LessThanOrClose(horizScrollBarHeight, cellsHeight) &&
                    MathUtilities.GreaterThan(totalVisibleWidth, cellsWidth) &&
                    MathUtilities.LessThan(totalVisibleFrozenWidth, cellsWidth))
                {
                    cellsHeight -= horizScrollBarHeight;
                    Debug.Assert(cellsHeight >= 0);
                    needHorizScrollbar = true;
                    UpdateDisplayedRows(DisplayData.FirstScrollingSlot, cellsHeight);
                }
                DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                ComputeDisplayedColumns();
            }
            needVertScrollbar = DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount;
        }
        else
        {
            Debug.Assert(forceHorizScrollbar && forceVertScrollbar);
            Debug.Assert(allowHorizScrollbar && allowVertScrollbar);
            DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
            ComputeDisplayedColumns();
            needVertScrollbar  = DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount;
            needHorizScrollbar = totalVisibleWidth > cellsWidth && totalVisibleFrozenWidth < cellsWidth;
        }

        UpdateHorizontalScrollBar(needHorizScrollbar, forceHorizScrollbar, totalVisibleWidth, totalVisibleFrozenWidth, cellsWidth);
        UpdateVerticalScrollBar(needVertScrollbar, forceVertScrollbar, totalVisibleHeight, cellsHeight);

        if (_topRightCornerHeader != null)
        {
            // Show the TopRightHeaderCell based on vertical ScrollBar visibility
            if (AreColumnHeadersVisible &&
                _vScrollBar != null && _vScrollBar.IsVisible)
            {
                _topRightCornerHeader.IsVisible = true;
            }
            else
            {
                _topRightCornerHeader.IsVisible = false;
            }
        }

        if (_bottomRightCorner != null)
        {
            // Show the BottomRightCorner when both scrollbars are visible.
            _bottomRightCorner.IsVisible =
                _hScrollBar != null && _hScrollBar.IsVisible &&
                _vScrollBar != null && _vScrollBar.IsVisible;
        }

        DisplayData.FullyRecycleElements();
    }

    /// <summary>
    /// Handles the current editing element's LostFocus event by performing any actions that
    /// were cached by the WaitForLostFocus method.
    /// </summary>
    /// <param name="sender">Editing element</param>
    /// <param name="e">RoutedEventArgs</param>
    private void HandleEditingElementLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is Control editingElement)
        {
            editingElement.LostFocus -= HandleEditingElementLostFocus;
            if (EditingRow != null && _editingColumnIndex != -1)
            {
                FocusEditingCell(true);
            }
            Debug.Assert(_lostFocusActions != null);
            try
            {
                _executingLostFocusActions = true;
                while (_lostFocusActions.Count > 0)
                {
                    _lostFocusActions.Dequeue()();
                }
            }
            finally
            {
                _executingLostFocusActions = false;
            }
        }
    }

    // Makes sure horizontal layout is updated to reflect any changes that affect it
    private void EnsureHorizontalLayout()
    {
        ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
        InvalidateColumnHeadersMeasure();
        InvalidateRowsMeasure(true);
        InvalidateMeasure();
    }

    private void EnsureRowHeaderWidth()
    {
        if (AreRowHeadersVisible)
        {
            if (AreColumnHeadersVisible)
            {
                EnsureTopLeftCornerHeader();
            }

            if (_rowsPresenter != null)
            {

                bool updated = false;

                foreach (Control element in _rowsPresenter.Children)
                {
                    if (element is DataGridRow row)
                    {
                        // If the RowHeader resulted in a different width the last time it was measured, we need
                        // to re-measure it
                        if (row.HeaderCell != null && row.HeaderCell.DesiredSize.Width != ActualRowHeaderWidth)
                        {
                            row.HeaderCell.InvalidateMeasure();
                            updated = true;
                        }
                    }
                    else if (element is DataGridRowGroupHeader groupHeader && groupHeader.HeaderCell != null && groupHeader.HeaderCell.DesiredSize.Width != ActualRowHeaderWidth)
                    {
                        groupHeader.HeaderCell.InvalidateMeasure();
                        updated = true;
                    }
                }

                if (updated)
                {
                    // We need to update the width of the horizontal scrollbar if the rowHeaders' width actually changed
                    InvalidateMeasure();
                }
            }
        }
    }

    private void EnsureRowsPresenterVisibility()
    {
        if (_rowsPresenter != null)
        {
            // RowCount doesn't need to be considered, doing so might cause extra Visibility changes
            _rowsPresenter.IsVisible = (ColumnsInternal.FirstVisibleNonFillerColumn != null);
        }
    }

    private void EnsureTopLeftCornerHeader()
    {
        if (_topLeftCornerHeader != null)
        {
            _topLeftCornerHeader.IsVisible = (HeadersVisibility == DataGridHeadersVisibility.All);

            if (_topLeftCornerHeader.IsVisible)
            {
                if (!double.IsNaN(RowHeaderWidth))
                {
                    // RowHeaderWidth is set explicitly so we should use that
                    _topLeftCornerHeader.Width = RowHeaderWidth;
                }
                else if (VisibleSlotCount > 0)
                {
                    // RowHeaders AutoSize and we have at least 1 row so take the desired width
                    _topLeftCornerHeader.Width = RowHeadersDesiredWidth;
                }
            }
        }
    }

    private void InvalidateCellsArrange()
    {
        foreach (DataGridRow row in GetAllRows())
        {
            row.InvalidateHorizontalArrange();
        }
    }

    private void InvalidateColumnHeadersArrange()
    {
        if (_columnHeadersPresenter != null)
        {
            _columnHeadersPresenter.InvalidateArrange();
        }
    }

    private void InvalidateColumnHeadersMeasure()
    {
        if (_columnHeadersPresenter != null)
        {
            EnsureColumnHeadersVisibility();
            _columnHeadersPresenter.InvalidateMeasure();
        }
    }

    private void InvalidateRowsArrange()
    {
        if (_rowsPresenter != null)
        {
            _rowsPresenter.InvalidateArrange();
        }
    }

    private void InvalidateRowsMeasure(bool invalidateIndividualElements)
    {
        if (_rowsPresenter != null)
        {
            _rowsPresenter.InvalidateMeasure();

            if (invalidateIndividualElements)
            {
                foreach (Control element in _rowsPresenter.Children)
                {
                    element.InvalidateMeasure();
                }
            }
        }
    }

    //TODO: Check
    private void DataGrid_IsEnabledChanged(AvaloniaPropertyChangedEventArgs e)
    {
    }

    private void DataGrid_KeyDown(object sender, KeyEventArgs e)
    {
        if (!e.Handled)
        {
            e.Handled = ProcessDataGridKey(e);
        }
    }

    private void DataGrid_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab && CurrentColumnIndex != -1 && e.Source == this)
        {
            bool success =
                ScrollSlotIntoView(
                    CurrentColumnIndex, CurrentSlot,
                    forCurrentCellChange: false,
                    forceHorizontalScroll: true);
            Debug.Assert(success);
            if (CurrentColumnIndex != -1 && SelectedItem == null)
            {
                SetRowSelection(CurrentSlot, isSelected: true, setAnchorSlot: true);
            }
        }
    }

    //TODO: Make override?
    private void DataGrid_LostFocus(object sender, RoutedEventArgs e)
    {
        _focusedObject = null;
        if (ContainsFocus)
        {
            bool           focusLeftDataGrid = true;
            bool           dataGridWillReceiveRoutedEvent = true;
           
            Visual?         focusedObject =  FocusUtils.GetFocusManager(this)?.GetFocusedElement() as Visual;
            DataGridColumn? editingColumn = null;

            while (focusedObject != null)
            {
                if (focusedObject == this)
                {
                    focusLeftDataGrid = false;
                    break;
                }

                // Walk up the visual tree.  If we hit the root, try using the framework element's
                // parent.  We do this because Popups behave differently with respect to the visual tree,
                // and it could have a parent even if the VisualTreeHelper doesn't find it.
                var parent = focusedObject.Parent as Visual;
                if (parent == null)
                {
                    parent = focusedObject.GetVisualParent();
                }
                else
                {
                    dataGridWillReceiveRoutedEvent = false;
                }
                focusedObject = parent;
            }

            if (EditingRow != null && EditingColumnIndex != -1)
            {
                editingColumn = ColumnsItemsInternal[EditingColumnIndex];

                if (focusLeftDataGrid && editingColumn is DataGridTemplateColumn)
                {
                    dataGridWillReceiveRoutedEvent = false;
                }
            }

            if (focusLeftDataGrid && !(editingColumn is DataGridTemplateColumn))
            {
                ContainsFocus = false;
                if (EditingRow != null)
                {
                    CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true);
                }
                ResetFocusedRow();
                ApplyDisplayedRowsState(DisplayData.FirstScrollingSlot, DisplayData.LastScrollingSlot);
                if (CurrentColumnIndex != -1 && IsSlotVisible(CurrentSlot))
                {
                    if (DisplayData.GetDisplayedElement(CurrentSlot) is DataGridRow row)
                    {
                        row.Cells[CurrentColumnIndex].UpdatePseudoClasses();
                    }
                }
            }
            else if (!dataGridWillReceiveRoutedEvent)
            {
                if (focusedObject is Control focusedElement)
                {
                    focusedElement.LostFocus += HandleExternalEditingElementLostFocus;
                }
            }
        }
    }

    private void HandleEditingElementInitialized(object? sender, EventArgs e)
    {
        if (sender is Control element)
        {
            element.Initialized -= HandleEditingElementInitialized;
            PreparingCellForEditPrivate(element);
        }
    }

    //TODO Validation
    //TODO Binding
    //TODO TabStop
    private bool EndCellEdit(DataGridEditAction editAction, bool exitEditingMode, bool keepFocus, bool raiseEvents)
    {
        if (_editingColumnIndex == -1)
        {
            return true;
        }

        var editingRow = EditingRow;
        if (editingRow is null)
        {
            return true;
        }

        Debug.Assert(_editingColumnIndex >= 0);
        Debug.Assert(_editingColumnIndex < ColumnsItemsInternal.Count);
        Debug.Assert(_editingColumnIndex == CurrentColumnIndex);
        Debug.Assert(CurrentColumn != null);

        // Cache these to see if they change later
        int currentSlot        = CurrentSlot;
        int currentColumnIndex = CurrentColumnIndex;

        // We're ready to start ending, so raise the event
        DataGridCell editingCell    = editingRow.Cells[_editingColumnIndex];
        var          editingElement = editingCell.Content as Control;
        if (editingElement == null)
        {
            return false;
        }
        if (raiseEvents)
        {
            Debug.Assert(CurrentColumn != null);
            DataGridCellEditEndingEventArgs e = new DataGridCellEditEndingEventArgs(CurrentColumn, editingRow, editingElement, editAction);
            NotifyCellEditEnding(e);
            if (e.Cancel)
            {
                // CellEditEnding has been cancelled
                return false;
            }

            // Ensure that the current cell wasn't changed in the user's CellEditEnding handler
            if (_editingColumnIndex == -1 ||
                currentSlot != CurrentSlot ||
                currentColumnIndex != CurrentColumnIndex)
            {
                return true;
            }
            Debug.Assert(EditingRow != null);
            Debug.Assert(EditingRow.Slot == currentSlot);
            Debug.Assert(_editingColumnIndex != -1);
            Debug.Assert(_editingColumnIndex == CurrentColumnIndex);
        }

        // If we're canceling, let the editing column repopulate its old value if it wants
        if (editAction == DataGridEditAction.Cancel)
        {
            Debug.Assert(_uneditedValue != null);
            CurrentColumn.CancelCellEditInternal(editingElement, _uneditedValue);

            // Ensure that the current cell wasn't changed in the user column's CancelCellEdit
            if (_editingColumnIndex == -1 ||
                currentSlot != CurrentSlot ||
                currentColumnIndex != CurrentColumnIndex)
            {
                return true;
            }
            Debug.Assert(EditingRow != null);
            Debug.Assert(EditingRow.Slot == currentSlot);
            Debug.Assert(_editingColumnIndex != -1);
            Debug.Assert(_editingColumnIndex == CurrentColumnIndex);
        }

        // If we're committing, explicitly update the source but watch out for any validation errors
        if (editAction == DataGridEditAction.Commit)
        {
            void SetValidationStatus(ICellEditBinding binding)
            {
                if (binding.IsValid)
                {
                    ResetValidationStatus();
                    DataValidationErrors.ClearErrors(editingElement);
                }
                else
                {
                    if (editingCell.IsValid)
                    {
                        editingCell.IsValid = false;
                        editingCell.UpdatePseudoClasses();
                    }

                    if (editingRow.IsValid)
                    {
                        editingRow.IsValid = false;
                        editingRow.ApplyState();
                    }

                    DataValidationErrors.SetError(editingElement,
                        new AggregateException(binding.ValidationErrors));
                }
            }

            var editBinding = CurrentColumn?.CellEditBinding;
            if (editBinding != null && !editBinding.CommitEdit())
            {
                SetValidationStatus(editBinding);
                _validationSubscription?.Dispose();
                _validationSubscription = editBinding.ValidationChanged.Subscribe(v => SetValidationStatus(editBinding));

                ScrollSlotIntoView(CurrentColumnIndex, CurrentSlot, forCurrentCellChange: false, forceHorizontalScroll: true);
                return false;
            }
        }

        ResetValidationStatus();
        Debug.Assert(CurrentColumn != null);
        if (exitEditingMode)
        {
    
            CurrentColumn.EndCellEditInternal();
            _editingColumnIndex = -1;
            editingCell.UpdatePseudoClasses();

            //IsTabStop = true;
            if (keepFocus && editingElement.ContainsFocusedElement())
            {
                Focus();
            }

            PopulateCellContent(
                isCellEdited: !exitEditingMode,
                dataGridColumn: CurrentColumn,
                dataGridRow: editingRow,
                dataGridCell: editingCell);

            editingRow.InvalidateDesiredHeight();
            var column = editingCell.OwningColumn;
            Debug.Assert(column != null);
            if (column.Width.IsSizeToCells || column.Width.IsAuto)
            {
                // Invalidate desired width and force recalculation
                column.SetWidthDesiredValue(0);
                editingRow.OwningGrid?.AutoSizeColumn(column, editingCell.DesiredSize.Width);
            }
        }

        // We're done, so raise the CellEditEnded event
        if (raiseEvents)
        {
            NotifyCellEditEnded(new DataGridCellEditEndedEventArgs(CurrentColumn, editingRow, editAction));
        }

        // There's a chance that somebody reopened this cell for edit within the CellEditEnded handler,
        // so we should return false if we were supposed to exit editing mode, but we didn't
        return !(exitEditingMode && currentColumnIndex == _editingColumnIndex);
    }

    //TODO Validation
    private bool EndRowEdit(DataGridEditAction editAction, bool exitEditingMode, bool raiseEvents)
    {
        if (EditingRow == null || DataConnection.CommittingEdit)
        {
            return true;
        }
        if (_editingColumnIndex != -1 || (editAction == DataGridEditAction.Cancel && raiseEvents &&
                                          !((DataConnection.EditableCollectionView != null && DataConnection.EditableCollectionView.CanCancelEdit) || (EditingRow.DataContext is IEditableObject))))
        {
            // Ending the row edit will fail immediately under the following conditions:
            // 1. We haven't ended the cell edit yet.
            // 2. We're trying to cancel edit when the underlying DataType is not an IEditableObject,
            //    because we have no way to properly restore the old value.  We will only allow this to occur
            //    if raiseEvents == false, which means we're internally forcing a cancel.
            return false;
        }
        DataGridRow editingRow = EditingRow;

        if (raiseEvents)
        {
            DataGridRowEditEndingEventArgs e = new DataGridRowEditEndingEventArgs(EditingRow, editAction);
            OnRowEditEnding(e);
            if (e.Cancel)
            {
                // RowEditEnding has been cancelled
                return false;
            }

            // Editing states might have been changed in the RowEditEnding handlers
            if (_editingColumnIndex != -1)
            {
                return false;
            }
            if (editingRow != EditingRow)
            {
                return true;
            }
        }

        // Call the appropriate commit or cancel methods
        if (editAction == DataGridEditAction.Commit)
        {
            if (!CommitRowEdit(exitEditingMode))
            {
                return false;
            }
        }
        else
        {
            if (!CancelRowEdit(exitEditingMode) && raiseEvents)
            {
                // We failed to cancel edit so we should abort unless we're forcing a cancel
                return false;
            }
        }
        ResetValidationStatus();

        // Update the previously edited row's state
        if (exitEditingMode && editingRow == EditingRow)
        {
            RemoveEditingElements();
            ResetEditingRow();
        }

        // Raise the RowEditEnded event
        if (raiseEvents)
        {
            OnRowEditEnded(new DataGridRowEditEndedEventArgs(editingRow, editAction));
        }

        return true;
    }

    private void EnsureColumnHeadersVisibility()
    {
        if (_columnHeadersPresenter != null)
        {
            _columnHeadersPresenter.IsVisible = AreColumnHeadersVisible;
        }
    }

    /// <summary>
    /// Exits editing mode without trying to commit or revert the editing, and
    /// without repopulating the edited row's cell.
    /// </summary>
    //TODO TabStop
    private void ExitEdit(bool keepFocus)
    {
        if (EditingRow == null || DataConnection.CommittingEdit)
        {
            Debug.Assert(_editingColumnIndex == -1);
            return;
        }

        if (_editingColumnIndex != -1)
        {
            Debug.Assert(_editingColumnIndex >= 0);
            Debug.Assert(_editingColumnIndex < ColumnsItemsInternal.Count);
            Debug.Assert(_editingColumnIndex == CurrentColumnIndex);
            Debug.Assert(EditingRow != null && EditingRow.Slot == CurrentSlot);

            _editingColumnIndex = -1;
            EditingRow.Cells[CurrentColumnIndex].UpdatePseudoClasses();
        }
        //IsTabStop = true;
        if (IsSlotVisible(EditingRow.Slot))
        {
            EditingRow.ApplyState();
        }
        ResetEditingRow();
        if (keepFocus)
        {
            Focus();
        }
    }

    private void HandleExternalEditingElementLostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is Control element)
        {
            element.LostFocus -= HandleExternalEditingElementLostFocus;
            DataGrid_LostFocus(sender, e);
        }
    }

    private void FlushSelectionChanged()
    {
        if (SelectionHasChanged && _noSelectionChangeCount == 0 && !_makeFirstDisplayedCellCurrentCellPending)
        {
            CoerceSelectedItem();
            if (NoCurrentCellChangeCount != 0)
            {
                // current cell is changing, don't raise SelectionChanged until it's done
                return;
            }
            SelectionHasChanged = false;

            if (_flushCurrentCellChanged)
            {
                FlushCurrentCellChanged();
            }

            SelectionChangedEventArgs e = _selectedItems.GetSelectionChangedEventArgs();
            if (e.AddedItems.Count > 0 || e.RemovedItems.Count > 0)
            {
                OnSelectionChanged(e);
            }
        }
    }

    //TODO TabStop
    private bool FocusEditingCell(bool setFocus)
    {
        Debug.Assert(CurrentColumnIndex >= 0);
        Debug.Assert(CurrentColumnIndex < ColumnsItemsInternal.Count);
        Debug.Assert(CurrentSlot >= -1);
        Debug.Assert(CurrentSlot < SlotCount);
        Debug.Assert(EditingRow != null && EditingRow.Slot == CurrentSlot);
        Debug.Assert(_editingColumnIndex != -1);

        //IsTabStop = false;
        _focusEditingControl = false;

        bool         success      = false;
        DataGridCell dataGridCell = EditingRow.Cells[_editingColumnIndex];
        if (setFocus)
        {
            if (dataGridCell.ContainsFocusedElement())
            {
                success = true;
            }
            else
            {
                dataGridCell.Focus();
                success = dataGridCell.ContainsFocusedElement();
            }

            _focusEditingControl = !success;
        }
        return success;
    }

    // Calculates the amount to scroll for the ScrollLeft button
    // This is a method rather than a property to emphasize a calculation
    private double GetHorizontalSmallScrollDecrease()
    {
        // If the first column is covered up, scroll to the start of it when the user clicks the left button
        if (_negHorizontalOffset > 0)
        {
            return _negHorizontalOffset;
        }
        // The entire first column is displayed, show the entire previous column when the user clicks
        // the left button
        DataGridColumn? previousColumn = ColumnsInternal.GetPreviousVisibleScrollingColumn(
            ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol]);
        if (previousColumn != null)
        {
            return GetEdgedColumnWidth(previousColumn);
        }
        // There's no previous column so don't move
        return 0;
    }

    // Calculates the amount to scroll for the ScrollRight button
    // This is a method rather than a property to emphasize a calculation
    private double GetHorizontalSmallScrollIncrease()
    {
        if (DisplayData.FirstDisplayedScrollingCol >= 0)
        {
            return GetEdgedColumnWidth(ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol]) - _negHorizontalOffset;
        }
        return 0;
    }

    // Calculates the amount the ScrollDown button should scroll
    // This is a method rather than a property to emphasize that calculations are taking place
    private double GetVerticalSmallScrollIncrease()
    {
        if (DisplayData.FirstScrollingSlot >= 0)
        {
            return GetExactSlotElementHeight(DisplayData.FirstScrollingSlot) - NegVerticalOffset;
        }
        return 0;
    }

    private void HandleHorizontalScrollBarScroll(object? sender, ScrollEventArgs e)
    {
        ProcessHorizontalScroll(e.ScrollEventType);
        HorizontalScroll?.Invoke(sender, e);
    }

    private bool IsColumnOutOfBounds(int columnIndex)
    {
        return columnIndex >= ColumnsItemsInternal.Count || columnIndex < 0;
    }

    private bool IsInnerCellOutOfBounds(int columnIndex, int slot)
    {
        return IsColumnOutOfBounds(columnIndex) || IsSlotOutOfBounds(slot);
    }

    private bool IsInnerCellOutOfSelectionBounds(int columnIndex, int slot)
    {
        return IsColumnOutOfBounds(columnIndex) || IsSlotOutOfSelectionBounds(slot);
    }

    private bool IsSlotOutOfBounds(int slot)
    {
        return slot >= SlotCount || slot < -1 || _collapsedSlotsTable.Contains(slot);
    }

    private bool IsSlotOutOfSelectionBounds(int slot)
    {
        if (RowGroupHeadersTable.Contains(slot))
        {
            Debug.Assert(slot >= 0 && slot < SlotCount);
            return false;
        }
        else
        {
            int rowIndex = RowIndexFromSlot(slot);
            return rowIndex < 0 || rowIndex >= DataConnection.Count;
        }
    }

    private void MakeFirstDisplayedCellCurrentCell()
    {
        if (CurrentColumnIndex != -1)
        {
            _makeFirstDisplayedCellCurrentCellPending = false;
            _desiredCurrentColumnIndex                = -1;
            FlushCurrentCellChanged();
            return;
        }
        if (SlotCount != SlotFromRowIndex(DataConnection.Count))
        {
            _makeFirstDisplayedCellCurrentCellPending = true;
            return;
        }

        // No current cell, therefore no selection either - try to set the current cell to the
        // ItemsSource's ICollectionView.CurrentItem if it exists, otherwise use the first displayed cell.
        int slot = 0;
        if (DataConnection.CollectionView != null)
        {
            if (DataConnection.CollectionView.IsCurrentBeforeFirst ||
                DataConnection.CollectionView.IsCurrentAfterLast)
            {
                slot = RowGroupHeadersTable.Contains(0) ? 0 : -1;
            }
            else
            {
                slot = SlotFromRowIndex(DataConnection.CollectionView.CurrentPosition);
            }
        }
        else
        {
            if (SelectedIndex == -1)
            {
                // Try to default to the first row
                slot = SlotFromRowIndex(0);
                if (!IsSlotVisible(slot))
                {
                    slot = -1;
                }
            }
            else
            {
                slot = SlotFromRowIndex(SelectedIndex);
            }
        }
        int columnIndex = FirstDisplayedNonFillerColumnIndex;
        if (_desiredCurrentColumnIndex >= 0 && _desiredCurrentColumnIndex < ColumnsItemsInternal.Count)
        {
            columnIndex = _desiredCurrentColumnIndex;
        }

        SetAndSelectCurrentCell(columnIndex, slot, forceCurrentCellSelection: false);
        AnchorSlot                                = slot;
        _makeFirstDisplayedCellCurrentCellPending = false;
        _desiredCurrentColumnIndex                = -1;
        FlushCurrentCellChanged();
    }

    private void PopulateCellContent(bool isCellEdited,
                                     DataGridColumn dataGridColumn,
                                     DataGridRow dataGridRow,
                                     DataGridCell dataGridCell)
    {
        Debug.Assert(dataGridColumn != null);
        Debug.Assert(dataGridRow != null);
        Debug.Assert(dataGridCell != null);

        Control?             element             = null;
        DataGridBoundColumn? dataGridBoundColumn = dataGridColumn as DataGridBoundColumn;
        Debug.Assert(dataGridRow.DataContext != null);
        if (isCellEdited)
        {
            // Generate EditingElement and apply column style if available
            element = dataGridColumn.GenerateEditingElementInternal(dataGridCell, dataGridRow.DataContext);
            if (element != null)
            {

                dataGridCell.Content = element;
                if (element.IsInitialized)
                {
                    PreparingCellForEditPrivate(element as Control);
                }
                else
                {
                    // Subscribe to the new element's events
                    element.Initialized += HandleEditingElementInitialized;
                }
            }
        }
        else
        {
            // Generate Element and apply column style if available
            element              = dataGridColumn.GenerateElementInternal(dataGridCell, dataGridRow.DataContext);
            dataGridCell.Content = element;
        }
    }

    private void PreparingCellForEditPrivate(Control editingElement)
    {
        Debug.Assert(EditingRow != null);
        if (_editingColumnIndex == -1 ||
            CurrentColumnIndex == -1 ||
            EditingRow.Cells[CurrentColumnIndex].Content != editingElement)
        {
            // The current cell has changed since the call to BeginCellEdit, so the fact
            // that this element has loaded is no longer relevant
            return;
        }

        Debug.Assert(EditingRow != null);
        Debug.Assert(_editingColumnIndex >= 0);
        Debug.Assert(_editingColumnIndex < ColumnsItemsInternal.Count);
        Debug.Assert(_editingColumnIndex == CurrentColumnIndex);
        Debug.Assert(EditingRow != null && EditingRow.Slot == CurrentSlot);

        FocusEditingCell(setFocus: ContainsFocus || _focusEditingControl);

        // Prepare the cell for editing and raise the PreparingCellForEdit event for all columns
        DataGridColumn? dataGridColumn = CurrentColumn;
        Debug.Assert(dataGridColumn != null);
        Debug.Assert(_editingEventArgs != null);
        _uneditedValue = dataGridColumn.PrepareCellForEditInternal(editingElement, _editingEventArgs);
        OnPreparingCellForEdit(new DataGridPreparingCellForEditEventArgs(dataGridColumn, EditingRow, _editingEventArgs, editingElement));
    }

    private bool ProcessAKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out bool ctrl, out bool shift, out bool alt);

        if (ctrl && !shift && !alt && SelectionMode == DataGridSelectionMode.Extended)
        {
            SelectAll();
            return true;
        }
        return false;
    }

    //TODO TabStop
    //TODO FlowDirection
    private bool ProcessDataGridKey(KeyEventArgs e)
    {
        bool focusDataGrid = false;
        switch (e.Key)
        {
            case Key.Tab:
                return ProcessTabKey(e);

            case Key.Up:
                focusDataGrid = ProcessUpKey(e);
                break;

            case Key.Down:
                focusDataGrid = ProcessDownKey(e);
                break;

            case Key.PageDown:
                focusDataGrid = ProcessNextKey(e);
                break;

            case Key.PageUp:
                focusDataGrid = ProcessPriorKey(e);
                break;

            case Key.Left:
                focusDataGrid = ProcessLeftKey(e);
                break;

            case Key.Right:
                focusDataGrid = ProcessRightKey(e);
                break;

            case Key.F2:
                return ProcessF2Key(e);

            case Key.Home:
                focusDataGrid = ProcessHomeKey(e);
                break;

            case Key.End:
                focusDataGrid = ProcessEndKey(e);
                break;

            case Key.Enter:
                focusDataGrid = ProcessEnterKey(e);
                break;

            case Key.Escape:
                return ProcessEscapeKey();

            case Key.A:
                return ProcessAKey(e);

            case Key.C:
                return ProcessCopyKey(e.KeyModifiers);

            case Key.Insert:
                return ProcessCopyKey(e.KeyModifiers);
        }
        if (focusDataGrid)
        {
            Focus();
        }
        return focusDataGrid;
    }

    private bool ProcessDownKeyInternal(bool shift, bool ctrl)
    {
        DataGridColumn? dataGridColumn          = ColumnsInternal.FirstVisibleColumn;
        int            firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
        int            lastSlot                = LastVisibleSlot;
        if (firstVisibleColumnIndex == -1 || lastSlot == -1)
        {
            return false;
        }

        if (WaitForLostFocus(() => ProcessDownKeyInternal(shift, ctrl)))
        {
            return true;
        }

        int nextSlot = -1;
        if (CurrentSlot != -1)
        {
            nextSlot = GetNextVisibleSlot(CurrentSlot);
            if (nextSlot >= SlotCount)
            {
                nextSlot = -1;
            }
        }

        _noSelectionChangeCount++;
        try
        {
            int                     desiredSlot;
            int                     columnIndex;
            DataGridSelectionAction action;
            if (CurrentColumnIndex == -1)
            {
                desiredSlot = FirstVisibleSlot;
                columnIndex = firstVisibleColumnIndex;
                action      = DataGridSelectionAction.SelectCurrent;
            }
            else if (ctrl)
            {
                if (shift)
                {
                    // Both Ctrl and Shift
                    desiredSlot = lastSlot;
                    columnIndex = CurrentColumnIndex;
                    action = (SelectionMode == DataGridSelectionMode.Extended)
                        ? DataGridSelectionAction.SelectFromAnchorToCurrent
                        : DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    // Ctrl without Shift
                    desiredSlot = lastSlot;
                    columnIndex = CurrentColumnIndex;
                    action      = DataGridSelectionAction.SelectCurrent;
                }
            }
            else
            {
                if (nextSlot == -1)
                {
                    return true;
                }
                if (shift)
                {
                    // Shift without Ctrl
                    desiredSlot = nextSlot;
                    columnIndex = CurrentColumnIndex;
                    action      = DataGridSelectionAction.SelectFromAnchorToCurrent;
                }
                else
                {
                    // Neither Ctrl nor Shift
                    desiredSlot = nextSlot;
                    columnIndex = CurrentColumnIndex;
                    action      = DataGridSelectionAction.SelectCurrent;
                }
            }

            UpdateSelectionAndCurrency(columnIndex, desiredSlot, action, scrollIntoView: true);
        }
        finally
        {
            NoSelectionChangeCount--;
        }
        return _successfullyUpdatedSelection;
    }

    private bool ProcessEndKey(bool shift, bool ctrl)
    {
        DataGridColumn? dataGridColumn         = ColumnsInternal.LastVisibleColumn;
        int            lastVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
        int            firstVisibleSlot       = FirstVisibleSlot;
        int            lastVisibleSlot        = LastVisibleSlot;
        if (lastVisibleColumnIndex == -1 || firstVisibleSlot == -1)
        {
            return false;
        }

        if (WaitForLostFocus(() => ProcessEndKey(shift, ctrl)))
        {
            return true;
        }

        _noSelectionChangeCount++;
        try
        {
            if (!ctrl)
            {
                return ProcessRightMost(lastVisibleColumnIndex, firstVisibleSlot);
            }
            else
            {
                DataGridSelectionAction action = (shift && SelectionMode == DataGridSelectionMode.Extended)
                    ? DataGridSelectionAction.SelectFromAnchorToCurrent
                    : DataGridSelectionAction.SelectCurrent;

                UpdateSelectionAndCurrency(lastVisibleColumnIndex, lastVisibleSlot, action, scrollIntoView: true);
            }
        }
        finally
        {
            NoSelectionChangeCount--;
        }
        return _successfullyUpdatedSelection;
    }

    private bool ProcessEnterKey(bool shift, bool ctrl)
    {
        int oldCurrentSlot = CurrentSlot;

        if (!ctrl)
        {
            // If Enter was used by a TextBox, we shouldn't handle the key
            if (FocusUtils.GetFocusManager(this)?.GetFocusedElement() is TextBox focusedTextBox
                && focusedTextBox.AcceptsReturn)
            {
                return false;
            }

            if (WaitForLostFocus(() => ProcessEnterKey(shift, ctrl)))
            {
                return true;
            }

            // Enter behaves like down arrow - it commits the potential editing and goes down one cell.
            if (!ProcessDownKeyInternal(false, ctrl))
            {
                return false;
            }
        }
        else if (WaitForLostFocus(() => ProcessEnterKey(shift, ctrl)))
        {
            return true;
        }

        // Try to commit the potential editing
        if (oldCurrentSlot == CurrentSlot &&
            EndCellEdit(DataGridEditAction.Commit, exitEditingMode: true, keepFocus: true, raiseEvents: true) &&
            EditingRow != null)
        {
            EndRowEdit(DataGridEditAction.Commit, exitEditingMode: true, raiseEvents: true);
            ScrollIntoView(CurrentItem, CurrentColumn);
        }

        return true;
    }

    private bool ProcessEscapeKey()
    {
        if (WaitForLostFocus(() => ProcessEscapeKey()))
        {
            return true;
        }

        if (_editingColumnIndex != -1)
        {
            // Revert the potential cell editing and exit cell editing.
            EndCellEdit(DataGridEditAction.Cancel, exitEditingMode: true, keepFocus: true, raiseEvents: true);
            return true;
        }
        if (EditingRow != null)
        {
            // Revert the potential row editing and exit row editing.
            EndRowEdit(DataGridEditAction.Cancel, exitEditingMode: true, raiseEvents: true);
            return true;
        }
        return false;
    }

    private bool ProcessF2Key(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out bool ctrl, out bool shift);

        if (!shift && !ctrl &&
            _editingColumnIndex == -1 && CurrentColumnIndex != -1 && GetRowSelection(CurrentSlot) &&
            !GetColumnEffectiveReadOnlyState(CurrentColumn))
        {
            if (ScrollSlotIntoView(CurrentColumnIndex, CurrentSlot, forCurrentCellChange: false, forceHorizontalScroll: true))
            {
                BeginCellEdit(e);
            }
            return true;
        }

        return false;
    }

    private bool ProcessHomeKey(bool shift, bool ctrl)
    {
        DataGridColumn? dataGridColumn          = ColumnsInternal.FirstVisibleNonFillerColumn;
        int            firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
        int            firstVisibleSlot        = FirstVisibleSlot;
        if (firstVisibleColumnIndex == -1 || firstVisibleSlot == -1)
        {
            return false;
        }

        if (WaitForLostFocus(() => ProcessHomeKey(shift, ctrl)))
        {
            return true;
        }

        _noSelectionChangeCount++;
        try
        {
            if (!ctrl)
            {
                return ProcessLeftMost(firstVisibleColumnIndex, firstVisibleSlot);
            }
            else
            {
                DataGridSelectionAction action = (shift && SelectionMode == DataGridSelectionMode.Extended)
                    ? DataGridSelectionAction.SelectFromAnchorToCurrent
                    : DataGridSelectionAction.SelectCurrent;

                UpdateSelectionAndCurrency(firstVisibleColumnIndex, firstVisibleSlot, action, scrollIntoView: true);
            }
        }
        finally
        {
            NoSelectionChangeCount--;
        }
        return _successfullyUpdatedSelection;
    }

    private bool ProcessLeftKey(bool shift, bool ctrl)
    {
        DataGridColumn? dataGridColumn          = ColumnsInternal.FirstVisibleNonFillerColumn;
        int            firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
        int            firstVisibleSlot        = FirstVisibleSlot;
        if (firstVisibleColumnIndex == -1 || firstVisibleSlot == -1)
        {
            return false;
        }

        if (WaitForLostFocus(() => ProcessLeftKey(shift, ctrl)))
        {
            return true;
        }

        int previousVisibleColumnIndex = -1;
        if (CurrentColumnIndex != -1)
        {
            dataGridColumn = ColumnsInternal.GetPreviousVisibleNonFillerColumn(ColumnsItemsInternal[CurrentColumnIndex]);
            if (dataGridColumn != null)
            {
                previousVisibleColumnIndex = dataGridColumn.Index;
            }
        }

        _noSelectionChangeCount++;
        try
        {
            if (ctrl)
            {
                return ProcessLeftMost(firstVisibleColumnIndex, firstVisibleSlot);
            }
            else
            {
                if (RowGroupHeadersTable.Contains(CurrentSlot))
                {
                    CollapseRowGroup(RowGroupHeadersTable.GetValueAt(CurrentSlot)!.CollectionViewGroup, collapseAllSubgroups: false);
                }
                else if (CurrentColumnIndex == -1)
                {
                    UpdateSelectionAndCurrency(
                        firstVisibleColumnIndex,
                        firstVisibleSlot,
                        DataGridSelectionAction.SelectCurrent,
                        scrollIntoView: true);
                }
                else
                {
                    if (previousVisibleColumnIndex == -1)
                    {
                        return true;
                    }

                    UpdateSelectionAndCurrency(
                        previousVisibleColumnIndex,
                        CurrentSlot,
                        DataGridSelectionAction.None,
                        scrollIntoView: true);
                }
            }
        }
        finally
        {
            NoSelectionChangeCount--;
        }
        return _successfullyUpdatedSelection;
    }

    // Ctrl Left <==> Home
    private bool ProcessLeftMost(int firstVisibleColumnIndex, int firstVisibleSlot)
    {
        _noSelectionChangeCount++;
        try
        {
            int                     desiredSlot;
            DataGridSelectionAction action;
            if (CurrentColumnIndex == -1)
            {
                desiredSlot = firstVisibleSlot;
                action      = DataGridSelectionAction.SelectCurrent;
                Debug.Assert(_selectedItems.Count == 0);
            }
            else
            {
                desiredSlot = CurrentSlot;
                action      = DataGridSelectionAction.None;
            }

            UpdateSelectionAndCurrency(firstVisibleColumnIndex, desiredSlot, action, scrollIntoView: true);
        }
        finally
        {
            NoSelectionChangeCount--;
        }
        return _successfullyUpdatedSelection;
    }

    private bool ProcessNextKey(bool shift, bool ctrl)
    {
        DataGridColumn? dataGridColumn          = ColumnsInternal.FirstVisibleNonFillerColumn;
        int            firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
        if (firstVisibleColumnIndex == -1 || DisplayData.FirstScrollingSlot == -1)
        {
            return false;
        }

        if (WaitForLostFocus(() => ProcessNextKey(shift, ctrl)))
        {
            return true;
        }

        int nextPageSlot = CurrentSlot == -1 ? DisplayData.FirstScrollingSlot : CurrentSlot;
        Debug.Assert(nextPageSlot != -1);
        int slot = GetNextVisibleSlot(nextPageSlot);

        int scrollCount = DisplayData.NumTotallyDisplayedScrollingElements;
        while (scrollCount > 0 && slot < SlotCount)
        {
            nextPageSlot = slot;
            scrollCount--;
            slot = GetNextVisibleSlot(slot);
        }

        _noSelectionChangeCount++;
        try
        {
            DataGridSelectionAction action;
            int                     columnIndex;
            if (CurrentColumnIndex == -1)
            {
                columnIndex = firstVisibleColumnIndex;
                action      = DataGridSelectionAction.SelectCurrent;
            }
            else
            {
                columnIndex = CurrentColumnIndex;
                action = (shift && SelectionMode == DataGridSelectionMode.Extended)
                    ? action = DataGridSelectionAction.SelectFromAnchorToCurrent
                    : action = DataGridSelectionAction.SelectCurrent;
            }

            UpdateSelectionAndCurrency(columnIndex, nextPageSlot, action, scrollIntoView: true);
        }
        finally
        {
            NoSelectionChangeCount--;
        }
        return _successfullyUpdatedSelection;
    }

    private bool ProcessPriorKey(bool shift, bool ctrl)
    {
        DataGridColumn? dataGridColumn          = ColumnsInternal.FirstVisibleNonFillerColumn;
        int            firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
        if (firstVisibleColumnIndex == -1 || DisplayData.FirstScrollingSlot == -1)
        {
            return false;
        }

        if (WaitForLostFocus(() => ProcessPriorKey(shift, ctrl)))
        {
            return true;
        }

        int previousPageSlot = (CurrentSlot == -1) ? DisplayData.FirstScrollingSlot : CurrentSlot;
        Debug.Assert(previousPageSlot != -1);

        int scrollCount = DisplayData.NumTotallyDisplayedScrollingElements;
        int slot        = GetPreviousVisibleSlot(previousPageSlot);
        while (scrollCount > 0 && slot != -1)
        {
            previousPageSlot = slot;
            scrollCount--;
            slot = GetPreviousVisibleSlot(slot);
        }
        Debug.Assert(previousPageSlot != -1);

        _noSelectionChangeCount++;
        try
        {
            int                     columnIndex;
            DataGridSelectionAction action;
            if (CurrentColumnIndex == -1)
            {
                columnIndex = firstVisibleColumnIndex;
                action      = DataGridSelectionAction.SelectCurrent;
            }
            else
            {
                columnIndex = CurrentColumnIndex;
                action = (shift && SelectionMode == DataGridSelectionMode.Extended)
                    ? DataGridSelectionAction.SelectFromAnchorToCurrent
                    : DataGridSelectionAction.SelectCurrent;
            }

            UpdateSelectionAndCurrency(columnIndex, previousPageSlot, action, scrollIntoView: true);
        }
        finally
        {
            NoSelectionChangeCount--;
        }
        return _successfullyUpdatedSelection;
    }

    private bool ProcessRightKey(bool shift, bool ctrl)
    {
        DataGridColumn? dataGridColumn         = ColumnsInternal.LastVisibleColumn;
        int            lastVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
        int            firstVisibleSlot       = FirstVisibleSlot;
        if (lastVisibleColumnIndex == -1 || firstVisibleSlot == -1)
        {
            return false;
        }

        if (WaitForLostFocus(delegate { ProcessRightKey(shift, ctrl); }))
        {
            return true;
        }

        int nextVisibleColumnIndex = -1;
        if (CurrentColumnIndex != -1)
        {
            dataGridColumn = ColumnsInternal.GetNextVisibleColumn(ColumnsItemsInternal[CurrentColumnIndex]);
            if (dataGridColumn != null)
            {
                nextVisibleColumnIndex = dataGridColumn.Index;
            }
        }
        _noSelectionChangeCount++;
        try
        {
            if (ctrl)
            {
                return ProcessRightMost(lastVisibleColumnIndex, firstVisibleSlot);
            }
            else
            {
                if (RowGroupHeadersTable.Contains(CurrentSlot))
                {
                    ExpandRowGroup(RowGroupHeadersTable.GetValueAt(CurrentSlot)!.CollectionViewGroup, expandAllSubgroups: false);
                }
                else if (CurrentColumnIndex == -1)
                {
                    int firstVisibleColumnIndex = ColumnsInternal.FirstVisibleColumn == null ? -1 : ColumnsInternal.FirstVisibleColumn.Index;

                    UpdateSelectionAndCurrency(
                        firstVisibleColumnIndex,
                        firstVisibleSlot,
                        DataGridSelectionAction.SelectCurrent,
                        scrollIntoView: true);
                }
                else
                {
                    if (nextVisibleColumnIndex == -1)
                    {
                        return true;
                    }

                    UpdateSelectionAndCurrency(
                        nextVisibleColumnIndex,
                        CurrentSlot,
                        DataGridSelectionAction.None,
                        scrollIntoView: true);
                }
            }
        }
        finally
        {
            NoSelectionChangeCount--;
        }
        return _successfullyUpdatedSelection;
    }

    // Ctrl Right <==> End
    private bool ProcessRightMost(int lastVisibleColumnIndex, int firstVisibleSlot)
    {
        _noSelectionChangeCount++;
        try
        {
            int                     desiredSlot;
            DataGridSelectionAction action;
            if (CurrentColumnIndex == -1)
            {
                desiredSlot = firstVisibleSlot;
                action      = DataGridSelectionAction.SelectCurrent;
            }
            else
            {
                desiredSlot = CurrentSlot;
                action      = DataGridSelectionAction.None;
            }

            UpdateSelectionAndCurrency(lastVisibleColumnIndex, desiredSlot, action, scrollIntoView: true);
        }
        finally
        {
            NoSelectionChangeCount--;
        }
        return _successfullyUpdatedSelection;
    }

    private bool ProcessTabKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out bool ctrl, out bool shift);
        return ProcessTabKey(e, shift, ctrl);
    }

    private bool ProcessTabKey(KeyEventArgs e, bool shift, bool ctrl)
    {
        if (ctrl || _editingColumnIndex == -1 || IsReadOnly)
        {
            //Go to the next/previous control on the page when
            // - Ctrl key is used
            // - Potential current cell is not edited, or the datagrid is read-only.
            return false;
        }

        // Try to locate a writable cell before/after the current cell
        Debug.Assert(CurrentColumnIndex != -1);
        Debug.Assert(CurrentSlot != -1);

        int            neighborVisibleWritableColumnIndex, neighborSlot;
        DataGridColumn? dataGridColumn;
        if (shift)
        {
            dataGridColumn = ColumnsInternal.GetPreviousVisibleWritableColumn(ColumnsItemsInternal[CurrentColumnIndex]);
            neighborSlot   = GetPreviousVisibleSlot(CurrentSlot);
            if (EditingRow != null)
            {
                while (neighborSlot != -1 && RowGroupHeadersTable.Contains(neighborSlot))
                {
                    neighborSlot = GetPreviousVisibleSlot(neighborSlot);
                }
            }
        }
        else
        {
            dataGridColumn = ColumnsInternal.GetNextVisibleWritableColumn(ColumnsItemsInternal[CurrentColumnIndex]);
            neighborSlot   = GetNextVisibleSlot(CurrentSlot);
            if (EditingRow != null)
            {
                while (neighborSlot < SlotCount && RowGroupHeadersTable.Contains(neighborSlot))
                {
                    neighborSlot = GetNextVisibleSlot(neighborSlot);
                }
            }
        }
        neighborVisibleWritableColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;

        if (neighborVisibleWritableColumnIndex == -1 && (neighborSlot == -1 || neighborSlot >= SlotCount))
        {
            // There is no previous/next row and no previous/next writable cell on the current row
            return false;
        }

        if (WaitForLostFocus(() => ProcessTabKey(e, shift, ctrl)))
        {
            return true;
        }

        int targetSlot = -1, targetColumnIndex = -1;

        _noSelectionChangeCount++;
        try
        {
            if (neighborVisibleWritableColumnIndex == -1)
            {
                targetSlot = neighborSlot;
                if (shift)
                {
                    Debug.Assert(ColumnsInternal.LastVisibleWritableColumn != null);
                    targetColumnIndex = ColumnsInternal.LastVisibleWritableColumn.Index;
                }
                else
                {
                    Debug.Assert(ColumnsInternal.FirstVisibleWritableColumn != null);
                    targetColumnIndex = ColumnsInternal.FirstVisibleWritableColumn.Index;
                }
            }
            else
            {
                targetSlot        = CurrentSlot;
                targetColumnIndex = neighborVisibleWritableColumnIndex;
            }

            DataGridSelectionAction action;
            if (targetSlot != CurrentSlot || (SelectionMode == DataGridSelectionMode.Extended))
            {
                if (IsSlotOutOfBounds(targetSlot))
                {
                    return true;
                }
                action = DataGridSelectionAction.SelectCurrent;
            }
            else
            {
                action = DataGridSelectionAction.None;
            }

            UpdateSelectionAndCurrency(targetColumnIndex, targetSlot, action, scrollIntoView: true);
        }
        finally
        {
            NoSelectionChangeCount--;
        }

        if (_successfullyUpdatedSelection && !RowGroupHeadersTable.Contains(targetSlot))
        {
            BeginCellEdit(e);
        }

        // Return true to say we handled the key event even if the operation was unsuccessful. If we don't
        // say we handled this event, the framework will continue to process the tab key and change focus.
        return true;
    }

    private bool ProcessUpKey(bool shift, bool ctrl)
    {
        DataGridColumn? dataGridColumn          = ColumnsInternal.FirstVisibleNonFillerColumn;
        int            firstVisibleColumnIndex = (dataGridColumn == null) ? -1 : dataGridColumn.Index;
        int            firstVisibleSlot        = FirstVisibleSlot;
        if (firstVisibleColumnIndex == -1 || firstVisibleSlot == -1)
        {
            return false;
        }

        if (WaitForLostFocus(() => ProcessUpKey(shift, ctrl)))
        {
            return true;
        }

        int previousVisibleSlot = (CurrentSlot != -1) ? GetPreviousVisibleSlot(CurrentSlot) : -1;

        _noSelectionChangeCount++;

        try
        {
            int                     slot;
            int                     columnIndex;
            DataGridSelectionAction action;
            if (CurrentColumnIndex == -1)
            {
                slot        = firstVisibleSlot;
                columnIndex = firstVisibleColumnIndex;
                action      = DataGridSelectionAction.SelectCurrent;
            }
            else if (ctrl)
            {
                if (shift)
                {
                    // Both Ctrl and Shift
                    slot        = firstVisibleSlot;
                    columnIndex = CurrentColumnIndex;
                    action = (SelectionMode == DataGridSelectionMode.Extended)
                        ? DataGridSelectionAction.SelectFromAnchorToCurrent
                        : DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    // Ctrl without Shift
                    slot        = firstVisibleSlot;
                    columnIndex = CurrentColumnIndex;
                    action      = DataGridSelectionAction.SelectCurrent;
                }
            }
            else
            {
                if (previousVisibleSlot == -1)
                {
                    return true;
                }
                if (shift)
                {
                    // Shift without Ctrl
                    slot        = previousVisibleSlot;
                    columnIndex = CurrentColumnIndex;
                    action      = DataGridSelectionAction.SelectFromAnchorToCurrent;
                }
                else
                {
                    // Neither Shift nor Ctrl
                    slot        = previousVisibleSlot;
                    columnIndex = CurrentColumnIndex;
                    action      = DataGridSelectionAction.SelectCurrent;
                }
            }
            UpdateSelectionAndCurrency(columnIndex, slot, action, scrollIntoView: true);
        }
        finally
        {
            NoSelectionChangeCount--;
        }
        return _successfullyUpdatedSelection;
    }

    private void RemoveDisplayedColumnHeader(DataGridColumn dataGridColumn)
    {
        if (_columnHeadersPresenter != null)
        {
            _columnHeadersPresenter.Children.Remove(dataGridColumn.HeaderCell);
        }
    }

    private void RemoveDisplayedColumnHeaders()
    {
        if (_columnHeadersPresenter != null)
        {
            _columnHeadersPresenter.Children.Clear();
        }
        Debug.Assert(ColumnsInternal.FillerColumn != null);
        ColumnsInternal.FillerColumn.IsRepresented = false;
    }

    private bool ResetCurrentCellCore()
    {
        return (CurrentColumnIndex == -1 || SetCurrentCellCore(-1, -1));
    }

    private void ResetEditingRow()
    {
        if (EditingRow != null
            && EditingRow != _focusedRow
            && !IsSlotVisible(EditingRow.Slot))
        {
            // Unload the old editing row if it's off screen
            EditingRow.Clip = null;
            UnloadRow(EditingRow);
            DisplayData.FullyRecycleElements();
        }
        EditingRow = null;
    }

    private void ResetFocusedRow()
    {
        if (_focusedRow != null
            && _focusedRow != EditingRow
            && !IsSlotVisible(_focusedRow.Slot))
        {
            // Unload the old focused row if it's off screen
            _focusedRow.Clip = null;
            UnloadRow(_focusedRow);
            DisplayData.FullyRecycleElements();
        }
        _focusedRow = null;
    }

    public void SelectAll()
    {
        SetRowsSelection(0, SlotCount - 1);
    }

    private void SetAndSelectCurrentCell(int columnIndex,
                                         int slot,
                                         bool forceCurrentCellSelection)
    {
        DataGridSelectionAction action = forceCurrentCellSelection ? DataGridSelectionAction.SelectCurrent : DataGridSelectionAction.None;
        UpdateSelectionAndCurrency(columnIndex, slot, action, scrollIntoView: false);
    }

    // columnIndex = 2, rowIndex = -1 --> current cell belongs to the 'new row'.
    // columnIndex = 2, rowIndex = 2 --> current cell is an inner cell
    // columnIndex = -1, rowIndex = -1 --> current cell is reset
    // columnIndex = -1, rowIndex = 2 --> Unexpected
    private bool SetCurrentCellCore(int columnIndex, int slot, bool commitEdit, bool endRowEdit)
    {
        Debug.Assert(columnIndex < ColumnsItemsInternal.Count);
        Debug.Assert(slot < SlotCount);
        Debug.Assert(columnIndex == -1 || ColumnsItemsInternal[columnIndex].IsVisible);
        Debug.Assert(!(columnIndex > -1 && slot == -1));

        if (columnIndex == CurrentColumnIndex &&
            slot == CurrentSlot)
        {
            Debug.Assert(DataConnection != null);
            Debug.Assert(_editingColumnIndex == -1 || _editingColumnIndex == CurrentColumnIndex);
            Debug.Assert(EditingRow == null || EditingRow.Slot == CurrentSlot || DataConnection.CommittingEdit);
            return true;
        }

        Control?                 oldDisplayedElement = null;
        DataGridCellCoordinates oldCurrentCell      = new DataGridCellCoordinates(CurrentCellCoordinates);

        object? newCurrentItem = null;
        if (!RowGroupHeadersTable.Contains(slot))
        {
            int rowIndex = RowIndexFromSlot(slot);
            if (rowIndex >= 0 && rowIndex < DataConnection.Count)
            {
                newCurrentItem = DataConnection.GetDataItem(rowIndex);
            }
        }

        if (CurrentColumnIndex > -1)
        {
            Debug.Assert(CurrentColumnIndex < ColumnsItemsInternal.Count);
            Debug.Assert(CurrentSlot < SlotCount);

            if (!IsInnerCellOutOfBounds(oldCurrentCell.ColumnIndex, oldCurrentCell.Slot) &&
                IsSlotVisible(oldCurrentCell.Slot))
            {
                oldDisplayedElement = DisplayData.GetDisplayedElement(oldCurrentCell.Slot);
            }

            if (!RowGroupHeadersTable.Contains(oldCurrentCell.Slot) && !_temporarilyResetCurrentCell)
            {
                bool keepFocus = ContainsFocus;
                if (commitEdit)
                {
                    if (!EndCellEdit(DataGridEditAction.Commit, exitEditingMode: true, keepFocus: keepFocus, raiseEvents: true))
                    {
                        return false;
                    }
                    // Resetting the current cell: setting it to (-1, -1) is not considered setting it out of bounds
                    if ((columnIndex != -1 && slot != -1 && IsInnerCellOutOfSelectionBounds(columnIndex, slot)) ||
                        IsInnerCellOutOfSelectionBounds(oldCurrentCell.ColumnIndex, oldCurrentCell.Slot))
                    {
                        return false;
                    }

                    if (endRowEdit && !EndRowEdit(DataGridEditAction.Commit, exitEditingMode: true, raiseEvents: true))
                    {
                        return false;
                    }
                }
                else
                {
                    CancelEdit(DataGridEditingUnit.Row, false);
                    ExitEdit(keepFocus);
                }
            }
        }

        if (newCurrentItem != null)
        {
            slot = SlotFromRowIndex(DataConnection.IndexOf(newCurrentItem));
        }
        if (slot == -1 && columnIndex != -1)
        {
            return false;
        }
        CurrentColumnIndex = columnIndex;
        CurrentSlot        = slot;

        if (_temporarilyResetCurrentCell)
        {
            if (columnIndex != -1)
            {
                _temporarilyResetCurrentCell = false;
            }
        }
        if (!_temporarilyResetCurrentCell && _editingColumnIndex != -1)
        {
            _editingColumnIndex = columnIndex;
        }

        if (oldDisplayedElement != null)
        {
            if (oldDisplayedElement is DataGridRow row)
            {
                // Don't reset the state of the current cell if we're editing it because that would put it in an invalid state
                UpdateCurrentState(oldDisplayedElement, oldCurrentCell.ColumnIndex, !(_temporarilyResetCurrentCell && row.IsEditing && _editingColumnIndex == oldCurrentCell.ColumnIndex));
            }
            else
            {
                UpdateCurrentState(oldDisplayedElement, oldCurrentCell.ColumnIndex, applyCellState: false);
            }
        }

        if (CurrentColumnIndex > -1)
        {
            Debug.Assert(CurrentSlot > -1);
            Debug.Assert(CurrentColumnIndex < ColumnsItemsInternal.Count);
            Debug.Assert(CurrentSlot < SlotCount);
            if (IsSlotVisible(CurrentSlot))
            {
                UpdateCurrentState(DisplayData.GetDisplayedElement(CurrentSlot), CurrentColumnIndex, applyCellState: true);
            }
        }

        return true;
    }

    private void SetVerticalOffset(double newVerticalOffset)
    {
        _verticalOffset = newVerticalOffset;
        if (_vScrollBar != null && !MathUtilities.AreClose(newVerticalOffset, _vScrollBar.Value))
        {
            _vScrollBar.Value = _verticalOffset;
        }
    }

    private void UpdateCurrentState(Control displayedElement, int columnIndex, bool applyCellState)
    {
        if (displayedElement is DataGridRow row)
        {
            if (AreRowHeadersVisible)
            {
                row.ApplyHeaderStatus();
            }
            DataGridCell cell = row.Cells[columnIndex];
            if (applyCellState)
            {
                cell.UpdatePseudoClasses();
            }
        }
        else if (displayedElement is DataGridRowGroupHeader groupHeader)
        {
            groupHeader.UpdatePseudoClasses();
            if (AreRowHeadersVisible)
            {
                groupHeader.ApplyHeaderStatus();
            }
        }
    }

    private void UpdateHorizontalScrollBar(bool needHorizScrollbar, bool forceHorizScrollbar, double totalVisibleWidth, double totalVisibleFrozenWidth, double cellsWidth)
    {
        if (_hScrollBar != null)
        {
            if (needHorizScrollbar || forceHorizScrollbar)
            {
                //          viewportSize
                //        v---v
                //|<|_____|###|>|
                //  ^     ^
                //  min   max

                // we want to make the relative size of the thumb reflect the relative size of the viewing area
                // viewportSize / (max + viewportSize) = cellsWidth / max
                // -> viewportSize = max * cellsWidth / (max - cellsWidth)

                // always zero
                _hScrollBar.Minimum = 0;
                if (needHorizScrollbar)
                {
                    // maximum travel distance -- not the total width
                    _hScrollBar.Maximum = totalVisibleWidth - cellsWidth;
                    Debug.Assert(totalVisibleFrozenWidth >= 0);
                    if (_frozenColumnScrollBarSpacer != null)
                    {
                        _frozenColumnScrollBarSpacer.Width = totalVisibleFrozenWidth;
                    }
                    Debug.Assert(_hScrollBar.Maximum >= 0);

                    // width of the scrollable viewing area
                    double viewPortSize = Math.Max(0, cellsWidth - totalVisibleFrozenWidth);
                    _hScrollBar.ViewportSize = viewPortSize;
                    _hScrollBar.LargeChange  = viewPortSize;
                    // The ScrollBar should be in sync with HorizontalOffset at this point.  There's a resize case
                    // where the ScrollBar will coerce an old value here, but we don't want that
                    if (!MathUtils.AreClose(_hScrollBar.Value, _horizontalOffset))
                    {
                        _hScrollBar.Value = _horizontalOffset;
                    }
                    _hScrollBar.IsEnabled = true;
                }
                else
                {
                    _hScrollBar.Maximum      = 0;
                    _hScrollBar.ViewportSize = 0;
                    _hScrollBar.IsEnabled    = false;
                }

                if (!_hScrollBar.IsVisible)
                {
                    // This will trigger a call to this method via Cells_SizeChanged for
                    _ignoreNextScrollBarsLayout = true;
                    // which no processing is needed.
                    _hScrollBar.IsVisible = true;
                    if (_hScrollBar.DesiredSize.Height == 0)
                    {
                        // We need to know the height for the rest of layout to work correctly so measure it now
                        _hScrollBar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                }
            }
            else
            {
                _hScrollBar.Maximum = 0;
                if (_hScrollBar.IsVisible)
                {
                    // This will trigger a call to this method via Cells_SizeChanged for
                    // which no processing is needed.
                    _hScrollBar.IsVisible       = false;
                    _ignoreNextScrollBarsLayout = true;
                }
            }
        }
    }

    private void UpdateVerticalScrollBar(bool needVertScrollbar, bool forceVertScrollbar, double totalVisibleHeight, double cellsHeight)
    {
        if (_vScrollBar != null)
        {
            if (needVertScrollbar || forceVertScrollbar)
            {
                //          viewportSize
                //        v---v
                //|<|_____|###|>|
                //  ^     ^
                //  min   max

                // we want to make the relative size of the thumb reflect the relative size of the viewing area
                // viewportSize / (max + viewportSize) = cellsWidth / max
                // -> viewportSize = max * cellsHeight / (totalVisibleHeight - cellsHeight)
                // ->              = max * cellsHeight / (totalVisibleHeight - cellsHeight)
                // ->              = max * cellsHeight / max
                // ->              = cellsHeight

                // always zero
                _vScrollBar.Minimum = 0;
                if (needVertScrollbar && !double.IsInfinity(cellsHeight))
                {
                    // maximum travel distance -- not the total height
                    _vScrollBar.Maximum = totalVisibleHeight - cellsHeight;
                    Debug.Assert(_vScrollBar.Maximum >= 0);

                    // total height of the display area
                    _vScrollBar.ViewportSize = cellsHeight;
                    _vScrollBar.IsEnabled    = true;
                }
                else
                {
                    _vScrollBar.Maximum      = 0;
                    _vScrollBar.ViewportSize = 0;
                    _vScrollBar.IsEnabled    = false;
                }

                if (!_vScrollBar.IsVisible)
                {
                    // This will trigger a call to this method via Cells_SizeChanged for
                    // which no processing is needed.
                    _vScrollBar.IsVisible = true;
                    if (_vScrollBar.DesiredSize.Width == 0)
                    {
                        // We need to know the width for the rest of layout to work correctly so measure it now
                        _vScrollBar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    _ignoreNextScrollBarsLayout = true;
                }
            }
            else
            {
                _vScrollBar.Maximum = 0;
                if (_vScrollBar.IsVisible)
                {
                    // This will trigger a call to this method via Cells_SizeChanged for
                    // which no processing is needed.
                    _vScrollBar.IsVisible       = false;
                    _ignoreNextScrollBarsLayout = true;
                }
            }
        }
    }

    private void HandleVerticalScrollBarScroll(object? sender, ScrollEventArgs e)
    {
        ProcessVerticalScroll(e.ScrollEventType);
        VerticalScroll?.Invoke(sender, e);
    }

    //TODO: Ensure right button is checked for
    private bool UpdateStateOnMouseRightButtonDown(PointerPressedEventArgs pointerPressedEventArgs, int columnIndex, int slot, bool allowEdit, bool shift, bool ctrl)
    {
        Debug.Assert(slot >= 0);

        if (shift || ctrl)
        {
            return true;
        }
        if (IsSlotOutOfBounds(slot))
        {
            return true;
        }
        if (GetRowSelection(slot))
        {
            return true;
        }
        // Unselect everything except the row that was clicked on
        _noSelectionChangeCount++;
        try
        {
            UpdateSelectionAndCurrency(columnIndex, slot, DataGridSelectionAction.SelectCurrent, scrollIntoView: false);
        }
        finally
        {
            NoSelectionChangeCount--;
        }
        return true;
    }

    //TODO: Ensure left button is checked for
    private bool UpdateStateOnMouseLeftButtonDown(PointerPressedEventArgs pointerPressedEventArgs, int columnIndex, int slot, bool allowEdit, bool shift, bool ctrl)
    {
        bool beginEdit;

        Debug.Assert(slot >= 0);

        // Before changing selection, check if the current cell needs to be committed, and
        // check if the current row needs to be committed. If any of those two operations are required and fail,
        // do not change selection, and do not change current cell.

        bool wasInEdit = EditingColumnIndex != -1;

        if (IsSlotOutOfBounds(slot))
        {
            return true;
        }

        if (wasInEdit && (columnIndex != EditingColumnIndex || slot != CurrentSlot) &&
            WaitForLostFocus(() => UpdateStateOnMouseLeftButtonDown(pointerPressedEventArgs, columnIndex, slot, allowEdit, shift, ctrl)))
        {
            return true;
        }

        try
        {
            _noSelectionChangeCount++;

            beginEdit = allowEdit &&
                        CurrentSlot == slot &&
                        columnIndex != -1 &&
                        (wasInEdit || CurrentColumnIndex == columnIndex) &&
                        !GetColumnEffectiveReadOnlyState(ColumnsItemsInternal[columnIndex]);

            DataGridSelectionAction action;
            if (SelectionMode == DataGridSelectionMode.Extended && shift)
            {
                // Shift select multiple rows
                action = DataGridSelectionAction.SelectFromAnchorToCurrent;
            }
            else if (GetRowSelection(slot))  // Unselecting single row or Selecting a previously multi-selected row
            {
                if (!ctrl && SelectionMode == DataGridSelectionMode.Extended && _selectedItems.Count != 0)
                {
                    // Unselect everything except the row that was clicked on
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else if (ctrl && EditingRow == null)
                {
                    action = DataGridSelectionAction.RemoveCurrentFromSelection;
                }
                else
                {
                    action = DataGridSelectionAction.None;
                }
            }
            else // Selecting a single row or multi-selecting with Ctrl
            {
                if (SelectionMode == DataGridSelectionMode.Single || !ctrl)
                {
                    // Unselect the currently selected rows except the new selected row
                    action = DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    action = DataGridSelectionAction.AddCurrentToSelection;
                }
            }

            UpdateSelectionAndCurrency(columnIndex, slot, action, scrollIntoView: false);
        }
        finally
        {
            NoSelectionChangeCount--;
        }

        if (_successfullyUpdatedSelection && beginEdit && BeginCellEdit(pointerPressedEventArgs))
        {
            FocusEditingCell(setFocus: true);
        }

        return true;
    }

    /// <summary>
    /// Returns the Group at the indicated level or null if the item is not in the ItemsSource
    /// </summary>
    /// <param name="item">item</param>
    /// <param name="groupLevel">groupLevel</param>
    /// <returns>The group the given item falls under or null if the item is not in the ItemsSource</returns>
    public DataGridCollectionViewGroup? GetGroupFromItem(object item, int groupLevel)
    {
        int itemIndex = DataConnection.IndexOf(item);
        if (itemIndex == -1)
        {
            return null;
        }
        int                  groupHeaderSlot = RowGroupHeadersTable.GetPreviousIndex(SlotFromRowIndex(itemIndex));
        DataGridRowGroupInfo? rowGroupInfo    = RowGroupHeadersTable.GetValueAt(groupHeaderSlot);
        while (rowGroupInfo != null && rowGroupInfo.Level != groupLevel)
        {
            groupHeaderSlot = RowGroupHeadersTable.GetPreviousIndex(rowGroupInfo.Slot);
            rowGroupInfo    = RowGroupHeadersTable.GetValueAt(groupHeaderSlot);
        }
        return rowGroupInfo?.CollectionViewGroup;
    }

    /// <summary>
    /// Raises the LoadingRowGroup event
    /// </summary>
    /// <param name="e">EventArgs</param>
    protected virtual void OnLoadingRowGroup(DataGridRowGroupHeaderEventArgs e)
    {
        EventHandler<DataGridRowGroupHeaderEventArgs>? handler = LoadingRowGroup;
        if (handler != null)
        {
            LoadingOrUnloadingRow = true;
            handler(this, e);
            LoadingOrUnloadingRow = false;
        }
    }

    /// <summary>
    /// Raises the UnLoadingRowGroup event
    /// </summary>
    /// <param name="e">EventArgs</param>
    protected virtual void OnUnloadingRowGroup(DataGridRowGroupHeaderEventArgs e)
    {
        EventHandler<DataGridRowGroupHeaderEventArgs>? handler = UnloadingRowGroup;
        if (handler != null)
        {
            LoadingOrUnloadingRow = true;
            handler(this, e);
            LoadingOrUnloadingRow = false;
        }
    }

    // Recursively expands parent RowGroupHeaders from the top down
    private void ExpandRowGroupParentChain(int level, int slot)
    {
        if (level < 0)
        {
            return;
        }
        int                  previousHeaderSlot = RowGroupHeadersTable.GetPreviousIndex(slot + 1);
        DataGridRowGroupInfo? rowGroupInfo       = null;
        while (previousHeaderSlot >= 0)
        {
            rowGroupInfo = RowGroupHeadersTable.GetValueAt(previousHeaderSlot);
            Debug.Assert(rowGroupInfo != null);
            if (level == rowGroupInfo.Level)
            {
                if (_collapsedSlotsTable.Contains(rowGroupInfo.Slot))
                {
                    // Keep going up the chain
                    ExpandRowGroupParentChain(level - 1, rowGroupInfo.Slot - 1);
                }
                if (!rowGroupInfo.IsVisible)
                {
                    EnsureRowGroupVisibility(rowGroupInfo, true, false);
                }
                return;
            }
            previousHeaderSlot = RowGroupHeadersTable.GetPreviousIndex(previousHeaderSlot);
        }
    }

    /// <summary>
    /// This method raises the CopyingRowClipboardContent event.
    /// </summary>
    /// <param name="e">Contains the necessary information for generating the row clipboard content.</param>
    protected virtual void OnCopyingRowClipboardContent(DataGridRowClipboardEventArgs e)
    {
        CopyingRowClipboardContent?.Invoke(this, e);
    }

    /// <summary>
    /// This method formats a row (specified by a DataGridRowClipboardEventArgs) into
    /// a single string to be added to the Clipboard when the DataGrid is copying its contents.
    /// </summary>
    /// <param name="e">DataGridRowClipboardEventArgs</param>
    /// <returns>The formatted string.</returns>
    private string FormatClipboardContent(DataGridRowClipboardEventArgs e)
    {
        var text                = StringBuilderCache.Acquire();
        var clipboardRowContent = e.ClipboardRowContent;
        var numberOfItem        = clipboardRowContent.Count;
        for (int cellIndex = 0; cellIndex < numberOfItem; cellIndex++)
        {
            var cellContent = clipboardRowContent[cellIndex].Content?.ToString();
            cellContent = cellContent?.Replace("\"", "\"\"");
            text.Append($"\"{cellContent}\"");
            if (cellIndex < numberOfItem - 1)
            {
                text.Append('\t');
            }
            else
            {
                text.Append('\r');
                text.Append('\n');
            }
        }
        return StringBuilderCache.GetStringAndRelease(text);
    }

    /// <summary>
    /// Handles the case where a 'Copy' key ('C' or 'Insert') has been pressed.  If pressed in combination with
    /// the control key, and the necessary prerequisites are met, the DataGrid will copy its contents
    /// to the Clipboard as text.
    /// </summary>
    /// <returns>Whether or not the DataGrid handled the key press.</returns>
    private bool ProcessCopyKey(KeyModifiers modifiers)
    {
        KeyboardHelper.GetMetaKeyState(this, modifiers, out bool ctrl, out bool shift, out bool alt);

        if (ctrl && !shift && !alt && ClipboardCopyMode != DataGridClipboardCopyMode.None && SelectedItems.Count > 0)
        {
            var textBuilder = StringBuilderCache.Acquire();

            if (ClipboardCopyMode == DataGridClipboardCopyMode.IncludeHeader)
            {
                DataGridRowClipboardEventArgs headerArgs = new DataGridRowClipboardEventArgs(null, true);
                foreach (DataGridColumn column in ColumnsInternal.GetVisibleColumns())
                {
                    headerArgs.ClipboardRowContent.Add(new DataGridClipboardCellContent(null, column, column.Header));
                }
                OnCopyingRowClipboardContent(headerArgs);
                textBuilder.Append(FormatClipboardContent(headerArgs));
            }

            for (int index = 0; index < SelectedItems.Count; index++)
            {
                var                        item     = SelectedItems[index];
                Debug.Assert(item != null);
                DataGridRowClipboardEventArgs itemArgs = new DataGridRowClipboardEventArgs(item, false);
                foreach (DataGridColumn column in ColumnsInternal.GetVisibleColumns())
                {
                    object? content = column.GetCellValue(item, column.ClipboardContentBinding);
                    itemArgs.ClipboardRowContent.Add(new DataGridClipboardCellContent(item, column, content));
                }
                OnCopyingRowClipboardContent(itemArgs);
                textBuilder.Append(FormatClipboardContent(itemArgs));
            }

            string text = StringBuilderCache.GetStringAndRelease(textBuilder);

            if (!string.IsNullOrEmpty(text))
            {
                CopyToClipboard(text);
                return true;
            }
        }
        return false;
    }

    private async void CopyToClipboard(string text)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;

        if (clipboard != null)
            await clipboard.SetTextAsync(text);
    }

    /// <summary>
    /// This is an empty content control that's used during the DataGrid's copy procedure
    /// to determine the value of a ClipboardContentBinding for a particular column and item.
    /// </summary>
    internal ContentControl ClipboardContentControl
    {
        get
        {
            if (_clipboardContentControl == null)
            {
                _clipboardContentControl = new ContentControl();
            }
            return _clipboardContentControl;
        }
    }

    //TODO Validation UI
    private void ResetValidationStatus()
    {
        // Clear the invalid status of the Cell, Row and DataGrid
        if (EditingRow != null)
        {
            EditingRow.IsValid = true;
            if (EditingRow.Index != -1)
            {
                foreach (DataGridCell cell in EditingRow.Cells)
                {
                    if (!cell.IsValid)
                    {
                        cell.IsValid = true;
                        cell.UpdatePseudoClasses();
                    }
                }
                EditingRow.ApplyState();
            }
        }
        IsValid = true;

        _validationSubscription?.Dispose();
        _validationSubscription = null;
    }

    /// <summary>
    /// Raises the AutoGeneratingColumn event.
    /// </summary>
    protected virtual void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e)
    {
        AutoGeneratingColumn?.Invoke(this, e);
    }
}