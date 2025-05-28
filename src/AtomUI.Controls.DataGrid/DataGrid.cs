// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using AtomUI.Controls.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;

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
    
    public static readonly StyledProperty<IBrush> VerticalGridLinesBrushProperty =
        AvaloniaProperty.Register<DataGrid, IBrush>(nameof(VerticalGridLinesBrush));
    
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
    
    public static readonly StyledProperty<IDataTemplate> RowDetailsTemplateProperty =
        AvaloniaProperty.Register<DataGrid, IDataTemplate>(nameof(RowDetailsTemplate));
    
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
    public IBrush VerticalGridLinesBrush
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
    public IDataTemplate RowDetailsTemplate
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

    #endregion

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

        UpdatePseudoClasses();
    }
}