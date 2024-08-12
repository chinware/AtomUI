using AtomUI.Icon;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal abstract class BaseButtonTheme : BaseControlTheme
{
   public const string LabelPart       = "PART_Label";
   public const string StackPanelPart  = "PART_StackPanel";
   public const string LoadingIconPart = "PART_LoadingIcon";
   public const string ButtonIconPart  = "PART_ButtonIcon";
   
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

         var loadingIcon = new PathIcon()
         {
            Kind = "LoadingOutlined",
            Name = LoadingIconPart
         };
         
         CreateTemplateParentBinding(label, Label.ContentProperty, Button.TextProperty);
         
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
         stackPanel.Children.Add(loadingIcon);
         stackPanel.Children.Add(label);
         return stackPanel;
      });
   }

   protected override void BuildStyles()
   {
      this.Add(Button.HorizontalAlignmentProperty, HorizontalAlignment.Left);
      this.Add(Button.VerticalAlignmentProperty, VerticalAlignment.Bottom);
      this.Add(Button.CursorProperty, new Cursor(StandardCursorType.Hand));
      this.Add(Button.DefaultShadowProperty, ButtonResourceKey.DefaultShadow);
      this.Add(Button.PrimaryShadowProperty, ButtonResourceKey.PrimaryShadow);
      this.Add(Button.DangerShadowProperty, ButtonResourceKey.DangerShadow);
      
      BuildSizeStyle();
      BuildIconSizeStyle();
      BuildLoadingStyle();
   }
   
   private void BuildSizeStyle()
   {
      var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Large));
      largeSizeStyle.Add(Button.ControlHeightTokenProperty, GlobalResourceKey.ControlHeightLG);
      largeSizeStyle.Add(Button.FontSizeProperty, ButtonResourceKey.ContentFontSizeLG);
      largeSizeStyle.Add(Button.PaddingProperty, ButtonResourceKey.PaddingLG);
      largeSizeStyle.Add(Button.CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
      {
         var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
         iconOnlyStyle.Add(Button.PaddingProperty, ButtonResourceKey.IconOnyPaddingLG);
         largeSizeStyle.Add(iconOnlyStyle);
      }
      Add(largeSizeStyle);
      
      var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Middle));
      middleSizeStyle.Add(Button.ControlHeightTokenProperty, GlobalResourceKey.ControlHeight);
      middleSizeStyle.Add(Button.FontSizeProperty, ButtonResourceKey.ContentFontSize);
      middleSizeStyle.Add(Button.PaddingProperty, ButtonResourceKey.Padding);
      middleSizeStyle.Add(Button.CornerRadiusProperty, GlobalResourceKey.BorderRadius);
      {
         var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
         iconOnlyStyle.Add(Button.PaddingProperty, ButtonResourceKey.IconOnyPadding);
         middleSizeStyle.Add(iconOnlyStyle);
      }
      Add(middleSizeStyle);
      
      var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Small));
      smallSizeStyle.Add(Button.ControlHeightTokenProperty, GlobalResourceKey.ControlHeightSM);
      smallSizeStyle.Add(Button.FontSizeProperty, ButtonResourceKey.ContentFontSizeSM);
      smallSizeStyle.Add(Button.PaddingProperty, ButtonResourceKey.PaddingSM);
      smallSizeStyle.Add(Button.CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      {
         var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
         iconOnlyStyle.Add(Button.PaddingProperty, ButtonResourceKey.IconOnyPaddingSM);
         smallSizeStyle.Add(iconOnlyStyle);
      }
      Add(smallSizeStyle);
   }

   private void BuildIconSizeStyle()
   {
      // text 和 icon 都存在的情况
      {
         var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Large));
         {
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
            iconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSizeLG);
            iconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSizeLG);
            iconStyle.Add(PathIcon.MarginProperty, ButtonResourceKey.IconMargin);
            largeSizeStyle.Add(iconStyle);
         }
         Add(largeSizeStyle);
      
         var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Middle));
         {
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
            iconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSize);
            iconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSize);
            iconStyle.Add(PathIcon.MarginProperty, ButtonResourceKey.IconMargin);
            middleSizeStyle.Add(iconStyle);
         }
         
         Add(middleSizeStyle);
      
         var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Small));
         {
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
            iconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSizeSM);
            iconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSizeSM);
            iconStyle.Add(PathIcon.MarginProperty, ButtonResourceKey.IconMargin);
            smallSizeStyle.Add(iconStyle);
         }
         Add(smallSizeStyle);
      }
      
      // icon only
      var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
      {
         // Icon only 状态没有 margin
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.MarginProperty, new Thickness());
         iconOnlyStyle.Add(iconStyle);
      }
      {
         var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Large));
         {
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
            iconStyle.Add(PathIcon.WidthProperty, ButtonResourceKey.OnlyIconSizeLG);
            iconStyle.Add(PathIcon.HeightProperty, ButtonResourceKey.OnlyIconSizeLG);
            largeSizeStyle.Add(iconStyle);
         }
         iconOnlyStyle.Add(largeSizeStyle);
      
         var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Middle));
         {
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
            iconStyle.Add(PathIcon.WidthProperty, ButtonResourceKey.OnlyIconSize);
            iconStyle.Add(PathIcon.HeightProperty, ButtonResourceKey.OnlyIconSize);
            middleSizeStyle.Add(iconStyle);
         }
         iconOnlyStyle.Add(middleSizeStyle);
      
         var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Small));
         {
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
            iconStyle.Add(PathIcon.WidthProperty, ButtonResourceKey.OnlyIconSizeSM);
            iconStyle.Add(PathIcon.HeightProperty, ButtonResourceKey.OnlyIconSizeSM);
            smallSizeStyle.Add(iconStyle);
         }
         iconOnlyStyle.Add(smallSizeStyle);
      }
      Add(iconOnlyStyle);

      var notIconOnyStyle = new Style(selector => selector.Nesting().Not(x => x.Nesting().Class(Button.IconOnlyPC)));
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         iconStyle.Add(PathIcon.MarginProperty, ButtonResourceKey.IconMargin);
         notIconOnyStyle.Add(iconStyle);
      }
      
      Add(notIconOnyStyle);
   }

   private void BuildLoadingStyle()
   {
      // 正常状态
      {
         var buttonIconStyle = new Style(selector => selector.Nesting().Template().Name(ButtonIconPart));
         buttonIconStyle.Add(PathIcon.IsVisibleProperty, true);
         Add(buttonIconStyle);

         var loadingIconStyle = new Style(selector => selector.Nesting().Template().Name(LoadingIconPart));
         loadingIconStyle.Add(PathIcon.IsVisibleProperty, false);
         Add(loadingIconStyle);
      }
      // loading 状态
      var loadingStyle = new Style(selector => selector.Nesting().Class(Button.LoadingPC));
      {
         var buttonIconStyle = new Style(selector => selector.Nesting().Template().Name(ButtonIconPart));
         buttonIconStyle.Add(PathIcon.IsVisibleProperty, false);
         loadingStyle.Add(buttonIconStyle);

         var loadingIconStyle = new Style(selector => selector.Nesting().Template().Name(LoadingIconPart));
         loadingIconStyle.Add(PathIcon.IsVisibleProperty, true);
         loadingIconStyle.Add(PathIcon.AnimationProperty, IconAnimation.Spin);
         loadingStyle.Add(loadingIconStyle);
      }
      loadingStyle.Add(Button.OpacityProperty, GlobalResourceKey.OpacityLoading);
      Add(loadingStyle);
   }
}