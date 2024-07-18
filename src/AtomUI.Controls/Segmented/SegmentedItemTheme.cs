using AtomUI.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class SegmentedItemTheme : ControlTheme
{
   public const string MainLayoutPart = "PART_MainLayout";
   
   public SegmentedItemTheme()
      : base(typeof(SegmentedItem))
   {
   }

   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<SegmentedItem>((item, scope) =>
      {
         var layout = new StackPanel()
         {
            Name = MainLayoutPart,
            Orientation = Orientation.Horizontal
         };
         layout.RegisterInNameScope(scope);
         
         var label = new Label()
         {
            Padding = new Thickness(0),
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
         };

         CreateTemplateParentBinding(label, Label.ContentProperty, SegmentedItem.TextProperty);
         layout.Children.Add(label);
         return layout;
      });
   }

   protected override void BuildStyles()
   {
      var hasIconStyle = new Style(selector => selector.Nesting().Not(x => x.Nesting().PropertyEquals(SegmentedItem.IconProperty, null)));
      {
         var labelStyle = new Style(selector => selector.Nesting().Template().OfType<Label>());
         labelStyle.Setters.Add(new Setter(Label.MarginProperty, new DynamicResourceExtension(SegmentedResourceKey.SegmentedTextLabelMargin)));
         hasIconStyle.Add(labelStyle);
      }
      
      Add(hasIconStyle);
      
      // 设置只有 Icon 的时候的样式
      var hasNoTextStyle = new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.TextProperty, null));
      {
         var labelStyle = new Style(selector => selector.Nesting().Template().OfType<Label>());
         labelStyle.Setters.Add(new Setter(Label.IsVisibleProperty, false));
         hasNoTextStyle.Add(labelStyle);
      }
      Add(hasNoTextStyle);
      
      var iconSelector = default(Selector).Nesting().Template().OfType<StackPanel>().Descendant().OfType<PathIcon>();
      var largeSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Large));
      {
         var iconStyle = new Style(selector => iconSelector);
         iconStyle.Setters.Add(new Setter(PathIcon.WidthProperty, new DynamicResourceExtension(GlobalResourceKey.IconSizeLG)));
         iconStyle.Setters.Add(new Setter(PathIcon.HeightProperty, new DynamicResourceExtension(GlobalResourceKey.IconSizeLG)));
         largeSizeStyle.Add(iconStyle);
      }
      Add(largeSizeStyle);
      
      var middleSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Middle));
      {
         var iconStyle = new Style(selector => iconSelector);
         iconStyle.Setters.Add(new Setter(PathIcon.WidthProperty, new DynamicResourceExtension(GlobalResourceKey.IconSize)));
         iconStyle.Setters.Add(new Setter(PathIcon.HeightProperty, new DynamicResourceExtension(GlobalResourceKey.IconSize)));
         middleSizeStyle.Add(iconStyle);
      }
      Add(middleSizeStyle);
      
      var smallSizeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Small));
      {
         var iconStyle = new Style(selector => iconSelector);
         iconStyle.Setters.Add(new Setter(PathIcon.WidthProperty, new DynamicResourceExtension(GlobalResourceKey.IconSizeSM)));
         iconStyle.Setters.Add(new Setter(PathIcon.HeightProperty, new DynamicResourceExtension(GlobalResourceKey.IconSizeSM)));
         smallSizeStyle.Add(iconStyle);
      }
      Add(smallSizeStyle);
   }
}