using AtomUI.Data;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Layout;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

public class RangeTimePicker : TemplatedControl
{
   #region 公共属性定义
   
   public static readonly StyledProperty<object?> LeftAddOnProperty =
      AvaloniaProperty.Register<RangeTimePicker, object?>(nameof(LeftAddOn));

   public static readonly StyledProperty<object?> RightAddOnProperty =
      AvaloniaProperty.Register<RangeTimePicker, object?>(nameof(RightAddOn));
   
   public static readonly StyledProperty<object?> InnerLeftContentProperty 
      = AvaloniaProperty.Register<RangeTimePicker, object?>(nameof(InnerLeftContent));
   
   public static readonly StyledProperty<object?> InnerRightContentProperty 
      = AvaloniaProperty.Register<RangeTimePicker, object?>(nameof(InnerRightContent));

   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AddOnDecoratedBox.SizeTypeProperty.AddOwner<RangeTimePicker>();

   public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
      AddOnDecoratedBox.StyleVariantProperty.AddOwner<RangeTimePicker>();

   public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
      AddOnDecoratedBox.StatusProperty.AddOwner<RangeTimePicker>();
   
   public static readonly StyledProperty<string?> RangeStartWatermarkProperty =
      AvaloniaProperty.Register<TextBox, string?>(nameof(RangeStartWatermark));
   
   public static readonly StyledProperty<string?> RangeEndWatermarkProperty =
      AvaloniaProperty.Register<TextBox, string?>(nameof(RangeEndWatermark));
   
   public static readonly StyledProperty<PlacementMode> PickerPlacementProperty =
      AvaloniaProperty.Register<RangeTimePicker, PlacementMode>(nameof(PickerPlacement), defaultValue: PlacementMode.BottomEdgeAlignedLeft);
   
   public static readonly StyledProperty<bool> IsShowArrowProperty =
      ArrowDecoratedBox.IsShowArrowProperty.AddOwner<RangeTimePicker>();
   
   public static readonly StyledProperty<bool> IsPointAtCenterProperty =
      Flyout.IsPointAtCenterProperty.AddOwner<RangeTimePicker>();
   
   public static readonly StyledProperty<double> MarginToAnchorProperty =
      Popup.MarginToAnchorProperty.AddOwner<RangeTimePicker>();

   public static readonly StyledProperty<int> MouseEnterDelayProperty =
      FlyoutStateHelper.MouseEnterDelayProperty.AddOwner<RangeTimePicker>();

   public static readonly StyledProperty<int> MouseLeaveDelayProperty =
      FlyoutStateHelper.MouseLeaveDelayProperty.AddOwner<RangeTimePicker>();
   
   public static readonly StyledProperty<int> MinuteIncrementProperty =
      TimePicker.MinuteIncrementProperty.AddOwner<RangeTimePicker>();

   public static readonly StyledProperty<int> SecondIncrementProperty =
      TimePicker.SecondIncrementProperty.AddOwner<RangeTimePicker>();
   
   public static readonly StyledProperty<ClockIdentifierType> ClockIdentifierProperty =
      TimePicker.ClockIdentifierProperty.AddOwner<RangeTimePicker>();
   
   public static readonly StyledProperty<TimeSpan?> RangeStartSelectedTimeProperty =
      AvaloniaProperty.Register<RangeTimePicker, TimeSpan?>(nameof(RangeStartSelectedTime),
                                                            defaultBindingMode: BindingMode.TwoWay,
                                                            enableDataValidation: true);
   
   public static readonly StyledProperty<TimeSpan?> RangeEndSelectedTimeProperty =
      AvaloniaProperty.Register<RangeTimePicker, TimeSpan?>(nameof(RangeEndSelectedTime),
                                                            defaultBindingMode: BindingMode.TwoWay,
                                                            enableDataValidation: true);
   
   public static readonly StyledProperty<TimeSpan?> RangeStartDefaultTimeProperty =
      AvaloniaProperty.Register<RangeTimePicker, TimeSpan?>(nameof(RangeStartDefaultTime),
                                                            enableDataValidation: true);
   
   public static readonly StyledProperty<TimeSpan?> RangeEndDefaultTimeProperty =
      AvaloniaProperty.Register<RangeTimePicker, TimeSpan?>(nameof(RangeEndDefaultTime),
                                                            enableDataValidation: true);
   
