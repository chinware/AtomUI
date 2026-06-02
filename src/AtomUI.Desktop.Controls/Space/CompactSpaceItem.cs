using System.Reactive.Disposables;
using AtomUI.Data;
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
    
    private CompositeDisposable? _disposables;

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
            if (change.OldValue != null)
            {
                _disposables?.Dispose();
                _disposables = null;
            }

            if (change.NewValue != null && change.NewValue is ICompactSpaceAware && Child != null)
            {
                _disposables = new CompositeDisposable();
                _disposables.Add(BindUtils.RelayBind(this, CompactSpaceItemPositionProperty, Child, CompactSpaceItemPositionProperty));
                _disposables.Add(BindUtils.RelayBind(this, CompactSpaceOrientationProperty, Child, CompactSpaceOrientationProperty));
                _disposables.Add(BindUtils.RelayBind(this, IsUsedInCompactSpaceProperty, Child, IsUsedInCompactSpaceProperty));
            }
        }
        if (change.Property == ChildProperty ||
            change.Property == CompactSpace.ItemSizeProperty ||
            change.Property == CompactSpaceOrientationProperty ||
            change.Property == IsUsedInCompactSpaceProperty ||
            change.Property == CompactSpaceItemPositionProperty)
        {
            ConfigureItemSize(CompactSpace.GetItemSize(this), IsUsedInCompactSpace, CompactSpaceOrientation);
        }
    }
    
    void ICompactSpaceAware.NotifyPositionChange(SpaceItemPosition? position)
    {
        var isUsedInCompactSpace = position != null;
        if (IsUsedInCompactSpace != isUsedInCompactSpace)
        {
            IsUsedInCompactSpace = isUsedInCompactSpace;
        }

        if (CompactSpaceItemPosition != position)
        {
            CompactSpaceItemPosition = position;
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
        var position = CompactSpaceItemPosition;

        if (position == null ||
            position == SpaceItemPosition.First)
        {
            ClearOffsetTransform();
            return size;
        }

        var itemPosition = position.Value;
        if (CompactSpace.HasPositionFlag(itemPosition, SpaceItemPosition.First) &&
            CompactSpace.HasPositionFlag(itemPosition, SpaceItemPosition.Last))
        {
            ClearOffsetTransform();
            return size;
        }
        var borderThickness = (this as ICompactSpaceAware).GetBorderThickness();
        var delta           = borderThickness * PositionIndex;
        if (CompactSpaceOrientation == Orientation.Horizontal)
        {
            SetOffsetTransform(-delta, 0);
        }
        else
        {
            SetOffsetTransform(0, -delta);
        }
        return size;
    }

    void ICompactSpaceAware.NotifyOrientationChange(Orientation orientation)
    {
        if (CompactSpaceOrientation != orientation)
        {
            CompactSpaceOrientation = orientation;
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

    private void SetOffsetTransform(double x, double y)
    {
        if (RenderTransform is TranslateTransform transform)
        {
            if (Math.Abs(transform.X - x) < 0.001 &&
                Math.Abs(transform.Y - y) < 0.001)
            {
                return;
            }

            transform.X = x;
            transform.Y = y;
            return;
        }

        RenderTransform = new TranslateTransform(x, y);
    }

    private void ClearOffsetTransform()
    {
        if (RenderTransform != null)
        {
            RenderTransform = null;
        }
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
                    SetCurrentValue(VerticalAlignmentProperty, HorizontalAlignment.Stretch);
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
