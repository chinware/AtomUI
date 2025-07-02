using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

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
    
    private Border? _headerFrame;
    

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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _headerFrame = e.NameScope.Find<Border>(DataGridHeaderViewItemThemeConstants.FramePart);
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
            if (Content is DataGridColumnGroupHeader columnGroupHeader)
            {
                columnGroupHeader.BorderThickness = EffectiveBorderThickness;
            }
            else if (Content is DataGridColumnHeader columnHeader)
            {
                columnHeader.BorderThickness = EffectiveBorderThickness;
            }
        }
    }
}