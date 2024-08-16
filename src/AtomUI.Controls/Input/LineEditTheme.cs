using AtomUI.Data;
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
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class LineEditTheme : BaseControlTheme
{
   public const string MainLayoutPart = "PART_FrameDecorator";
   public const string TextPresenterPart = "PART_TextPresenter";
   public const string WatermarkPart = "PART_Watermark";
   public const string ScrollViewerPart = "PART_ScrollViewer";
   public const string LeftAddOnPart = "PART_LeftAddOn";
   public const string RightAddOnPart = "PART_RightAddOn";
   public const string LeftInnerContentPart = "PART_LeftInnerContent";
   public const string RightInnerContentPart = "PART_RightInnerContent";
   public const string ClearButtonPart = "PART_ClearButton";
   public const string RevealButtonPart = "PART_RevealButton";
   public const string LineEditKernelPart = "PART_LineEditKernel";
   public const string LineEditKernelDecoratorPart = "PART_LineEditKernelDecorator";

   public const int NormalZIndex    = 1000;
   public const int ActivatedZIndex = 2000;

   public LineEditTheme(Type targetType) : base(targetType) { }
   public LineEditTheme() : base(typeof(LineEdit)) { }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<LineEdit>((lineEdit, scope) =>
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
         BuildGridChildren(lineEdit, mainLayout, scope);
         return mainLayout;
      });
   }

   protected virtual void BuildGridChildren(LineEdit lineEdit, Grid layout, INameScope scope)
   {
      BuildLeftAddOn(layout, scope);
      BuildLineEditKernel(lineEdit, layout, scope);
      BuildRightAddOn(layout, scope);
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
                                  LineEdit.LeftAddOnProperty);
      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.BorderThicknessProperty,
                                  LineEdit.LeftAddOnBorderThicknessProperty);
      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.CornerRadiusProperty,
                                  LineEdit.LeftAddOnCornerRadiusProperty);
      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.IsVisibleProperty,
                                  LineEdit.LeftAddOnProperty,
                                  BindingMode.Default, ObjectConverters.IsNotNull);
      leftAddOnContentPresenter.RegisterInNameScope(scope);

      TokenResourceBinder.CreateTokenBinding(leftAddOnContentPresenter, ContentPresenter.BackgroundProperty,
                                             LineEditResourceKey.AddonBg);
      TokenResourceBinder.CreateTokenBinding(leftAddOnContentPresenter, ContentPresenter.BorderBrushProperty,
                                             GlobalResourceKey.ColorBorder);

      Grid.SetColumn(leftAddOnContentPresenter, 0);
      layout.Children.Add(leftAddOnContentPresenter);
   }

   protected virtual void BuildLineEditKernel(LineEdit lineEdit, Grid layout, INameScope scope)
   {
      var kernelDecorator = new Border()
      {
         Name = LineEditKernelDecoratorPart,
         Transitions = new Transitions()
         {
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BorderBrushProperty),
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
         }
      };

      kernelDecorator.RegisterInNameScope(scope);

      var kernelLayout = new LineEditKernel()
      {
         Name = LineEditKernelPart,
         Cursor = new Cursor(StandardCursorType.Ibeam)
      };

      CreateTemplateParentBinding(kernelDecorator, Border.BorderThicknessProperty, LineEdit.BorderThicknessProperty);
      CreateTemplateParentBinding(kernelDecorator, Border.CornerRadiusProperty,
                                  LineEdit.EditKernelCornerRadiusProperty);

      BuildInnerLeftContent(kernelLayout, scope);
      BuildTextPresenter(lineEdit, kernelLayout, scope);
      BuildClearButton(kernelLayout, scope);
      BuildRevealButton(kernelLayout, scope);
      BuildInnerRightContent(kernelLayout, scope);

      kernelDecorator.Child = kernelLayout;
      layout.Children.Add(kernelDecorator);
      Grid.SetColumn(kernelDecorator, 1);
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
                                  LineEdit.RightAddOnProperty);
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.BorderThicknessProperty,
                                  LineEdit.RightAddOnBorderThicknessProperty);
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.CornerRadiusProperty,
                                  LineEdit.RightAddOnCornerRadiusProperty);
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.IsVisibleProperty,
                                  LineEdit.RightAddOnProperty,
                                  BindingMode.Default, ObjectConverters.IsNotNull);

      TokenResourceBinder.CreateTokenBinding(rightAddOnContentPresenter, ContentPresenter.BackgroundProperty,
                                             LineEditResourceKey.AddonBg);
      TokenResourceBinder.CreateTokenBinding(rightAddOnContentPresenter, ContentPresenter.BorderBrushProperty,
                                             GlobalResourceKey.ColorBorder);

      rightAddOnContentPresenter.RegisterInNameScope(scope);
      layout.Children.Add(rightAddOnContentPresenter);
      Grid.SetColumn(rightAddOnContentPresenter, 2);
   }

   protected virtual void BuildInnerLeftContent(LineEditKernel layout, INameScope scope)
   {
      var innerLeftContentPresenter = new ContentPresenter()
      {
         Name = LeftInnerContentPart,
         VerticalAlignment = VerticalAlignment.Center
      };
      CreateTemplateParentBinding(innerLeftContentPresenter, ContentPresenter.IsVisibleProperty,
                                  LineEdit.InnerLeftContentProperty,
                                  BindingMode.Default, ObjectConverters.IsNotNull);
      CreateTemplateParentBinding(innerLeftContentPresenter, ContentPresenter.ContentProperty,
                                  LineEdit.InnerLeftContentProperty);
      innerLeftContentPresenter.RegisterInNameScope(scope);
      layout.LeftInnerContent = innerLeftContentPresenter;
   }

   protected virtual void BuildInnerRightContent(LineEditKernel layout, INameScope scope)
   {
      var innerRightContentPresenter = new ContentPresenter()
      {
         Name = RightInnerContentPart
      };
      CreateTemplateParentBinding(innerRightContentPresenter, ContentPresenter.IsVisibleProperty,
                                  LineEdit.InnerRightContentProperty,
                                  BindingMode.Default, ObjectConverters.IsNotNull);
      CreateTemplateParentBinding(innerRightContentPresenter, ContentPresenter.ContentProperty,
                                  LineEdit.InnerRightContentProperty);
      innerRightContentPresenter.RegisterInNameScope(scope);
      layout.RightInnerContent = innerRightContentPresenter;
   }

   protected virtual void BuildTextPresenter(LineEdit lineEdit, LineEditKernel layout, INameScope scope)
   {
      var scrollViewer = new ScrollViewer
      {
         Name = ScrollViewerPart,
      };

      // TODO attach 属性不知道怎么指定 Avalonia 控件所在的名称控件，无法用模板绑定的方式进行绑定
      BindUtils.RelayBind(lineEdit, ScrollViewer.AllowAutoHideProperty, scrollViewer,
                          ScrollViewer.AllowAutoHideProperty);
      BindUtils.RelayBind(lineEdit, ScrollViewer.HorizontalScrollBarVisibilityProperty, scrollViewer,
                          ScrollViewer.HorizontalScrollBarVisibilityProperty);
      BindUtils.RelayBind(lineEdit, ScrollViewer.VerticalScrollBarVisibilityProperty, scrollViewer,
                          ScrollViewer.VerticalScrollBarVisibilityProperty);
      BindUtils.RelayBind(lineEdit, ScrollViewer.VerticalScrollBarVisibilityProperty, scrollViewer,
                          ScrollViewer.IsScrollChainingEnabledProperty);

      scrollViewer.RegisterInNameScope(scope);

      var textPresenterLayout = new Panel();

      var watermark = new TextBlock
      {
         Name = WatermarkPart,
         Opacity = 0.5
      };
      CreateTemplateParentBinding(watermark, TextBlock.HorizontalAlignmentProperty,
                                  LineEdit.HorizontalContentAlignmentProperty);
      CreateTemplateParentBinding(watermark, TextBlock.VerticalAlignmentProperty,
                                  LineEdit.VerticalContentAlignmentProperty);
      CreateTemplateParentBinding(watermark, TextBlock.TextProperty, LineEdit.WatermarkProperty);
      CreateTemplateParentBinding(watermark, TextBlock.TextAlignmentProperty, LineEdit.TextAlignmentProperty);
      CreateTemplateParentBinding(watermark, TextBlock.TextWrappingProperty, LineEdit.TextWrappingProperty);
      CreateTemplateParentBinding(watermark, TextBlock.IsVisibleProperty, LineEdit.TextProperty, BindingMode.Default,
                                  StringConverters.IsNullOrEmpty);

      watermark.RegisterInNameScope(scope);

      var textPresenter = new TextPresenter
      {
         Name = TextPresenterPart,
      };

      CreateTemplateParentBinding(textPresenter, TextPresenter.HorizontalAlignmentProperty,
                                  LineEdit.HorizontalContentAlignmentProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.VerticalAlignmentProperty,
                                  LineEdit.VerticalContentAlignmentProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.CaretBlinkIntervalProperty,
                                  LineEdit.CaretBlinkIntervalProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.CaretBrushProperty, LineEdit.CaretBrushProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.CaretIndexProperty, LineEdit.CaretIndexProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.LineHeightProperty, LineEdit.LineHeightProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.PasswordCharProperty, LineEdit.PasswordCharProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.RevealPasswordProperty, LineEdit.RevealPasswordProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.SelectionBrushProperty, LineEdit.SelectionBrushProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.SelectionStartProperty, LineEdit.SelectionStartProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.SelectionEndProperty, LineEdit.SelectionEndProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.SelectionForegroundBrushProperty,
                                  LineEdit.SelectionForegroundBrushProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.TextProperty, LineEdit.TextProperty, BindingMode.TwoWay);
      CreateTemplateParentBinding(textPresenter, TextPresenter.TextAlignmentProperty, LineEdit.TextAlignmentProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.TextWrappingProperty, LineEdit.TextWrappingProperty);

      textPresenterLayout.Children.Add(watermark);
      textPresenterLayout.Children.Add(textPresenter);

      textPresenter.RegisterInNameScope(scope);
      scrollViewer.Content = textPresenterLayout;
      layout.TextPresenter = scrollViewer;
   }

   protected virtual void BuildClearButton(LineEditKernel layout, INameScope scope)
   {
      var closeIcon = new PathIcon()
      {
         Kind = "CloseCircleFilled"
      };
      var clearButton = new IconButton()
      {
         Name = ClearButtonPart,
         Icon = closeIcon
      };

      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.WidthProperty, GlobalResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.HeightProperty, GlobalResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.NormalFilledBrushProperty,
                                                   GlobalResourceKey.ColorTextQuaternary);
      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.ActiveFilledBrushProperty,
                                                   GlobalResourceKey.ColorTextTertiary);
      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.SelectedFilledBrushProperty,
                                                   GlobalResourceKey.ColorText);
      CreateTemplateParentBinding(clearButton, IconButton.CommandProperty, nameof(LineEdit.Clear));

      clearButton.RegisterInNameScope(scope);
      CreateTemplateParentBinding(clearButton, IconButton.IsVisibleProperty,
                                  LineEdit.IsEffectiveShowClearButtonProperty);
      layout.ClearButton = clearButton;
   }

   protected virtual void BuildRevealButton(LineEditKernel layout, INameScope scope)
   {
      var checkedIcon = new PathIcon()
      {
         Kind = "EyeTwoTone"
      };

      TokenResourceBinder.CreateGlobalTokenBinding(checkedIcon, PathIcon.WidthProperty, GlobalResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(checkedIcon, PathIcon.HeightProperty, GlobalResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(checkedIcon, PathIcon.PrimaryFilledBrushProperty,
                                                   LineEditResourceKey.ActiveBorderColor);
      TokenResourceBinder.CreateGlobalTokenBinding(checkedIcon, PathIcon.SecondaryFilledBrushProperty,
                                                   GlobalResourceKey.ColorPrimaryBgHover);

      var unCheckedIcon = new PathIcon()
      {
         Kind = "EyeInvisibleOutlined"
      };

      TokenResourceBinder.CreateGlobalTokenBinding(unCheckedIcon, PathIcon.WidthProperty, GlobalResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(unCheckedIcon, PathIcon.HeightProperty, GlobalResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(unCheckedIcon, PathIcon.NormalFilledBrushProperty,
                                                   GlobalResourceKey.ColorTextQuaternary);
      TokenResourceBinder.CreateGlobalTokenBinding(unCheckedIcon, PathIcon.ActiveFilledBrushProperty,
                                                   GlobalResourceKey.ColorTextTertiary);
      TokenResourceBinder.CreateGlobalTokenBinding(unCheckedIcon, PathIcon.SelectedFilledBrushProperty,
                                                   GlobalResourceKey.ColorText);

      var revealButton = new ToggleIconButton()
      {
         Name = RevealButtonPart,
         CheckedIcon = checkedIcon,
         UnCheckedIcon = unCheckedIcon
      };

      revealButton.RegisterInNameScope(scope);

      CreateTemplateParentBinding(revealButton, ToggleIconButton.IsVisibleProperty,
                                  LineEdit.IsEnableRevealButtonProperty);
      CreateTemplateParentBinding(revealButton, ToggleIconButton.IsCheckedProperty, LineEdit.RevealPasswordProperty,
                                  BindingMode.TwoWay);

      layout.RevealButton = revealButton;
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

   private void BuildFixedStyle()
   {
      this.Add(LineEdit.SelectionBrushProperty, GlobalResourceKey.SelectionBackground);
      this.Add(LineEdit.SelectionForegroundBrushProperty, GlobalResourceKey.SelectionForeground);
      this.Add(LineEdit.VerticalAlignmentProperty, VerticalAlignment.Center);
      this.Add(LineEdit.VerticalContentAlignmentProperty, VerticalAlignment.Center);
      this.Add(ScrollViewer.IsScrollChainingEnabledProperty, true);
   }

   private void BuildCommonStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(LineEdit.ForegroundProperty, GlobalResourceKey.ColorText);

      var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart));
      decoratorStyle.Add(Border.ZIndexProperty, NormalZIndex);
      commonStyle.Add(decoratorStyle);

      // 输入框左右小组件的 margin 设置
      var leftInnerContentStyle = new Style(selector => selector.Nesting().Template().Name(LeftInnerContentPart));
      leftInnerContentStyle.Add(ContentPresenter.PaddingProperty, LineEditResourceKey.LeftInnerAddOnMargin);
      commonStyle.Add(leftInnerContentStyle);

      var rightInnerContentStyle = new Style(selector => selector.Nesting().Template().Name(RightInnerContentPart));
      rightInnerContentStyle.Add(ContentPresenter.PaddingProperty, LineEditResourceKey.RightInnerAddOnMargin);
      commonStyle.Add(rightInnerContentStyle);

      var clearButtonStyle = new Style(selector => selector.Nesting().Template().Name(ClearButtonPart));
      clearButtonStyle.Add(IconButton.PaddingProperty, LineEditResourceKey.RightInnerAddOnMargin);
      commonStyle.Add(clearButtonStyle);

      var revealButtonStyle = new Style(selector => selector.Nesting().Template().Name(RevealButtonPart));
      revealButtonStyle.Add(ToggleIconButton.PaddingProperty, LineEditResourceKey.RightInnerAddOnMargin);
      commonStyle.Add(revealButtonStyle);
      
      {
         // 左右 inner icon 的大小
         var innerContentIconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftInnerContentPart).Descendant().OfType<PathIcon>(),
                                                                        selector.Nesting().Template().Name(RightInnerContentPart).Descendant().OfType<PathIcon>()));
         innerContentIconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSize);
         innerContentIconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSize);
         commonStyle.Add(innerContentIconStyle);
      }

      var largeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(LineEdit.SizeTypeProperty, SizeType.Large));
      {
         var editKernelDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart));
         editKernelDecoratorStyle.Add(Border.PaddingProperty, LineEditResourceKey.PaddingLG);
         largeStyle.Add(editKernelDecoratorStyle);
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

      largeStyle.Add(LineEdit.FontSizeProperty, LineEditResourceKey.InputFontSizeLG);
      largeStyle.Add(LineEdit.LineHeightProperty, GlobalResourceKey.FontHeightLG);
      largeStyle.Add(LineEdit.CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
      commonStyle.Add(largeStyle);

      var middleStyle =
         new Style(selector => selector.Nesting().PropertyEquals(LineEdit.SizeTypeProperty, SizeType.Middle));
      {
         var editKernelDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart));
         editKernelDecoratorStyle.Add(Border.PaddingProperty, LineEditResourceKey.Padding);
         middleStyle.Add(editKernelDecoratorStyle);
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

      middleStyle.Add(LineEdit.FontSizeProperty, LineEditResourceKey.InputFontSize);
      middleStyle.Add(LineEdit.LineHeightProperty, GlobalResourceKey.FontHeight);
      middleStyle.Add(LineEdit.CornerRadiusProperty, GlobalResourceKey.BorderRadius);
      commonStyle.Add(middleStyle);

      var smallStyle =
         new Style(selector => selector.Nesting().PropertyEquals(LineEdit.SizeTypeProperty, SizeType.Small));
      {
         var editKernelDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart));
         editKernelDecoratorStyle.Add(Border.PaddingProperty, LineEditResourceKey.PaddingSM);
         smallStyle.Add(editKernelDecoratorStyle);
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
      
      smallStyle.Add(LineEdit.FontSizeProperty, LineEditResourceKey.InputFontSizeSM);
      smallStyle.Add(LineEdit.LineHeightProperty, GlobalResourceKey.FontHeightSM);
      smallStyle.Add(LineEdit.CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      commonStyle.Add(smallStyle);

      Add(commonStyle);
   }

   private void BuildOutLineStyle()
   {
      var outlineStyle =
         new Style(selector => selector.Nesting()
                                       .PropertyEquals(LineEdit.StyleVariantProperty, TextBoxVariant.Outline));

      {
         var editKernelDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart));
         editKernelDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorBorder);

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Border.BorderBrushProperty, LineEditResourceKey.HoverBorderColor);
         editKernelDecoratorStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         focusStyle.Add(Border.BorderBrushProperty, LineEditResourceKey.ActiveBorderColor);
         editKernelDecoratorStyle.Add(focusStyle);
         outlineStyle.Add(editKernelDecoratorStyle);
      }
      {
         var errorStyle = new Style(selector => selector.Nesting().Class(LineEdit.ErrorPC));
         {
            var innerContentIconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftInnerContentPart).Descendant().OfType<PathIcon>(),
                                                                           selector.Nesting().Template().Name(RightInnerContentPart).Descendant().OfType<PathIcon>()));
            innerContentIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorError);
            errorStyle.Add(innerContentIconStyle);
         }

         var editKernelDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart));
         editKernelDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorError);

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorErrorBorderHover);
         editKernelDecoratorStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         focusStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorError);
         editKernelDecoratorStyle.Add(focusStyle);

         errorStyle.Add(editKernelDecoratorStyle);
         outlineStyle.Add(errorStyle);
      }

      {
         var warningStyle = new Style(selector => selector.Nesting().Class(LineEdit.WarningPC));

         {
            var innerContentIconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftInnerContentPart).Descendant().OfType<PathIcon>(),
                                                                           selector.Nesting().Template().Name(RightInnerContentPart).Descendant().OfType<PathIcon>()));
            innerContentIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorWarning);
            warningStyle.Add(innerContentIconStyle);
         }
         
         var editKernelDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart));
         editKernelDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorWarning);

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorWarningBorderHover);
         editKernelDecoratorStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         focusStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorWarning);
         editKernelDecoratorStyle.Add(focusStyle);

         warningStyle.Add(editKernelDecoratorStyle);
         outlineStyle.Add(warningStyle);
      }

      Add(outlineStyle);
   }

   private void BuildBorderlessStyle()
   {
      var borderlessStyle =
         new Style(selector => selector.Nesting()
                                       .PropertyEquals(LineEdit.StyleVariantProperty, TextBoxVariant.Borderless));
      
      {
         var errorStyle = new Style(selector => selector.Nesting().Class(LineEdit.ErrorPC));
         {
            var innerContentIconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftInnerContentPart).Descendant().OfType<PathIcon>(),
                                                                           selector.Nesting().Template().Name(RightInnerContentPart).Descendant().OfType<PathIcon>()));
            innerContentIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorError);
            errorStyle.Add(innerContentIconStyle);
         }
         var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart)
                                                               .Descendant().OfType<ScrollViewer>());
         scrollViewerStyle.Add(ScrollViewer.ForegroundProperty, GlobalResourceKey.ColorErrorText);
         errorStyle.Add(scrollViewerStyle);
         borderlessStyle.Add(errorStyle);
      }

      {
         var warningStyle = new Style(selector => selector.Nesting().Class(LineEdit.WarningPC));
         {
            var innerContentIconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftInnerContentPart).Descendant().OfType<PathIcon>(),
                                                                           selector.Nesting().Template().Name(RightInnerContentPart).Descendant().OfType<PathIcon>()));
            innerContentIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorWarning);
            warningStyle.Add(innerContentIconStyle);
         }
         var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart)
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
         new Style(selector => selector.Nesting().PropertyEquals(LineEdit.StyleVariantProperty, TextBoxVariant.Filled));

      {
         var editKernelDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart));

         editKernelDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorTransparent);
         editKernelDecoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorFillTertiary);
         filledStyle.Add(editKernelDecoratorStyle);

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorFillSecondary);
         editKernelDecoratorStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         focusStyle.Add(Border.BorderBrushProperty, LineEditResourceKey.ActiveBorderColor);
         focusStyle.Add(Border.BackgroundProperty, LineEditResourceKey.ActiveBg);
         editKernelDecoratorStyle.Add(focusStyle);
         filledStyle.Add(editKernelDecoratorStyle);
      }

      {
         var errorStyle = new Style(selector => selector.Nesting().Class(LineEdit.ErrorPC));

         {
            var innerContentIconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftInnerContentPart).Descendant().OfType<PathIcon>(),
                                                                           selector.Nesting().Template().Name(RightInnerContentPart).Descendant().OfType<PathIcon>()));
            innerContentIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorError);
            errorStyle.Add(innerContentIconStyle);
         }
         
         var editKernelDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart));

         editKernelDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorTransparent);
         editKernelDecoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorErrorBg);

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorErrorBgHover);
         editKernelDecoratorStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         focusStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorError);
         focusStyle.Add(Border.BackgroundProperty, LineEditResourceKey.ActiveBg);

         editKernelDecoratorStyle.Add(focusStyle);

         var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart)
                                                               .Class(StdPseudoClass.FocusWithIn)
                                                               .Descendant().OfType<ScrollViewer>());
         scrollViewerStyle.Add(ScrollViewer.ForegroundProperty, GlobalResourceKey.ColorErrorText);

         errorStyle.Add(scrollViewerStyle);

         errorStyle.Add(editKernelDecoratorStyle);
         filledStyle.Add(errorStyle);
      }

      {
         var warningStyle = new Style(selector => selector.Nesting().Class(LineEdit.WarningPC));
         
         {
            var innerContentIconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftInnerContentPart).Descendant().OfType<PathIcon>(),
                                                                           selector.Nesting().Template().Name(RightInnerContentPart).Descendant().OfType<PathIcon>()));
            innerContentIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorWarning);
            warningStyle.Add(innerContentIconStyle);
         }

         var editKernelDecoratorStyle =
            new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart));

         editKernelDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorTransparent);
         editKernelDecoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorWarningBg);
         warningStyle.Add(editKernelDecoratorStyle);

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         hoverStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorWarningBgHover);
         editKernelDecoratorStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         focusStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorWarning);
         focusStyle.Add(Border.BackgroundProperty, LineEditResourceKey.ActiveBg);
         
         {
            var innerContentIconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftInnerContentPart).Descendant().OfType<PathIcon>(),
                                                                           selector.Nesting().Template().Name(RightInnerContentPart).Descendant().OfType<PathIcon>()));
            innerContentIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorWarning);
            focusStyle.Add(innerContentIconStyle);
         }
         
         editKernelDecoratorStyle.Add(focusStyle);

         var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart)
                                                               .Descendant().OfType<ScrollViewer>());
         scrollViewerStyle.Add(ScrollViewer.ForegroundProperty, GlobalResourceKey.ColorWarningText);

         warningStyle.Add(scrollViewerStyle);

         warningStyle.Add(editKernelDecoratorStyle);
         filledStyle.Add(warningStyle);
      }

      Add(filledStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disabledStyle.Add(LineEdit.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelDecoratorPart));
      decoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
      disabledStyle.Add(decoratorStyle);
      // TODO 暂时这么简单处理吧
      var addOnStyle = new Style(selector => selector.Nesting().Template().OfType<ContentPresenter>());
      addOnStyle.Add(ContentPresenter.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      disabledStyle.Add(addOnStyle);
      Add(disabledStyle);
   }
}