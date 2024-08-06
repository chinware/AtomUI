using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
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
   public const string FrameDecoratorPart       = "PART_FrameDecorator";
   public const string TextPresenterPart        = "PART_TextPresenter";
   public const string WatermarkPart            = "PART_Watermark";
   public const string ScrollViewerPart         = "PART_ScrollViewer";
   public const string LeftAddOnPart            = "PART_LeftAddOn";
   public const string RightAddOnPart           = "PART_RightAddOn";
   public const string LeftInnerContentPart     = "PART_LeftInnerContent";
   public const string RightInnerContentPart    = "PART_RightInnerContent";
   public const string ClearButtonPart          = "PART_ClearButton";
   public const string RevealButtonPart         = "PART_RevealButton";
   public const string LineEditKernelPart       = "PART_LineEditKernel";
   
   public LineEditTheme() : base(typeof(LineEdit)) { }
   
   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<LineEdit>((lineEdit, scope) =>
      {
         var border = new Border()
         {
            Name = FrameDecoratorPart
         };
         
         var grid = new Grid()
         {
            ShowGridLines = false
         };
         ConfigureGrid(grid);
         BuildGridChildren(lineEdit, grid, scope);
         border.Child = grid;
         return border;
      });
   }

   protected virtual void ConfigureGrid(Grid grid)
   {
      grid.ColumnDefinitions = new ColumnDefinitions()
      {
         new ColumnDefinition(GridLength.Auto),
         new ColumnDefinition(GridLength.Star),
         new ColumnDefinition(GridLength.Auto)
      };
   }

   protected virtual void BuildGridChildren(LineEdit lineEdit, Grid grid, INameScope scope)
   {
      BuildLeftAddOn(grid, scope);
      BuildLineEditKernel(lineEdit, grid, scope);
      BuildRightAddOn(grid, scope);
   }

   protected virtual void BuildLeftAddOn(Grid grid, INameScope scope)
   {
      var leftAddOnContentPresenter = new ContentPresenter()
      {
         Name = LeftAddOnPart,
         VerticalAlignment = VerticalAlignment.Stretch,
         VerticalContentAlignment = VerticalAlignment.Center
      };
      
      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.ContentProperty, LineEdit.LeftAddOnProperty);
      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.BorderThicknessProperty, LineEdit.BorderThicknessProperty);
      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.CornerRadiusProperty, LineEdit.LeftAddOnCornerRadiusProperty);
      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.IsVisibleProperty, LineEdit.LeftAddOnProperty,
         BindingMode.Default, ObjectConverters.IsNotNull);   
      leftAddOnContentPresenter.RegisterInNameScope(scope);
      
      TokenResourceBinder.CreateTokenBinding(leftAddOnContentPresenter, ContentPresenter.BackgroundProperty, LineEditResourceKey.AddonBg);
      TokenResourceBinder.CreateTokenBinding(leftAddOnContentPresenter, ContentPresenter.BorderBrushProperty, GlobalResourceKey.ColorBorder);
      
      Grid.SetColumn(leftAddOnContentPresenter, 0);
      grid.Children.Add(leftAddOnContentPresenter);
   }

   protected virtual void BuildLineEditKernel(LineEdit lineEdit, Grid grid, INameScope scope)
   {
      var kernelLayout = new LineEditKernel()
      {
         Name = LineEditKernelPart
      };
      
      CreateTemplateParentBinding(kernelLayout, LineEditKernel.BorderThicknessProperty, LineEdit.BorderThicknessProperty);
      CreateTemplateParentBinding(kernelLayout, LineEditKernel.CornerRadiusProperty, LineEdit.EditKernelCornerRadiusProperty);
      
      BuildInnerLeftContent(kernelLayout, scope);
      BuildTextPresenter(lineEdit, kernelLayout, scope);
      BuildClearButton(kernelLayout, scope);
      BuildRevealButton(kernelLayout, scope);
      BuildInnerRightContent(kernelLayout, scope);
      grid.Children.Add(kernelLayout);
      Grid.SetColumn(kernelLayout, 1);
      kernelLayout.RegisterInNameScope(scope);
   }

   protected virtual void BuildRightAddOn(Grid grid, INameScope scope)
   {
      var rightAddOnContentPresenter = new ContentPresenter()
      {
         Name = RightAddOnPart,
         VerticalAlignment = VerticalAlignment.Stretch,
         VerticalContentAlignment = VerticalAlignment.Center
      };
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.ContentProperty, LineEdit.RightAddOnProperty);
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.BorderThicknessProperty, LineEdit.BorderThicknessProperty);
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.CornerRadiusProperty, LineEdit.RightAddOnCornerRadiusProperty);
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.IsVisibleProperty, LineEdit.RightAddOnProperty,
                                  BindingMode.Default, ObjectConverters.IsNotNull);
      
      TokenResourceBinder.CreateTokenBinding(rightAddOnContentPresenter, ContentPresenter.BackgroundProperty, LineEditResourceKey.AddonBg);
      TokenResourceBinder.CreateTokenBinding(rightAddOnContentPresenter, ContentPresenter.BorderBrushProperty, GlobalResourceKey.ColorBorder);
      
      rightAddOnContentPresenter.RegisterInNameScope(scope);
      grid.Children.Add(rightAddOnContentPresenter);
      Grid.SetColumn(rightAddOnContentPresenter, 2);
   }
   
   protected virtual void BuildInnerLeftContent(LineEditKernel layout, INameScope scope)
   {
      var innerLeftContentPresenter = new ContentPresenter()
      {
         Name = LeftInnerContentPart
      };
      CreateTemplateParentBinding(innerLeftContentPresenter, ContentPresenter.IsVisibleProperty, LineEdit.InnerLeftContentProperty,
                                  BindingMode.Default, ObjectConverters.IsNotNull);      
      innerLeftContentPresenter.RegisterInNameScope(scope);
      layout.RightInnerContent = innerLeftContentPresenter;
   }

   protected virtual void BuildInnerRightContent(LineEditKernel layout, INameScope scope)
   {
      var innerRightContentPresenter = new ContentPresenter()
      {
         Name = RightInnerContentPart
      };
      CreateTemplateParentBinding(innerRightContentPresenter, ContentPresenter.IsVisibleProperty, LineEdit.InnerRightContentProperty,
                                  BindingMode.Default, ObjectConverters.IsNotNull);      
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
      BindUtils.RelayBind(lineEdit, ScrollViewer.AllowAutoHideProperty, scrollViewer, ScrollViewer.AllowAutoHideProperty);
      BindUtils.RelayBind(lineEdit, ScrollViewer.HorizontalScrollBarVisibilityProperty, scrollViewer, ScrollViewer.HorizontalScrollBarVisibilityProperty);
      BindUtils.RelayBind(lineEdit, ScrollViewer.VerticalScrollBarVisibilityProperty, scrollViewer, ScrollViewer.VerticalScrollBarVisibilityProperty);
      BindUtils.RelayBind(lineEdit, ScrollViewer.VerticalScrollBarVisibilityProperty, scrollViewer, ScrollViewer.IsScrollChainingEnabledProperty);
      
      scrollViewer.RegisterInNameScope(scope);
      
      var textPresenterLayout = new Panel();

      var watermark = new TextBlock
      {
         Name = WatermarkPart,
         Opacity = 0.5
      };
      CreateTemplateParentBinding(watermark, TextBlock.HorizontalAlignmentProperty, LineEdit.HorizontalContentAlignmentProperty);
      CreateTemplateParentBinding(watermark, TextBlock.VerticalAlignmentProperty, LineEdit.VerticalContentAlignmentProperty);
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

      CreateTemplateParentBinding(textPresenter, TextPresenter.HorizontalAlignmentProperty, LineEdit.HorizontalContentAlignmentProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.VerticalAlignmentProperty, LineEdit.VerticalContentAlignmentProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.CaretBlinkIntervalProperty, LineEdit.CaretBlinkIntervalProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.CaretBrushProperty, LineEdit.CaretBrushProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.CaretIndexProperty, LineEdit.CaretIndexProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.LineHeightProperty, LineEdit.LineHeightProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.PasswordCharProperty, LineEdit.PasswordCharProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.RevealPasswordProperty, LineEdit.RevealPasswordProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.SelectionBrushProperty, LineEdit.SelectionBrushProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.SelectionStartProperty, LineEdit.SelectionStartProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.SelectionEndProperty, LineEdit.SelectionEndProperty);
      CreateTemplateParentBinding(textPresenter, TextPresenter.SelectionForegroundBrushProperty, LineEdit.SelectionForegroundBrushProperty);
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
      var clearButton = new IconButton()
      {
         Name = ClearButtonPart
      };
      clearButton.RegisterInNameScope(scope);
      layout.ClearButton = clearButton;
   }
   
   protected virtual void BuildRevealButton(LineEditKernel layout, INameScope scope)
   {
      var revealButton = new ToggleIconButton()
      {
         Name = RevealButtonPart
      };
      revealButton.RegisterInNameScope(scope);
      layout.RevealButton = revealButton;
   }

   protected override void BuildStyles()
   {
      BuildFixedStyle();
      BuildCommonStyle();
      BuildFilledStyle();
      BuildOutLineStyle();
      BuildDisabledStyle();
   }

   private void BuildFixedStyle()
   {
      this.Add(LineEdit.CursorProperty, new Cursor(StandardCursorType.Ibeam));
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
      
      var largeStyle = new Style(selector => selector.Nesting().PropertyEquals(LineEdit.SizeTypeProperty, SizeType.Large));
      {
         var editKernelStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelPart));
         editKernelStyle.Add(LineEditKernel.PaddingProperty, LineEditResourceKey.PaddingLG);
         largeStyle.Add(editKernelStyle);
      }

      {
         var addOnStyle = new Style(selector => selector.Nesting().Template().OfType<ContentPresenter>());
         addOnStyle.Add(ContentPresenter.PaddingProperty, LineEditResourceKey.PaddingLG);
         largeStyle.Add(addOnStyle);
      }
      
      largeStyle.Add(LineEdit.FontSizeProperty, LineEditResourceKey.InputFontSizeLG);
      largeStyle.Add(LineEdit.LineHeightProperty, GlobalResourceKey.FontHeightLG);
      largeStyle.Add(LineEdit.CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
      commonStyle.Add(largeStyle);
      
      var middleStyle = new Style(selector => selector.Nesting().PropertyEquals(LineEdit.SizeTypeProperty, SizeType.Middle));
      {
         var editKernelStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelPart));
         editKernelStyle.Add(LineEditKernel.PaddingProperty, LineEditResourceKey.Padding);
         middleStyle.Add(editKernelStyle);
      }
      
      {
         var addOnStyle = new Style(selector => selector.Nesting().Template().OfType<ContentPresenter>());
         addOnStyle.Add(ContentPresenter.PaddingProperty, LineEditResourceKey.Padding);
         middleStyle.Add(addOnStyle);
      }
      
      middleStyle.Add(LineEdit.FontSizeProperty, LineEditResourceKey.InputFontSize);
      middleStyle.Add(LineEdit.LineHeightProperty, GlobalResourceKey.FontHeight);
      middleStyle.Add(LineEdit.CornerRadiusProperty, GlobalResourceKey.BorderRadius);
      commonStyle.Add(middleStyle);
      
      var smallStyle = new Style(selector => selector.Nesting().PropertyEquals(LineEdit.SizeTypeProperty, SizeType.Small));
      {
         var editKernelStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelPart));
         editKernelStyle.Add(LineEditKernel.PaddingProperty, LineEditResourceKey.PaddingSM);
         smallStyle.Add(editKernelStyle);
      }
      
      {
         var addOnStyle = new Style(selector => selector.Nesting().Template().OfType<ContentPresenter>());
         addOnStyle.Add(ContentPresenter.PaddingProperty, LineEditResourceKey.PaddingSM);
         smallStyle.Add(addOnStyle);
      }
      smallStyle.Add(LineEdit.FontSizeProperty, LineEditResourceKey.InputFontSizeSM);
      smallStyle.Add(LineEdit.LineHeightProperty, GlobalResourceKey.FontHeightSM);
      smallStyle.Add(LineEdit.CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      commonStyle.Add(smallStyle);
      
      Add(commonStyle);
   }

   private void BuildOutLineStyle()
   {
       var outlineStyle = new Style(selector => selector.Nesting().PropertyEquals(LineEdit.StyleVariantProperty, TextBoxVariant.Outline));
       {
          var editKernelStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelPart)); 
          editKernelStyle.Add(LineEditKernel.BorderBrushProperty, GlobalResourceKey.ColorBorder);
          outlineStyle.Add(editKernelStyle);
       }

       var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
       {
          var editKernelStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelPart)); 
          editKernelStyle.Add(LineEditKernel.BorderBrushProperty, LineEditResourceKey.HoverBorderColor);
          hoverStyle.Add(editKernelStyle);
       }
       outlineStyle.Add(hoverStyle);
       
       var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
       {
          var editKernelStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelPart)); 
          editKernelStyle.Add(LineEditKernel.BorderBrushProperty, LineEditResourceKey.ActiveBorderColor);
          focusStyle.Add(editKernelStyle);
       }
       outlineStyle.Add(focusStyle);
       
       Add(outlineStyle);
   }

   private void BuildFilledStyle()
   {
      var filledStyle = new Style(selector => selector.Nesting().PropertyEquals(LineEdit.StyleVariantProperty, TextBoxVariant.Filled));

      {
         var editKernelStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelPart)); 
      
         editKernelStyle.Add(LineEditKernel.BorderBrushProperty, GlobalResourceKey.ColorTransparent);
         editKernelStyle.Add(LineEditKernel.BackgroundProperty, GlobalResourceKey.ColorFillTertiary);

         filledStyle.Add(editKernelStyle);
      }

      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      {
         var editKernelStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelPart)); 
         editKernelStyle.Add(LineEditKernel.BackgroundProperty, GlobalResourceKey.ColorFillSecondary);

         hoverStyle.Add(editKernelStyle);
      }
      
      filledStyle.Add(hoverStyle);
       
      var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
      
      {
         var editKernelStyle = new Style(selector => selector.Nesting().Template().Name(LineEditKernelPart)); 
      
         editKernelStyle.Add(LineEditKernel.BorderBrushProperty, LineEditResourceKey.ActiveBorderColor);
         editKernelStyle.Add(LineEditKernel.BackgroundProperty, LineEditResourceKey.ActiveBg);

         focusStyle.Add(editKernelStyle);
      }
      
      filledStyle.Add(focusStyle);
       
      Add(filledStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      // TODO 暂时这么简单处理吧
      var addOnStyle = new Style(selector => selector.Nesting().Template().OfType<ContentPresenter>());
      addOnStyle.Add(ContentPresenter.BackgroundProperty, GlobalResourceKey.ColorFillTertiary);
      addOnStyle.Add(ContentPresenter.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      disabledStyle.Add(addOnStyle);
      Add(disabledStyle);
   }
}