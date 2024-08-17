using AtomUI.Icon;
using AtomUI.Media;
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
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ExpanderTheme : BaseControlTheme
{
   public const string FrameDecoratorPart         = "PART_FrameDecorator";
   public const string MainLayoutPart             = "PART_MainLayout";
   public const string ExpandButtonPart           = "PART_ExpandButton";
   public const string HeaderLayoutTransformPart  = "PART_HeaderLayoutTransform";
   public const string HeaderLayoutPart           = "PART_HeaderLayout";
   public const string HeaderPresenterPart        = "PART_HeaderPresenter";
   public const string HeaderDecoratorPart        = "PART_HeaderDecorator";
   public const string ContentPresenterPart       = "PART_ContentPresenter";
   public const string AddOnContentPresenterPart  = "PART_AddOnContentPresenter";
   public const string ContentAnimationTargetPart = "PART_ContentAnimationTarget";
   
   public ExpanderTheme() : base(typeof(Expander)) {}
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<Expander>((expander, scope) =>
      {
         var frameDecorator = new Border()
         {
            Name = FrameDecoratorPart,
            ClipToBounds = true
         };
         CreateTemplateParentBinding(frameDecorator, Border.BorderThicknessProperty, Expander.EffectiveBorderThicknessProperty);
         
         var mainLayout = new DockPanel()
         {
            Name = MainLayoutPart,
            LastChildFill = true,
         };

         BuildHeader(mainLayout, scope);
         var animationPanel = new AnimationTargetPanel()
         {
            Name = ContentAnimationTargetPart,
            ClipToBounds = true
         };
         var contentPresenter = new ContentPresenter()
         {
            Name = ContentPresenterPart,
         };
         animationPanel.SetCurrentValue(AnimationTargetPanel.IsVisibleProperty, false);
         animationPanel.Children.Add(contentPresenter);
         TokenResourceBinder.CreateGlobalTokenBinding(contentPresenter, ContentPresenter.BorderBrushProperty, GlobalResourceKey.ColorBorder);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, Expander.ContentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, Expander.ContentTemplateProperty);
         
         mainLayout.Children.Add(animationPanel);
         animationPanel.RegisterInNameScope(scope);
         contentPresenter.RegisterInNameScope(scope);

         frameDecorator.Child = mainLayout;
         return frameDecorator;
      });
   }
   
   private void BuildHeader(DockPanel layout, INameScope scope)
   {
      var headerLayoutTransformDecorator = new LayoutTransformControl
      {
         Name = HeaderLayoutTransformPart
      };
      var headerDecorator = new Border()
      {
         Name = HeaderDecoratorPart,
      };
      headerDecorator.RegisterInNameScope(scope);
      
      CreateTemplateParentBinding(headerDecorator, Border.BorderThicknessProperty, Expander.HeaderBorderThicknessProperty);

      var headerLayout = new Grid()
      {
         Name = HeaderLayoutPart,
         ColumnDefinitions =
         {
            new ColumnDefinition(GridLength.Auto),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Auto),
            new ColumnDefinition(GridLength.Auto)
         }
      };
      headerLayout.RegisterInNameScope(scope);
      var expandButton = new IconButton()
      {
         Name = ExpandButtonPart,
         VerticalAlignment = VerticalAlignment.Center,
         HorizontalAlignment = HorizontalAlignment.Left,
      };
      TokenResourceBinder.CreateGlobalTokenBinding(expandButton, IconButton.IconWidthProperty, GlobalResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(expandButton, IconButton.IconHeightProperty, GlobalResourceKey.IconSize);
      
      expandButton.RegisterInNameScope(scope);
      expandButton.Transitions = new Transitions();
      expandButton.Transitions.Add(AnimationUtils.CreateTransition<TransformOperationsTransition>(IconButton.RenderTransformProperty));
      CreateTemplateParentBinding(expandButton, IconButton.IconProperty, Expander.ExpandIconProperty);
      CreateTemplateParentBinding(expandButton, IconButton.IsVisibleProperty, Expander.IsShowExpandIconProperty);
      CreateTemplateParentBinding(expandButton, IconButton.IsEnabledProperty, Expander.IsEnabledProperty);
      headerLayout.Children.Add(expandButton);
      
      var headerPresenter = new ContentPresenter()
      {
         Name = HeaderPresenterPart,
         HorizontalAlignment = HorizontalAlignment.Stretch,
         HorizontalContentAlignment = HorizontalAlignment.Left,
      };
      Grid.SetColumn(headerPresenter, 1);
      CreateTemplateParentBinding(headerPresenter, ContentPresenter.ContentProperty, Expander.HeaderProperty);
      CreateTemplateParentBinding(headerPresenter, ContentPresenter.ContentTemplateProperty, Expander.HeaderTemplateProperty);
      headerPresenter.RegisterInNameScope(scope);
      headerLayout.Children.Add(headerPresenter);

      var addOnContentPresenter = new ContentPresenter()
      {
         Name = AddOnContentPresenterPart,
         VerticalAlignment = VerticalAlignment.Center,
         HorizontalAlignment = HorizontalAlignment.Left
      };
      Grid.SetColumn(addOnContentPresenter, 2);
      CreateTemplateParentBinding(addOnContentPresenter, ContentPresenter.ContentProperty, Expander.AddOnContentProperty);
      CreateTemplateParentBinding(addOnContentPresenter, ContentPresenter.ContentTemplateProperty, Expander.AddOnContentTemplateProperty);
      headerLayout.Children.Add(addOnContentPresenter);
      
      headerDecorator.Child = headerLayout;
      headerLayoutTransformDecorator.Child = headerDecorator;
      layout.Children.Add(headerLayoutTransformDecorator);
   }
   
   protected override void BuildStyles()
   {
      BuildCommonStyle();
      BuildExpandButtonPositionStyle();
      BuildExpandDirectionStyle();
      BuildTriggerStyle();
      BuildSizeTypeStyle();
      BuildSelectedStyle();
      BuildBorderlessStyle();
      BuildGhostStyle();
      BuildDisabledStyle();
      BuildAddOnContentStyle();
   }

   private void BuildCommonStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      
      var frameDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(FrameDecoratorPart));
      frameDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorBorder);
      frameDecoratorStyle.Add(Border.CornerRadiusProperty, ExpanderResourceKey.ExpanderBorderRadius);
      commonStyle.Add(frameDecoratorStyle);

      {
         var headerDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
         headerDecoratorStyle.Add(Border.BackgroundProperty, ExpanderResourceKey.HeaderBg);
         headerDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorTransparent);
         commonStyle.Add(headerDecoratorStyle);
      }

      var headerPresenter = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
      headerPresenter.Add(ContentPresenter.ForegroundProperty, GlobalResourceKey.ColorTextHeading);
      commonStyle.Add(headerPresenter);
      
      // ExpandIcon 
      var expandIconStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart).Descendant().OfType<PathIcon>());
      expandIconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSizeSM);
      expandIconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSizeSM);
      commonStyle.Add(expandIconStyle);
      
      // 设置打开状态
      var expandedStyle = new Style(selector => selector.Nesting().PropertyEquals(Expander.IsExpandedProperty, true));
      {
         var headerDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
         headerDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorBorder);
         expandedStyle.Add(headerDecoratorStyle);
      }
      commonStyle.Add(expandedStyle);
      
      Add(commonStyle);
   }
   
   private void BuildExpandButtonPositionStyle()
   {
      // 设置展开按钮的样式
      var startPositionStyle = new Style(selector => selector.Nesting().PropertyEquals(Expander.ExpandIconPositionProperty, ExpanderIconPosition.Start));
      {
         var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
         expandButtonStyle.Add(Grid.ColumnProperty, 0);
         expandButtonStyle.Add(IconButton.MarginProperty, ExpanderResourceKey.LeftExpandButtonHMargin);
         startPositionStyle.Add(expandButtonStyle);
      }
      Add(startPositionStyle);
      var endPositionStyle = new Style(selector => selector.Nesting().PropertyEquals(Expander.ExpandIconPositionProperty, ExpanderIconPosition.End));
      {
         var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
         expandButtonStyle.Add(Grid.ColumnProperty, 3);
         expandButtonStyle.Add(IconButton.MarginProperty, ExpanderResourceKey.RightExpandButtonHMargin);
         endPositionStyle.Add(expandButtonStyle);
      }
      Add(endPositionStyle);
   }

   private void BuildExpandDirectionStyle()
   {
      var upStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandUpPC));
      
      // 设置水平和垂直对齐
      upStyle.Add(Expander.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
      upStyle.Add(Expander.VerticalAlignmentProperty, VerticalAlignment.Bottom);
      // TODO 看看到底是不是需要，暂时注释掉
      // {
      //    var expandedStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandedPC));
      //    expandedStyle.Add(Expander.VerticalAlignmentProperty, VerticalAlignment.Stretch);
      //    upStyle.Add(expandedStyle);
      // }
      
      {
         var headerLayoutTransformStyle = new Style(selector => selector.Nesting().Template().Name(HeaderLayoutTransformPart));
         headerLayoutTransformStyle.Add(DockPanel.DockProperty, Dock.Bottom);
         upStyle.Add(headerLayoutTransformStyle);
      }
      Add(upStyle);
      
      var downStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandDownPC));
      // 设置水平和垂直对齐
      downStyle.Add(Expander.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
      downStyle.Add(Expander.VerticalAlignmentProperty, VerticalAlignment.Top);
      // TODO 看看到底是不是需要，暂时注释掉
      // {
      //    var expandedStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandedPC));
      //    expandedStyle.Add(Expander.VerticalAlignmentProperty, VerticalAlignment.Stretch);
      //    downStyle.Add(expandedStyle);
      // }
      
      {
         var headerLayoutTransformStyle = new Style(selector => selector.Nesting().Template().Name(HeaderLayoutTransformPart));
         headerLayoutTransformStyle.Add(DockPanel.DockProperty, Dock.Top);
         downStyle.Add(headerLayoutTransformStyle);
      }
      Add(downStyle);
      
      var leftStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandLeftPC));
      // 设置水平和垂直对齐
      leftStyle.Add(Expander.HorizontalAlignmentProperty, HorizontalAlignment.Right);
      leftStyle.Add(Expander.VerticalAlignmentProperty, VerticalAlignment.Stretch);
      // TODO 看看到底是不是需要，暂时注释掉
      // {
      //    var expandedStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandedPC));
      //    expandedStyle.Add(Expander.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
      //    leftStyle.Add(expandedStyle);
      // }
      
      {
         var headerLayoutTransformStyle = new Style(selector => selector.Nesting().Template().Name(HeaderLayoutTransformPart));
         headerLayoutTransformStyle.Add(DockPanel.DockProperty, Dock.Right);
         headerLayoutTransformStyle.Add(LayoutTransformControl.LayoutTransformProperty, new RotateTransform(90));
         leftStyle.Add(headerLayoutTransformStyle);
      }
      Add(leftStyle);
      
      var rightStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandRightPC));
      // 设置水平和垂直对齐
      rightStyle.Add(Expander.HorizontalAlignmentProperty, HorizontalAlignment.Left);
      // TODO 看看到底是不是需要，暂时注释掉
      // {
      //    var expandedStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandedPC));
      //    expandedStyle.Add(Expander.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
      //    rightStyle.Add(expandedStyle);
      // }
      rightStyle.Add(Expander.VerticalAlignmentProperty, VerticalAlignment.Stretch);
      {
         var headerLayoutTransformStyle = new Style(selector => selector.Nesting().Template().Name(HeaderLayoutTransformPart));
         headerLayoutTransformStyle.Add(LayoutTransformControl.LayoutTransformProperty, new RotateTransform(90));
         headerLayoutTransformStyle.Add(DockPanel.DockProperty, Dock.Left);
         rightStyle.Add(headerLayoutTransformStyle);
      }
      Add(rightStyle);
   }

   private void BuildSelectedStyle()
   {
      var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Expanded));
      var upStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandUpPC));

      {
            
         // Expand Button
         var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
         var transformOptions = new TransformOperations.Builder(1);
         transformOptions.AppendRotate(MathUtils.Deg2Rad(-90));
         expandButtonStyle.Add(IconButton.RenderTransformProperty, transformOptions.Build());
      
         upStyle.Add(expandButtonStyle);
      }
      
      selectedStyle.Add(upStyle);
      
      var downStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandDownPC));
      {
            
         // Expand Button
         var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
         var transformOptions = new TransformOperations.Builder(1);
         transformOptions.AppendRotate(MathUtils.Deg2Rad(90));
         expandButtonStyle.Add(IconButton.RenderTransformProperty, transformOptions.Build());
      
         downStyle.Add(expandButtonStyle);
      }
      
      selectedStyle.Add(downStyle);
      
      var leftStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandLeftPC));
      {
            
         // Expand Button
         var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
         var transformOptions = new TransformOperations.Builder(1);
         transformOptions.AppendRotate(MathUtils.Deg2Rad(90));
         expandButtonStyle.Add(IconButton.RenderTransformProperty, transformOptions.Build());
      
         leftStyle.Add(expandButtonStyle);
      }
      selectedStyle.Add(leftStyle);
      
      var rightStyle = new Style(selector => selector.Nesting().Class(Expander.ExpandRightPC));
      {
            
         // Expand Button
         var expandButtonStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
         var transformOptions = new TransformOperations.Builder(1);
         transformOptions.AppendRotate(MathUtils.Deg2Rad(-90));
         expandButtonStyle.Add(IconButton.RenderTransformProperty, transformOptions.Build());
      
         rightStyle.Add(expandButtonStyle);
      }
      selectedStyle.Add(rightStyle);
      
      Add(selectedStyle);
   }
   
   private void BuildSizeTypeStyle()
   {
      var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Expander.SizeTypeProperty, SizeType.Large));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
         decoratorStyle.Add(Border.PaddingProperty, ExpanderResourceKey.HeaderPaddingLG);
         decoratorStyle.Add(TextElement.FontSizeProperty, GlobalResourceKey.FontSizeLG);
         decoratorStyle.Add(TextBlock.LineHeightProperty, GlobalResourceKey.FontHeightLG);
         largeSizeStyle.Add(decoratorStyle);
      }

      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
         contentPresenterStyle.Add(ContentPresenter.PaddingProperty, ExpanderResourceKey.ContentPaddingLG);
         largeSizeStyle.Add(contentPresenterStyle);
      }
   
      Add(largeSizeStyle);
      
      var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Expander.SizeTypeProperty, SizeType.Middle));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
         decoratorStyle.Add(Border.PaddingProperty, ExpanderResourceKey.HeaderPadding);
         decoratorStyle.Add(TextElement.FontSizeProperty, GlobalResourceKey.FontSize);
         decoratorStyle.Add(TextBlock.LineHeightProperty, GlobalResourceKey.FontHeight);
         middleSizeStyle.Add(decoratorStyle);
      }
      
      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
         contentPresenterStyle.Add(ContentPresenter.PaddingProperty, ExpanderResourceKey.ContentPadding);
         middleSizeStyle.Add(contentPresenterStyle);
      }
      Add(middleSizeStyle);
      
      var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(Expander.SizeTypeProperty, SizeType.Small));
      {
         var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
         decoratorStyle.Add(Border.PaddingProperty, ExpanderResourceKey.HeaderPaddingSM);
         decoratorStyle.Add(TextElement.FontSizeProperty, GlobalResourceKey.FontSize);
         decoratorStyle.Add(TextBlock.LineHeightProperty, GlobalResourceKey.FontHeight);
         smallSizeStyle.Add(decoratorStyle);
      }
      
      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
         contentPresenterStyle.Add(ContentPresenter.PaddingProperty, ExpanderResourceKey.ContentPaddingSM);
         smallSizeStyle.Add(contentPresenterStyle);
      }

      Add(smallSizeStyle);
   }

   private void BuildTriggerStyle()
   {
      var headerTriggerHandleStyle = new Style(selector => selector.Nesting().PropertyEquals(Expander.TriggerTypeProperty, ExpanderTriggerType.Header));
      var headerDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
      headerDecoratorStyle.Add(Border.CursorProperty, new Cursor(StandardCursorType.Hand));
      headerTriggerHandleStyle.Add(headerDecoratorStyle);
      Add(headerTriggerHandleStyle);
      
      var iconTriggerHandleStyle = new Style(selector => selector.Nesting().PropertyEquals(Expander.TriggerTypeProperty, ExpanderTriggerType.Icon));
      var expandIconStyle = new Style(selector => selector.Nesting().Template().Name(ExpandButtonPart));
      expandIconStyle.Add(IconButton.CursorProperty, new Cursor(StandardCursorType.Hand));
      iconTriggerHandleStyle.Add(expandIconStyle);
      Add(iconTriggerHandleStyle);
   }
   
   private void BuildBorderlessStyle()
   {
      var borderlessStyle =
         new Style(selector => selector.Nesting().PropertyEquals(Expander.IsBorderlessProperty, true));
      var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
      contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, CollapseResourceKey.HeaderBg);
      borderlessStyle.Add(contentPresenterStyle);
      Add(borderlessStyle);
   }

   private void BuildGhostStyle()
   {
      var ghostStyle =
         new Style(selector => selector.Nesting().PropertyEquals(Expander.IsGhostStyleProperty, true));
      var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
      decoratorStyle.Add(Border.BackgroundProperty, ExpanderResourceKey.ContentBg);
      ghostStyle.Add(decoratorStyle);
      Add(ghostStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle =
         new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(Expander.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      var headerContentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart).Descendant().OfType<ContentPresenter>());
      headerContentPresenterStyle.Add(ContentPresenter.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      disabledStyle.Add(headerContentPresenterStyle);
      
      Add(disabledStyle);
   }

   private void BuildAddOnContentStyle()
   {
      var addOnContentStyle = new Style(selector => selector.Nesting().Template().Name(AddOnContentPresenterPart).Descendant().OfType<PathIcon>());
      addOnContentStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSize);
      addOnContentStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSize);
      Add(addOnContentStyle);
   }

}