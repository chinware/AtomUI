using AtomUI.Icon;
using AtomUI.Styling;
using Avalonia;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

public class IconButton : AvaloniaButton, ICustomHitTest
{
   public static readonly StyledProperty<PathIcon?> IconProperty
      = AvaloniaProperty.Register<IconButton, PathIcon?>(nameof(Icon));
   
   private ControlStyleState _styleState;
   
   public PathIcon? Icon
   {
      get => GetValue(IconProperty);
      set => SetValue(IconProperty, value);
   }

   private bool _initialized = false;

   static IconButton()
   {
      AffectsMeasure<IconButton>(IconProperty);
   }

   public IconButton()
   {
      Cursor = new Cursor(StandardCursorType.Hand);
   }
   
   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         if (Icon is not null) {
            Content = Icon;
         }
         _initialized = true;
      }
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      if (_initialized) {
         if (e.Property == IconProperty) {
            Content = e.GetNewValue<PathIcon?>();
         } else if (e.Property == IsPressedProperty ||
                    e.Property == IsPointerOverProperty) {
            CollectStyleState();
            if (Icon is not null) {
               if (_styleState.HasFlag(ControlStyleState.Enabled)) {
                  Icon.IconMode = IconMode.Normal;
                  if (_styleState.HasFlag(ControlStyleState.Active)) {
                     Icon.IconMode = IconMode.Selected;
                  } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
                     Icon.IconMode = IconMode.Active;
                  }
               } else {
                  Icon.IconMode = IconMode.Disabled;
               }
            }
         }
      }
   }
   
   private void CollectStyleState()
   {
      StyleUtils.InitCommonState(this, ref _styleState);
      if (IsPressed) {
         _styleState |= ControlStyleState.Sunken;
      } else {
         _styleState |= ControlStyleState.Raised;
      }
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      return size;
   }
   
   public bool HitTest(Point point)
   {
      return true;
   }
}