   public object? LeftAddOn
   {
      get => GetValue(LeftAddOnProperty);
      set => SetValue(LeftAddOnProperty, value);
   }

   public object? RightAddOn
   {
      get => GetValue(RightAddOnProperty);
      set => SetValue(RightAddOnProperty, value);
   }
   
   public object? InnerLeftContent
   {
      get => GetValue(InnerLeftContentProperty);
      set => SetValue(InnerLeftContentProperty, value);
   }

   public object? InnerRightContent
   {
      get => GetValue(InnerRightContentProperty);
      set => SetValue(InnerRightContentProperty, value);
   }

   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }

   public AddOnDecoratedVariant StyleVariant
   {
      get => GetValue(StyleVariantProperty);
      set => SetValue(StyleVariantProperty, value);
   }

   public AddOnDecoratedStatus Status
   {
      get => GetValue(StatusProperty);
      set => SetValue(StatusProperty, value);
   }
   
   public string? RangeStartWatermark
   {
      get => GetValue(RangeStartWatermarkProperty);
      set => SetValue(RangeStartWatermarkProperty, value);
   }
   
   public string? RangeEndWatermark
   {
      get => GetValue(RangeEndWatermarkProperty);
      set => SetValue(RangeEndWatermarkProperty, value);
   }
   
   public PlacementMode PickerPlacement
   {
      get => GetValue(PickerPlacementProperty);
      set => SetValue(PickerPlacementProperty, value);
   }
   
   public bool IsShowArrow
   {
      get => GetValue(IsShowArrowProperty);
      set => SetValue(IsShowArrowProperty, value);
   }
    
   public bool IsPointAtCenter
   {
      get => GetValue(IsPointAtCenterProperty);
      set => SetValue(IsPointAtCenterProperty, value);
   }
   
   public double MarginToAnchor
   {
      get => GetValue(MarginToAnchorProperty);
      set => SetValue(MarginToAnchorProperty, value);
   }

   public int MouseEnterDelay
   {
      get => GetValue(MouseEnterDelayProperty);
      set => SetValue(MouseEnterDelayProperty, value);
   }

   public int MouseLeaveDelay
   {
      get => GetValue(MouseLeaveDelayProperty);
      set => SetValue(MouseLeaveDelayProperty, value);
   }
   
   public int MinuteIncrement
   {
      get => GetValue(MinuteIncrementProperty);
      set => SetValue(MinuteIncrementProperty, value);
   }

   public int SecondIncrement
   {
      get => GetValue(SecondIncrementProperty);
      set => SetValue(SecondIncrementProperty, value);
   }
   
   public ClockIdentifierType ClockIdentifier
   {
      get => GetValue(ClockIdentifierProperty);
      set => SetValue(ClockIdentifierProperty, value);
   }
   
   public TimeSpan? RangeStartSelectedTime
   {
      get => GetValue(RangeStartSelectedTimeProperty);
      set => SetValue(RangeStartSelectedTimeProperty, value);
   }
      
   public TimeSpan? RangeEndSelectedTime
   {
      get => GetValue(RangeEndSelectedTimeProperty);
      set => SetValue(RangeEndSelectedTimeProperty, value);
   }
   
   public TimeSpan? RangeStartDefaultTime
   {
      get => GetValue(RangeStartDefaultTimeProperty);
      set => SetValue(RangeStartDefaultTimeProperty, value);
   }
   
   public TimeSpan? RangeEndDefaultTime
   {
      get => GetValue(RangeEndDefaultTimeProperty);
      set => SetValue(RangeEndDefaultTimeProperty, value);
   }

   #endregion

   #region 内部属性定义

   internal static readonly DirectProperty<RangeTimePicker, RangeActivatedPart> RangeActivatedPartProperty =
      AvaloniaProperty.RegisterDirect<RangeTimePicker, RangeActivatedPart>(nameof(RangeActivatedPart),
         o => o.RangeActivatedPart);

   private RangeActivatedPart _rangeActivatedPart;
   internal RangeActivatedPart RangeActivatedPart
   {
      get => _rangeActivatedPart;
      set => SetAndRaise(RangeActivatedPartProperty, ref _rangeActivatedPart, value);
   }

   internal static readonly StyledProperty<double> PickerIndicatorOffsetXProperty =
      AvaloniaProperty.Register<RangeTimePicker, double>(nameof(PickerIndicatorOffsetX), Double.NaN);

   internal double PickerIndicatorOffsetX
   {
      get => GetValue(PickerIndicatorOffsetXProperty);
      set => SetValue(PickerIndicatorOffsetXProperty, value);
   }
   
   internal static readonly StyledProperty<double> PickerIndicatorOffsetYProperty =
      AvaloniaProperty.Register<RangeTimePicker, double>(nameof(PickerIndicatorOffsetY));

   internal double PickerIndicatorOffsetY
   {
      get => GetValue(PickerIndicatorOffsetYProperty);
      set => SetValue(PickerIndicatorOffsetYProperty, value);
   }

   #endregion

   private AddOnDecoratedBox? _decoratedBox;
   private PickerClearUpButton? _pickerClearUpButton;
   private readonly FlyoutStateHelper _flyoutStateHelper;
   private RangeTimePickerFlyout? _pickerFlyout;
   private Rectangle? _rangePickerIndicator;
   private TextBox? _rangeStartTextBox;
   private TextBox? _rangeEndTextBox;
   private bool _currentValidSelected;
   private IDisposable? _clearUpButtonDetectDisposable;
   private AddOnDecoratedInnerBox? _rangePickerInner;

   static RangeTimePicker()
   {
      HorizontalAlignmentProperty.OverrideDefaultValue<RangeTimePicker>(HorizontalAlignment.Left);
      VerticalAlignmentProperty.OverrideDefaultValue<RangeTimePicker>(VerticalAlignment.Top);
      AffectsArrange<RangeTimePicker>(PickerIndicatorOffsetXProperty, PickerIndicatorOffsetYProperty);
   }
   
   public RangeTimePicker()
   {
      _flyoutStateHelper = new FlyoutStateHelper()
      {
         TriggerType = FlyoutTriggerType.Click
      };
      _flyoutStateHelper.FlyoutAboutToShow += HandleFlyoutAboutToShow;
      _flyoutStateHelper.FlyoutAboutToClose += HandleFlyoutAboutToClose;
      _flyoutStateHelper.OpenFlyoutPredicate = FlyoutOpenPredicate;
      _flyoutStateHelper.ClickHideFlyoutPredicate = ClickHideFlyoutPredicate;
   }
   
    private bool FlyoutOpenPredicate(Point position)
   {
      if (!IsEnabled) {
         return false;
      }
      
      return PositionInEditKernel(position);
   }

   private bool PositionInEditKernel(Point position)
   {
      if (_rangeStartTextBox is not null && _rangeEndTextBox is not null) {
         return PositionInRangeStartTextBox(position) ||
                PositionInRangeEndTextBox(position);
      }
      return false;
   }

   private bool PositionInRangeStartTextBox(Point position)
   {
      if (_rangeStartTextBox is null) {
         return false;
      }
      var pos = _rangeStartTextBox.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
      if (!pos.HasValue) {
         return false;
      }
      var targetWidth = _rangeStartTextBox.Bounds.Width;
      var targetHeight = _rangeStartTextBox.Bounds.Height;
      var startOffsetX = pos.Value.X;
      var endOffsetX = startOffsetX + targetWidth;
      var offsetY = pos.Value.Y;
      if (InnerLeftContent is Control leftContent) {
         var leftContentPos = leftContent.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
         if (leftContentPos.HasValue) {
            startOffsetX = leftContentPos.Value.X + leftContent.Bounds.Width;
         }
      }
      
      targetWidth = endOffsetX - startOffsetX;
      var bounds = new Rect(new Point(startOffsetX, offsetY), new Size(targetWidth, targetHeight));
      if (bounds.Contains(position)) {
         RangeActivatedPart = RangeActivatedPart.Start;
         return true;
      }

      return false;
   }
   
   private bool PositionInRangeEndTextBox(Point position)
   {
      if (_rangeEndTextBox is null) {
         return false;
      }
      var pos = _rangeEndTextBox.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
      if (!pos.HasValue) {
         return false;
      }
      var targetWidth = _rangeEndTextBox.Bounds.Width;
      var targetHeight = _rangeEndTextBox.Bounds.Height;
      var startOffsetX = pos.Value.X;
      var endOffsetX = startOffsetX + targetWidth;
      var offsetY = pos.Value.Y;
      
      if (InnerRightContent is Control rightContent) {
         var rightContentPos = rightContent.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
         if (rightContentPos.HasValue) {
            endOffsetX = rightContentPos.Value.X;
         }
      }
      
      targetWidth = endOffsetX - startOffsetX;
      var bounds = new Rect(new Point(startOffsetX, offsetY), new Size(targetWidth, targetHeight));
      if (bounds.Contains(position)) {
         RangeActivatedPart = RangeActivatedPart.End;
         return true;
      }

      return false;
   }

   private bool ClickHideFlyoutPredicate(IPopupHostProvider hostProvider, RawPointerEventArgs args)
   {
      if (hostProvider.PopupHost != args.Root) {
         if (!PositionInEditKernel(args.Position)) {
            return true;
         }
      }

      return false;
   }

   private void HandleFlyoutAboutToShow(object? sender, EventArgs args)
   {
      _rangeStartTextBox?.Focus();
      _currentValidSelected = false;
   }
   
   private void HandleFlyoutAboutToClose(object? sender, EventArgs args)
   {
      RangeActivatedPart = RangeActivatedPart.None;
      // if (!_currentValidSelected) {
      //    if (SelectedTime.HasValue) {
      //       Text = DateTimeUtils.FormatTimeSpan(SelectedTime.Value, ClockIdentifier == ClockIdentifierType.HourClock12);
      //    } else {
      //       ResetTimeValue();
      //    }
      // }
   }
   
   protected override Size ArrangeOverride(Size finalSize)
   {
      var borderThickness = _decoratedBox?.BorderThickness ?? default;
      var size = base.ArrangeOverride(finalSize).Inflate(borderThickness);
      if (_rangePickerIndicator is not null) {
         Canvas.SetLeft(_rangePickerIndicator, PickerIndicatorOffsetX);
         Canvas.SetTop(_rangePickerIndicator, PickerIndicatorOffsetY);
      }

      return size;
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _decoratedBox = e.NameScope.Get<AddOnDecoratedBox>(RangeTimePickerTheme.DecoratedBoxPart);
      _rangeStartTextBox = e.NameScope.Get<TextBox>(RangeTimePickerTheme.RangeStartTextBoxPart);
      _rangeEndTextBox = e.NameScope.Get<TextBox>(RangeTimePickerTheme.RangeEndTextBoxPart);
      _rangePickerIndicator = e.NameScope.Get<Rectangle>(RangeTimePickerTheme.RangePickerIndicatorPart);
      
      _rangePickerInner = e.NameScope.Get<AddOnDecoratedInnerBox>(RangeTimePickerTheme.RangePickerInnerPart);
      if (InnerRightContent is null) {
         _pickerClearUpButton = new PickerClearUpButton();
         _pickerClearUpButton.ClearRequest += (sender, args) =>
         {
            // ResetTimeValue();
            // SelectedTime = null;
         };
         InnerRightContent = _pickerClearUpButton;
      }
      
      if (_pickerFlyout is null) {
         _pickerFlyout = new RangeTimePickerFlyout(this);
         _flyoutStateHelper.Flyout = _pickerFlyout;
      }
      
      _flyoutStateHelper.AnchorTarget = _rangePickerInner;
      SetupFlyoutProperties();

      if (_rangePickerIndicator.Transitions is null) {
         _rangePickerIndicator.Transitions = new Transitions()
         {
            AnimationUtils.CreateTransition<DoubleTransition>(Rectangle.OpacityProperty),
            AnimationUtils.CreateTransition<DoubleTransition>(Rectangle.OpacityProperty)
         };
      }

      if (Transitions is null) {
         Transitions = new Transitions()
         {
            AnimationUtils.CreateTransition<DoubleTransition>(PickerIndicatorOffsetXProperty)
         };
      }
   }

   private void SetupPickerIndicatorPosition()
   {
      if (_rangePickerIndicator is null || 
          _decoratedBox is null || 
          _rangeStartTextBox is null ||
          _rangeEndTextBox is null) {
         return;
      }
      
      if (_rangeActivatedPart == RangeActivatedPart.None) {
         _rangePickerIndicator.Opacity = 0;
      } else if (_rangeActivatedPart == RangeActivatedPart.Start) {
         _rangePickerIndicator.Opacity = 1;
         _rangePickerIndicator.Width = _rangeStartTextBox.Bounds.Width;
         var offset = _rangeStartTextBox.TranslatePoint(new Point(0, 0), this) ?? default;
         PickerIndicatorOffsetX = offset.X;
      } else if (_rangeActivatedPart == RangeActivatedPart.End) {
         _rangePickerIndicator.Opacity = 1;
         _rangePickerIndicator.Width = _rangeEndTextBox.Bounds.Width;
         var offset = _rangeEndTextBox.TranslatePoint(new Point(0, 0), this) ?? default;
         PickerIndicatorOffsetX = offset.X;
      }
   }
   
   protected void SetupFlyoutProperties()
   {
      if (_pickerFlyout is not null) {
         BindUtils.RelayBind(this, PickerPlacementProperty, _pickerFlyout, RangeTimePickerFlyout.PlacementProperty);
         BindUtils.RelayBind(this, IsShowArrowProperty, _pickerFlyout);
         BindUtils.RelayBind(this, IsPointAtCenterProperty, _pickerFlyout);
         BindUtils.RelayBind(this, MarginToAnchorProperty, _pickerFlyout);
      }
   }
   
   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      BindUtils.RelayBind(this, MouseEnterDelayProperty, _flyoutStateHelper, FlyoutStateHelper.MouseEnterDelayProperty);
      BindUtils.RelayBind(this, MouseLeaveDelayProperty, _flyoutStateHelper, FlyoutStateHelper.MouseLeaveDelayProperty);
      // if (DefaultTime is not null) {
      //    SelectedTime = DefaultTime;
      // }
   }
   
   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      _flyoutStateHelper.NotifyAttachedToVisualTree();
      if (_clearUpButtonDetectDisposable is null) {
         var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
         _clearUpButtonDetectDisposable = inputManager.Process.Subscribe(DetectClearUpButtonState);
      }
   }
   
   private void DetectClearUpButtonState(RawInputEventArgs args)
   {
      if (IsEnabled) {
         if (args is RawPointerEventArgs pointerEventArgs) {
            // if (_textBoxInnerBox is not null) {
            //    var pos = _textBoxInnerBox.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
            //    if (!pos.HasValue) {
            //       return;
            //    }
            //
            //    var bounds = new Rect(pos.Value, _textBoxInnerBox.Bounds.Size);
            //    if (bounds.Contains(pointerEventArgs.Position)) {
            //       if (SelectedTime is not null) {
            //          _pickerClearUpButton!.IsInClearMode = true;
            //       }
            //    } else {
            //       _pickerClearUpButton!.IsInClearMode = false;
            //    }
            // }
         }
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (change.Property == RangeActivatedPartProperty) {
         HandleRangeActivatedPartChanged();
      }
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      if (_decoratedBox is not null) {
         PickerIndicatorOffsetY = _decoratedBox.DesiredSize.Height - _rangePickerIndicator!.Height;
      }

      if (double.IsNaN(PickerIndicatorOffsetX)) {
         if (_rangeActivatedPart == RangeActivatedPart.None) {
            var offset = _rangeStartTextBox!.TranslatePoint(new Point(0, 0), this) ?? default;
            PickerIndicatorOffsetX = offset.X;
         }
      }
      return size;
   }

   private void HandleRangeActivatedPartChanged()
   {
      if (_rangeActivatedPart == RangeActivatedPart.Start) {
         PickerPlacement = PlacementMode.BottomEdgeAlignedLeft;
      } else if (_rangeActivatedPart == RangeActivatedPart.End) {
         PickerPlacement = PlacementMode.BottomEdgeAlignedRight;
      }

      SetupPickerIndicatorPosition();
   }

   internal void NotifyConfirmed(TimeSpan value)
   {
   }
   
   internal void NotifyTemporaryTimeSelected(TimeSpan selected)
   {
   }
   
   internal void ClosePickerFlyout()
   {
      _flyoutStateHelper.HideFlyout(true);
   }
}

internal enum RangeActivatedPart
{
   None,
   Start,
   End
}