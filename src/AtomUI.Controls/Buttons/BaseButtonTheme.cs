using AtomUI.Icon;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal abstract class BaseButtonTheme : BaseControlTheme
{
   public const string LabelPart       = "PART_Label";
   public const string MainInfoLayoutPart  = "PART_MainInfoLayout";
   public const string RootLayoutPart  = "PART_RootLayout";
   public const string LoadingIconPart = "PART_LoadingIcon";
   public const string ButtonIconPart  = "PART_ButtonIcon";
   public const string RightExtraContentPart  = "PART_RightExtraContent";
   
   public BaseButtonTheme(Type targetType) : base(targetType) {}

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<Button>((button, scope) =>
      {
         var loadingIcon = new PathIcon()
         {
            Kind = "LoadingOutlined",
            Name = LoadingIconPart
         };

         loadingIcon.RegisterInNameScope(scope);

         CreateTemplateParentBinding(loadingIcon, PathIcon.WidthProperty, Button.IconSizeProperty);
         CreateTemplateParentBinding(loadingIcon, PathIcon.HeightProperty, Button.IconSizeProperty);
         CreateTemplateParentBinding(loadingIcon, PathIcon.MarginProperty, Button.IconMarginProperty);

         var iconPresenter = new ContentPresenter()
         {
            Name = ButtonIconPart,
         };
         CreateTemplateParentBinding(iconPresenter, ContentPresenter.ContentProperty, Button.IconProperty);

         var label = new Label
         {
            Name = LabelPart,
            Padding = new Thickness(0),
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
         };
         CreateTemplateParentBinding(label, Label.ContentProperty, Button.TextProperty);
         label.RegisterInNameScope(scope);
         
         var mainInfoLayout = new StackPanel()
         {
            Name = MainInfoLayoutPart,
            UseLayoutRounding = false,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Orientation = Orientation.Horizontal,
            ClipToBounds = true
         };
         
         mainInfoLayout.RegisterInNameScope(scope);
         mainInfoLayout.Children.Add(loadingIcon);
         mainInfoLayout.Children.Add(iconPresenter);
         mainInfoLayout.Children.Add(label);

         var extraContentPresenter = new ContentPresenter()
         {
            Name = RightExtraContentPart,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
         };

         CreateTemplateParentBinding(extraContentPresenter, ContentPresenter.ContentProperty, Button.ExtraContentProperty);
         
         DockPanel.SetDock(extraContentPresenter, Dock.Right);

         var rootLayout = new DockPanel()
         {
            Name = RootLayoutPart,
            LastChildFill = true
         };
         
         rootLayout.Children.Add(extraContentPresenter);
         rootLayout.Children.Add(mainInfoLayout);
         return rootLayout;
      });
   }

   protected override void BuildStyles()
   {
      this.Add(Button.HorizontalAlignmentProperty, HorizontalAlignment.Left);
      this.Add(Button.VerticalAlignmentProperty, VerticalAlignment.Bottom);
      this.Add(Button.CursorProperty, new Cursor(StandardCursorType.Hand));
      this.Add(Button.DefaultShadowProperty, ButtonTokenResourceKey.DefaultShadow);
      this.Add(Button.PrimaryShadowProperty, ButtonTokenResourceKey.PrimaryShadow);
      this.Add(Button.DangerShadowProperty, ButtonTokenResourceKey.DangerShadow);
      
      BuildSizeStyle();
      BuildIconSizeStyle();
      BuildLoadingStyle();
   }
   
   private void BuildSizeStyle()
   {
      var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Large));
      largeSizeStyle.Add(Button.ControlHeightTokenProperty, GlobalTokenResourceKey.ControlHeightLG);
      largeSizeStyle.Add(Button.FontSizeProperty, ButtonTokenResourceKey.ContentFontSizeLG);
      largeSizeStyle.Add(Button.PaddingProperty, ButtonTokenResourceKey.PaddingLG);
      largeSizeStyle.Add(Button.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusLG);
      {
         var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
         iconOnlyStyle.Add(Button.PaddingProperty, ButtonTokenResourceKey.IconOnyPaddingLG);
         largeSizeStyle.Add(iconOnlyStyle);
      }
      Add(largeSizeStyle);
      
      var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Middle));
      middleSizeStyle.Add(Button.ControlHeightTokenProperty, GlobalTokenResourceKey.ControlHeight);
      middleSizeStyle.Add(Button.FontSizeProperty, ButtonTokenResourceKey.ContentFontSize);
      middleSizeStyle.Add(Button.PaddingProperty, ButtonTokenResourceKey.Padding);
      middleSizeStyle.Add(Button.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
      {
         var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
         iconOnlyStyle.Add(Button.PaddingProperty, ButtonTokenResourceKey.IconOnyPadding);
         middleSizeStyle.Add(iconOnlyStyle);
      }
      Add(middleSizeStyle);
      
      var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Small));
      smallSizeStyle.Add(Button.ControlHeightTokenProperty, GlobalTokenResourceKey.ControlHeightSM);
      smallSizeStyle.Add(Button.FontSizeProperty, ButtonTokenResourceKey.ContentFontSizeSM);
      smallSizeStyle.Add(Button.PaddingProperty, ButtonTokenResourceKey.PaddingSM);
      smallSizeStyle.Add(Button.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
      {
         var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
         iconOnlyStyle.Add(Button.PaddingProperty, ButtonTokenResourceKey.IconOnyPaddingSM);
         smallSizeStyle.Add(iconOnlyStyle);
      }
      Add(smallSizeStyle);
   }

   private void BuildIconSizeStyle()
   {
      // text 和 icon 都存在的情况
      {
         var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Large));
         largeSizeStyle.Add(Button.IconSizeProperty, GlobalTokenResourceKey.IconSizeLG);
         Add(largeSizeStyle);
      
         var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Middle));
         middleSizeStyle.Add(Button.IconSizeProperty, GlobalTokenResourceKey.IconSize);
         Add(middleSizeStyle);
      
         var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Small));
         smallSizeStyle.Add(Button.IconSizeProperty, GlobalTokenResourceKey.IconSizeSM);
         Add(smallSizeStyle);
      }
      
      // icon only
      var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
      iconOnlyStyle.Add(Button.IconMarginProperty, new Thickness());
      {
         var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Large));
         largeSizeStyle.Add(Button.IconSizeProperty, ButtonTokenResourceKey.OnlyIconSizeLG);
         iconOnlyStyle.Add(largeSizeStyle);
      
         var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Middle));
         middleSizeStyle.Add(Button.IconSizeProperty, ButtonTokenResourceKey.OnlyIconSize);
         iconOnlyStyle.Add(middleSizeStyle);
      
         var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Small));
         smallSizeStyle.Add(Button.IconSizeProperty, ButtonTokenResourceKey.OnlyIconSizeSM);
         iconOnlyStyle.Add(smallSizeStyle);
      }
      Add(iconOnlyStyle);

      var notIconOnyStyle = new Style(selector => selector.Nesting().Not(x => x.Nesting().Class(Button.IconOnlyPC)));
      notIconOnyStyle.Add(Button.IconMarginProperty, ButtonTokenResourceKey.IconMargin);
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
         loadingIconStyle.Add(PathIcon.LoadingAnimationProperty, IconAnimation.Spin);
         loadingStyle.Add(loadingIconStyle);
      }
      loadingStyle.Add(Button.OpacityProperty, GlobalTokenResourceKey.OpacityLoading);
      Add(loadingStyle);
   }
}