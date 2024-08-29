using AtomUI.Controls.Utils;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class NotificationCardTheme : BaseControlTheme
{
   public const string FrameDecoratorPart = "PART_FrameDecorator";
   public const string IconContentPart = "PART_IconContent";
   public const string HeaderContainerPart = "PART_HeaderContainer";
   public const string HeaderTitlePart = "PART_HeaderTitle";
   public const string ContentPart = "PART_Content";
   public const string CloseButtonPart = "PART_CloseButton";

   public NotificationCardTheme()
      : base(typeof(NotificationCard))
   {
   }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<NotificationCard>((card, scope) =>
      {
         BuildInstanceStyles(card);
         var frameDecorator = new Border()
         {
            Name = FrameDecoratorPart
         };

         var mainLayout = new Grid()
         {
            ColumnDefinitions = new ColumnDefinitions()
            {
               new ColumnDefinition(GridLength.Auto),
               new ColumnDefinition(GridLength.Star)
            },
            RowDefinitions = new RowDefinitions()
            {
               new RowDefinition(GridLength.Auto),
               new RowDefinition(GridLength.Star)
            }
         };

         frameDecorator.Child = mainLayout;
         BuildHeader(mainLayout, scope);
         BuildContent(mainLayout, scope);
         frameDecorator.RegisterInNameScope(scope);
         return frameDecorator;
      });
   }

   private void BuildHeader(Grid layout, INameScope scope)
   {
      var iconContent = new ContentPresenter()
      {
         Name = IconContentPart
      };
      CreateTemplateParentBinding(iconContent, ContentPresenter.IsVisibleProperty, NotificationCard.IconProperty,
                                  BindingMode.Default,
                                  ObjectConverters.IsNotNull);
      CreateTemplateParentBinding(iconContent, ContentPresenter.ContentProperty, NotificationCard.IconProperty);
      TokenResourceBinder.CreateTokenBinding(iconContent, ContentPresenter.MarginProperty,
                                             NotificationTokenResourceKey.NotificationIconMargin);
      Grid.SetRow(iconContent, 0);
      Grid.SetColumn(iconContent, 0);

      layout.Children.Add(iconContent);

      var headerContainer = new DockPanel()
      {
         Name = HeaderContainerPart,
         LastChildFill = true
      };

      var headerTitle = new SelectableTextBlock()
      {
         Name = HeaderTitlePart
      };
      TokenResourceBinder.CreateGlobalTokenBinding(headerTitle, SelectableTextBlock.SelectionBrushProperty, GlobalTokenResourceKey.SelectionBackground);
      TokenResourceBinder.CreateGlobalTokenBinding(headerTitle, SelectableTextBlock.SelectionForegroundBrushProperty, GlobalTokenResourceKey.SelectionForeground);
    
      CreateTemplateParentBinding(headerTitle, SelectableTextBlock.TextProperty, NotificationCard.TitleProperty);
      var closeIcon = new PathIcon()
      {
         Kind = "CloseOutlined",
      };
      TokenResourceBinder.CreateTokenBinding(closeIcon, PathIcon.NormalFilledBrushProperty,
                                             GlobalTokenResourceKey.ColorIcon);
      TokenResourceBinder.CreateTokenBinding(closeIcon, PathIcon.ActiveFilledBrushProperty,
                                             GlobalTokenResourceKey.ColorIconHover);
      var closeIconButton = new IconButton()
      {
         Name = CloseButtonPart,
         Icon = closeIcon,
         IsEnableHoverEffect = true,
         HorizontalAlignment = HorizontalAlignment.Right,
         VerticalAlignment = VerticalAlignment.Center
      };
      TokenResourceBinder.CreateTokenBinding(closeIconButton, IconButton.PaddingProperty,
                                             GlobalTokenResourceKey.PaddingXXS, BindingPriority.Template,
                                             o =>
                                             {
                                                if (o is double dval) {
                                                   return new Thickness(dval);
                                                }

                                                return new Thickness(0);
                                             });
      TokenResourceBinder.CreateTokenBinding(closeIconButton, IconButton.CornerRadiusProperty,
                                             GlobalTokenResourceKey.BorderRadiusSM);
      TokenResourceBinder.CreateTokenBinding(closeIconButton, IconButton.IconHeightProperty,
                                             GlobalTokenResourceKey.IconSizeSM);
      TokenResourceBinder.CreateTokenBinding(closeIconButton, IconButton.IconWidthProperty,
                                             GlobalTokenResourceKey.IconSizeSM);
      DockPanel.SetDock(closeIconButton, Dock.Right);
      headerContainer.Children.Add(closeIconButton);
      headerContainer.Children.Add(headerTitle);

      Grid.SetRow(headerContainer, 0);
      Grid.SetColumn(headerContainer, 1);
      layout.Children.Add(headerContainer);
   }

   private void BuildContent(Grid layout, INameScope scope)
   {
      var contentPresenter = new ContentPresenter()
      {
         Name = ContentPart,
         TextWrapping = TextWrapping.Wrap
      };
      CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                                  NotificationCard.CardContentProperty);
      CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                                  NotificationCard.CardContentTemplateProperty);
      Grid.SetColumn(contentPresenter, 1);
      Grid.SetRow(contentPresenter, 1);
      layout.Children.Add(contentPresenter);
   }

   protected override void BuildStyles()
   {
      BuildCommonStyle();
      BuildHeaderStyle();
      BuildContentStyle();
   }

   private void BuildCommonStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      
      var motionConfig = MotionFactory.BuildSlideRightInMotion(TimeSpan.FromMilliseconds(400));
      foreach (var animation in motionConfig.Animations) {
         commonStyle.Animations.Add(animation);
      }
      
      commonStyle.Add(NotificationCard.MarginProperty, NotificationTokenResourceKey.NotificationMarginEdge);
      commonStyle.Add(NotificationCard.WidthProperty, NotificationTokenResourceKey.NotificationWidth);
      commonStyle.Add(NotificationCard.RenderTransformOriginProperty, new RelativePoint(1.0, 0.5, RelativeUnit.Relative));

      var frameDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(FrameDecoratorPart));
      frameDecoratorStyle.Add(Border.PaddingProperty, NotificationTokenResourceKey.NotificationPadding);
      frameDecoratorStyle.Add(Border.BoxShadowProperty, GlobalTokenResourceKey.BoxShadows);
      frameDecoratorStyle.Add(Border.BackgroundProperty, NotificationTokenResourceKey.NotificationBg);
      frameDecoratorStyle.Add(Border.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusLG);
      commonStyle.Add(frameDecoratorStyle);

      Add(commonStyle);
   }

   private void BuildHeaderStyle()
   {
      var titleStyle = new Style(selector => selector.Nesting().Template().Name(HeaderTitlePart));
      titleStyle.Add(ContentPresenter.LineHeightProperty, GlobalTokenResourceKey.FontHeightLG);
      titleStyle.Add(ContentPresenter.FontSizeProperty, GlobalTokenResourceKey.FontSizeLG);
      titleStyle.Add(ContentPresenter.ForegroundProperty, GlobalTokenResourceKey.ColorTextHeading);
      Add(titleStyle);

      var headerContainer = new Style(selector => selector.Nesting().Template().Name(HeaderContainerPart));
      headerContainer.Add(DockPanel.MarginProperty, NotificationTokenResourceKey.HeaderMargin);
      Add(headerContainer);
   }

   private void BuildContentStyle()
   {
      var contentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPart));
      contentStyle.Add(ContentPresenter.ForegroundProperty, GlobalTokenResourceKey.ColorText);
      contentStyle.Add(ContentPresenter.FontSizeProperty, GlobalTokenResourceKey.FontSize);
      contentStyle.Add(ContentPresenter.LineHeightProperty, GlobalTokenResourceKey.FontHeight);
      Add(contentStyle);
   }

   protected override void BuildInstanceStyles(Control control)
   {
      var iconStyle = new Style(selector => selector.Name(IconContentPart).Child().OfType<PathIcon>());
      iconStyle.Add(PathIcon.WidthProperty, NotificationTokenResourceKey.NotificationIconSize);
      iconStyle.Add(PathIcon.HeightProperty, NotificationTokenResourceKey.NotificationIconSize);
      control.Styles.Add(iconStyle);
   }

   protected override void BuildAnimations()
   {
      // var motionConfig = MotionFactory.BuildSlideLeftInMotion(TimeSpan.FromMilliseconds(300));
      // foreach (var animation in motionConfig.Animations) {
      //    Animations.Add(animation);
      // }
   }
}