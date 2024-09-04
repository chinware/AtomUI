using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class PickerIndicator : TemplatedControl
{
   public event EventHandler? ClearRequest;

   public static readonly StyledProperty<bool> IsInClearModeProperty =
      AvaloniaProperty.Register<PickerIndicator, bool>(nameof(IsInClearMode));

   public bool IsInClearMode
   {
      get => GetValue(IsInClearModeProperty);
      set => SetValue(IsInClearModeProperty, value);
   }
}