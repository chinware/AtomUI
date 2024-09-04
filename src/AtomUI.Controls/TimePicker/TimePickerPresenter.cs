using System.Globalization;
using AtomUI.Controls.TimePickerLang;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;

namespace AtomUI.Controls;

/// <summary>
/// Defines the presenter used for selecting a time. Intended for use with
/// <see cref="TimePicker"/> but can be used independently
/// </summary>
[TemplatePart(TimePickerPresenterTheme.HourSelectorPart, typeof(DateTimePickerPanel), IsRequired = true)]
[TemplatePart(TimePickerPresenterTheme.MinuteSelectorPart, typeof(DateTimePickerPanel), IsRequired = true)]
[TemplatePart(TimePickerPresenterTheme.SecondSelectorPart, typeof(DateTimePickerPanel), IsRequired = true)]
[TemplatePart(TimePickerPresenterTheme.PeriodHostPart, typeof(Panel), IsRequired = true)]
[TemplatePart(TimePickerPresenterTheme.PeriodSelectorPart, typeof(DateTimePickerPanel), IsRequired = true)]
[TemplatePart(TimePickerPresenterTheme.PickerContainerPart, typeof(Grid), IsRequired = true)]
[TemplatePart(TimePickerPresenterTheme.SecondSpacerPart, typeof(Rectangle), IsRequired = true)]
public class TimePickerPresenter : PickerPresenterBase
{
   #region 公共属性定义

   /// <summary>
   /// Defines the <see cref="MinuteIncrement"/> property
   /// </summary>
   public static readonly StyledProperty<int> MinuteIncrementProperty =
      TimePicker.MinuteIncrementProperty.AddOwner<TimePickerPresenter>();

   public static readonly StyledProperty<int> SecondIncrementProperty =
      TimePicker.SecondIncrementProperty.AddOwner<TimePickerPresenter>();

   /// <summary>
   /// Defines the <see cref="ClockIdentifier"/> property
   /// </summary>
   public static readonly StyledProperty<string> ClockIdentifierProperty =
      TimePicker.ClockIdentifierProperty.AddOwner<TimePickerPresenter>();

   /// <summary>
   /// Defines the <see cref="Time"/> property
   /// </summary>
   public static readonly StyledProperty<TimeSpan> TimeProperty =
      AvaloniaProperty.Register<TimePickerPresenter, TimeSpan>(nameof(Time));
   
   public static readonly StyledProperty<TimeSpan> TemporaryTimeProperty =
      AvaloniaProperty.Register<TimePickerPresenter, TimeSpan>(nameof(TemporaryTime));

   public static readonly StyledProperty<bool> IsShowHeaderProperty =
      AvaloniaProperty.Register<TimePickerPresenter, bool>(nameof(IsShowHeader), true);

   /// <summary>
   /// Gets or sets the minute increment in the selector
   /// </summary>
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

   /// <summary>
   /// Gets or sets the current clock identifier, either 12HourClock or 24HourClock
   /// </summary>
   public string ClockIdentifier
   {
      get => GetValue(ClockIdentifierProperty);
      set => SetValue(ClockIdentifierProperty, value);
   }

   /// <summary>
   /// Gets or sets the current time
   /// </summary>
   public TimeSpan Time
   {
      get => GetValue(TimeProperty);
      set => SetValue(TimeProperty, value);
   }
   
   public TimeSpan TemporaryTime
   {
      get => GetValue(TemporaryTimeProperty);
      set => SetValue(TemporaryTimeProperty, value);
   }

   public bool IsShowHeader
   {
      get => GetValue(IsShowHeaderProperty);
      set => SetValue(IsShowHeaderProperty, value);
   }

   #endregion

   #region 私有属性定义

   internal static readonly DirectProperty<TimePickerPresenter, double> SpacerThicknessProperty =
      AvaloniaProperty.RegisterDirect<TimePickerPresenter, double>(nameof(SpacerWidth),
                                                                   o => o.SpacerWidth,
                                                                   (o, v) => o.SpacerWidth = v);

   private double _spacerWidth;

   public double SpacerWidth
   {
      get => _spacerWidth;
      set => SetAndRaise(SpacerThicknessProperty, ref _spacerWidth, value);
   }

   #endregion

   public TimePickerPresenter()
   {
      SetCurrentValue(TimeProperty, DateTime.Now.TimeOfDay);
   }

   static TimePickerPresenter()
   {
      KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<TimePickerPresenter>(KeyboardNavigationMode.Cycle);
   }

