using AtomUI.Controls.Utils;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls.Presenters;
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

   private bool _initialized;
   private ControlStyleState _styleState;
   
   static ToggleIconButton()
   {
      AffectsMeasure<ToggleIconButton>(CheckedIconProperty);
      AffectsMeasure<ToggleIconButton>(UnCheckedIconProperty);
   }

   public ToggleIconButton()
   {
      Cursor = new Cursor(StandardCursorType.Hand);
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      SetupStatusIcon();
      if (!_initialized) {
         if (CheckedIcon is not null) {
            UIStructureUtils.SetTemplateParent(CheckedIcon, this);
         }
         if (UnCheckedIcon is not null) {
            UIStructureUtils.SetTemplateParent(UnCheckedIcon, this);
         }
         _initialized = true;
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (VisualRoot is not null) {
         if (change.Property == IsCheckedProperty) {
            SetupStatusIcon();
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

   public bool HitTest(Point point)
   {
      return true;
   }

}