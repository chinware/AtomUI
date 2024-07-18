using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TagTheme : ControlTheme
{
   public const string MainContainerPart = "PART_MainContainer";
   public const string CloseButtonPart = "PART_CloseButton";
   public const string TagTextLabelPart = "PART_TagTextLabel";
   
   public TagTheme()
      : base(typeof(Tag))
   {
   }
   
   protected override void BuildStyles()
   {
      Add(new Setter(Tag.BackgroundProperty, new DynamicResourceExtension(TagResourceKey.DefaultBg)));
      Add(new Setter(Tag.ForegroundProperty, new DynamicResourceExtension(TagResourceKey.DefaultColor)));
      Add(new Setter(Tag.FontSizeProperty, new DynamicResourceExtension(TagResourceKey.TagFontSize)));
      Add(new Setter(Tag.PaddingProperty, new DynamicResourceExtension(TagResourceKey.TagPadding)));
      Add(new Setter(Tag.BorderBrushProperty, new DynamicResourceExtension(GlobalResourceKey.ColorBorder)));
      Add(new Setter(Tag.CornerRadiusProperty, new DynamicResourceExtension(GlobalResourceKey.BorderRadiusSM)));
   }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<Tag>((tag, scope) =>
      {
         var layout = new Canvas()
         {
            Name = MainContainerPart
         };
         layout.RegisterInNameScope(scope);
         
         var textBlock = new TextBlock
         {
            Name = TagTextLabelPart,
            VerticalAlignment = VerticalAlignment.Center,
         };
         textBlock.RegisterInNameScope(scope);
         CreateTemplateParentBinding(textBlock, TextBlock.TextProperty, Tag.TagTextProperty);
         layout.Children.Add(textBlock);

         var closeBtn = new IconButton()
         {
            Name = CloseButtonPart
         };
         closeBtn.RegisterInNameScope(scope);
      
         BindUtils.CreateTokenBinding(closeBtn, IconButton.WidthProperty, GlobalResourceKey.IconSizeSM);
         BindUtils.CreateTokenBinding(closeBtn, IconButton.HeightProperty, GlobalResourceKey.IconSizeSM);
         BindUtils.CreateTokenBinding(textBlock, TextBlock.HeightProperty, TagResourceKey.TagLineHeight);
         BindUtils.CreateTokenBinding(textBlock, TextBlock.LineHeightProperty, TagResourceKey.TagLineHeight);

         CreateTemplateParentBinding(closeBtn, IconButton.IsVisibleProperty, Tag.IsClosableProperty);
         CreateTemplateParentBinding(closeBtn, IconButton.IconProperty, Tag.CloseIconProperty);
         
         layout.Children.Add(closeBtn);
         
         return layout;
      });
   }
}