   // TemplateItems
   private Grid? _pickerContainer;
   private Rectangle? _spacer3;
   private Panel? _periodHost;
   private TextBlock? _headerText;
   private DateTimePickerPanel? _hourSelector;
   private DateTimePickerPanel? _minuteSelector;
   private DateTimePickerPanel? _secondSelector;
   private DateTimePickerPanel? _periodSelector;

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      TokenResourceBinder.CreateGlobalTokenBinding(this, SpacerThicknessProperty, GlobalTokenResourceKey.LineWidth,
                                                   BindingPriority.Template,
                                                   new RenderScaleAwareDoubleConfigure(this));
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);

      _pickerContainer = e.NameScope.Get<Grid>(TimePickerPresenterTheme.PickerContainerPart);
      _periodHost = e.NameScope.Get<Panel>(TimePickerPresenterTheme.PeriodHostPart);
      _headerText = e.NameScope.Get<TextBlock>(TimePickerPresenterTheme.HeaderTextPart);

      _hourSelector = e.NameScope.Get<DateTimePickerPanel>(TimePickerPresenterTheme.HourSelectorPart);
      _minuteSelector = e.NameScope.Get<DateTimePickerPanel>(TimePickerPresenterTheme.MinuteSelectorPart);
      _secondSelector = e.NameScope.Get<DateTimePickerPanel>(TimePickerPresenterTheme.SecondSelectorPart);
      _periodSelector = e.NameScope.Get<DateTimePickerPanel>(TimePickerPresenterTheme.PeriodSelectorPart);

      if (_hourSelector is not null) {
         _hourSelector.SelectionChanged += HandleSelectionChanged;
      }
      
      if (_minuteSelector is not null) {
         _minuteSelector.SelectionChanged += HandleSelectionChanged;
      }
      
      if (_secondSelector is not null) {
         _secondSelector.SelectionChanged += HandleSelectionChanged;
      }
      
      if (_periodSelector is not null) {
         _periodSelector.SelectionChanged += HandleSelectionChanged;
      }
      
      _spacer3 = e.NameScope.Get<Rectangle>(TimePickerPresenterTheme.ThirdSpacerPart);
      
      InitPicker();
   }

   private void HandleSelectionChanged(object? sender, EventArgs args)
   {
      var selectedValue = CollectValue();
      TemporaryTime = selectedValue;
      if (IsShowHeader) {
         var dateTime = DateTime.Today.Add(selectedValue);
         if (ClockIdentifier == "12HourClock") {
            var formatInfo = new DateTimeFormatInfo();
            formatInfo.AMDesignator = LanguageResourceBinder.GetLangResource(TimePickerLangResourceKey.AMText)!;
            formatInfo.PMDesignator = LanguageResourceBinder.GetLangResource(TimePickerLangResourceKey.PMText)!;
            if (_headerText is not null) {
               _headerText.Text = dateTime.ToString(@"hh:mm:ss tt", formatInfo);
            }
         } else {
            if (_headerText is not null) {
               _headerText.Text = dateTime.ToString(@"HH:mm:ss");
            }
         }
      }
   }

   private TimeSpan CollectValue()
   {
      var hour = _hourSelector!.SelectedValue;
      var minute = _minuteSelector!.SelectedValue;
      var second = _secondSelector!.SelectedValue;
      var period = _periodSelector!.SelectedValue;

      if (ClockIdentifier == "12HourClock") {
         hour = period == 1 ? (hour == 12) ? 12 : hour + 12 : period == 0 && hour == 12 ? 0 : hour;
      }

      return new TimeSpan(hour, minute, second);
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);

      if (change.Property == MinuteIncrementProperty ||
          change.Property == SecondIncrementProperty ||
          change.Property == ClockIdentifierProperty ||
          change.Property == TimeProperty) {
         InitPicker();
      }
   }

   protected override void OnKeyDown(KeyEventArgs e)
   {
      switch (e.Key) {
         case Key.Escape:
            OnDismiss();
            e.Handled = true;
            break;
         case Key.Tab:
            if (FocusUtils.GetFocusManager(this)?.GetFocusedElement() is { } focus) {
               var nextFocus = KeyboardNavigationHandler.GetNext(focus, NavigationDirection.Next);
               nextFocus?.Focus(NavigationMethod.Tab);
               e.Handled = true;
            }

            break;
         case Key.Enter:
            OnConfirmed();
            e.Handled = true;
            break;
      }

      base.OnKeyDown(e);
   }

   protected override void OnConfirmed()
   {
      var value = CollectValue();
      SetCurrentValue(TimeProperty, value);
      base.OnConfirmed();
   }

   internal void Confirm()
   {
      OnConfirmed();
   }

   internal void Dismiss()
   {
      OnDismiss();
   }
   
   private void InitPicker()
   {
      if (_pickerContainer == null) return;

      bool clock12 = ClockIdentifier == "12HourClock";
      bool use24HourClock = ClockIdentifier == "24HourClock";
      _hourSelector!.MaximumValue = clock12 ? 12 : 23;
      _hourSelector.MinimumValue = clock12 ? 1 : 0;
      _hourSelector.ItemFormat = "%h";
      var hour = Time.Hours;
      _hourSelector.SelectedValue = !clock12 ? hour :
         hour > 12 ? hour - 12 :
         hour == 0 ? 12 : hour;

      _minuteSelector!.MaximumValue = 59;
      _minuteSelector.MinimumValue = 0;
      _minuteSelector.Increment = MinuteIncrement;
      _minuteSelector.SelectedValue = Time.Minutes;
      _minuteSelector.ItemFormat = "mm";

      _secondSelector!.MaximumValue = 59;
      _secondSelector.MinimumValue = 0;
      _secondSelector.Increment = SecondIncrement;
      _secondSelector.SelectedValue = Time.Seconds;
      _secondSelector.ItemFormat = "ss";

      _periodSelector!.MaximumValue = 1;
      _periodSelector.MinimumValue = 0;
      _periodSelector.SelectedValue = hour >= 12 ? 1 : 0;
      
      _spacer3!.IsVisible = !use24HourClock;
      _periodHost!.IsVisible = !use24HourClock;

      _hourSelector?.Focus(NavigationMethod.Pointer);
   }
}