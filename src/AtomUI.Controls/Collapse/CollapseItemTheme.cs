using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media.Transformation;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CollapseItemTheme : BaseControlTheme
{
   public const string MainLayoutPart             = "PART_MainLayout";
   public const string ExpandButtonPart           = "PART_ExpandButton";
   public const string HeaderPresenterPart        = "PART_HeaderPresenter";
   public const string HeaderDecoratorPart        = "PART_HeaderDecorator";
   public const string ContentPresenterPart       = "PART_ContentPresenter";
   public const string ContentAnimationTargetPart = "PART_ContentAnimationTarget";
   
   public CollapseItemTheme() : base(typeof(CollapseItem)) {}
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<CollapseItem>((collapseItem, scope) =>
      {
         var mainLayout = new DockPanel()
         {
            Name = MainLayoutPart,
            LastChildFill = true,
         };

         BuildHeader(mainLayout, scope);
         var animationPanel = new AnimationTargetPanel()
         {
            Name = ContentAnimationTargetPart
         };
         var contentPresenter = new ContentPresenter()
         {
            Name = ContentPresenterPart,
         };
         animationPanel.Children.Add(contentPresenter);
         TokenResourceBinder.CreateGlobalTokenBinding(contentPresenter, ContentPresenter.BorderBrushProperty, GlobalResourceKey.ColorBorder);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, CollapseItem.ContentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, CollapseItem.ContentTemplateProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.BorderThicknessProperty, CollapseItem.ContentBorderThicknessProperty);
         mainLayout.Children.Add(animationPanel);
         animationPanel.RegisterInNameScope(scope);
         contentPresenter.RegisterInNameScope(scope);
         return mainLayout;
      });
   }

   private void BuildHeader(DockPanel layout, INameScope scope)
   {
      var headerDecorator = new Border()
      {
         Name = HeaderDecoratorPart
      };
      headerDecorator.RegisterInNameScope(scope);
      DockPanel.SetDock(headerDecorator, Dock.Top);
         
      TokenResourceBinder.CreateGlobalTokenBinding(headerDecorator, Border.BorderBrushProperty, GlobalResourceKey.ColorBorder);
      CreateTemplateParentBinding(headerDecorator, Border.BorderThicknessProperty, CollapseItem.HeaderBorderThicknessProperty);
      
      var headerLayout = new Grid()
      {
         ColumnDefinitions = new ColumnDefinitions()
         {
            new ColumnDefinition(GridLength.Auto),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Auto),
            new ColumnDefinition(GridLength.Auto)
         }
      };
      
      var expandButton = new IconButton()
      {
         Name = ExpandButtonPart,
         VerticalAlignment = VerticalAlignment.Center,
      };
      expandButton.Transitions = new Transitions();
      expandButton.Transitions.Add(AnimationUtils.CreateTransition<TransformOperationsTransition>(IconButton.RenderTransformProperty));
      CreateTemplateParentBinding(expandButton, IconButton.IconProperty, CollapseItem.ExpandIconProperty);
      headerLayout.Children.Add(expandButton);
      
      var headerPresenter = new ContentPresenter()
      {
         Name = HeaderPresenterPart,
         HorizontalAlignment = HorizontalAlignment.Stretch,
         HorizontalContentAlignment = HorizontalAlignment.Left,
      };
      CreateTemplateParentBinding(headerPresenter, ContentPresenter.ContentProperty, CollapseItem.HeaderProperty);
      CreateTemplateParentBinding(headerPresenter, ContentPresenter.ContentTemplateProperty, CollapseItem.HeaderTemplateProperty);
      Grid.SetColumn(headerPresenter, 1);
      headerPresenter.RegisterInNameScope(scope);
      headerLayout.Children.Add(headerPresenter);
      headerDecorator.Child = headerLayout;
      layout.Children.Add(headerDecorator);
   }
   
   protected override void BuildStyles()
   {
      BuildCommonStyle();
      BuildTriggerStyle();
      BuildTriggerPositionStyle();
      BuildSizeTypeStyle();
      BuildSelectedStyle();
   }

   private void BuildCommonStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
      decoratorStyle.Add(Border.BackgroundProperty, CollapseResourceKey.HeaderBg);
      commonStyle.Add(decoratorStyle);

      var headerPresenter = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
      headerPresenter.Add(ContentPresenter.ForegroundProperty, GlobalResourceKey.ColorTextHeading);
      commonStyle.Add(headerPresenter);
      
      // ExpandIcon 
      var expandIconStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart).Descendant().OfType<PathIcon>());
      expandIconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSizeSM);
      expandIconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSizeSM);
      commonStyle.Add(expandIconStyle);
      
      Add(commonStyle);
   }

   private void BuildSelectedStyle()
   {
      var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
      // Expand Button
      var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
      var transformOptions = new TransformOperations.Builder(1);
      transformOptions.AppendRotate(MathUtils.Deg2Rad(90));
      expandButtonStyle.Add(IconButton.RenderTransformProperty, transformOptions.Build());
      
      selectedStyle.Add(expandButtonStyle);
      Add(selectedStyle);
   }

   private void BuildSizeTypeStyle()
   {
      var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.SizeTypeProperty, SizeType.Large));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
         decoratorStyle.Add(Border.PaddingProperty, CollapseResourceKey.CollapseHeaderPaddingLG);
         decoratorStyle.Add(TextElement.FontSizeProperty, GlobalResourceKey.FontSizeLG);
         decoratorStyle.Add(TextBlock.LineHeightProperty, GlobalResourceKey.FontHeightLG);
         largeSizeStyle.Add(decoratorStyle);
      }

      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
         contentPresenterStyle.Add(ContentPresenter.PaddingProperty, CollapseResourceKey.CollapseContentPaddingLG);
         largeSizeStyle.Add(contentPresenterStyle);
      }
   
      Add(largeSizeStyle);
      
      var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.SizeTypeProperty, SizeType.Middle));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
         decoratorStyle.Add(Border.PaddingProperty, CollapseResourceKey.HeaderPadding);
         decoratorStyle.Add(TextElement.FontSizeProperty, GlobalResourceKey.FontSize);
         decoratorStyle.Add(TextBlock.LineHeightProperty, GlobalResourceKey.FontHeight);
         middleSizeStyle.Add(decoratorStyle);
      }
      
      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
         contentPresenterStyle.Add(ContentPresenter.PaddingProperty, CollapseResourceKey.ContentPadding);
         middleSizeStyle.Add(contentPresenterStyle);
      }
      Add(middleSizeStyle);
      
      var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.SizeTypeProperty, SizeType.Small));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
         decoratorStyle.Add(Border.PaddingProperty, CollapseResourceKey.CollapseHeaderPaddingSM);
         decoratorStyle.Add(TextElement.FontSizeProperty, GlobalResourceKey.FontSize);
         decoratorStyle.Add(TextBlock.LineHeightProperty, GlobalResourceKey.FontHeight);
         smallSizeStyle.Add(decoratorStyle);
      }
      
      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
         contentPresenterStyle.Add(ContentPresenter.PaddingProperty, CollapseResourceKey.CollapseContentPaddingSM);
         smallSizeStyle.Add(contentPresenterStyle);
      }

      Add(smallSizeStyle);
   }

   private void BuildTriggerStyle()
   {
      var headerTriggerHandleStyle = new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.TriggerTypeProperty, CollapseTriggerType.Header));
      var headerDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
      headerDecoratorStyle.Add(Border.CursorProperty, new Cursor(StandardCursorType.Hand));
      headerTriggerHandleStyle.Add(headerDecoratorStyle);
      Add(headerTriggerHandleStyle);
      
      var iconTriggerHandleStyle = new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.TriggerTypeProperty, CollapseTriggerType.Icon));
      var expandIconStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
      expandIconStyle.Add(IconButton.CursorProperty, new Cursor(StandardCursorType.Hand));
      iconTriggerHandleStyle.Add(expandIconStyle);
      Add(iconTriggerHandleStyle);
   }

   private void BuildTriggerPositionStyle()
   {
      var startPositionStyle = new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.ExpandIconPositionProperty, CollapseExpandIconPosition.Start));
      {
         var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
         expandButtonStyle.Add(Grid.ColumnProperty, 0);
         expandButtonStyle.Add(IconButton.MarginProperty, CollapseResourceKey.LeftExpandButtonMargin);
         startPositionStyle.Add(expandButtonStyle);
      }
      Add(startPositionStyle);
      var endPositionStyle = new Style(selector => selector.Nesting().PropertyEquals(CollapseItem.ExpandIconPositionProperty, CollapseExpandIconPosition.End));
      {
         var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
         expandButtonStyle.Add(Grid.ColumnProperty, 3);
         expandButtonStyle.Add(IconButton.MarginProperty, CollapseResourceKey.RightExpandButtonMargin);
         endPositionStyle.Add(expandButtonStyle);
      }
      Add(endPositionStyle);
   }
}