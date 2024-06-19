using System.Globalization;
using Avalonia.Data.Converters;

namespace AtomUI.Converters;

public class PositionToAngleConverter : IValueConverter
{
   public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
   {
      if (value is double d) {
         return d * 3.6;
      }

      return 0;
   }

   public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
   {
      if (value is double d) {
         return d / 3.6;
      }

      return 0;
   }
}