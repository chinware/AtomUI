using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ComboBoxTheme : BaseControlTheme
{
   public const string DecoratedBoxPart = "PART_DecoratedBox";
   public const string SpinnerInnerBoxPart = "PART_SpinnerInnerBox";
   public const string OpenIndicatorButtonPart = "PART_OpenIndicatorButton";
   public const string SpinnerHandleDecoratorPart = "PART_SpinnerHandleDecorator";
   public const string ItemsPresenterPart = "PART_ItemsPresenter";
   public const string PopupPart = "PART_Popup";
   public const string PlaceholderTextPart = "PART_PlaceholderText";
   public const string SelectedContentPresenterPart = "PART_SelectedContentPresenter";
   
   public ComboBoxTheme() : base(typeof(ComboBox)) {}
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<ComboBox>((comboBox, scope) =>
      {
         var panel = new Panel();
         var decoratedBox = BuildSpinnerDecoratedBox(comboBox, scope);
         var innerBox = BuildSpinnerContent(comboBox, scope);
         decoratedBox.Content = innerBox;
         innerBox.RegisterInNameScope(scope);
         var popup = BuildPopup(comboBox, scope);
         
         panel.Children.Add(decoratedBox);
         panel.Children.Add(popup);
         
         return panel;
      });
   }
   
   protected virtual AddOnDecoratedBox BuildSpinnerDecoratedBox(ComboBox comboBox, INameScope scope)
   {
      var decoratedBox = new AddOnDecoratedBox
      {
         Name = DecoratedBoxPart,
         Focusable = true,
      };
      CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StyleVariantProperty, ComboBox.StyleVariantProperty);
      CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.SizeTypeProperty, ComboBox.SizeTypeProperty);
      CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StatusProperty, ComboBox.StatusProperty);
      CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.LeftAddOnProperty, ComboBox.LeftAddOnProperty);
      CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.RightAddOnProperty, ComboBox.RightAddOnProperty);

      decoratedBox.RegisterInNameScope(scope);
      return decoratedBox;
   }
   
    protected virtual ComboBoxSpinnerInnerBox BuildSpinnerContent(ComboBox comboBox, INameScope scope)
   {
      var spinnerInnerBox = new ComboBoxSpinnerInnerBox()
      {
         Name = SpinnerInnerBoxPart,
         Focusable = true,
      };
      CreateTemplateParentBinding(spinnerInnerBox, ComboBoxSpinnerInnerBox.LeftAddOnContentProperty, ComboBox.InnerLeftContentProperty);
      CreateTemplateParentBinding(spinnerInnerBox, ComboBoxSpinnerInnerBox.RightAddOnContentProperty, ComboBox.InnerRightContentProperty);
      CreateTemplateParentBinding(spinnerInnerBox, ComboBoxSpinnerInnerBox.StyleVariantProperty, ComboBox.StyleVariantProperty);
      CreateTemplateParentBinding(spinnerInnerBox, ComboBoxSpinnerInnerBox.StatusProperty, ComboBox.StatusProperty);
      CreateTemplateParentBinding(spinnerInnerBox, ComboBoxSpinnerInnerBox.SizeTypeProperty, ComboBox.SizeTypeProperty);

      var content = BuildComboBoxContent(comboBox, scope);
      spinnerInnerBox.Content = content;
      
      var spinnerHandleDecorator = new Border()
      {
         Name = SpinnerHandleDecoratorPart,
         BackgroundSizing = BackgroundSizing.InnerBorderEdge,
         ClipToBounds = true
      };
      
      spinnerHandleDecorator.RegisterInNameScope(scope);

      var decreaseButtonIcon = new PathIcon()
      {
         Kind = "DownOutlined"
      };
      
      TokenResourceBinder.CreateGlobalTokenBinding(decreaseButtonIcon, PathIcon.ActiveFilledBrushProperty, ButtonSpinnerTokenResourceKey.HandleHoverColor);
      TokenResourceBinder.CreateGlobalTokenBinding(decreaseButtonIcon, PathIcon.SelectedFilledBrushProperty, GlobalTokenResourceKey.ColorPrimaryActive);
      
      var openButton = new IconButton()
      {
         Name = OpenIndicatorButtonPart,
         Icon = decreaseButtonIcon,
         VerticalAlignment = VerticalAlignment.Stretch,
         HorizontalAlignment = HorizontalAlignment.Stretch,
         BackgroundSizing = BackgroundSizing.InnerBorderEdge
      };
      
      openButton.RegisterInNameScope(scope);
      
      TokenResourceBinder.CreateTokenBinding(openButton, IconButton.WidthProperty, ComboBoxTokenResourceKey.OpenIndicatorWidth);
      TokenResourceBinder.CreateTokenBinding(openButton, IconButton.IconWidthProperty, GlobalTokenResourceKey.IconSizeSM);
      TokenResourceBinder.CreateTokenBinding(openButton, IconButton.IconHeightProperty, GlobalTokenResourceKey.IconSizeSM);

      spinnerHandleDecorator.Child = openButton;
      spinnerInnerBox.SpinnerContent = spinnerHandleDecorator;
      
      return spinnerInnerBox;
   }

   private Panel BuildComboBoxContent(ComboBox comboBox, INameScope scope)
   {
      var contentLayout = new Panel();
      var placeholder = new TextBlock()
      {
         Name = PlaceholderTextPart,
         HorizontalAlignment = HorizontalAlignment.Left,
         VerticalAlignment = VerticalAlignment.Center,
         TextTrimming = TextTrimming.CharacterEllipsis,
         Opacity = 0.3
      };
      
      CreateTemplateParentBinding(placeholder, TextBlock.IsVisibleProperty, ComboBox.SelectedItemProperty, 
                                  BindingMode.Default,
                                  ObjectConverters.IsNull);
      CreateTemplateParentBinding(placeholder, TextBlock.TextProperty, ComboBox.PlaceholderTextProperty);
      contentLayout.Children.Add(placeholder);

      var contentPresenter = new ContentPresenter()
      {
         Name = SelectedContentPresenterPart,
         HorizontalAlignment = HorizontalAlignment.Left,
         VerticalAlignment = VerticalAlignment.Center,
      };
      
      CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, ComboBox.SelectionBoxItemProperty);
      CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, ComboBox.ItemTemplateProperty);
      
      contentLayout.Children.Add(contentPresenter);
      
      return contentLayout;
   }

   private Popup BuildPopup(ComboBox comboBox, INameScope scope)
   {
      var popup = new Popup()
      {
         Name = PopupPart,
         WindowManagerAddShadowHint = false,
         IsLightDismissEnabled = true,
         Placement = PlacementMode.BottomEdgeAlignedLeft,
      };
      popup.RegisterInNameScope(scope);
      
      var border = new Border();
      
      TokenResourceBinder.CreateTokenBinding(border, Border.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
      TokenResourceBinder.CreateTokenBinding(border, Border.CornerRadiusProperty, ComboBoxTokenResourceKey.PopupBorderRadius);
      TokenResourceBinder.CreateTokenBinding(border, Border.PaddingProperty, ComboBoxTokenResourceKey.PopupContentPadding);
      
      var scrollViewer = new MenuScrollViewer();
      var itemsPresenter = new ItemsPresenter
      {
         Name = ItemsPresenterPart,
      };
      CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, ComboBox.ItemsPanelProperty);
      Grid.SetIsSharedSizeScope(itemsPresenter, true);
      scrollViewer.Content = itemsPresenter;
      border.Child = scrollViewer;
      
      popup.Child = border;
      
      TokenResourceBinder.CreateTokenBinding(popup, Popup.MarginToAnchorProperty, ComboBoxTokenResourceKey.PopupMarginToAnchor);
      TokenResourceBinder.CreateTokenBinding(popup, Popup.MaskShadowsProperty, ComboBoxTokenResourceKey.PopupBoxShadows);
      CreateTemplateParentBinding(popup, Popup.MaxHeightProperty, ComboBox.MaxDropDownHeightProperty);
      CreateTemplateParentBinding(popup, Popup.IsOpenProperty, ComboBox.IsDropDownOpenProperty, BindingMode.TwoWay);
      
      return popup;
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      var largeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Large));
      largeStyle.Add(AddOnDecoratedBox.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusLG);
      largeStyle.Add(TextElement.FontSizeProperty, GlobalTokenResourceKey.FontSizeLG);
      {
         var spinnerInnerBox = new Style(selector => selector.Nesting().Template().Name(SpinnerInnerBoxPart));
         spinnerInnerBox.Add(ComboBoxSpinnerInnerBox.FontSizeProperty, GlobalTokenResourceKey.FontSizeLG);
         largeStyle.Add(spinnerInnerBox);
      }
      commonStyle.Add(largeStyle);

      var middleStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Middle));
      middleStyle.Add(AddOnDecoratedBox.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
      middleStyle.Add(TextElement.FontSizeProperty, GlobalTokenResourceKey.FontSize);
      {
         var spinnerInnerBox = new Style(selector => selector.Nesting().Template().Name(SpinnerInnerBoxPart));
         spinnerInnerBox.Add(ComboBoxSpinnerInnerBox.FontSizeProperty, GlobalTokenResourceKey.FontSize);
         middleStyle.Add(spinnerInnerBox);
      }
      commonStyle.Add(middleStyle);

      var smallStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Small));
      smallStyle.Add(AddOnDecoratedBox.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
      smallStyle.Add(TextElement.FontSizeProperty, GlobalTokenResourceKey.FontSizeSM);
      {
         var spinnerInnerBox = new Style(selector => selector.Nesting().Template().Name(SpinnerInnerBoxPart));
         spinnerInnerBox.Add(ComboBoxSpinnerInnerBox.FontSizeProperty, GlobalTokenResourceKey.FontSizeSM);
         smallStyle.Add(spinnerInnerBox);
      }
      commonStyle.Add(smallStyle);
      
      Add(commonStyle);
   }
}