using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace AtomUI.Controls;

using AvaloniaButtonSpinner = Avalonia.Controls.ButtonSpinner;

public class ButtonSpinner : AvaloniaButtonSpinner
{
   #region 公共属性定义

   public static readonly StyledProperty<object?> LeftAddOnProperty =
      AvaloniaProperty.Register<LineEdit, object?>(nameof(LeftAddOn));

   public static readonly StyledProperty<object?> RightAddOnProperty =
      AvaloniaProperty.Register<LineEdit, object?>(nameof(RightAddOn));

   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AddOnDecoratedBox.SizeTypeProperty.AddOwner<LineEdit>();

   public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
      AddOnDecoratedBox.StyleVariantProperty.AddOwner<LineEdit>();

   public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
      AddOnDecoratedBox.StatusProperty.AddOwner<LineEdit>();

   public object? LeftAddOn
   {
      get => GetValue(LeftAddOnProperty);
      set => SetValue(LeftAddOnProperty, value);
   }

   public object? RightAddOn
   {
      get => GetValue(RightAddOnProperty);
      set => SetValue(RightAddOnProperty, value);
   }

   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }

   public AddOnDecoratedVariant StyleVariant
   {
      get => GetValue(StyleVariantProperty);
      set => SetValue(StyleVariantProperty, value);
   }

   public AddOnDecoratedStatus Status
   {
      get => GetValue(StatusProperty);
      set => SetValue(StatusProperty, value);
   }

   #endregion

   private Border? _spinnerHandleDecorator;
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (change.Property == CornerRadiusProperty) {
         SetupSpinnerHandleCornerRadius();
      } else if (change.Property == ButtonSpinnerLocationProperty) {
         SetupSpinnerHandleCornerRadius();
      }
   }

   private void SetupSpinnerHandleCornerRadius()
   {
      if (_spinnerHandleDecorator is not null) {
         if (ButtonSpinnerLocation == Location.Left) {
            _spinnerHandleDecorator.CornerRadius = new CornerRadius(CornerRadius.TopLeft,
                                                                    0,
                                                                    0,
                                                                    CornerRadius.BottomLeft);
         } else {
            _spinnerHandleDecorator.CornerRadius = new CornerRadius(0,
                                                                    CornerRadius.TopRight,
                                                                    CornerRadius.BottomRight,
                                                                   0);
         }
      }
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      _spinnerHandleDecorator = e.NameScope.Find<Border>(ButtonSpinnerTheme.SpinnerHandleDecoratorPart);
      base.OnApplyTemplate(e);
      SetupSpinnerHandleCornerRadius();
   }
}