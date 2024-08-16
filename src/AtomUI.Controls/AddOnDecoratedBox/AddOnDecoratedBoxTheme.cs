using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls.AddOnDecoratedBox;

[ControlThemeProvider]
internal class AddOnDecoratedBoxTheme : BaseControlTheme
{
   public const string MainLayoutPart = "PART_FrameDecorator";
   public const string LeftAddOnPart = "PART_LeftAddOn";
   public const string RightAddOnPart = "PART_RightAddOn";
   public const string InnerBoxContentPart = "PART_InnerBoxContent";
   public const string InnerBoxDecoratorPart = "PART_InnerBoxDecorator";

   public const int NormalZIndex = 1000;
   public const int ActivatedZIndex = 2000;

   public AddOnDecoratedBoxTheme(Type targetType) : base(targetType) { }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<AddOnDecoratedBox>((decoratedBox, scope) =>
      {
         var mainLayout = new Grid()
         {
            Name = MainLayoutPart,
            ColumnDefinitions = new ColumnDefinitions()
            {
               new ColumnDefinition(GridLength.Auto),
               new ColumnDefinition(GridLength.Star),
               new ColumnDefinition(GridLength.Auto)
            }
         };
         BuildGridChildren(decoratedBox, mainLayout, scope);
         return mainLayout;
      });
   }

   protected override void BuildStyles()
   {
      BuildFixedStyle();
      BuildCommonStyle();
      BuildFilledStyle();
      BuildOutLineStyle();
      BuildBorderlessStyle();
      BuildDisabledStyle();
   }

   protected virtual void BuildGridChildren(AddOnDecoratedBox decoratedBox, Grid mainLayout, INameScope scope)
   {
      BuildLeftAddOn(mainLayout, scope);
      BuildInnerBox(decoratedBox, mainLayout, scope);
      BuildRightAddOn(mainLayout, scope);
   }

   protected virtual void BuildLeftAddOn(Grid layout, INameScope scope)
   {
      var leftAddOnContentPresenter = new ContentPresenter()
      {
         Name = LeftAddOnPart,
         VerticalAlignment = VerticalAlignment.Stretch,
         VerticalContentAlignment = VerticalAlignment.Center,
         HorizontalAlignment = HorizontalAlignment.Left,
         Focusable = false
      };

      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.ContentProperty,
                                  AddOnDecoratedBox.LeftAddOnProperty);
      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.BorderThicknessProperty,
                                  AddOnDecoratedBox.LeftAddOnBorderThicknessProperty);
      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.CornerRadiusProperty,
                                  AddOnDecoratedBox.LeftAddOnCornerRadiusProperty);
      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.IsVisibleProperty,
                                  AddOnDecoratedBox.LeftAddOnProperty,
                                  BindingMode.Default, ObjectConverters.IsNotNull);
      leftAddOnContentPresenter.RegisterInNameScope(scope);

      TokenResourceBinder.CreateTokenBinding(leftAddOnContentPresenter, ContentPresenter.BackgroundProperty,
                                             AddOnDecoratedBoxResourceKey.AddonBg);
      TokenResourceBinder.CreateTokenBinding(leftAddOnContentPresenter, ContentPresenter.BorderBrushProperty,
                                             GlobalResourceKey.ColorBorder);

      Grid.SetColumn(leftAddOnContentPresenter, 0);
      layout.Children.Add(leftAddOnContentPresenter);
   }

   protected virtual void BuildInnerBox(AddOnDecoratedBox decoratedBox, Grid layout, INameScope scope)
   {
      var innerBoxDecorator = new Border()
      {
         Name = InnerBoxDecoratorPart,
         Transitions = new Transitions()
         {
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BorderBrushProperty),
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
         }
      };
      CreateTemplateParentBinding(innerBoxDecorator, Border.BorderThicknessProperty, AddOnDecoratedBox.BorderThicknessProperty);
      CreateTemplateParentBinding(innerBoxDecorator, Border.CornerRadiusProperty, AddOnDecoratedBox.InnerBoxCornerRadiusProperty);

      innerBoxDecorator.RegisterInNameScope(scope);

      var innerBox = new ContentPresenter()
      {
         Name = InnerBoxContentPart,
      };
      
      CreateTemplateParentBinding(innerBox, ContentPresenter.ContentProperty, AddOnDecoratedBox.ContentProperty);
      CreateTemplateParentBinding(innerBox, ContentPresenter.ContentTemplateProperty, AddOnDecoratedBox.ContentTemplateProperty);
      
      innerBoxDecorator.Child = innerBox;
      layout.Children.Add(innerBoxDecorator);
      Grid.SetColumn(innerBoxDecorator, 1);
   }

   protected virtual void BuildRightAddOn(Grid layout, INameScope scope)
   {
      var rightAddOnContentPresenter = new ContentPresenter()
      {
         Name = RightAddOnPart,
         VerticalAlignment = VerticalAlignment.Stretch,
         VerticalContentAlignment = VerticalAlignment.Center,
         HorizontalAlignment = HorizontalAlignment.Right,
         Focusable = false
      };
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.ContentProperty,
                                  AddOnDecoratedBox.RightAddOnProperty);
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.BorderThicknessProperty,
                                  AddOnDecoratedBox.RightAddOnBorderThicknessProperty);
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.CornerRadiusProperty,
                                  AddOnDecoratedBox.RightAddOnCornerRadiusProperty);
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.IsVisibleProperty,
                                  AddOnDecoratedBox.RightAddOnProperty,
                                  BindingMode.Default, ObjectConverters.IsNotNull);

      TokenResourceBinder.CreateTokenBinding(rightAddOnContentPresenter, ContentPresenter.BackgroundProperty,
                                             AddOnDecoratedBoxResourceKey.AddonBg);
      TokenResourceBinder.CreateTokenBinding(rightAddOnContentPresenter, ContentPresenter.BorderBrushProperty,
                                             GlobalResourceKey.ColorBorder);

      rightAddOnContentPresenter.RegisterInNameScope(scope);
      layout.Children.Add(rightAddOnContentPresenter);
      Grid.SetColumn(rightAddOnContentPresenter, 2);
   }
   
    private void BuildFixedStyle()
   {
      this.Add(AddOnDecoratedBox.VerticalAlignmentProperty, VerticalAlignment.Center);
      this.Add(AddOnDecoratedBox.VerticalContentAlignmentProperty, VerticalAlignment.Center);
      this.Add(ScrollViewer.IsScrollChainingEnabledProperty, true);
   }

   private void BuildCommonStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(AddOnDecoratedBox.ForegroundProperty, GlobalResourceKey.ColorText);

      var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
      decoratorStyle.Add(Border.ZIndexProperty, NormalZIndex);
      commonStyle.Add(decoratorStyle);

      var largeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Large));
      {
         var innerBoxDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
         innerBoxDecoratorStyle.Add(Border.PaddingProperty, LineEditResourceKey.PaddingLG);
         largeStyle.Add(innerBoxDecoratorStyle);
      }

      {
         var addOnStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart),
                                                             selector.Nesting().Template().Name(RightAddOnPart)));
         addOnStyle.Add(ContentPresenter.PaddingProperty, LineEditResourceKey.PaddingLG);
         largeStyle.Add(addOnStyle);
      }
      
      {
         // 左右 AddOn icon 的大小
         var addOnContentIconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart).Descendant().OfType<PathIcon>(),
                                                                        selector.Nesting().Template().Name(RightAddOnPart).Descendant().OfType<PathIcon>()));
         addOnContentIconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSizeLG);
         addOnContentIconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSizeLG);
         largeStyle.Add(addOnContentIconStyle);
      }

      largeStyle.Add(AddOnDecoratedBox.FontSizeProperty, LineEditResourceKey.InputFontSizeLG);
      largeStyle.Add(AddOnDecoratedBox.MinHeightProperty, GlobalResourceKey.FontHeightLG);
      largeStyle.Add(AddOnDecoratedBox.CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
      commonStyle.Add(largeStyle);

      var middleStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Middle));
      {
         var innerBoxDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
         innerBoxDecoratorStyle.Add(Border.PaddingProperty, LineEditResourceKey.Padding);
         middleStyle.Add(innerBoxDecoratorStyle);
      }

      {
         var addOnStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart),
                                                             selector.Nesting().Template().Name(RightAddOnPart)));
         addOnStyle.Add(ContentPresenter.PaddingProperty, LineEditResourceKey.Padding);
         middleStyle.Add(addOnStyle);
      }
      
      {
         // 左右 AddOn icon 的大小
         var addOnContentIconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart).Descendant().OfType<PathIcon>(),
                                                                        selector.Nesting().Template().Name(RightAddOnPart).Descendant().OfType<PathIcon>()));
         addOnContentIconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSize);
         addOnContentIconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSize);
         middleStyle.Add(addOnContentIconStyle);
      }

      middleStyle.Add(AddOnDecoratedBox.FontSizeProperty, LineEditResourceKey.InputFontSize);
      middleStyle.Add(AddOnDecoratedBox.MinHeightProperty, GlobalResourceKey.FontHeight);
      middleStyle.Add(AddOnDecoratedBox.CornerRadiusProperty, GlobalResourceKey.BorderRadius);
      commonStyle.Add(middleStyle);

      var smallStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.SizeTypeProperty, SizeType.Small));
      {
         var innerBoxDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
         innerBoxDecoratorStyle.Add(Border.PaddingProperty, LineEditResourceKey.PaddingSM);
         smallStyle.Add(innerBoxDecoratorStyle);
      }

      {
         var addOnStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart),
                                                             selector.Nesting().Template().Name(RightAddOnPart)));
         addOnStyle.Add(ContentPresenter.PaddingProperty, LineEditResourceKey.PaddingSM);
         smallStyle.Add(addOnStyle);
      }
      
      {
         // 左右 AddOn icon 的大小
         var addOnContentIconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart).Descendant().OfType<PathIcon>(),
                                                                        selector.Nesting().Template().Name(RightAddOnPart).Descendant().OfType<PathIcon>()));
         addOnContentIconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSizeSM);
         addOnContentIconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSizeSM);
         smallStyle.Add(addOnContentIconStyle);
      }
      
      smallStyle.Add(AddOnDecoratedBox.FontSizeProperty, LineEditResourceKey.InputFontSizeSM);
      smallStyle.Add(AddOnDecoratedBox.MinHeightProperty, GlobalResourceKey.FontHeightSM);
      smallStyle.Add(AddOnDecoratedBox.CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      commonStyle.Add(smallStyle);

      Add(commonStyle);
   }

   private void BuildOutLineStyle()
   {
      var outlineStyle =
         new Style(selector => selector.Nesting()
                                       .PropertyEquals(AddOnDecoratedBox.StyleVariantProperty, TextBoxVariant.Outline));

      {
         var innerBoxDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
         innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorBorder);

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Border.BorderBrushProperty, LineEditResourceKey.HoverBorderColor);
         innerBoxDecoratorStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         focusStyle.Add(Border.BorderBrushProperty, LineEditResourceKey.ActiveBorderColor);
         innerBoxDecoratorStyle.Add(focusStyle);
         outlineStyle.Add(innerBoxDecoratorStyle);
      }
      {
         var errorStyle = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.ErrorPC));

         var innerBoxDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
         innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorError);

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorErrorBorderHover);
         innerBoxDecoratorStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         focusStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorError);
         innerBoxDecoratorStyle.Add(focusStyle);

         errorStyle.Add(innerBoxDecoratorStyle);
         outlineStyle.Add(errorStyle);
      }

      {
         var warningStyle = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.WarningPC));
         
         var innerBoxDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
         innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorWarning);

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorWarningBorderHover);
         innerBoxDecoratorStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         focusStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorWarning);
         innerBoxDecoratorStyle.Add(focusStyle);

         warningStyle.Add(innerBoxDecoratorStyle);
         outlineStyle.Add(warningStyle);
      }

      Add(outlineStyle);
   }

   private void BuildBorderlessStyle()
   {
      var borderlessStyle =
         new Style(selector => selector.Nesting()
                                       .PropertyEquals(AddOnDecoratedBox.StyleVariantProperty, TextBoxVariant.Borderless));
      
      {
         var errorStyle = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.ErrorPC));
         var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart)
                                                               .Descendant().OfType<ScrollViewer>());
         scrollViewerStyle.Add(ScrollViewer.ForegroundProperty, GlobalResourceKey.ColorErrorText);
         errorStyle.Add(scrollViewerStyle);
         borderlessStyle.Add(errorStyle);
      }

      {
         var warningStyle = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.WarningPC));
         var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart)
                                                               .Descendant().OfType<ScrollViewer>());
         scrollViewerStyle.Add(ScrollViewer.ForegroundProperty, GlobalResourceKey.ColorWarningText);
         warningStyle.Add(scrollViewerStyle);
         borderlessStyle.Add(warningStyle);
      }

      Add(borderlessStyle);
   }

   private void BuildFilledStyle()
   {
      var filledStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.StyleVariantProperty, TextBoxVariant.Filled));

      {
         var innerBoxDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));

         innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorTransparent);
         innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorFillTertiary);
         filledStyle.Add(innerBoxDecoratorStyle);

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorFillSecondary);
         innerBoxDecoratorStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         focusStyle.Add(Border.BorderBrushProperty, LineEditResourceKey.ActiveBorderColor);
         focusStyle.Add(Border.BackgroundProperty, LineEditResourceKey.ActiveBg);
         innerBoxDecoratorStyle.Add(focusStyle);
         filledStyle.Add(innerBoxDecoratorStyle);
      }

      {
         var errorStyle = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.ErrorPC));
         
         var innerBoxDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));

         innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorTransparent);
         innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorErrorBg);

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorErrorBgHover);
         innerBoxDecoratorStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         focusStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorError);
         focusStyle.Add(Border.BackgroundProperty, LineEditResourceKey.ActiveBg);

         innerBoxDecoratorStyle.Add(focusStyle);

         var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart)
                                                               .Class(StdPseudoClass.FocusWithIn)
                                                               .Descendant().OfType<ScrollViewer>());
         scrollViewerStyle.Add(ScrollViewer.ForegroundProperty, GlobalResourceKey.ColorErrorText);

         errorStyle.Add(scrollViewerStyle);

         errorStyle.Add(innerBoxDecoratorStyle);
         filledStyle.Add(errorStyle);
      }

      {
         var warningStyle = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.WarningPC));

         var innerBoxDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));

         innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorTransparent);
         innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorWarningBg);
         warningStyle.Add(innerBoxDecoratorStyle);

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorWarningBgHover);
         innerBoxDecoratorStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         focusStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorWarning);
         focusStyle.Add(Border.BackgroundProperty, LineEditResourceKey.ActiveBg);
         
         innerBoxDecoratorStyle.Add(focusStyle);

         var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart)
                                                               .Descendant().OfType<ScrollViewer>());
         scrollViewerStyle.Add(ScrollViewer.ForegroundProperty, GlobalResourceKey.ColorWarningText);

         warningStyle.Add(scrollViewerStyle);

         warningStyle.Add(innerBoxDecoratorStyle);
         filledStyle.Add(warningStyle);
      }

      Add(filledStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(AddOnDecoratedBox.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
      decoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
      disabledStyle.Add(decoratorStyle);
      // TODO 暂时这么简单处理吧
      var addOnStyle = new Style(selector => selector.Nesting().Template().OfType<ContentPresenter>());
      addOnStyle.Add(ContentPresenter.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      disabledStyle.Add(addOnStyle);
      Add(disabledStyle);
   }
}