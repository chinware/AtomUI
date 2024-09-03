using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace AtomUI.Controls;

public class TimePicker : TemplatedControl
{
   /// <summary>
   /// Defines the <see cref="MinuteIncrement"/> property
   /// </summary>
   public static readonly StyledProperty<int> MinuteIncrementProperty =
      AvaloniaProperty.Register<TimePicker, int>(nameof(MinuteIncrement), 1, coerce: CoerceMinuteIncrement);
   
   public static readonly StyledProperty<int> SecondIncrementProperty =
      AvaloniaProperty.Register<TimePicker, int>(nameof(SecondIncrement), 1, coerce: CoerceSecondIncrement);
   
   /// <summary>
   /// Defines the <see cref="ClockIdentifier"/> property
   /// </summary>
   public static readonly StyledProperty<string> ClockIdentifierProperty =
      AvaloniaProperty.Register<TimePicker, string>(nameof(ClockIdentifier), "12HourClock", coerce: CoerceClockIdentifier);
   
   /// <summary>
   /// Defines the <see cref="SelectedTime"/> property
   /// </summary>
   public static readonly StyledProperty<TimeSpan?> SelectedTimeProperty =
      AvaloniaProperty.Register<TimePicker, TimeSpan?>(nameof(SelectedTime),
                                                       defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true);
   
   /// <summary>
   /// Gets or sets the minute increment in the picker
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
   /// Gets or sets the clock identifier, either 12HourClock or 24HourClock
   /// </summary>
   public string ClockIdentifier
   {

      get => GetValue(ClockIdentifierProperty);
      set => SetValue(ClockIdentifierProperty, value);
   }

   /// <summary>
   /// Gets or sets the selected time. Can be null.
   /// </summary>
   public TimeSpan? SelectedTime
   {
      get => GetValue(SelectedTimeProperty);
      set => SetValue(SelectedTimeProperty, value);
   }

   private static int CoerceMinuteIncrement(AvaloniaObject sender, int value)
   {
      if (value < 1 || value > 59)
         throw new ArgumentOutOfRangeException(null, "1 >= MinuteIncrement <= 59");

      return value;
   }
   
   private static int CoerceSecondIncrement(AvaloniaObject sender, int value)
   {
      if (value < 1 || value > 59)
         throw new ArgumentOutOfRangeException(null, "1 >= SecondIncrement <= 59");

      return value;
   }
   
   private static string CoerceClockIdentifier(AvaloniaObject sender, string value)
   {
      if (!(string.IsNullOrEmpty(value) || value == "12HourClock" || value == "24HourClock"))
         throw new ArgumentException("Invalid ClockIdentifier", default(string));

      return value;
   }
   
}