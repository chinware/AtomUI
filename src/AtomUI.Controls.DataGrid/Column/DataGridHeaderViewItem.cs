using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

public class DataGridHeaderViewItem : ContentControl
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
    
    internal static readonly DirectProperty<DataGridHeaderViewItem, Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<DataGridHeaderViewItem, Thickness>(
            nameof(EffectiveBorderThickness), o => o.EffectiveBorderThickness);
    
    internal Thickness EffectiveBorderThickness
    {
        get => _effectiveBorderThickness;
        set => SetAndRaise(EffectiveBorderThicknessProperty, ref _effectiveBorderThickness, value);
    }
    private Thickness _effectiveBorderThickness;
    
    internal DataGridColumn? OwningColumn { get; set; }
    internal DataGrid OwningGrid { get; set; }
    
    #endregion
    
    public DataGridHeaderViewItem(DataGrid owningGrid)
    {
        OwningGrid = owningGrid;
        BindUtils.RelayBind(OwningGrid, BorderThicknessProperty, this, BorderThicknessProperty);
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
    
}