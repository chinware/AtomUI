using AtomUI.Controls.Utils;
using AtomUI.Icon;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;

namespace AtomUI.Controls;

public class ToggleIconButton : ToggleButton
{
   #region 公共属性定义

   public static readonly StyledProperty<PathIcon?> CheckedIconProperty
      = AvaloniaProperty.Register<ToggleIconButton, PathIcon?>(nameof(CheckedIcon));
   
   public static readonly StyledProperty<PathIcon?> UnCheckedIconProperty
      = AvaloniaProperty.Register<ToggleIconButton, PathIcon?>(nameof(UnCheckedIcon));
   
   public PathIcon? CheckedIcon
   {
      get => GetValue(CheckedIconProperty);
      set => SetValue(CheckedIconProperty, value);
   }
   
   public PathIcon? UnCheckedIcon
   {
      get => GetValue(UnCheckedIconProperty);
      set => SetValue(UnCheckedIconProperty, value);
   }

   #endregion
   
   private ControlStyleState _styleState;
   
   static ToggleIconButton()
   {
      AffectsMeasure<ToggleIconButton>(CheckedIconProperty);
      AffectsMeasure<ToggleIconButton>(UnCheckedIconProperty);
      AffectsMeasure<ToggleIconButton>(IsCheckedProperty);
   }

   public ToggleIconButton()
   {
      Cursor = new Cursor(StandardCursorType.Hand);
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      SetupStatusIcon();
      if (CheckedIcon is not null) {
         CheckedIcon.SetCurrentValue(PathIcon.HorizontalAlignmentProperty, HorizontalAlignment.Center);
         CheckedIcon.SetCurrentValue(PathIcon.VerticalAlignmentProperty, VerticalAlignment.Center);
         UIStructureUtils.SetTemplateParent(CheckedIcon, this);
      }
      if (UnCheckedIcon is not null) {
         UnCheckedIcon.SetCurrentValue(PathIcon.HorizontalAlignmentProperty, HorizontalAlignment.Center);
         UnCheckedIcon.SetCurrentValue(PathIcon.VerticalAlignmentProperty, VerticalAlignment.Center);
         UIStructureUtils.SetTemplateParent(UnCheckedIcon, this);
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (VisualRoot is not null) {
         if (change.Property == IsCheckedProperty) {
            SetupStatusIcon();
         } else if (change.Property == IsPressedProperty ||
                    change.Property == IsPointerOverProperty) {
            CollectStyleState();
            var pathIcon = IsChecked.HasValue && IsChecked.Value ? CheckedIcon : UnCheckedIcon;
            if (pathIcon is not null) {
               if (_styleState.HasFlag(ControlStyleState.Enabled)) {
                  pathIcon.IconMode = IconMode.Normal;
                  if (_styleState.HasFlag(ControlStyleState.Sunken)) {
                     pathIcon.IconMode = IconMode.Selected;
                  } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
                     pathIcon.IconMode = IconMode.Active;
                  }
               } else {
                  pathIcon.IconMode = IconMode.Disabled;
               }
            }
         }
      }
   }

   private void SetupStatusIcon()
   {
      if (Presenter is not null) {
         if (IsChecked.HasValue && IsChecked.Value) {
            Presenter.Content = CheckedIcon;
         } else {
            Presenter.Content = UnCheckedIcon;
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