using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace AtomUI.Controls;

using AvaloniaComboBox = Avalonia.Controls.ComboBox;

public class ComboBox : AvaloniaComboBox
{
   #region 公共属性定义

   public static readonly StyledProperty<object?> LeftAddOnProperty =
      AddOnDecoratedBox.LeftAddOnProperty.AddOwner<ComboBox>();

   public static readonly StyledProperty<object?> RightAddOnProperty =
      AddOnDecoratedBox.RightAddOnProperty.AddOwner<ComboBox>();
   
   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AddOnDecoratedBox.SizeTypeProperty.AddOwner<ComboBox>();

   public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
      AddOnDecoratedBox.StyleVariantProperty.AddOwner<ComboBox>();

   public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
      AddOnDecoratedBox.StatusProperty.AddOwner<ComboBox>();
   
   public static readonly StyledProperty<object?> InnerLeftContentProperty 
      = TextBox.InnerLeftContentProperty.AddOwner<ComboBox>();
   
   public static readonly StyledProperty<object?> InnerRightContentProperty 
      = TextBox.InnerRightContentProperty.AddOwner<ComboBox>();
   
   public static readonly StyledProperty<bool> IsEnableClearButtonProperty =
      TextBox.IsEnableClearButtonProperty.AddOwner<ComboBox>();
   
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
   
   public bool IsEnableClearButton
   {
      get => GetValue(IsEnableClearButtonProperty);
      set => SetValue(IsEnableClearButtonProperty, value);
   }
   
   #endregion

   static ComboBox()
   {
      ComboBox.HorizontalAlignmentProperty.OverrideDefaultValue<ComboBox>(HorizontalAlignment.Left);
      ComboBox.VerticalAlignmentProperty.OverrideDefaultValue<ComboBox>(VerticalAlignment.Top);
   }

   private IconButton? _openIndicatorButton;

   protected override Size ArrangeOverride(Size finalSize)
   {
      var size = base.ArrangeOverride(finalSize);
      if (_popup is not null) {
         _popup.MinWidth = size.Width;
      }

      return size;
   }

   private Popup? _popup;
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _popup = e.NameScope.Find<Popup>(ComboBoxTheme.PopupPart);
      _openIndicatorButton = e.NameScope.Find<IconButton>(ComboBoxTheme.OpenIndicatorButtonPart);
      if (_openIndicatorButton is not null) {
         _openIndicatorButton.Click += (sender, args) =>
         {
            SetCurrentValue(IsDropDownOpenProperty, true);
         };
      }
   }
   
   protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
   {
      base.PrepareContainerForItemOverride(container, item, index);
      if (container is ComboBoxItem comboBoxItem) {
         BindUtils.RelayBind(this, SizeTypeProperty, comboBoxItem, ComboBoxItem.SizeTypeProperty);
      }
   }
}