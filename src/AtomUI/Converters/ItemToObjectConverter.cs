using System.Globalization;
using Avalonia.Data.Converters;

namespace AtomUI.Converters;

public class ItemToObjectConverter : IValueConverter
{
   public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
   {
      if (value is int i) {
         return Enumerable.Repeat(new object(), i).ToList();
      }

      return new List<object>();
   }

   public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
   {
      throw new NotImplementedException();
   }
}