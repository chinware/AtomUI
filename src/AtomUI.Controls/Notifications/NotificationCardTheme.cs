using AtomUI.Controls.Utils;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
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
   public const string LayoutTransformControlPart = "PART_LayoutTransformControl";
   public const string ProgressBarPart = "PART_ProgressBar";

   public NotificationCardTheme()
      : base(typeof(NotificationCard))
   {
   }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<NotificationCard>((card, scope) =>
      {
         BuildInstanceStyles(card);
         var layoutTransformControl = new LayoutTransformControl()
         {
            Name = LayoutTransformControlPart,
            ClipToBounds = false
         };
         layoutTransformControl.RegisterInNameScope(scope);
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
               new RowDefinition(GridLength.Star),
               new RowDefinition(GridLength.Auto)
            }
         };

         frameDecorator.Child = mainLayout;
         BuildHeader(mainLayout, scope);
         BuildContent(mainLayout, scope);
         BuildProgressBar(mainLayout, scope);
         frameDecorator.RegisterInNameScope(scope);

         layoutTransformControl.Child = frameDecorator;
         return layoutTransformControl;
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
      closeIconButton.RegisterInNameScope(scope);
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
         TextWrapping = TextWrapping.Wrap,
      };
      TokenResourceBinder.CreateTokenBinding(contentPresenter, ContentPresenter.MarginProperty, NotificationTokenResourceKey.NotificationContentMargin);
      CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                                  NotificationCard.CardContentProperty);
      CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                                  NotificationCard.CardContentTemplateProperty);
      Grid.SetColumn(contentPresenter, 1);
      Grid.SetRow(contentPresenter, 1);
      layout.Children.Add(contentPresenter);
   }

   private void BuildProgressBar(Grid layout, INameScope scope)
   {
      var progressBar = new NotificationProgressBar()
      {
         Name = ProgressBarPart
      };
      progressBar.RegisterInNameScope(scope);
      CreateTemplateParentBinding(progressBar, NotificationProgressBar.IsVisibleProperty, NotificationCard.EffectiveShowProgressProperty);
      TokenResourceBinder.CreateTokenBinding(progressBar, NotificationProgressBar.MarginProperty, NotificationTokenResourceKey.NotificationProgressMargin);
      Grid.SetColumn(progressBar, 0);
      Grid.SetRow(progressBar, 1);
      Grid.SetColumnSpan(progressBar, 2);
      layout.Children.Add(progressBar);
   }

   protected override void BuildStyles()
   {
      BuildCommonStyle();
      BuildHeaderStyle();
      BuildContentStyle();
      BuildAnimationStyle();
   }

   private void BuildCommonStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      
      commonStyle.Add(NotificationCard.MarginProperty, NotificationTokenResourceKey.NotificationMarginEdge);
      commonStyle.Add(NotificationCard.WidthProperty, NotificationTokenResourceKey.NotificationWidth);

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

   private void BuildAnimationStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      

      {
         var layoutTransformStyle = new Style(selector => selector.Nesting().Template().Name(LayoutTransformControlPart));
         var moveRightInMotionConfig = MotionFactory.BuildMoveRightInMotion(400, TimeSpan.FromMilliseconds(400), new QuadraticEaseOut(), 
                                                                            FillMode.Forward);
         foreach (var animation in moveRightInMotionConfig.Animations) {
            layoutTransformStyle.Animations.Add(animation);
         }

         commonStyle.Add(layoutTransformStyle);
      }

      var isClosingStyle = new Style(selector => selector.Nesting().PropertyEquals(NotificationCard.IsClosingProperty, true));
      {
         var layoutTransformStyle = new Style(selector => selector.Nesting().Template().Name(LayoutTransformControlPart));
      
         var moveRightOutMotionConfig = MotionFactory.BuildMoveRightOutMotion(400, TimeSpan.FromMilliseconds(400), new QuadraticEaseIn(), FillMode.Forward);
      
         foreach (var animation in moveRightOutMotionConfig.Animations) {
            layoutTransformStyle.Animations.Add(animation);
         }
      
         isClosingStyle.Animations.Add(new Animation()
         {
            Duration = TimeSpan.FromMilliseconds(600),
            Easing = new QuadraticEaseIn(),
            FillMode = FillMode.Forward,
            Children =
            {
               new KeyFrame()
               {
                  Cue = new Cue(1.0),
                  Setters =
                  {
                     new Setter(NotificationCard.IsClosedProperty, true),
                     new Setter(NotificationCard.MarginProperty, new Thickness(0)),
                  }
               }
            }
         });
      
         isClosingStyle.Add(layoutTransformStyle);
      }
      commonStyle.Add(isClosingStyle);
      
      Add(commonStyle);
   }
}