using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace AtomUI.Controls;

public class RangeTimePicker : TemplatedControl
{
   #region 公共属性定义
   
   public static readonly StyledProperty<object?> LeftAddOnProperty =
      AvaloniaProperty.Register<RangeTimePicker, object?>(nameof(LeftAddOn));

   public static readonly StyledProperty<object?> RightAddOnProperty =
      AvaloniaProperty.Register<RangeTimePicker, object?>(nameof(RightAddOn));
   
   public static readonly StyledProperty<object?> InnerLeftContentProperty 
      = AvaloniaProperty.Register<RangeTimePicker, object?>(nameof(InnerLeftContent));
   
   public static readonly StyledProperty<object?> InnerRightContentProperty 
      = AvaloniaProperty.Register<RangeTimePicker, object?>(nameof(InnerRightContent));

   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AddOnDecoratedBox.SizeTypeProperty.AddOwner<RangeTimePicker>();

   public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
      AddOnDecoratedBox.StyleVariantProperty.AddOwner<RangeTimePicker>();

   public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
      AddOnDecoratedBox.StatusProperty.AddOwner<RangeTimePicker>();
   
   public static readonly StyledProperty<string?> RangeStartWatermarkProperty =
      AvaloniaProperty.Register<TextBox, string?>(nameof(RangeStartWatermark));
   
   public static readonly StyledProperty<string?> RangeEndWatermarkProperty =
      AvaloniaProperty.Register<TextBox, string?>(nameof(RangeEndWatermark));
   
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
   
   public object? InnerLeftContent
   {
      get => GetValue(InnerLeftContentProperty);
      set => SetValue(InnerLeftContentProperty, value);
   }

   public object? InnerRightContent
   {
      get => GetValue(InnerRightContentProperty);
      set => SetValue(InnerRightContentProperty, value);
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
   
   public string? RangeStartWatermark
   {
      get => GetValue(RangeStartWatermarkProperty);
      set => SetValue(RangeStartWatermarkProperty, value);
   }
   
   public string? RangeEndWatermark
   {
      get => GetValue(RangeEndWatermarkProperty);
      set => SetValue(RangeEndWatermarkProperty, value);
   }
    
   #endregion

   private AddOnDecoratedBox? _decoratedBox;
   private PickerIndicator? _pickerIndicator;

   public RangeTimePicker()
   {
      HorizontalAlignmentProperty.OverrideDefaultValue<RangeTimePicker>(HorizontalAlignment.Left);
   }
   
   protected override Size ArrangeOverride(Size finalSize)
   {
      var borderThickness = _decoratedBox?.BorderThickness ?? default;
      return base.ArrangeOverride(finalSize).Inflate(borderThickness);
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      _decoratedBox = e.NameScope.Get<AddOnDecoratedBox>(RangeTimePickerTheme.DecoratedBoxPart);
      base.OnApplyTemplate(e);
      
      if (InnerRightContent is null) {
         _pickerIndicator = new PickerIndicator();
         _pickerIndicator.ClearRequest += (sender, args) =>
         {
            // ResetTimeValue();
            // SelectedTime = null;
         };
         InnerRightContent = _pickerIndicator;
      }
   }
}