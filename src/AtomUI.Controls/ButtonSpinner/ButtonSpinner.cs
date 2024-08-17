using Avalonia;

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
}