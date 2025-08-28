using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

internal class DataGridHeaderViewItem : ContentControl
{
    #region 公共属性定义

    public static readonly DirectProperty<DataGridHeaderViewItem, int> LevelProperty =
        AvaloniaProperty.RegisterDirect<DataGridHeaderViewItem, int>(
            nameof(Level), o => o.Level);
    
    public static readonly DirectProperty<DataGridHeaderViewItem, bool> IsLeafProperty =
        AvaloniaProperty.RegisterDirect<DataGridHeaderViewItem, bool>(
            nameof(IsLeaf), o => o.IsLeaf);
    
    public static readonly DirectProperty<DataGridHeaderViewItem, bool> IsFrozenProperty =
        AvaloniaProperty.RegisterDirect<DataGridHeaderViewItem, bool>(nameof(IsFrozen), 
            o => o.IsFrozen,
            (o, v) => o.IsFrozen = v);
    
    public static readonly DirectProperty<DataGridHeaderViewItem, bool> IsShowLeftFrozenShadowProperty =
        AvaloniaProperty.RegisterDirect<DataGridHeaderViewItem, bool>(
            nameof(IsShowLeftFrozenShadow),
            o => o.IsShowLeftFrozenShadow, 
            (o, v) => o.IsShowLeftFrozenShadow = v);
    
    public static readonly DirectProperty<DataGridHeaderViewItem, bool> IsShowRightFrozenShadowProperty =
        AvaloniaProperty.RegisterDirect<DataGridHeaderViewItem, bool>(
            nameof(IsShowRightFrozenShadow),
            o => o.IsShowRightFrozenShadow, 
            (o, v) => o.IsShowRightFrozenShadow = v);
    
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
    
    public bool IsFrozen
    {
        get => _isFrozen;
        internal set => SetAndRaise(IsFrozenProperty, ref _isFrozen, value);
    }
    private bool _isFrozen;
    
    bool _isShowLeftFrozenShadow = false;

    internal bool IsShowLeftFrozenShadow
    {
        get => _isShowLeftFrozenShadow;
        set => SetAndRaise(IsShowLeftFrozenShadowProperty, ref _isShowLeftFrozenShadow, value);
    }
    
    bool _isShowRightFrozenShadow = false;

    internal bool IsShowRightFrozenShadow
    {
        get => _isShowRightFrozenShadow;
        set => SetAndRaise(IsShowRightFrozenShadowProperty, ref _isShowRightFrozenShadow, value);
    }

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
    
    private IDisposable? _bindingDisposable;
    
    public DataGridHeaderViewItem(DataGrid owningGrid)
    {
        OwningGrid = owningGrid;
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _bindingDisposable?.Dispose();
        _bindingDisposable = BindUtils.RelayBind(OwningGrid, BorderThicknessProperty, this, BorderThicknessProperty);
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        _bindingDisposable?.Dispose();
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