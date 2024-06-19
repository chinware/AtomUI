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
   public static readonly DirectProperty<IconButton, PathIcon?> IconProperty
      = AvaloniaProperty.RegisterDirect<IconButton, PathIcon?>(nameof(Icon),
         o => o.Icon,
         (o, v) => o.Icon = v);
   
   private ControlStyleState _styleState;

   private PathIcon? _icon;
   public PathIcon? Icon
   {
      get => _icon;
      set => SetAndRaise(IconProperty, ref _icon, value);
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
         if (_icon is not null) {
            Content = _icon;
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
            if (_icon is not null) {
               if (_styleState.HasFlag(ControlStyleState.Enabled)) {
                  _icon.IconMode = IconMode.Normal;
                  if (_styleState.HasFlag(ControlStyleState.Active)) {
                     _icon.IconMode = IconMode.Selected;
                  } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
                     _icon.IconMode = IconMode.Active;
                  }
               } else {
                  _icon.IconMode = IconMode.Disabled;
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