// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AtomUI.Desktop.Controls.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public abstract partial class DataGridColumn : AvaloniaObject
{
    #region 公共属性定义

    public static readonly StyledProperty<DataGridLength> WidthProperty = 
        AvaloniaProperty.Register<DataGridColumn, DataGridLength>(nameof(Width), coerce: CoerceWidth);
    
    /// <summary>
    /// Defines the <see cref="IsVisible"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        Control.IsVisibleProperty.AddOwner<DataGridColumn>();
    
    /// <summary>
    /// Backing field for CellTheme property.
    /// </summary>
    public static readonly DirectProperty<DataGridColumn, ControlTheme?> CellThemeProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumn, ControlTheme?>(
            nameof(CellTheme),
            o => o.CellTheme,
            (o, v) => o.CellTheme = v);
    
    /// <summary>
    /// Backing field for Header property
    /// </summary>
    public static readonly DirectProperty<DataGridColumn, object?> HeaderProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumn, object?>(
            nameof(Header),
            o => o.Header,
            (o, v) => o.Header = v);
    
    /// <summary>
    /// Backing field for Header property
    /// </summary>
    public static readonly DirectProperty<DataGridColumn, IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumn, IDataTemplate?>(
            nameof(HeaderTemplate),
            o => o.HeaderTemplate,
            (o, v) => o.HeaderTemplate = v);
    
    /// <summary>
    /// Supported sorting directions
    /// </summary>
    public static readonly StyledProperty<DataGridSortDirections> SupportedSortDirectionsProperty =
        AvaloniaProperty.Register<DataGridColumn, DataGridSortDirections>(nameof(SupportedSortDirections), DataGridSortDirections.All);
    
    /// <summary>
    /// Horizontal alignment method for Header content
    /// </summary>
    public static readonly StyledProperty<HorizontalAlignment> HeaderContentHorizontalAlignmentProperty = 
        AvaloniaProperty.Register<DataGridColumn, HorizontalAlignment>(nameof (HeaderContentHorizontalAlignment), HorizontalAlignment.Left);
    
    /// <summary>
    /// How to vertically align the content of the Header
    /// </summary>
    public static readonly StyledProperty<VerticalAlignment> HeaderContentVerticalAlignmentProperty =
        AvaloniaProperty.Register<DataGridColumn, VerticalAlignment>(nameof (HeaderContentVerticalAlignment), VerticalAlignment.Center);
    
    /// <summary>
    /// Determines whether or not this column is visible.
    /// </summary>
    public bool IsVisible
    {
        get => GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }
    
    public DataGridLength Width
    {
        get => GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the <see cref="DataGridColumnHeader"/> cell theme.
    /// </summary>
    public ControlTheme? CellTheme
    {
        get => _cellTheme;
        set => SetAndRaise(CellThemeProperty, ref _cellTheme, value);
    }
    
    /// <summary>
    /// Gets or sets the <see cref="DataGridColumnHeader"/> content
    /// </summary>
    public object? Header
    {
        get => _header;
        set => SetAndRaise(HeaderProperty, ref _header, value);
    }
    
    /// <summary>
    /// Gets or sets an <see cref="IDataTemplate"/> for the <see cref="Header"/>
    /// </summary>
    public IDataTemplate? HeaderTemplate
    {
        get => _headerTemplate;
        set => SetAndRaise(HeaderTemplateProperty, ref _headerTemplate, value);
    }
    
    public DataGridSortDirections SupportedSortDirections
    {
        get => GetValue(SupportedSortDirectionsProperty);
        set => SetValue(SupportedSortDirectionsProperty, value);
    }
    
    public HorizontalAlignment HeaderContentHorizontalAlignment
    {
        get => GetValue(HeaderContentHorizontalAlignmentProperty);
        set => SetValue(HeaderContentHorizontalAlignmentProperty, value);
    }
    
    public VerticalAlignment HeaderContentVerticalAlignment
    {
        get => GetValue(HeaderContentVerticalAlignmentProperty);
        set => SetValue(HeaderContentVerticalAlignmentProperty, value);
    }

    /// <summary>
    /// Actual visible width after Width, MinWidth, and MaxWidth setting at the Column level and DataGrid level
    /// have been taken into account
    /// </summary>
    public double ActualWidth
    {
        get
        {
            if (OwningGrid == null || double.IsNaN(Width.DisplayValue))
            {
                return ActualMinWidth;
            }
            return Width.DisplayValue;
        }
    }
    
    public double MaxWidth
    {
        get => _maxWidth ?? double.PositiveInfinity;
        set
        {
            if (value < 0)
            {
                throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MaxWidth", 0);
            }
            if (value < ActualMinWidth)
            {
                throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MaxWidth", "MinWidth");
            }
            if (!_maxWidth.HasValue || !MathUtils.AreClose(_maxWidth.Value, value))
            {
                double oldValue = ActualMaxWidth;
                _maxWidth = value;
                if (OwningGrid != null)
                {
                    OwningGrid.HandleColumnMaxWidthChanged(this, oldValue);
                }
            }
        }
    }

    public double MinWidth
    {
        get => _minWidth ?? 0;
        set
        {
            if (double.IsNaN(value))
            {
                throw DataGridError.DataGrid.ValueCannotBeSetToNAN("MinWidth");
            }
            if (value < 0)
            {
                throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "MinWidth", 0);
            }
            if (double.IsPositiveInfinity(value))
            {
                throw DataGridError.DataGrid.ValueCannotBeSetToInfinity("MinWidth");
            }
            if (value > ActualMaxWidth)
            {
                throw DataGridError.DataGrid.ValueMustBeLessThanOrEqualTo("value", "MinWidth", "MaxWidth");
            }
            if (!_minWidth.HasValue || !MathUtils.AreClose(_minWidth.Value, value))
            {
                double oldValue = ActualMinWidth;
                _minWidth = value;
                if (OwningGrid != null)
                {
                    OwningGrid.HandleColumnMinWidthChanged(this, oldValue);
                }
            }
        }
    }
    
    /// <summary>
    /// Gets or sets a value that indicates whether the user can change the column display position by
    /// dragging the column header.
    /// </summary>
    /// <returns>
    /// true if the user can drag the column header to a new position; otherwise, false. The default is the current <see cref="P:AtomUI.Desktop.Controls.DataGrid.CanUserReorderColumns" /> property value.
    /// </returns>
    public bool CanUserReorder
    {
        get => CanUserReorderInternal ??
               OwningGrid?.CanUserReorderColumns ??
               DataGrid.DefaultCanUserReorderColumns;
        set =>  CanUserReorderInternal = value;
    }
    
    /// <summary>
    /// Gets or sets a value that indicates whether the user can adjust the column width using the mouse.
    /// </summary>
    /// <returns>
    /// true if the user can resize the column; false if the user cannot resize the column. The default is the current <see cref="P:AtomUI.Desktop.Controls.DataGrid.CanUserResizeColumns" /> property value.
    /// </returns>
    public bool CanUserResize
    {
        get => CanUserResizeInternal ??
               OwningGrid?.CanUserResizeColumns ??
               DataGrid.DefaultCanUserResizeColumns;
        set
        {
            CanUserResizeInternal = value;
            OwningGrid?.HandleColumnCanUserResizeChanged(this);
        }
    }

    public bool CanUserFilter
    {
        get
        {
            var canUserFilter = CanUserFilterInternal ??
                                OwningGrid?.CanUserFilterColumns ??
                                DataGrid.DefaultCanUserFilterColumns;
            if (HasHeaderCell)
            {
                HeaderCell.CanUserFilter = canUserFilter;
            }
            return canUserFilter;
        }
        set
        {
            CanUserFilterInternal = value;
            if (HasHeaderCell)
            {
                HeaderCell.CanUserFilter = value;
            }
        }
    }

    public bool ShowSorterTooltip
    {
        get
        {
            var showSorterTooltip = ShowSorterTooltipInternal ??
                                    OwningGrid?.ShowSorterTooltip ??
                                    DataGrid.DefaultShowSorterTooltip;
            if (HasHeaderCell)
            {
                HeaderCell.ShowSorterTooltip = showSorterTooltip;
            }
            return showSorterTooltip;
        }
        
        set
        {
            ShowSorterTooltipInternal = value;
            if (HasHeaderCell)
            {
                HeaderCell.ShowSorterTooltip = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the user can sort the column by clicking the column header.
    /// </summary>
    /// <returns>
    /// true if the user can sort the column; false if the user cannot sort the column. The default is the current <see cref="P:AtomUI.Desktop.Controls.DataGrid.CanUserSortColumns" /> property value.
    /// </returns>
    public bool CanUserSort
    {
        get
        {
            var canUserSort = false;
            if (CanUserSortInternal.HasValue)
            {
                canUserSort = CanUserSortInternal.Value;
            } 
            else if (OwningGrid != null && OwningGrid.CanUserSortColumns)
            {
                string? propertyPath = GetSortPropertyName();
                Type?  propertyType = OwningGrid.DataConnection.DataType.GetNestedPropertyType(propertyPath);
            
                // if the type is nullable, then we will compare the non-nullable type
                if (propertyType != null && propertyType.IsNullableType())
                {
                    propertyType = TypeHelper.GetNonNullableType(propertyType);
                }
            
                // return whether or not the property type can be compared
                canUserSort = typeof(IComparable).IsAssignableFrom(propertyType);
            }
            else
            {
                canUserSort = DataGrid.DefaultCanUserSortColumns;
            }

            if (HasHeaderCell)
            {
                HeaderCell.CanUserSort = canUserSort;
            }
            return canUserSort;
        }
        set
        {
            CanUserSortInternal = value;
            if (HasHeaderCell)
            {
                HeaderCell.CanUserSort = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the display position of the column relative to the other columns in the <see cref="T:AtomUI.Desktop.Controls.DataGrid" />.
    /// </summary>
    /// <returns>
    /// The zero-based position of the column as it is displayed in the associated <see cref="T:AtomUI.Desktop.Controls.DataGrid" />. The default is the index of the corresponding <see cref="P:System.Collections.ObjectModel.Collection`1.Item(System.Int32)" /> in the <see cref="P:AtomUI.Desktop.Controls.DataGrid.Columns" /> collection.
    /// </returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// When setting this property, the specified value is less than -1 or equal to <see cref="F:System.Int32.MaxValue" />.
    ///
    /// -or-
    ///
    /// When setting this property on a column in a <see cref="T:AtomUI.Desktop.Controls.DataGrid" />, the specified value is less than zero or greater than or equal to the number of columns in the <see cref="T:AtomUI.Desktop.Controls.DataGrid" />.
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// When setting this property, the <see cref="T:AtomUI.Desktop.Controls.DataGrid" /> is already making <see cref="P:AtomUI.Desktop.Controls.DataGridColumn.DisplayIndex" /> adjustments. For example, this exception is thrown when you attempt to set <see cref="P:AtomUI.Desktop.Controls.DataGridColumn.DisplayIndex" /> in a <see cref="E:Avalonia.Controls.DataGrid.ColumnDisplayIndexChanged" /> event handler.
    ///
    /// -or-
    ///
    /// When setting this property, the specified value would result in a frozen column being displayed in the range of unfrozen columns, or an unfrozen column being displayed in the range of frozen columns.
    /// </exception>
    public int DisplayIndex
    {
        get
        {
            if (OwningGrid != null && 
                OwningGrid.ColumnsInternal.RowGroupSpacerColumn != null && 
                OwningGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented)
            {
                return _displayIndexWithFiller - 1;
            }
            return _displayIndexWithFiller;
        }
        set
        {
            if (value == Int32.MaxValue)
            {
                throw DataGridError.DataGrid.ValueMustBeLessThan(nameof(value), nameof(DisplayIndex), Int32.MaxValue);
            }
            if (OwningGrid != null)
            {
                if (OwningGrid.ColumnsInternal.RowGroupSpacerColumn != null && OwningGrid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented)
                {
                    value++;
                }
                if (_displayIndexWithFiller != value)
                {
                    if (value < 0 || value >= OwningGrid.ColumnsItemsInternal.Count)
                    {
                        throw DataGridError.DataGrid.ValueMustBeBetween(nameof(value), nameof(DisplayIndex), 0, true, OwningGrid.Columns.Count, false);
                    }
                    // Will throw an error if a visible frozen column is placed inside a non-frozen area or vice-versa.
                    OwningGrid.HandleColumnDisplayIndexChanging(this, value);
                    _displayIndexWithFiller = value;
                    try
                    {
                        OwningGrid.InDisplayIndexAdjustments = true;
                        OwningGrid.HandleColumnDisplayIndexChanged(this);
                        OwningGrid.HandleColumnDisplayIndexChangedPostNotification();
                    }
                    finally
                    {
                        OwningGrid.InDisplayIndexAdjustments = false;
                    }
                }
            }
            else
            {
                if (value < -1)
                {
                    throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo(nameof(value), nameof(DisplayIndex), -1);
                }
                _displayIndexWithFiller = value;
            }
        }
    }

    public Classes CellStyleClasses => _cellStyleClasses ??= new();
    
    /// <summary>
    /// The binding that will be used to get or set cell content for the clipboard.
    /// </summary>
    public virtual IBinding? ClipboardContentBinding { get; set; }
    
    /// <summary>
    /// Holds the name of the member to use for sorting, if not using the default.
    /// </summary>
    public string? SortMemberPath { get; set; }

    /// <summary>
    /// Gets or sets an object associated with this column.
    /// </summary>
    public object? Tag { get; set; }

    /// <summary>
    /// Holds a Comparer to use for sorting, if not using the default.
    /// </summary>
    public IComparer? CustomSortComparer { get; set; }
    
    /// <summary>
    /// Holds the name of the member to use for filter, if not using the default.
    /// </summary>
    public string? FilterMemberPath { get; set; }
    
    /// <summary>
    /// Filter menu config
    /// </summary>
    public ObservableCollection<DataGridFilterItem> Filters { get; } = new();
    
    /// <summary>
    /// Custom filter, the first parameter is the filter condition, the second parameter is the data record
    /// </summary>
    public Func<object, object, bool>? OnFilter { get; set; }
    
    /// <summary>
    /// Whether multiple filters can be selected
    /// </summary>
    public DataGridFilterMode FilterMode { get; set; }
    
    /// <summary>
    /// Whether multiple filters can be selected
    /// </summary>
    public bool FilterMultiple { get; set; } = true;
    
    /// <summary>
    /// Whether to trigger filter when the filter menu closes
    /// </summary>
    public bool FilterOnClose { get; set; }
    
    public virtual bool IsReadOnly
    {
        get
        {
            if (OwningGrid == null)
            {
                return _isReadOnly ?? DefaultIsReadOnly;
            }
            if (_isReadOnly != null)
            {
                return _isReadOnly.Value || OwningGrid.IsReadOnly;
            }
            return OwningGrid.GetColumnReadOnlyState(this, DefaultIsReadOnly);
        }
        set
        {
            if (value != _isReadOnly)
            {
                OwningGrid?.HandleColumnReadOnlyStateChanging(this, value);
                _isReadOnly = value;
            }
        }
    }
    
    public bool IsAutoGenerated { get; internal set; }

    public bool IsFrozen
    {
        get => IsLeftFrozen || IsRightFrozen;
        set  {
            IsLeftFrozen  = value;
            IsRightFrozen = value;
        }
    }
    
    public bool IsLeftFrozen { get; internal set; }
    
    public bool IsRightFrozen { get; internal set; }
    
    #endregion

    #region 公共事件定义

    /// <summary>
    /// Occurs when the pointer is pressed over the column's header
    /// </summary>
    public event EventHandler<PointerPressedEventArgs>? HeaderPointerPressed;
    /// <summary>
    /// Occurs when the pointer is released over the column's header
    /// </summary>
    public event EventHandler<PointerReleasedEventArgs>? HeaderPointerReleased;

    #endregion
    
    #region 继承属性定义
    
    private DataGrid? _owningGrid;
    protected internal DataGrid? OwningGrid
    {
        get => _owningGrid;
        internal set
        {
            _owningGrid = value;
            NotifyOwningGridAttached(_owningGrid);
        }
    }
    #endregion
    
    /// <summary>
    /// Initializes a new instance of the <see cref="T:AtomUI.Desktop.Controls.DataGridColumn" /> class.
    /// </summary>
    protected internal DataGridColumn()
    {
        _displayIndexWithFiller         = -1;
        IsInitialDesiredWidthDetermined = false;
        InheritsWidth                   = true;
    }

    protected virtual void NotifyOwningGridAttached(DataGrid? owningGrid)
    {
    }
    
    protected internal virtual void NotifyOwningGridAboutToDetached()
    {
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsVisibleProperty)
        {
            OwningGrid?.HandleColumnVisibleStateChanging(this);
            var isVisible = change.GetNewValue<bool>();
        
            if (_headerCell != null)
            {
                _headerCell.IsVisible = isVisible;
            }
        
            OwningGrid?.HandleColumnVisibleStateChanged(this);
            NotifyPropertyChanged(change.Property.Name);
        }
        else if (change.Property == WidthProperty)
        {
            if (!_settingWidthInternally)
            {
                InheritsWidth = false;
            }
            if (_setWidthInternalNoCallback == false)
            {
                var grid  = OwningGrid;
                if (change is AvaloniaPropertyChangedEventArgs<DataGridLength> dataGridLengthChange)
                {
                    var width = dataGridLengthChange.NewValue.Value;
                    if (grid != null)
                    {
                        var oldWidth = dataGridLengthChange.OldValue.Value;
                        if (width.IsStar != oldWidth.IsStar)
                        {
                            SetWidthInternalNoCallback(width);
                            IsInitialDesiredWidthDetermined = false;
                            grid.HandleColumnWidthChanged(this);
                        }
                        else
                        {
                            Resize(oldWidth, width, false);
                        }
                    }
                    else
                    {
                        SetWidthInternalNoCallback(width);
                    }
                }
                
            }
        }
    }
    
    public Control? GetCellContent(DataGridRow dataGridRow)
    {
        dataGridRow = dataGridRow ?? throw new ArgumentNullException(nameof(dataGridRow));
        if (OwningGrid == null)
        {
            throw DataGridError.DataGrid.NoOwningGrid(GetType());
        }
        if (dataGridRow.OwningGrid == OwningGrid)
        {
            DataGridCell dataGridCell = dataGridRow.Cells[Index];
            return dataGridCell.Content as Control;
        }
        return null;
    }

    public Control? GetCellContent(object dataItem)
    {
        dataItem = dataItem ?? throw new ArgumentNullException(nameof(dataItem));
        if (OwningGrid == null)
        {
            throw DataGridError.DataGrid.NoOwningGrid(GetType());
        }
        DataGridRow? dataGridRow = OwningGrid.GetRowFromItem(dataItem);
        if (dataGridRow == null)
        {
            return null;
        }
        return GetCellContent(dataGridRow);
    }

    /// <summary>
    /// Returns the column which contains the given element
    /// </summary>
    /// <param name="element">element contained in a column</param>
    /// <returns>Column that contains the element, or null if not found
    /// </returns>
    public static DataGridColumn? GetColumnContainingElement(Control element)
    {
        // Walk up the tree to find the DataGridCell or DataGridColumnHeader that contains the element
        Visual? parent = element;
        while (parent != null)
        {
            if (parent is DataGridCell cell)
            {
                return cell.OwningColumn;
            }
            if (parent is DataGridColumnHeader columnHeader)
            {
                return columnHeader.OwningColumn;
            }
            parent = parent.GetVisualParent();
        }
        return null;
    }

    /// <summary>
    /// Clears the current sort direction
    /// </summary>
    public void ClearSort()
    {
        //InvokeProcessSort is already validating if sorting is possible
        _headerCell?.InvokeClearSort();
    }
    
    /// <summary>
    /// Changes the sort direction of this column
    /// </summary>
    /// <param name="direction">New sort direction</param>
    public void Sort(ListSortDirection? direction = null)
    {
        //InvokeProcessSort is already validating if sorting is possible
        _headerCell?.InvokeProcessSort(KeyModifiers.None, direction);
    }

    /// <summary>
    /// Clears the current filter
    /// </summary>
    public void ClearFilter()
    {
        _headerCell?.InvokeClearFilter();
    }

    public void Filter(List<object> filterValues)
    {
        _headerCell?.InvokeProcessFilter(filterValues);
    }

    /// <summary>
    /// When overridden in a derived class, causes the column cell being edited to revert to the unedited value.
    /// </summary>
    /// <param name="editingElement">
    /// The element that the column displays for a cell in editing mode.
    /// </param>
    /// <param name="uneditedValue">
    /// The previous, unedited value in the cell being edited.
    /// </param>
    protected virtual void CancelCellEdit(Control editingElement, object uneditedValue)
    { }

    /// <summary>
    /// When overridden in a derived class, gets an editing element that is bound to the column's <see cref="P:AtomUI.Desktop.Controls.DataGridBoundColumn.Binding" /> property value.
    /// </summary>
    /// <param name="cell">
    /// The cell that will contain the generated element.
    /// </param>
    /// <param name="dataItem">
    /// The data item represented by the row that contains the intended cell.
    /// </param>
    /// <param name="binding">When the method returns, contains the applied binding.</param>
    /// <returns>
    /// A new editing element that is bound to the column's <see cref="P:AtomUI.Desktop.Controls.DataGridBoundColumn.Binding" /> property value.
    /// </returns>
    protected abstract Control? GenerateEditingElement(DataGridCell cell, object dataItem, out ICellEditBinding? binding);

    /// <summary>
    /// When overridden in a derived class, gets a read-only element that is bound to the column's
    /// <see cref="P:AtomUI.Desktop.Controls.DataGridBoundColumn.Binding" /> property value.
    /// </summary>
    /// <param name="cell">
    /// The cell that will contain the generated element.
    /// </param>
    /// <param name="dataItem">
    /// The data item represented by the row that contains the intended cell.
    /// </param>
    /// <returns>
    /// A new, read-only element that is bound to the column's <see cref="P:AtomUI.Desktop.Controls.DataGridBoundColumn.Binding" /> property value.
    /// </returns>
    protected abstract Control? GenerateElement(DataGridCell cell, object dataItem);

    /// <summary>
    /// Called by a specific column type when one of its properties changed,
    /// and its current cells need to be updated.
    /// </summary>
    /// <param name="propertyName">Indicates which property changed and caused this call</param>
    protected void NotifyPropertyChanged(string propertyName)
    {
        OwningGrid?.RefreshColumnElements(this, propertyName);
    }
    
    /// <summary>
    /// Set whether to allow editing
    /// </summary>
    /// <returns></returns>
    public virtual bool IsEditable()
    {
        return true;
    }

    /// <summary>
    /// When overridden in a derived class, called when a cell in the column enters editing mode.
    /// </summary>
    /// <param name="editingElement">
    /// The element that the column displays for a cell in editing mode.
    /// </param>
    /// <param name="editingEventArgs">
    /// Information about the user gesture that is causing a cell to enter editing mode.
    /// </param>
    /// <returns>
    /// The unedited value.
    /// </returns>
    protected abstract object? PrepareCellForEdit(Control editingElement, RoutedEventArgs editingEventArgs);

    /// <summary>
    /// Called by the DataGrid control when a column asked for its
    /// elements to be refreshed, typically because one of its properties changed.
    /// </summary>
    /// <param name="element">Indicates the element that needs to be refreshed</param>
    /// <param name="propertyName">Indicates which property changed and caused this call</param>
    protected internal virtual void RefreshCellContent(Control element, string propertyName)
    { }

    /// <summary>
    /// When overridden in a derived class, called when a cell in the column exits editing mode.
    /// </summary>
    protected virtual void EndCellEdit(Control editingElement)
    { }
}