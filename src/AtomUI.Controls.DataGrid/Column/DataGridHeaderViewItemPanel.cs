using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

internal class DataGridHeaderViewItemPanel : Control
{
    #region 公共属性定义

    public static readonly StyledProperty<double> VerticalSpacingProperty =
        AvaloniaProperty.Register<DataGridHeaderViewItemPanel, double>(
            nameof(VerticalSpacing));
    
    public static readonly StyledProperty<Control?> HeaderProperty =
        AvaloniaProperty.Register<DataGridHeaderViewItemPanel, Control?>(
            nameof(Header));
    
    public static readonly StyledProperty<Control?> ItemsPresenterProperty =
        AvaloniaProperty.Register<DataGridHeaderViewItemPanel, Control?>(
            nameof(ItemsPresenter));
    
    public double VerticalSpacing
    {
        get => GetValue(VerticalSpacingProperty);
        set => SetValue(VerticalSpacingProperty, value);
    }

    public Control? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
    
    public Control? ItemsPresenter
    {
        get => GetValue(ItemsPresenterProperty);
        set => SetValue(ItemsPresenterProperty, value);
    }

    #endregion
    
    static DataGridHeaderViewItemPanel()
    {
        AffectsMeasure<DataGridHeaderViewItemPanel>(VerticalSpacingProperty, HeaderProperty, ItemsPresenterProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ItemsPresenterProperty || change.Property == HeaderProperty)
        {
            if (change.OldValue is Control oldChild)
            {
                LogicalChildren.Remove(oldChild);
                VisualChildren.Remove(oldChild);
            }

            if (change.NewValue is Control newChild)
            {
                LogicalChildren.Add(newChild);
                VisualChildren.Add(newChild);
            }
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        if (ItemsPresenter != null)
        {
            ItemsPresenter.PropertyChanged += HandleItemsPresenterPropertyChanged;
        }
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        if (ItemsPresenter != null)
        {
            ItemsPresenter.PropertyChanged -= HandleItemsPresenterPropertyChanged;
        }
    }

    private void HandleItemsPresenterPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == IsVisibleProperty)
        {
            InvalidateMeasure();
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var parentWidth       = 0d;
        var parentHeight      = 0d;
        var accumulatedWidth  = 0d;
        var accumulatedHeight = 0d;
        
        Debug.Assert(Header != null && ItemsPresenter != null);

        if (!ItemsPresenter.IsVisible)
        {
            var childConstraint = new Size(
                Math.Max(0, availableSize.Width - accumulatedWidth),
                Math.Max(0, availableSize.Height - accumulatedHeight));
    
            Header.Measure(childConstraint);
            var childDesiredSize = Header.DesiredSize;
            parentHeight      =  Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
            parentWidth       =  Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
            accumulatedHeight += childDesiredSize.Height;
            accumulatedWidth  += childDesiredSize.Width;
        }
        else
        {
            var headerConstraint = new Size(
                Math.Max(0, availableSize.Width - accumulatedWidth),
                Math.Max(0, availableSize.Height - accumulatedHeight));
        
            Header.Measure(headerConstraint);
            var headerDesiredSize = Header.DesiredSize;
            
            parentWidth       =  Math.Max(parentWidth, accumulatedWidth + headerDesiredSize.Width);
            accumulatedHeight += VerticalSpacing;
            accumulatedHeight += headerDesiredSize.Height;
            
            var itemsPresenterConstraint = new Size(
                Math.Max(0, availableSize.Width - accumulatedWidth),
                Math.Max(0, availableSize.Height - accumulatedHeight));
    
            ItemsPresenter.Measure(itemsPresenterConstraint);
            var itemsPresenterDesiredSize = ItemsPresenter.DesiredSize;
            parentHeight      =  Math.Max(parentHeight, accumulatedHeight + itemsPresenterDesiredSize.Height);
            parentWidth       =  Math.Max(parentWidth, accumulatedWidth + itemsPresenterDesiredSize.Width);
            accumulatedHeight += itemsPresenterDesiredSize.Height;
            accumulatedWidth  += itemsPresenterDesiredSize.Width;
        }
    
        // Make sure the final accumulated size is reflected in parentSize.
        parentWidth  = Math.Max(parentWidth, accumulatedWidth);
        parentHeight = Math.Max(parentHeight, accumulatedHeight);
        return new Size(parentWidth, parentHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Debug.Assert(Header != null && ItemsPresenter != null);
    
        var currentBounds = new Rect(finalSize);

        if (!ItemsPresenter.IsVisible)
        {
            Header.Arrange(new Rect(currentBounds.X, currentBounds.Y, currentBounds.Width, currentBounds.Height));
        }
        else
        {
            double height = Math.Min(Header.DesiredSize.Height, currentBounds.Height);
            Header.Arrange(currentBounds.WithHeight(height));
            height += VerticalSpacing;
            currentBounds = new Rect(currentBounds.X, currentBounds.Y + height, currentBounds.Width, Math.Max(0, currentBounds.Height - height));
            ItemsPresenter.Arrange(new Rect(currentBounds.X, currentBounds.Y, currentBounds.Width, currentBounds.Height));
        }
    
        return finalSize;
    }
}