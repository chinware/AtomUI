using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace AtomUI.Controls;

internal class RangeTimePickerFlyoutPresenter : FlyoutPresenter
{
   public static readonly StyledProperty<int> MinuteIncrementProperty =
      TimePicker.MinuteIncrementProperty.AddOwner<RangeTimePickerFlyoutPresenter>();

   public static readonly StyledProperty<int> SecondIncrementProperty =
      TimePicker.SecondIncrementProperty.AddOwner<RangeTimePickerFlyoutPresenter>();
   
   public static readonly StyledProperty<TimeSpan> TimeProperty =
      AvaloniaProperty.Register<RangeTimePickerFlyoutPresenter, TimeSpan>(nameof(Time));
   
   public static readonly StyledProperty<ClockIdentifierType> ClockIdentifierProperty =
      TimePicker.ClockIdentifierProperty.AddOwner<RangeTimePickerFlyoutPresenter>();
   
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

   public TimeSpan Time
   {
      get => GetValue(TimeProperty);
      set => SetValue(TimeProperty, value);
   }
   
   public ClockIdentifierType ClockIdentifier
   {
      get => GetValue(ClockIdentifierProperty);
      set => SetValue(ClockIdentifierProperty, value);
   }
   
   protected override Type StyleKeyOverride => typeof(TimePickerFlyoutPresenter);
   
   internal RangeTimePicker TimePickerRef { get; set; }

   private TimePickerPresenter? _timePickerPresenter;
   private CompositeDisposable? _compositeDisposable;
   private Button? _confirmButton;
   private Button? _nowButton;
   
   public RangeTimePickerFlyoutPresenter(RangeTimePicker timePicker)
   {
      TimePickerRef = timePicker;
      HorizontalAlignment = HorizontalAlignment.Left;
      SetCurrentValue(TimeProperty, DateTime.Now.TimeOfDay);
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _timePickerPresenter = e.NameScope.Get<TimePickerPresenter>(TimePickerFlyoutPresenterTheme.ContentPresenterPart);
      _confirmButton = e.NameScope.Get<Button>(TimePickerFlyoutPresenterTheme.ConfirmButtonPart);
      _nowButton = e.NameScope.Get<Button>(TimePickerFlyoutPresenterTheme.NowButtonPart);
      if (_timePickerPresenter is not null) {
         _timePickerPresenter.Confirmed += (sender, args) =>
         {
            TimePickerRef.NotifyConfirmed(_timePickerPresenter.Time);
         };

         SetupTime();
      }

      if (_confirmButton is not null) {
         _confirmButton.Click += HandleConfirmButtonClicked;
      }

      if (_nowButton is not null) {
         _nowButton.Click += HandleNowButtonClicked;
      }
   }

   private void SetupTime()
   {
      if (TimePickerRef.RangeActivatedPart == RangeActivatedPart.Start) {
         if (TimePickerRef.RangeStartSelectedTime is not null) {
            _timePickerPresenter!.Time = TimePickerRef.RangeStartSelectedTime.Value;
     
         }
      } else if (TimePickerRef.RangeActivatedPart == RangeActivatedPart.End) {
         if (TimePickerRef.RangeEndSelectedTime is not null) {
            _timePickerPresenter!.Time = TimePickerRef.RangeEndSelectedTime.Value;
         }
      }
   }
   
   private void HandleNowButtonClicked(object? sender, RoutedEventArgs args)
   {
      _timePickerPresenter?.NowConfirm();
      TimePickerRef.ClosePickerFlyout();
   }

   private void HandleConfirmButtonClicked(object? sender, RoutedEventArgs args)
   {
      _timePickerPresenter?.Confirm();
      if (TimePickerRef.RangeActivatedPart == RangeActivatedPart.Start) {
         if (TimePickerRef.RangeEndSelectedTime is null) {
            TimePickerRef.RangeActivatedPart = RangeActivatedPart.End;
            return;
         }
      } else if (TimePickerRef.RangeActivatedPart == RangeActivatedPart.End) {
         if (TimePickerRef.RangeStartSelectedTime is null) {
            TimePickerRef.RangeActivatedPart = RangeActivatedPart.Start;
            return;
         }
      }
      TimePickerRef.ClosePickerFlyout();
   }
   
   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      _compositeDisposable = new CompositeDisposable();
      if (_timePickerPresenter is not null) {
         _compositeDisposable.Add(TimePickerPresenter.TemporaryTimeProperty.Changed.Subscribe(args =>
         {
            TimePickerRef.NotifyTemporaryTimeSelected(args.GetNewValue<TimeSpan>());
         }));
      }
      _compositeDisposable.Add(RangeTimePicker.RangeActivatedPartProperty.Changed.Subscribe(HandleRangeActivatedPartChanged));
      SetupTime();
   }

   private void HandleRangeActivatedPartChanged(AvaloniaPropertyChangedEventArgs args)
   {
      SetupTime();
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      _compositeDisposable?.Dispose();
      _compositeDisposable = null;
   }
}