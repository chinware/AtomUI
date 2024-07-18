using AtomUI.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal abstract class BaseButtonTheme : ControlTheme
{
   public const string LabelPart = "PART_Label";
   public const string StackPanelPart = "PART_StackPanel";
   
   public BaseButtonTheme(Type targetType) : base(targetType) {}

   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<Button>((button, scope) =>
      {
         var label = new Label()
         {
            Name = LabelPart,
            Padding = new Thickness(0),
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
         };
         label.Bind(Label.ContentProperty, new Binding("Text")
         {
            RelativeSource = new RelativeSource()
            {
               Mode = RelativeSourceMode.TemplatedParent
            }
         });
         label.RegisterInNameScope(scope);
         var stackPanel = new StackPanel()
         {
            Name = StackPanelPart,
            UseLayoutRounding = false,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Orientation = Orientation.Horizontal,
            ClipToBounds = true
         };
         stackPanel.RegisterInNameScope(scope);
         stackPanel.Children.Add(label);
         return stackPanel;
      });
   }

   protected override void BuildStyles()
   {
      BuildSizeStyle();
      
   }
   
   private void BuildSizeStyle()
   {
      var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Large));
      largeSizeStyle.Setters.Add(new Setter(Button.ControlHeightTokenProperty, new DynamicResourceExtension(GlobalResourceKey.ControlHeightLG)));
      largeSizeStyle.Setters.Add(new Setter(Button.FontSizeProperty, new DynamicResourceExtension(ButtonResourceKey.ContentFontSizeLG)));
      largeSizeStyle.Setters.Add(new Setter(Button.PaddingProperty, new DynamicResourceExtension(ButtonResourceKey.PaddingLG)));
      largeSizeStyle.Setters.Add(new Setter(Button.CornerRadiusProperty, new DynamicResourceExtension(GlobalResourceKey.BorderRadiusLG)));
      Add(largeSizeStyle);
      
      var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Middle));
      middleSizeStyle.Setters.Add(new Setter(Button.ControlHeightTokenProperty, new DynamicResourceExtension(GlobalResourceKey.ControlHeight)));
      middleSizeStyle.Setters.Add(new Setter(Button.FontSizeProperty, new DynamicResourceExtension(ButtonResourceKey.ContentFontSize)));
      middleSizeStyle.Setters.Add(new Setter(Button.PaddingProperty, new DynamicResourceExtension(ButtonResourceKey.Padding)));
      middleSizeStyle.Setters.Add(new Setter(Button.CornerRadiusProperty, new DynamicResourceExtension(GlobalResourceKey.BorderRadius)));
      Add(middleSizeStyle);
      
      var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Small));
      smallSizeStyle.Setters.Add(new Setter(Button.ControlHeightTokenProperty, new DynamicResourceExtension(GlobalResourceKey.ControlHeightSM)));
      smallSizeStyle.Setters.Add(new Setter(Button.FontSizeProperty, new DynamicResourceExtension(ButtonResourceKey.ContentFontSizeSM)));
      smallSizeStyle.Setters.Add(new Setter(Button.PaddingProperty, new DynamicResourceExtension(ButtonResourceKey.PaddingSM)));
      smallSizeStyle.Setters.Add(new Setter(Button.CornerRadiusProperty, new DynamicResourceExtension(GlobalResourceKey.BorderRadiusSM)));
      Add(smallSizeStyle);
   }
}