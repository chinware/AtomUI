using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class DataGridHeaderViewItem : HeaderedItemsControl
{
    #region 公共属性定义

    public static readonly DirectProperty<DataGridHeaderViewItem, int> LevelProperty =
        AvaloniaProperty.RegisterDirect<DataGridHeaderViewItem, int>(
            nameof(Level), o => o.Level);
    
    public static readonly DirectProperty<DataGridHeaderViewItem, bool> IsLeafProperty =
        AvaloniaProperty.RegisterDirect<DataGridHeaderViewItem, bool>(
            nameof(IsLeaf), o => o.IsLeaf);
    
    /// <summary>
    /// Gets the level/indentation of the item.
    /// </summary>
    public int Level
    {
        get => _level;
        private set => SetAndRaise(LevelProperty, ref _level, value);
    }
    private int _level;
    
    public bool IsLeaf
    {
        get => _isLeaf;
        internal set => SetAndRaise(IsLeafProperty, ref _isLeaf, value);
    }
    private bool _isLeaf;

    #endregion

    #region 内部属性定义
    internal static readonly StyledProperty<Thickness> BorderThicknessProperty =
        Border.BorderThicknessProperty.AddOwner<DataGridHeaderViewItem>();
    
    internal static readonly DirectProperty<DataGridHeaderViewItem, Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<DataGridHeaderViewItem, Thickness>(
            nameof(EffectiveBorderThickness), o => o.EffectiveBorderThickness);
    
    internal Thickness BorderThickness
    {
        get => GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }
    
    internal Thickness EffectiveBorderThickness
    {
        get => _effectiveBorderThickness;
        set => SetAndRaise(EffectiveBorderThicknessProperty, ref _effectiveBorderThickness, value);
    }
    private Thickness _effectiveBorderThickness;
    
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel()
        {
            Orientation = Orientation.Horizontal
        });
    
    internal DataGridHeaderView? OwningHeaderView { get; set; }
    internal DataGrid OwningGrid { get; set; }
    
    #endregion
    
    private Border? _headerFrame;
    
    static DataGridHeaderViewItem()
    {
        ItemsPanelProperty.OverrideDefaultValue<DataGridHeaderViewItem>(DefaultPanel);
    }

    public DataGridHeaderViewItem(DataGrid owningGrid)
    {
        OwningGrid = owningGrid;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsLeafProperty)
        {
            UpdatePseudoClasses();
            CalculateEffectiveBorderThickness();
        }
        else if (change.Property == BorderThicknessProperty)
        {
            CalculateEffectiveBorderThickness();
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == EffectiveBorderThicknessProperty)
            {
                ApplyEffectiveBorder();
            }
        }
    }
    
    protected override Control CreateContainerForItemOverride(
        object? item,
        int index,
        object? recycleKey)
    {
        return new DataGridHeaderViewItem(OwningGrid);
    }
    
    protected override bool NeedsContainerOverride(
        object? item,
        int index,
        out object? recycleKey)
    {
        return NeedsContainer<DataGridHeaderViewItem>(item, out recycleKey);
    }
    
    protected override void ContainerForItemPreparedOverride(
        Control container,
        object? item,
        int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);
        if (container is DataGridHeaderViewItem headerViewItem)
        {
            headerViewItem.OwningHeaderView = OwningHeaderView;
            BindUtils.RelayBind(OwningGrid, DataGrid.BorderThicknessProperty, headerViewItem, BorderThicknessProperty);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _headerFrame = e.NameScope.Find<Border>(DataGridHeaderViewItemTheme.FramePart);
        ApplyEffectiveBorder();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(DataGridPseudoClass.GroupItemLeaf, IsLeaf);
    }

    private void CalculateEffectiveBorderThickness()
    {
        if (IsLeaf)
        {
            EffectiveBorderThickness = new Thickness(0, 0, BorderThickness.Right, 0);
        }
        else
        {
            EffectiveBorderThickness = new Thickness(0, 0, BorderThickness.Right, BorderThickness.Bottom);
        }
    }

    private void ApplyEffectiveBorder()
    {
        if (IsLeaf)
        {
            if (_headerFrame != null)
            {
                _headerFrame.BorderThickness = EffectiveBorderThickness;
            }
        }
        else
        {
            if (Header is DataGridColumnGroupHeader columnGroupHeader)
            {
                columnGroupHeader.BorderThickness = EffectiveBorderThickness;
            }
            else if (Header is DataGridColumnHeader columnHeader)
            {
                columnHeader.BorderThickness = EffectiveBorderThickness;
            }
        }
    }
}