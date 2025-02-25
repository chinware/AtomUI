using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Threading;

namespace AtomUI.Controls;

public class AvatarGroup : ContentControl/*, IChildIndexProvider*/
{
    #region 公共属性定义
    
    public static readonly StyledProperty<AvatarShape> AvatarShapeProperty =
        AvaloniaProperty.Register<AvatarGroup, AvatarShape>(nameof(Shape), AvatarShape.Circle, true);
    
    public static readonly StyledProperty<AvatarSize> AvatarSizeProperty =
        AvaloniaProperty.Register<AvatarGroup, AvatarSize>(nameof(Size), defaultValue: new AvatarSize(AvatarSizeType.Default), true);
    
    public static readonly StyledProperty<int> MaxProperty =
        AvaloniaProperty.Register<AvatarGroup, int>(nameof(Max), defaultValue: Int32.MaxValue);
    
    public static readonly StyledProperty<FlyoutTriggerType> TriggerTypeProperty =
        AvaloniaProperty.Register<AvatarGroup, FlyoutTriggerType>(nameof(TriggerType), FlyoutTriggerType.Click);
    
    public static readonly StyledProperty<double> HideSpaceProperty =
        AvaloniaProperty.Register<AvatarGroup, double>(nameof(HideSpace));
    
    public static readonly StyledProperty<IBrush?> PlaceHolderBackgroundProperty =
        AvaloniaProperty.Register<AvatarGroup, IBrush?>(nameof(HideSpace));
    public static readonly StyledProperty<IBrush?> PlaceHolderColorProperty =
        AvaloniaProperty.Register<AvatarGroup, IBrush?>(nameof(HideSpace));
    
  
    
    public AvatarShape Shape
    {
        get => GetValue(AvatarShapeProperty);
        set => SetValue(AvatarShapeProperty, value);
    }

    public AvatarSize Size
    {
        get => GetValue(AvatarSizeProperty);
        set => SetValue(AvatarSizeProperty, value);
    }
    
    public double HideSpace
    {
        get => GetValue(HideSpaceProperty);
        set => SetValue(HideSpaceProperty, value);
    }
    
    public int Max
    {
        get => GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }
    
    public FlyoutTriggerType TriggerType
    {
        get => GetValue(TriggerTypeProperty);
        set => SetValue(TriggerTypeProperty, value);
    }
    
    public IBrush? PlaceHolderBackground
    {
        get => GetValue(PlaceHolderBackgroundProperty);
        set => SetValue(PlaceHolderBackgroundProperty, value);
    }
    public IBrush? PlaceHolderColor
    {
        get => GetValue(PlaceHolderColorProperty);
        set => SetValue(PlaceHolderColorProperty, value);
    }

    #endregion
    
    private Panel? _childrenContainer;
    
    [Content]
    public Avalonia.Controls.Controls Children { get; } = new Avalonia.Controls.Controls();
    
    
    public AvatarGroup()
    {
        Children.CollectionChanged += ChildrenChanged;
    }

    static AvatarGroup()
    {
        AffectsMeasure<Segmented>(
            AvatarSizeProperty);
        AffectsRender<Segmented>(
            AvatarSizeProperty,
            AvatarShapeProperty);
    }
    
    protected virtual void ChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    { 
        
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _childrenContainer = e.NameScope.Find<StackPanel>(AvatarGroupTheme.ContainerPart);
        _RenderChild();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == MaxProperty)
        {
            _RenderChild();
        }
        else if(change.Property == HideSpaceProperty)
        {
            _hidePanel.Spacing = change.GetNewValue<double>();
        }
    }

    private Avatar? _NumAvatar;
    public StackPanel _hidePanel { get; } = new StackPanel();
    private void _RenderChild()
    {
        if (_childrenContainer == null)
        {
            return;
        }
        _childrenContainer.Children.Clear();
        
        _hidePanel.Children.Clear();
        _hidePanel.Orientation = Orientation.Horizontal;
        _hidePanel.Spacing = HideSpace;
        
        for (int i = 0; i < Children.Count; i++)
        {
            var control = Children[i];
            if (i <= Max - 1)
            {
                _childrenContainer.Children.Add(control);
            }else if (i == Max)
            {
                if (_NumAvatar == null)
                {
                    _NumAvatar = new Avatar();
                    _NumAvatar.Shape = Shape;
                    _NumAvatar.Size = Size;
                    _NumAvatar.Content = "+"+(Children.Count - Max);
                    if (PlaceHolderColor != null)
                    {
                        _NumAvatar.Foreground = PlaceHolderColor;
                    }
                    if (PlaceHolderBackground != null)
                    {
                        _NumAvatar.Background = PlaceHolderBackground;
                    }

                    if (TriggerType == FlyoutTriggerType.Click)
                    {
                        _NumAvatar.PointerPressed  += _PointerPressed; 
                    }
                    else
                    {
                        _NumAvatar.PointerEntered += _PointerEnter;
                        _NumAvatar.PointerExited += _PointerExit;
                    }
                }
                else
                {
                    if (TriggerType == FlyoutTriggerType.Click)
                    {
                        _NumAvatar.PointerPressed  -= _PointerPressed;
                        _NumAvatar.PointerPressed  += _PointerPressed; 
                    }
                    else
                    {
                        _NumAvatar.PointerEntered -= _PointerEnter;
                        _NumAvatar.PointerExited -= _PointerExit;
                        _NumAvatar.PointerEntered += _PointerEnter;
                        _NumAvatar.PointerExited += _PointerExit;
                    }
                }
                _childrenContainer.Children.Add(_NumAvatar);
                _hidePanel.Children.Add(control);
            }
            else
            {
                _hidePanel.Children.Add(control);
            }
        }
        
        if (TriggerType == FlyoutTriggerType.Hover)
        {
            _hidePanel.PointerEntered -= _PointerEnter;
            _hidePanel.PointerExited -= _PointerExit;
            _hidePanel.PointerEntered += _PointerEnter;
            _hidePanel.PointerExited += _PointerExit;
        }
    }

    private Flyout? _flyout;
    private void _PointerPressed(object? sender, PointerPressedEventArgs args)
    {
        if (_flyout != null && _flyout.IsOpen)
        {
            _HideFlyout();
            return;
        }
        _CreateFlyout();
    }

    private bool _hideFlyoutDelay;
    private void _PointerEnter(object? sender, PointerEventArgs args)
    {
        _hideFlyoutDelay = false;
        if (_flyout != null && _flyout.IsOpen)
        {
            _timer.Stop();
            return;
        }
        _CreateFlyout();
    }
    
    private void _PointerExit(object? sender, PointerEventArgs args)
    {
        _hideFlyoutDelay = true;
        _timer.Stop();
        _timer.Start();
    }


    private void _TimeTick(object? sender, EventArgs args)
    {
        if (_hideFlyoutDelay)
        {
            _HideFlyout();
        }
        _timer.Stop();
    }

    private DispatcherTimer _timer = new DispatcherTimer();

    private void _HideFlyout()
    {
        if (_flyout != null && _flyout.IsOpen)
        {
            _flyout.Hide();
        }
    }

    private void _CreateFlyout()
    {
        
        if (_flyout == null)
        {
            _flyout = new Flyout
            {
                Placement = PlacementMode.Top,
                Content = _hidePanel
            };
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += _TimeTick;
        }
        else
        {
            _flyout.Content = _hidePanel;
        }
        
        if (_timer.IsEnabled)
        {
            _timer.Stop();
        }

        _flyout.ShowAt(_NumAvatar!);
    }
}