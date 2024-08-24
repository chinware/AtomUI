using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia.Animation;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ComboBoxItemTheme : BaseControlTheme
{
   public const string ContentPresenterPart = "PART_ContentPresenter";
   
   public ComboBoxItemTheme() : base(typeof(ComboBoxItem)) {}
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<ComboBoxItem>((listBoxItem, scope) =>
      {
         var contentPresenter = new ContentPresenter()
         {
            Name = ContentPresenterPart
         };

         contentPresenter.Transitions = new Transitions()
         {
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(ContentPresenter.BackgroundProperty)
         };

         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, ComboBoxItem.ContentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, ComboBoxItem.ContentTemplateProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalContentAlignmentProperty, ComboBoxItem.HorizontalContentAlignmentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalContentAlignmentProperty, ComboBoxItem.VerticalContentAlignmentProperty);
         return contentPresenter;
      });
   }
   
   protected override void BuildStyles()
   {
      BuildCommonStyle();
      BuildDisabledStyle();
   }

   private void BuildCommonStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(ComboBoxItem.FontSizeProperty, GlobalTokenResourceKey.FontSize);
      commonStyle.Add(ComboBoxItem.MarginProperty, ComboBoxTokenResourceKey.ItemMargin);
      
      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
         contentPresenterStyle.Add(ContentPresenter.ForegroundProperty, ComboBoxTokenResourceKey.ItemColor);
         contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, ComboBoxTokenResourceKey.ItemBgColor);
         contentPresenterStyle.Add(ContentPresenter.MinHeightProperty, GlobalTokenResourceKey.ControlHeight);
         contentPresenterStyle.Add(ContentPresenter.PaddingProperty, ComboBoxTokenResourceKey.ItemPadding);
         contentPresenterStyle.Add(ContentPresenter.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
         commonStyle.Add(contentPresenterStyle);
      }

      var hoveredStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
         contentPresenterStyle.Add(ContentPresenter.ForegroundProperty, ComboBoxTokenResourceKey.ItemHoverColor);
         contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, ComboBoxTokenResourceKey.ItemHoverBgColor);
         hoveredStyle.Add(contentPresenterStyle);
      }
      commonStyle.Add(hoveredStyle);

      var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
         contentPresenterStyle.Add(ContentPresenter.ForegroundProperty, ComboBoxTokenResourceKey.ItemSelectedColor);
         contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, ComboBoxTokenResourceKey.ItemSelectedBgColor);
         selectedStyle.Add(contentPresenterStyle);
      }
      commonStyle.Add(selectedStyle);
      Add(commonStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
      contentPresenterStyle.Add(ContentPresenter.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
      disabledStyle.Add(contentPresenterStyle);
      Add(disabledStyle);
   }
}