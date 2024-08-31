using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class SegmentedItemTheme : BaseControlTheme
{
   public const string MainFramePart = "PART_MainFrame";
   public const string IconContentPart = "PART_IconContent";
   public const string ContentPart = "PART_Content";

   public SegmentedItemTheme() : base(typeof(SegmentedItem)) { }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<SegmentedItem>((segmentedItem, scope) =>
      {
         var mainFrame = new Border()
         {
            Name = MainFramePart
         };
         CreateTemplateParentBinding(mainFrame, Border.BackgroundProperty, SegmentedItem.BackgroundProperty);
         CreateTemplateParentBinding(mainFrame, Border.CornerRadiusProperty, SegmentedItem.CornerRadiusProperty);
         CreateTemplateParentBinding(mainFrame, Border.PaddingProperty, SegmentedItem.PaddingProperty);

         var contentLayout = new DockPanel()
         {
            LastChildFill = true
         };

         var iconContent = new ContentPresenter()
         {
            Name = IconContentPart
         };
         iconContent.RegisterInNameScope(scope);
         CreateTemplateParentBinding(iconContent, ContentPresenter.ContentProperty, SegmentedItem.IconProperty);
         CreateTemplateParentBinding(iconContent, ContentPresenter.IsVisibleProperty, SegmentedItem.IconProperty,
                                     BindingMode.Default,
                                     ObjectConverters.IsNotNull);

         contentLayout.Children.Add(iconContent);

         var contentPresenter = new ContentPresenter()
         {
            Name = ContentPart,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center
         };
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                                     SegmentedItem.ContentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.IsVisibleProperty, SegmentedItem.ContentProperty,
                                     BindingMode.Default,
                                     ObjectConverters.IsNotNull);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                                     SegmentedItem.ContentTemplateProperty);
         contentLayout.Children.Add(contentPresenter);

         mainFrame.Child = contentLayout;

         return mainFrame;
      });
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());

      // 没有被选择的正常状态
      var enabledStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.IsEnabledProperty, true));
      enabledStyle.Add(SegmentedItem.CursorProperty, new Cursor(StandardCursorType.Hand));

      // 选中状态
      var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
      selectedStyle.Add(SegmentedItem.ForegroundProperty, SegmentedTokenResourceKey.ItemSelectedColor);
      selectedStyle.Add(SegmentedItem.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
      enabledStyle.Add(selectedStyle);

      // 没有被选中的状态
      var notSelectedStyle =
         new Style(selector => selector.Nesting().Not(x => x.Nesting().Class(StdPseudoClass.Selected)));
      notSelectedStyle.Add(SegmentedItem.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
      notSelectedStyle.Add(SegmentedItem.ForegroundProperty, SegmentedTokenResourceKey.ItemColor);

      // Hover 状态
      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      hoverStyle.Add(SegmentedItem.BackgroundProperty, SegmentedTokenResourceKey.ItemHoverBg);
      hoverStyle.Add(SegmentedItem.ForegroundProperty, SegmentedTokenResourceKey.ItemHoverColor);
      notSelectedStyle.Add(hoverStyle);

      // Pressed 状态
      var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
      pressedStyle.Add(SegmentedItem.BackgroundProperty, SegmentedTokenResourceKey.ItemActiveBg);
      notSelectedStyle.Add(pressedStyle);

      enabledStyle.Add(notSelectedStyle);
      commonStyle.Add(enabledStyle);
      Add(commonStyle);

      BuildSizeTypeStyle();
      BuildIconStyle();
      BuildDisabledStyle();
   }

   private void BuildSizeTypeStyle()
   {
      var largeSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Large));
      largeSizeStyle.Add(SegmentedItem.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
      largeSizeStyle.Add(SegmentedItem.FontSizeProperty, GlobalTokenResourceKey.FontSizeLG);
      largeSizeStyle.Add(SegmentedItem.MinHeightProperty, SegmentedTokenResourceKey.ItemMinHeightLG);
      largeSizeStyle.Add(SegmentedItem.PaddingProperty, SegmentedTokenResourceKey.SegmentedItemPadding);
      Add(largeSizeStyle);

      var middleSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Middle));
      middleSizeStyle.Add(SegmentedItem.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
      middleSizeStyle.Add(SegmentedItem.FontSizeProperty, GlobalTokenResourceKey.FontSize);
      middleSizeStyle.Add(SegmentedItem.MinHeightProperty, SegmentedTokenResourceKey.ItemMinHeight);
      middleSizeStyle.Add(SegmentedItem.PaddingProperty, SegmentedTokenResourceKey.SegmentedItemPadding);
      Add(middleSizeStyle);

      var smallSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Small));
      smallSizeStyle.Add(SegmentedItem.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusXS);
      smallSizeStyle.Add(SegmentedItem.FontSizeProperty, GlobalTokenResourceKey.FontSize);
      smallSizeStyle.Add(SegmentedItem.MinHeightProperty, SegmentedTokenResourceKey.ItemMinHeightSM);
      smallSizeStyle.Add(SegmentedItem.PaddingProperty, SegmentedTokenResourceKey.SegmentedItemPaddingSM);

      Add(smallSizeStyle);
   }

   private void BuildIconStyle()
   {
      var hasIconStyle =
         new Style(selector => selector.Nesting()
                                       .Not(x => x.Nesting().PropertyEquals(SegmentedItem.IconProperty, null)));
      {
         var labelStyle = new Style(selector => selector.Nesting().Template().Name(ContentPart));
         labelStyle.Add(ContentPresenter.MarginProperty, SegmentedTokenResourceKey.SegmentedItemContentMargin);
         hasIconStyle.Add(labelStyle);
      }

      Add(hasIconStyle);
      
      var iconSelector = default(Selector).Nesting().Template().Name(IconContentPart).Child().OfType<PathIcon>();
      var largeSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Large));
      {
         var iconStyle = new Style(selector => iconSelector);
         iconStyle.Add(PathIcon.WidthProperty, GlobalTokenResourceKey.IconSizeLG);
         iconStyle.Add(PathIcon.HeightProperty, GlobalTokenResourceKey.IconSizeLG);
         largeSizeStyle.Add(iconStyle);
      }
      Add(largeSizeStyle);

      var middleSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Middle));
      {
         var iconStyle = new Style(selector => iconSelector);
         iconStyle.Add(PathIcon.WidthProperty, GlobalTokenResourceKey.IconSize);
         iconStyle.Add(PathIcon.HeightProperty, GlobalTokenResourceKey.IconSize);
         middleSizeStyle.Add(iconStyle);
      }
      Add(middleSizeStyle);

      var smallSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Small));
      {
         var iconStyle = new Style(selector => iconSelector);
         iconStyle.Add(PathIcon.WidthProperty, GlobalTokenResourceKey.IconSizeSM);
         iconStyle.Add(PathIcon.HeightProperty, GlobalTokenResourceKey.IconSizeSM);
         smallSizeStyle.Add(iconStyle);
      }
      Add(smallSizeStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(SegmentedItem.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
      Add(disabledStyle);
   }
}