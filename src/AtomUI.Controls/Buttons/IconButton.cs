using AtomUI.Icon;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
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

   static IconButton()
   {
      AffectsMeasure<IconButton>(IconProperty);
   }

   public IconButton()
   {
      Cursor = new Cursor(StandardCursorType.Hand);
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      if (Icon is not null) {
         Icon.SetCurrentValue(PathIcon.HorizontalAlignmentProperty, HorizontalAlignment.Center);
         Icon.SetCurrentValue(PathIcon.VerticalAlignmentProperty, VerticalAlignment.Center);
         Content = Icon;
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      if (VisualRoot is not null) {
     
         if (e.Property == IconProperty) {
            var oldIcon = e.GetOldValue<PathIcon?>();
            if (oldIcon is not null) {
               ((ISetLogicalParent)oldIcon).SetParent(null);
            }
            Content = e.GetNewValue<PathIcon?>();
         } else if (e.Property == IsPressedProperty ||
                    e.Property == IsPointerOverProperty) {
            CollectStyleState();
            if (Icon is not null) {
               if (_styleState.HasFlag(ControlStyleState.Enabled)) {
                  Icon.IconMode = IconMode.Normal;
                  if (_styleState.HasFlag(ControlStyleState.Sunken)) {
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
      ControlStateUtils.InitCommonState(this, ref _styleState);
      if (IsPressed) {
         _styleState |= ControlStyleState.Sunken;
      } else {
         _styleState |= ControlStyleState.Raised;
      }
   }
   
   public bool HitTest(Point point)
   {
      return true;
   }
}