using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// Specifies values for the different modes of operation of a
/// <see cref="T:Avalonia.Controls.Calendar" />.
/// </summary>
public enum CalendarMode
{
   /// <summary>
   /// The <see cref="T:Avalonia.Controls.Calendar" /> displays a
   /// month at a time.
   /// </summary>
   Month = 0,

   /// <summary>
   /// The <see cref="T:Avalonia.Controls.Calendar" /> displays a
   /// year at a time.
   /// </summary>
   Year = 1,

   /// <summary>
   /// The <see cref="T:Avalonia.Controls.Calendar" /> displays a
   /// decade at a time.
   /// </summary>
   Decade = 2
}

/// <summary>
/// Specifies values that describe the available selection modes for a
/// <see cref="T:Avalonia.Controls.Calendar" />.
/// </summary>
/// <remarks>
/// This enumeration provides the values that are used by the SelectionMode
/// property.
/// </remarks>
internal enum CalendarSelectionMode
{
   /// <summary>
   /// Only a single date can be selected. Use the
   /// <see cref="P:Avalonia.Controls.Calendar.SelectedDate" />
   /// property to retrieve the selected date.
   /// </summary>
   SingleDate = 0,

   /// <summary>
   /// A single range of dates can be selected. Use
   /// <see cref="P:Avalonia.Controls.Calendar.SelectedDates" />
   /// property to retrieve the selected dates.
   /// </summary>
   SingleRange = 1,

   /// <summary>
   /// Multiple non-contiguous ranges of dates can be selected. Use the
   /// <see cref="P:Avalonia.Controls.Calendar.SelectedDates" />
   /// property to retrieve the selected dates.
   /// </summary>
   MultipleRange = 2,

   /// <summary>
   /// No selections are allowed.
   /// </summary>
   None = 3
}

/// <summary>
/// Provides data for the
/// <see cref="E:Avalonia.Controls.Calendar.DisplayDateChanged" />
/// event.
/// </summary>
internal class CalendarDateChangedEventArgs : RoutedEventArgs
{
   /// <summary>
   /// Initializes a new instance of the CalendarDateChangedEventArgs
   /// class.
   /// </summary>
   /// <param name="removedDate">
   /// The date that was previously displayed.
   /// </param>
   /// <param name="addedDate">The date to be newly displayed.</param>
   internal CalendarDateChangedEventArgs(DateTime? removedDate, DateTime? addedDate)
    {
        RemovedDate = removedDate;
        AddedDate   = addedDate;
    }

   /// <summary>
   /// Gets the date that was previously displayed.
   /// </summary>
   /// <value>
   /// The date previously displayed.
   /// </value>
   public DateTime? RemovedDate { get; private set; }

   /// <summary>
   /// Gets the date to be newly displayed.
   /// </summary>
   /// <value>The new date to display.</value>
   public DateTime? AddedDate { get; private set; }
}

/// <summary>
/// Provides data for the
/// <see cref="E:Avalonia.Controls.Calendar.DisplayModeChanged" />
/// event.
/// </summary>
internal class CalendarModeChangedEventArgs : RoutedEventArgs
{
   /// <summary>
   /// Initializes a new instance of the
   /// <see cref="T:Avalonia.Controls.CalendarModeChangedEventArgs" />
   /// class.
   /// </summary>
   /// <param name="oldMode">The previous mode.</param>
   /// <param name="newMode">The new mode.</param>
   public CalendarModeChangedEventArgs(CalendarMode oldMode, CalendarMode newMode)
    {
        OldMode = oldMode;
        NewMode = newMode;
    }

   /// <summary>
   /// Gets the previous mode of the
   /// <see cref="T:Avalonia.Controls.Calendar" />.
   /// </summary>
   /// <value>
   /// A <see cref="T:Avalonia.Controls.CalendarMode" /> representing
   /// the previous mode.
   /// </value>
   public CalendarMode OldMode { get; private set; }

   /// <summary>
   /// Gets the new mode of the
   /// <see cref="T:Avalonia.Controls.Calendar" />.
   /// </summary>
   /// <value>
   /// A <see cref="T:Avalonia.Controls.CalendarMode" />
   /// the new mode.
   /// </value>
   public CalendarMode NewMode { get; private set; }
}
