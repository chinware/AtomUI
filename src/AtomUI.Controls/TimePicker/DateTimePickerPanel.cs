﻿using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls.TimePickerLang;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Utilities;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public enum DateTimePickerPanelType
{
    Year,
    Month,
    Day,
    Hour,
    Minute,
    Second,
    TimePeriod //AM or PM
}

public struct CellHoverInfo
{
    public DateTimePickerPanelType PanelType { get; }
    public int CellValue { get; }

    public CellHoverInfo(DateTimePickerPanelType panelType, int cellValue)
    {
        PanelType = panelType;
        CellValue = cellValue;
    }
}

internal class CellHoverEventArgs : EventArgs
{
    public CellHoverInfo? CellHoverInfo { get; }

    public CellHoverEventArgs(CellHoverInfo? hoverInfo)
    {
        CellHoverInfo = hoverInfo;
    }
}

internal class CellDbClickedEventArgs : EventArgs
{
    public bool IsSelected { get; }

    public CellDbClickedEventArgs(bool isSelected)
    {
        IsSelected = isSelected;
    }
}

internal class DateTimePickerPanel : Panel,
                                     ILogicalScrollable,
                                     IResourceBindingManager
{
    #region 公共属性定义
    
    public static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<DateTimePickerPanel, double>(nameof(ItemHeight), 40.0);
    
    public static readonly StyledProperty<DateTimePickerPanelType> PanelTypeProperty =
        AvaloniaProperty.Register<DateTimePickerPanel, DateTimePickerPanelType>(nameof(PanelType));
    
    public static readonly StyledProperty<string> ItemFormatProperty =
        AvaloniaProperty.Register<DateTimePickerPanel, string>(nameof(ItemFormat), "yyyy");

    public static readonly StyledProperty<bool> ShouldLoopProperty =
        AvaloniaProperty.Register<DateTimePickerPanel, bool>(nameof(ShouldLoop));
    
    public double ItemHeight
    {
        get => GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }
    
    public DateTimePickerPanelType PanelType
    {
        get => GetValue(PanelTypeProperty);
        set => SetValue(PanelTypeProperty, value);
    }
    
    public string ItemFormat
    {
        get => GetValue(ItemFormatProperty);
        set => SetValue(ItemFormatProperty, value);
    }
    
    public bool ShouldLoop
    {
        get => GetValue(ShouldLoopProperty);
        set => SetValue(ShouldLoopProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DateTimePickerPanel>();

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable
    {
        get => _resourceBindingsDisposable;
        set => _resourceBindingsDisposable = value;
    }
    #endregion

    #region 内部事件定义

    internal event EventHandler<CellHoverEventArgs>? CellHovered;
    internal event EventHandler<CellDbClickedEventArgs>? CellDbClicked;

    #endregion
    
    private CompositeDisposable? _resourceBindingsDisposable;
    //Backing fields for properties
    private int _minimumValue = 1;
    private int _maximumValue = 2;
    private int _selectedValue = 1;
    private int _increment = 1;

    //Helper fields
    private int _selectedIndex;
    private int _totalItems;
    private int _numItemsAboveBelowSelected;
    private int _range;
    private double _extentOne;
    private Size _extent;
    private Vector _offset;
    private bool _hasInit;
    private bool _suppressUpdateOffset;
    private ScrollContentPresenter? _parentScroller;

    public DateTimePickerPanel()
    {
        FormatDate = DateTime.Now;
        AddHandler(TappedEvent, HandleItemTapped, RoutingStrategies.Bubble);
        AddHandler(DoubleTappedEvent, HandleItemDoubleTapped, RoutingStrategies.Bubble);
        _resourceBindingsDisposable = new CompositeDisposable();
    }

    static DateTimePickerPanel()
    {
        FocusableProperty.OverrideDefaultValue<DateTimePickerPanel>(true);
        BackgroundProperty.OverrideDefaultValue<DateTimePickerPanel>(Brushes.Transparent);
        AffectsMeasure<DateTimePickerPanel>(ItemHeightProperty);
    }

    /// <summary>
    /// Gets or sets the minimum value
    /// </summary>
    public int MinimumValue
    {
        get => _minimumValue;

        set
        {
            if (value > MaximumValue)
            {
                throw new InvalidOperationException("Minimum cannot be greater than Maximum");
            }

            _minimumValue = value;
            UpdateHelperInfo();
            var sel = CoerceSelected(SelectedValue);
            if (sel != SelectedValue)
            {
                SelectedValue = sel;
            }

            UpdateItems();
            InvalidateArrange();
            RaiseScrollInvalidated(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or sets the maximum value
    /// </summary>
    public int MaximumValue
    {
        get => _maximumValue;

        set
        {
            if (value < MinimumValue)
            {
                throw new InvalidOperationException("Maximum cannot be less than Minimum");
            }

            _maximumValue = value;
            UpdateHelperInfo();
            var sel = CoerceSelected(SelectedValue);
            if (sel != SelectedValue)
            {
                SelectedValue = sel;
            }

            UpdateItems();
            InvalidateArrange();
            RaiseScrollInvalidated(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or sets the selected value
    /// </summary>
    public int SelectedValue
    {
        get => _selectedValue;

        set
        {
            if (value > MaximumValue || value < MinimumValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            var sel = CoerceSelected(value);
            _selectedValue = sel;
            _selectedIndex = (value - MinimumValue) / Increment;

            if (!ShouldLoop)
            {
                CreateOrDestroyItems(Children);
            }

            if (!_suppressUpdateOffset)
            {
                _offset = new Vector(
                    0, ShouldLoop ? _selectedIndex * ItemHeight + _extentOne * 50 : _selectedIndex * ItemHeight);
            }

            UpdateItems();
            InvalidateArrange();
            RaiseScrollInvalidated(EventArgs.Empty);

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets or sets the increment
    /// </summary>
    public int Increment
    {
        get => _increment;

        set
        {
            if (value <= 0 || value > _range)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            _increment = value;
            UpdateHelperInfo();
            var sel = CoerceSelected(SelectedValue);
            if (sel != SelectedValue)
            {
                SelectedValue = sel;
            }

            UpdateItems();
            InvalidateArrange();
            RaiseScrollInvalidated(EventArgs.Empty);
        }
    }

    //Used to help format the date (if applicable), for ex.,
    //if we're want to display the day of week, we need context
    //for the month/year, this is our context
    internal DateTime FormatDate { get; set; }

    public Vector Offset
    {
        get => _offset;

        set
        {
            var old = _offset;
            _offset = value;
            var dy       = _offset.Y - old.Y;
            var children = Children;

            if (dy > 0) // Scroll Down
            {
                var numCountsToMove = 0;
                for (var i = 0; i < children.Count; i++)
                {
                    if (children[i].Bounds.Bottom - dy < 0)
                    {
                        numCountsToMove++;
                    }
                    else
                    {
                        break;
                    }
                }
                children.MoveRange(0, numCountsToMove, children.Count - 1);

                var scrollHeight = _extent.Height - Viewport.Height;
                if (ShouldLoop && value.Y >= scrollHeight - _extentOne)
                {
                    _offset = new Vector(0, value.Y - _extentOne * 50);
                }
            }
            else if (dy < 0) // Scroll Up
            {
                var numCountsToMove = 0;
                for (var i = children.Count - 1; i >= 0; i--)
                {
                    if (children[i].Bounds.Top - dy > Bounds.Height)
                    {
                        numCountsToMove++;
                    }
                    else
                    {
                        break;
                    }
                }

                children.MoveRange(children.Count - numCountsToMove, numCountsToMove, 0);
                if (ShouldLoop && value.Y < _extentOne)
                {
                    _offset = new Vector(0, value.Y + _extentOne * 50);
                }
            }

            //Setting selection will handle all invalidation
            var newSel = Offset.Y / ItemHeight % _totalItems;
            _suppressUpdateOffset = true;
            SelectedValue         = (int)newSel * Increment + MinimumValue;
            _suppressUpdateOffset = false;

            Debug.WriteLine(FormattableString.Invariant($"Offset: {_offset} ItemHeight: {ItemHeight}"));
        }
    }

    public bool CanHorizontallyScroll
    {
        get => false;
        set { }
    }

    public bool CanVerticallyScroll
    {
        get => true;
        set { }
    }

    public bool IsLogicalScrollEnabled => true;

    public Size ScrollSize => new(0, ItemHeight);

    public Size PageScrollSize => new(0, ItemHeight * 4);

    public Size Extent => _extent;

    public Size Viewport => Bounds.Size;

    public event EventHandler? ScrollInvalidated;

    public event EventHandler? SelectionChanged;

    protected override Size MeasureOverride(Size availableSize)
    {
        if (double.IsInfinity(availableSize.Width) ||
            double.IsInfinity(availableSize.Height))
        {
            throw new InvalidOperationException("Panel must have finite height");
        }

        if (!_hasInit)
        {
            UpdateHelperInfo();
        }

        var initY = availableSize.Height / 2.0 - ItemHeight / 2.0;
        _numItemsAboveBelowSelected = (int)Math.Ceiling(initY / ItemHeight) + 1;

        var children = Children;

        CreateOrDestroyItems(children);

        for (var i = 0; i < children.Count; i++)
        {
            children[i].Measure(availableSize);
        }

        if (!_hasInit)
        {
            UpdateItems();
            RaiseScrollInvalidated(EventArgs.Empty);
            EnableCellHoverAnimation();
            _hasInit = true;
        }

        return availableSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count == 0)
        {
            return base.ArrangeOverride(finalSize);
        }

        var  itemHgt  = ItemHeight;
        var  children = Children;
        Rect rc;
        var  initY = finalSize.Height / 2.0 - itemHgt / 2.0;

        if (ShouldLoop)
        {
            var currentSet = Math.Truncate(Offset.Y / _extentOne);
            initY += _extentOne * currentSet + (_selectedIndex - _numItemsAboveBelowSelected) * ItemHeight;

            for (var i = 0; i < children.Count; i++)
            {
                rc = new Rect(0, initY - Offset.Y, finalSize.Width, itemHgt);
                children[i].Arrange(rc);
                initY += itemHgt;
            }
        }
        else
        {
            var first = Math.Max(0, _selectedIndex - _numItemsAboveBelowSelected);
            for (var i = 0; i < children.Count; i++)
            {
                rc = new Rect(0, initY + first * itemHgt - Offset.Y, finalSize.Width, itemHgt);
                children[i].Arrange(rc);
                initY += itemHgt;
            }
        }

        return finalSize;
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _parentScroller             = this.GetVisualParent() as ScrollContentPresenter;
        _parentScroller?.AddHandler(Gestures.ScrollGestureEndedEvent, OnScrollGestureEnded);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _parentScroller?.RemoveHandler(Gestures.ScrollGestureEndedEvent, OnScrollGestureEnded);
        _parentScroller = null;
        this.DisposeTokenBindings();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
                ScrollUp();
                e.Handled = true;
                break;
            case Key.Down:
                ScrollDown();
                e.Handled = true;
                break;
            case Key.PageUp:
                ScrollUp(4);
                e.Handled = true;
                break;
            case Key.PageDown:
                ScrollDown(4);
                e.Handled = true;
                break;
        }

        base.OnKeyDown(e);
    }

    /// <summary>
    /// Refreshes the content of the visible items
    /// </summary>
    public void RefreshItems()
    {
        UpdateItems();
    }

    /// <summary>
    /// Scrolls up the specified number of items
    /// </summary>
    public void ScrollUp(int numItems = 1)
    {
        var newY = Math.Max(Offset.Y - numItems * ItemHeight, 0);
        Offset = new Vector(0, newY);
    }

    /// <summary>
    /// Scrolls down the specified number of items
    /// </summary>
    public void ScrollDown(int numItems = 1)
    {
        var scrollHeight = Math.Max(Extent.Height - ItemHeight, 0);
        var newY         = Math.Min(Offset.Y + numItems * ItemHeight, scrollHeight);
        Offset = new Vector(0, newY);
    }

    /// <summary>
    /// Updates helper fields used in various calculations
    /// </summary>
    private void UpdateHelperInfo()
    {
        _range      = _maximumValue - _minimumValue + 1;
        _totalItems = (int)Math.Ceiling((double)_range / _increment);

        var itemHgt = ItemHeight;
        //If looping, measure 100x as many items as we actually have
        _extent = new Size(0, ShouldLoop ? _totalItems * itemHgt * 100 : _totalItems * itemHgt);

        //Height of 1 "set" of items
        _extentOne = _totalItems * itemHgt;
        _offset    = new Vector(0, ShouldLoop ? _extentOne * 50 + _selectedIndex * itemHgt : _selectedIndex * itemHgt);
    }

    /// <summary>
    /// Ensures enough containers are visible in the viewport
    /// </summary>
    /// <param name="children"></param>
    private void CreateOrDestroyItems(Avalonia.Controls.Controls children)
    {
        var totalItemsInViewport = _numItemsAboveBelowSelected * 2 + 1;

        if (!ShouldLoop)
        {
            var numItemAboveSelect = _numItemsAboveBelowSelected;
            if (_selectedIndex - _numItemsAboveBelowSelected < 0)
            {
                numItemAboveSelect = _selectedIndex;
            }

            var numItemBelowSelect = _numItemsAboveBelowSelected;
            if (_selectedIndex + _numItemsAboveBelowSelected >= _totalItems)
            {
                numItemBelowSelect = _totalItems - _selectedIndex - 1;
            }

            totalItemsInViewport = numItemBelowSelect + numItemAboveSelect + 1;
        }

        while (children.Count < totalItemsInViewport)
        {
            var item = new ListBoxItem
            {
                MinHeight                  = ItemHeight,
                Classes                    = { $"{PanelType}Item" },
                VerticalContentAlignment   = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Focusable                  = false,
                CornerRadius               = new CornerRadius(0),
                SizeType                   = SizeType.Middle,
                Cursor                     = new Cursor(StandardCursorType.Hand),
                IsMotionEnabled            = false
            };
            item.PointerEntered += (sender, args) =>
            {
                if (sender is ListBoxItem target)
                {
                    var cellValue = (int)target.Tag!;
                    CellHovered?.Invoke(this, new CellHoverEventArgs(new CellHoverInfo(PanelType, cellValue)));
                }
            };

            this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(item, TemplatedControl.PaddingProperty,
                TimePickerTokenKey.ItemPadding, BindingPriority.LocalValue));
            children.Add(item);
        }

        if (children.Count > totalItemsInViewport)
        {
            var numToRemove = children.Count - totalItemsInViewport;
            children.RemoveRange(children.Count - numToRemove, numToRemove);
        }
    }

    /// <summary>
    /// Updates item content based on the current selection
    /// and the panel type
    /// </summary>
    private void UpdateItems()
    {
        var children  = Children;
        var min       = MinimumValue;
        var panelType = PanelType;
        var selected  = SelectedValue;
        var max       = MaximumValue;

        int first;
        if (ShouldLoop)
        {
            first = (_selectedIndex - _numItemsAboveBelowSelected) % _totalItems;
            first = first < 0 ? min + (first + _totalItems) * Increment : min + first * Increment;
        }
        else
        {
            first = min + Math.Max(0, _selectedIndex - _numItemsAboveBelowSelected) * Increment;
        }

        for (var i = 0; i < children.Count; i++)
        {
            var item = (ListBoxItem)children[i];
            var textBlock = new TextBlock
            {
                VerticalAlignment   = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                IsHitTestVisible    = false
            };
            textBlock.Text  =  FormatContent(first, panelType);
            item.Content    =  textBlock;
            item.Tag        =  first;
            item.IsSelected =  first == selected;
            first           += Increment;
            if (first > max)
            {
                first = min;
            }
        }
    }

    private string FormatContent(int value, DateTimePickerPanelType panelType)
    {
        switch (panelType)
        {
            case DateTimePickerPanelType.Year:
                return new DateTime(value, 1, 1).ToString(ItemFormat);
            case DateTimePickerPanelType.Month:
                return new DateTime(FormatDate.Year, value, 1).ToString(ItemFormat);
            case DateTimePickerPanelType.Day:
                return new DateTime(FormatDate.Year, FormatDate.Month, value).ToString(ItemFormat);
            case DateTimePickerPanelType.Hour:
                return new TimeSpan(value, 0, 0).ToString(ItemFormat);
            case DateTimePickerPanelType.Minute:
                return new TimeSpan(0, value, 0).ToString(ItemFormat);
            case DateTimePickerPanelType.Second:
                return new TimeSpan(0, 0, value).ToString(ItemFormat);
            case DateTimePickerPanelType.TimePeriod:
                return value == MinimumValue
                    ? LanguageResourceBinder.GetLangResource(TimePickerLangResourceKey.AMText)!
                    : LanguageResourceBinder.GetLangResource(TimePickerLangResourceKey.PMText)!;
            default:
                return "";
        }
    }

    /// <summary>
    /// Ensures the <see cref="SelectedValue" /> is within the bounds and
    /// follows the current Increment
    /// </summary>
    private int CoerceSelected(int newValue)
    {
        if (newValue < MinimumValue)
        {
            return MinimumValue;
        }

        if (newValue > MaximumValue)
        {
            return MaximumValue;
        }

        if (newValue % Increment != 0)
        {
            var items   = Enumerable.Range(MinimumValue, MaximumValue + 1).Where(i => i % Increment == 0).ToList();
            var nearest = items.Aggregate((x, y) => Math.Abs(x - newValue) > Math.Abs(y - newValue) ? y : x);
            return items.IndexOf(nearest) * Increment;
        }

        return newValue;
    }

    private void HandleItemTapped(object? sender, TappedEventArgs e)
    {
        if (e.Source is Visual source &&
            GetItemFromSource(source) is ListBoxItem listBoxItem &&
            listBoxItem.Tag is int tag)
        {
            SelectedValue = tag;
            e.Handled     = true;
        }
    }

    private void HandleItemDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (e.Source is Visual source &&
            GetItemFromSource(source) is ListBoxItem listBoxItem)
        {
            CellDbClicked?.Invoke(this, new CellDbClickedEventArgs(listBoxItem.IsSelected));
            e.Handled = true;
        }
    }

    //Helper to get ListBoxItem from pointerevent source
    private ListBoxItem? GetItemFromSource(Visual src)
    {
        var item = src;
        while (item != null && !(item is ListBoxItem))
        {
            item = item.GetVisualParent();
        }

        return (ListBoxItem?)item;
    }

    public bool BringIntoView(Control target, Rect targetRect)
    {
        return false;
    }

    public Control? GetControlInDirection(NavigationDirection direction, Control? from)
    {
        return null;
    }

    public void RaiseScrollInvalidated(EventArgs e)
    {
        ScrollInvalidated?.Invoke(this, e);
    }

    private void OnScrollGestureEnded(object? sender, ScrollGestureEndedEventArgs e)
    {
        var snapY = Math.Round(Offset.Y / ItemHeight) * ItemHeight;

        if (!MathUtilities.AreClose(snapY, Offset.Y))
        {
            Offset = Offset.WithY(snapY);
        }
    }

    private void EnableCellHoverAnimation()
    {
        var children = Children;
        for (var i = 0; i < children.Count; i++)
        {
            var item = (ListBoxItem)children[i];
            item.IsMotionEnabled = IsMotionEnabled;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                EnableCellHoverAnimation();
            }
        }
    }
}