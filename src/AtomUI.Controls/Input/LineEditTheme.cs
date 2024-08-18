using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
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
   public const string DecoratedBoxPart = "PART_DecoratedBox";
   public const string LineEditInnerBoxPart = "PART_LineEditInnerBox";
   public const string TextPresenterPart = "PART_TextPresenter";
   public const string WatermarkPart = "PART_Watermark";
   public const string ScrollViewerPart = "PART_ScrollViewer";

   public LineEditTheme(Type targetType) : base(targetType) { }
   public LineEditTheme() : base(typeof(LineEdit)) { }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<LineEdit>((lineEdit, scope) =>
      {
         var decoratedBox = BuildAddOnDecoratedBox(lineEdit, scope);
         var lineEditKernel = BuildLineEditKernel(lineEdit, scope);
         decoratedBox.Content = lineEditKernel;
         lineEditKernel.RegisterInNameScope(scope);
         return decoratedBox;
      });
   }

   protected virtual AddOnDecoratedBox BuildAddOnDecoratedBox(LineEdit lineEdit, INameScope scope)
   {
      var decoratedBox = new AddOnDecoratedBox()
      {
         Name = DecoratedBoxPart,
         Focusable = true
      };
      CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StyleVariantProperty, LineEdit.StyleVariantProperty);
      CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.SizeTypeProperty, LineEdit.SizeTypeProperty);
      CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StatusProperty, LineEdit.StatusProperty);
      CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.LeftAddOnProperty, LineEdit.LeftAddOnProperty);
      CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.RightAddOnProperty, LineEdit.RightAddOnProperty);
      decoratedBox.RegisterInNameScope(scope);
      return decoratedBox;
   }

   protected virtual LineEditInnerBox BuildLineEditKernel(LineEdit lineEdit, INameScope scope)
   {
      var editInnerBox = new LineEditInnerBox(lineEdit)
      {
         Name = LineEditInnerBoxPart,
         Cursor = new Cursor(StandardCursorType.Ibeam)
      };

      editInnerBox.RegisterInNameScope(scope);
      
      CreateTemplateParentBinding(editInnerBox, LineEditInnerBox.LeftAddOnContentProperty, LineEdit.InnerLeftContentProperty);
      CreateTemplateParentBinding(editInnerBox, LineEditInnerBox.RightAddOnContentProperty, LineEdit.InnerRightContentProperty);
      CreateTemplateParentBinding(editInnerBox, LineEditInnerBox.SizeTypeProperty, LineEdit.SizeTypeProperty);
      CreateTemplateParentBinding(editInnerBox, LineEditInnerBox.StyleVariantProperty, LineEdit.StyleVariantProperty);
      CreateTemplateParentBinding(editInnerBox, LineEditInnerBox.IsClearButtonVisibleProperty, LineEdit.IsEffectiveShowClearButtonProperty);
      CreateTemplateParentBinding(editInnerBox, LineEditInnerBox.IsRevealButtonVisibleProperty, LineEdit.IsEnableRevealButtonProperty);
      CreateTemplateParentBinding(editInnerBox, LineEditInnerBox.IsRevealButtonCheckedProperty, LineEdit.RevealPasswordProperty, BindingMode.TwoWay);
      CreateTemplateParentBinding(editInnerBox, LineEditInnerBox.StatusProperty, LineEdit.StatusProperty);
      
      var scrollViewer = new ScrollViewer
      {
         Name = ScrollViewerPart,
         Focusable = true
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
         Name = TextPresenterPart
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
      
      editInnerBox.Content = scrollViewer;
      
      return editInnerBox;
   }

   protected override void BuildStyles()
   {
      BuildCommonStyle();
      BuildFixedStyle();
      BuildStatusStyle();
   }

   private void BuildCommonStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      var largeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(LineEdit.SizeTypeProperty, SizeType.Large));
      largeStyle.Add(LineEdit.LineHeightProperty, GlobalResourceKey.FontHeightLG);
      commonStyle.Add(largeStyle);

      var middleStyle =
         new Style(selector => selector.Nesting().PropertyEquals(LineEdit.SizeTypeProperty, SizeType.Middle));
      middleStyle.Add(LineEdit.LineHeightProperty, GlobalResourceKey.FontHeight);
      commonStyle.Add(middleStyle);

      var smallStyle =
         new Style(selector => selector.Nesting().PropertyEquals(LineEdit.SizeTypeProperty, SizeType.Small));
      smallStyle.Add(LineEdit.LineHeightProperty, GlobalResourceKey.FontHeightSM);
      commonStyle.Add(smallStyle);
      
      Add(commonStyle);
   }
   
   private void BuildFixedStyle()
   {
      this.Add(LineEdit.SelectionBrushProperty, GlobalResourceKey.SelectionBackground);
      this.Add(LineEdit.SelectionForegroundBrushProperty, GlobalResourceKey.SelectionForeground);
      this.Add(LineEdit.VerticalAlignmentProperty, VerticalAlignment.Center);
      this.Add(LineEdit.VerticalContentAlignmentProperty, VerticalAlignment.Center);
      this.Add(ScrollViewer.IsScrollChainingEnabledProperty, true);
   }

   private void BuildStatusStyle()
   {
      var borderlessStyle =
         new Style(selector => selector.Nesting()
                                       .PropertyEquals(AddOnDecoratedBox.StyleVariantProperty, AddOnDecoratedVariant.Borderless));
      
      {
         var errorStyle = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.ErrorPC));
         var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(ScrollViewerPart));
         scrollViewerStyle.Add(ScrollViewer.ForegroundProperty, GlobalResourceKey.ColorErrorText);
         errorStyle.Add(scrollViewerStyle);
         borderlessStyle.Add(errorStyle);
      }

      {
         var warningStyle = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.WarningPC));
         var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(ScrollViewerPart));
         scrollViewerStyle.Add(ScrollViewer.ForegroundProperty, GlobalResourceKey.ColorWarningText);
         warningStyle.Add(scrollViewerStyle);
         borderlessStyle.Add(warningStyle);
      }

      Add(borderlessStyle);
      
       var filledStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.StyleVariantProperty, AddOnDecoratedVariant.Filled));


       {
          var errorStyle = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.ErrorPC));
          
         var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(ScrollViewerPart));
         scrollViewerStyle.Add(ScrollViewer.ForegroundProperty, GlobalResourceKey.ColorErrorText);
         errorStyle.Add(scrollViewerStyle);
         filledStyle.Add(errorStyle);
       }

      {
         var warningStyle = new Style(selector => selector.Nesting().Class(AddOnDecoratedBox.WarningPC));
         var scrollViewerStyle = new Style(selector => selector.Nesting().Template().Name(ScrollViewerPart));
         scrollViewerStyle.Add(ScrollViewer.ForegroundProperty, GlobalResourceKey.ColorWarningText);
         warningStyle.Add(scrollViewerStyle);
         filledStyle.Add(warningStyle);
      }

      Add(filledStyle);
   }

}