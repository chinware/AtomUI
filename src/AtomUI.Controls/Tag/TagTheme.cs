using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TagTheme : BaseControlTheme
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
      this.Add(Tag.BackgroundProperty, TagTokenResourceKey.DefaultBg);
      this.Add(Tag.ForegroundProperty, TagTokenResourceKey.DefaultColor);
      this.Add(Tag.FontSizeProperty, TagTokenResourceKey.TagFontSize);
      this.Add(Tag.PaddingProperty, TagTokenResourceKey.TagPadding);
      this.Add(Tag.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
      this.Add(Tag.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
      this.Add(Tag.TagTextPaddingInlineProperty, TagTokenResourceKey.TagTextPaddingInline);
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
      
         TokenResourceBinder.CreateTokenBinding(closeBtn, IconButton.WidthProperty, GlobalTokenResourceKey.IconSizeSM);
         TokenResourceBinder.CreateTokenBinding(closeBtn, IconButton.HeightProperty, GlobalTokenResourceKey.IconSizeSM);
         TokenResourceBinder.CreateTokenBinding(textBlock, TextBlock.HeightProperty, TagTokenResourceKey.TagLineHeight);
         TokenResourceBinder.CreateTokenBinding(textBlock, TextBlock.LineHeightProperty, TagTokenResourceKey.TagLineHeight);

         CreateTemplateParentBinding(closeBtn, IconButton.IsVisibleProperty, Tag.IsClosableProperty);
         CreateTemplateParentBinding(closeBtn, IconButton.IconProperty, Tag.CloseIconProperty);
         
         layout.Children.Add(closeBtn);
         
         return layout;
      });
   }
}