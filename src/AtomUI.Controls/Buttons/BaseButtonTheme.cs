using AtomUI.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
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
      this.Add(Button.HorizontalAlignmentProperty, HorizontalAlignment.Left);
      this.Add(Button.VerticalAlignmentProperty, VerticalAlignment.Bottom);
      this.Add(Button.CursorProperty, new Cursor(StandardCursorType.Hand));
      BuildSizeStyle();
   }
   
   private void BuildSizeStyle()
   {
      var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Large));
      largeSizeStyle.Add(Button.ControlHeightTokenProperty, new DynamicResourceExtension(GlobalResourceKey.ControlHeightLG));
      largeSizeStyle.Add(Button.FontSizeProperty, new DynamicResourceExtension(ButtonResourceKey.ContentFontSizeLG));
      largeSizeStyle.Add(Button.PaddingProperty, new DynamicResourceExtension(ButtonResourceKey.PaddingLG));
      largeSizeStyle.Add(Button.CornerRadiusProperty, new DynamicResourceExtension(GlobalResourceKey.BorderRadiusLG));
      Add(largeSizeStyle);
      
      var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Middle));
      middleSizeStyle.Add(Button.ControlHeightTokenProperty, new DynamicResourceExtension(GlobalResourceKey.ControlHeight));
      middleSizeStyle.Add(Button.FontSizeProperty, new DynamicResourceExtension(ButtonResourceKey.ContentFontSize));
      middleSizeStyle.Add(Button.PaddingProperty, new DynamicResourceExtension(ButtonResourceKey.Padding));
      middleSizeStyle.Add(Button.CornerRadiusProperty, new DynamicResourceExtension(GlobalResourceKey.BorderRadius));
      Add(middleSizeStyle);
      
      var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Small));
      smallSizeStyle.Add(Button.ControlHeightTokenProperty, new DynamicResourceExtension(GlobalResourceKey.ControlHeightSM));
      smallSizeStyle.Add(Button.FontSizeProperty, new DynamicResourceExtension(ButtonResourceKey.ContentFontSizeSM));
      smallSizeStyle.Add(Button.PaddingProperty, new DynamicResourceExtension(ButtonResourceKey.PaddingSM));
      smallSizeStyle.Add(Button.CornerRadiusProperty, new DynamicResourceExtension(GlobalResourceKey.BorderRadiusSM));
      Add(smallSizeStyle);
   }
}