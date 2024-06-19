using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AtomUI.Controls;

public partial class SegmentedItem : IControlCustomStyle
{
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private TokenResourceBinder _tokenResourceBinder;
   private Label? _label;
   private bool _isPressed = false;
   private ControlStyleState _styleState;

   void IControlCustomStyle.SetupUi()
   {
      _label = new Label()
      {
         Content = Text,
         Padding = new Thickness(0),
         VerticalContentAlignment = VerticalAlignment.Center,
         HorizontalContentAlignment = HorizontalAlignment.Center,
         VerticalAlignment = VerticalAlignment.Center,
      };

      _customStyle.CollectStyleState();
      _customStyle.ApplySizeTypeStyleConfig();
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplyVariableStyleConfig();
      _customStyle.SetupTransitions();

      LogicalChildren.Add(_label);
      VisualChildren.Add(_label);

      ApplyIconStyleConfig();
   }

   void IControlCustomStyle.SetupTransitions()
   {
      Transitions = new Transitions()
      {
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty,
            GlobalResourceKey.MotionDurationMid)
      };
   }

   void IControlCustomStyle.CollectStyleState()
   {
      StyleUtils.InitCommonState(this, ref _styleState);
      if (IsPressed) {
         _styleState |= ControlStyleState.Sunken;
      } else {
         _styleState |= ControlStyleState.Raised;
      }

      if (IsCurrentItem) {
         _styleState |= ControlStyleState.Selected;
      }
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _tokenResourceBinder.AddBinding(PaddingXXSTokenProperty, GlobalResourceKey.PaddingXXS);
   }

   void IControlCustomStyle.ApplySizeTypeStyleConfig()
   {
      if (SizeType == SizeType.Small) {
         _tokenResourceBinder.AddBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeightSM);
      } else if (SizeType == SizeType.Middle) {
         _tokenResourceBinder.AddBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeight);
      } else if (SizeType == SizeType.Large) {
         _tokenResourceBinder.AddBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeightLG);
      }
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (_initialized) {
         if (e.Property == IsPointerOverProperty ||
             e.Property == IsPressedProperty ||
             e.Property == IsCurrentItemProperty) {
            _customStyle.CollectStyleState();
            _customStyle.ApplyVariableStyleConfig();
         } else if (e.Property == SizeTypeProperty) {
            _customStyle.ApplySizeTypeStyleConfig();
         } else if (e.Property == TextProperty) {
            _label!.Content = Text;
         } else if (e.Property == IconProperty) {
            var oldIcon = e.GetOldValue<PathIcon?>();
            if (oldIcon is not null) {
               _tokenResourceBinder.ReleaseBindings(oldIcon);
               LogicalChildren.Remove(oldIcon);
               VisualChildren.Remove(oldIcon);
            }

            ApplyIconStyleConfig();
         }
      }
   }

   // 设置大小和颜色
   private void ApplyIconStyleConfig()
   {
      if (Icon is not null) {
         if (Icon.ThemeType != IconThemeType.TwoTone) {
            _tokenResourceBinder.AddBinding(Icon, PathIcon.NormalFillBrushProperty, SegmentedResourceKey.ItemColor);
            _tokenResourceBinder.AddBinding(Icon, PathIcon.ActiveFilledBrushProperty, SegmentedResourceKey.ItemHoverColor);
            _tokenResourceBinder.AddBinding(Icon, PathIcon.SelectedFilledBrushProperty, SegmentedResourceKey.ItemSelectedColor);
         }

         if (SizeType == SizeType.Small) {
            _tokenResourceBinder.AddBinding(Icon, WidthProperty, GlobalResourceKey.IconSizeSM);
            _tokenResourceBinder.AddBinding(Icon, HeightProperty, GlobalResourceKey.IconSizeSM);
         } else if (SizeType == SizeType.Middle) {
            _tokenResourceBinder.AddBinding(Icon, WidthProperty, GlobalResourceKey.IconSize);
            _tokenResourceBinder.AddBinding(Icon, HeightProperty, GlobalResourceKey.IconSize);
         } else if (SizeType == SizeType.Large) {
            _tokenResourceBinder.AddBinding(Icon, WidthProperty, GlobalResourceKey.IconSizeLG);
            _tokenResourceBinder.AddBinding(Icon, HeightProperty, GlobalResourceKey.IconSizeLG);
         }

         LogicalChildren.Add(Icon);
         VisualChildren.Add(Icon);
      }
   }
}