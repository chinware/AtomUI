using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal class CompactSpaceItem : Decorator, ICompactSpaceAware
{
    internal static readonly StyledProperty<SpaceItemPosition?> CompactSpaceItemPositionProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceItemPositionProperty.AddOwner<CompactSpaceItem>();
    
    internal static readonly StyledProperty<Orientation> CompactSpaceOrientationProperty = 
        CompactSpaceAwareControlProperty.CompactSpaceOrientationProperty.AddOwner<CompactSpaceItem>();
    
    internal static readonly StyledProperty<bool> IsUsedInCompactSpaceProperty = 
        CompactSpaceAwareControlProperty.IsUsedInCompactSpaceProperty.AddOwner<CompactSpaceItem>();
    
    public static readonly StyledProperty<int> PositionIndexProperty = 
        AvaloniaProperty.Register<CompactSpaceItem, int>(nameof(PositionIndex));
    
    internal SpaceItemPosition? CompactSpaceItemPosition
    {
        get => GetValue(CompactSpaceItemPositionProperty);
        set => SetValue(CompactSpaceItemPositionProperty, value);
    }
    
    internal Orientation CompactSpaceOrientation
    {
        get => GetValue(CompactSpaceOrientationProperty);
        set => SetValue(CompactSpaceOrientationProperty, value);
    }
    
    internal bool IsUsedInCompactSpace
    {
        get => GetValue(IsUsedInCompactSpaceProperty);
        set => SetValue(IsUsedInCompactSpaceProperty, value);
    }
    
    internal int PositionIndex
    {
        get => GetValue(PositionIndexProperty);
        set => SetValue(PositionIndexProperty, value);
    }
    
    private TranslateTransform? _overlapTransform;
    private double _currentOverlapX = double.NaN;
    private double _currentOverlapY = double.NaN;

    static CompactSpaceItem()
    {
        AffectsMeasure<CompactSpaceItem>(CompactSpaceItemPositionProperty, CompactSpaceOrientationProperty, IsUsedInCompactSpaceProperty);
        ClipToBoundsProperty.OverrideDefaultValue<CompactSpaceItem>(false);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ChildProperty)
        {
            if (change.OldValue is ICompactSpaceAware oldCompactSpaceAware)
            {
                oldCompactSpaceAware.NotifyPositionChange(null);
            }

            if (change.NewValue is ICompactSpaceAware newCompactSpaceAware)
            {
                newCompactSpaceAware.NotifyOrientationChange(CompactSpaceOrientation);
                newCompactSpaceAware.NotifyPositionChange(CompactSpaceItemPosition);
            }
        }
        if (change.Property == ChildProperty ||
            change.Property == CompactSpace.ItemSizeProperty ||
            change.Property == CompactSpaceOrientationProperty ||
            change.Property == PositionIndexProperty ||
            change.Property == IsUsedInCompactSpaceProperty ||
            change.Property == CompactSpaceItemPositionProperty)
        {
            ConfigureItemSize(CompactSpace.GetItemSize(this), IsUsedInCompactSpace, CompactSpaceOrientation);
            UpdateOverlapTransform();
        }
    }
    
    void ICompactSpaceAware.NotifyPositionChange(SpaceItemPosition? position)
    {
        var isUsedInCompactSpace = position != null;
        if (IsUsedInCompactSpace == isUsedInCompactSpace &&
            CompactSpaceItemPosition == position)
        {
            return;
        }

        IsUsedInCompactSpace     = isUsedInCompactSpace;
        CompactSpaceItemPosition = position;
        if (Child is ICompactSpaceAware compactSpaceAware)
        {
            compactSpaceAware.NotifyPositionChange(position);
        }
    }

    bool ICompactSpaceAware.IsAlwaysActiveZIndex()
    {
        if (Child is ICompactSpaceAware compactSpaceAware)
        {
            return compactSpaceAware.IsAlwaysActiveZIndex();
        }

        return false;
    }

    bool ICompactSpaceAware.IgnoreZIndexChange()
    {
        if (Child is ICompactSpaceAware compactSpaceAware)
        {
            return compactSpaceAware.IgnoreZIndexChange();
        }

        return false;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        UpdateOverlapTransform();
        return size;
    }

    private void UpdateOverlapTransform()
    {
        if (CompactSpaceItemPosition == null ||
            CompactSpaceItemPosition == SpaceItemPosition.First ||
            (CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.First) && CompactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.Last)))
        {
            ClearOverlapTransform();
            return;
        }
        var borderThickness = (this as ICompactSpaceAware).GetBorderThickness();
        var delta           = borderThickness * PositionIndex;
        var x               = CompactSpaceOrientation == Orientation.Horizontal ? -delta : 0;
        var y               = CompactSpaceOrientation == Orientation.Horizontal ? 0 : -delta;
        if (Math.Abs(x) < 0.001 && Math.Abs(y) < 0.001)
        {
            ClearOverlapTransform();
            return;
        }

        _overlapTransform ??= new TranslateTransform();
        if (!ReferenceEquals(RenderTransform, _overlapTransform))
        {
            RenderTransform = _overlapTransform;
        }
        if (double.IsNaN(_currentOverlapX) || Math.Abs(_currentOverlapX - x) > 0.001)
        {
            _overlapTransform.X = x;
            _currentOverlapX    = x;
        }
        if (double.IsNaN(_currentOverlapY) || Math.Abs(_currentOverlapY - y) > 0.001)
        {
            _overlapTransform.Y = y;
            _currentOverlapY    = y;
        }
    }

    private void ClearOverlapTransform()
    {
        if (RenderTransform != null)
        {
            RenderTransform = null;
        }
        _currentOverlapX = double.NaN;
        _currentOverlapY = double.NaN;
    }

    void ICompactSpaceAware.NotifyOrientationChange(Orientation orientation)
    {
        if (CompactSpaceOrientation == orientation)
        {
            return;
        }

        CompactSpaceOrientation = orientation;
        if (Child is ICompactSpaceAware compactSpaceAware)
        {
            compactSpaceAware.NotifyOrientationChange(orientation);
        }
    }

    double ICompactSpaceAware.GetBorderThickness()
    {
        if (Child is ICompactSpaceAware compactSpaceAware)
        {
            return compactSpaceAware.GetBorderThickness();
        }

        return 0.0;
    }
    
    private void ConfigureItemSize(CompactSpaceSize size, bool isUsedInCompactSpace, Orientation compactSpaceOrientation) 
    {
        if (!isUsedInCompactSpace)
        {
            ClearValue(HorizontalAlignmentProperty);
            ClearValue(VerticalAlignmentProperty);
        }
        else
        {
            if (compactSpaceOrientation == Orientation.Horizontal)
            {
                if (size.IsStar)
                {
                    ClearValue(WidthProperty);
                    SetCurrentValue(HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
                }
                else if (size.IsAbsolute)
                {
                    SetCurrentValue(HorizontalAlignmentProperty, HorizontalAlignment.Left);
                    SetCurrentValue(WidthProperty, size.Value);
                }
                else
                {
                    ClearValue(WidthProperty);
                    ClearValue(HorizontalAlignmentProperty);
                }

                if (Child != null)
                {
                    Child.SetCurrentValue(HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
                }
            }
            else
            {
                if (size.IsStar)
                {
                    ClearValue(HeightProperty);
                    SetCurrentValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);
                }
                else if (size.IsAbsolute)
                {
                    SetCurrentValue(VerticalAlignmentProperty, VerticalAlignment.Top);
                    SetCurrentValue(HeightProperty, size.Value);
                }
                else
                {
                    ClearValue(HeightProperty);
                    ClearValue(VerticalAlignmentProperty);
                }
                
                if (Child != null)
                {
                    Child.SetCurrentValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);
                }
            }
        }
    }
}
