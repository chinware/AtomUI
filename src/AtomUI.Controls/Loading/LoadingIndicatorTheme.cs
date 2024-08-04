using AtomUI.Icon;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class LoadingIndicatorTheme : BaseControlTheme
{
   public const string MainContainerPart = "PART_MainContainer";
   public const string LoadingTextPart = "PART_LoadingText";
   
   public LoadingIndicatorTheme()
      : base(typeof(LoadingIndicator))
   {
   }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<LoadingIndicator>((indicator, scope) =>
      {
         var textBlock = new TextBlock()
         {
            Name = LoadingTextPart,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
         };
         CreateTemplateParentBinding(textBlock, TextBlock.TextProperty, LoadingIndicator.LoadingMsgProperty);
         textBlock.RegisterInNameScope(scope);
         var mainContainer = new Canvas()
         {
            Name = MainContainerPart
         };
         mainContainer.Children.Add(textBlock);
         mainContainer.RegisterInNameScope(scope);
         return mainContainer;
      });
   }
   
   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(LoadingIndicator.HorizontalAlignmentProperty, HorizontalAlignment.Left);
      commonStyle.Add(LoadingIndicator.VerticalAlignmentProperty, VerticalAlignment.Top);
      commonStyle.Add(LoadingIndicator.FontSizeProperty, GlobalResourceKey.FontSize);
      commonStyle.Add(LoadingIndicator.MotionEasingCurveProperty, new LinearEasing());
      commonStyle.Add(LoadingIndicator.MotionDurationProperty, LoadingIndicatorResourceKey.IndicatorDuration);
      commonStyle.Add(LoadingIndicator.DotBgBrushProperty, GlobalResourceKey.ColorPrimary);
      commonStyle.Add(LoadingIndicator.IndicatorTextMarginProperty, GlobalResourceKey.MarginXXS);
      var loadingTextStyle = new Style(selector => selector.Nesting().Template().OfType<TextBlock>());
      loadingTextStyle.Add(TextBlock.ForegroundProperty, GlobalResourceKey.ColorPrimary);
      commonStyle.Add(loadingTextStyle);
      BuildDotSizeStyle(commonStyle);
      BuildCustomIconStyle();
      Add(commonStyle);
   }

   private void BuildCustomIconStyle()
   {
      var customIconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
      customIconStyle.Add(PathIcon.IconModeProperty, IconMode.Normal);
      customIconStyle.Add(PathIcon.VerticalAlignmentProperty, VerticalAlignment.Center);
      customIconStyle.Add(PathIcon.HorizontalAlignmentProperty, HorizontalAlignment.Center);
      Add(customIconStyle);
      
      var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(LoadingIndicator.SizeTypeProperty, SizeType.Large));
      var largeIconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
      largeIconStyle.Add(PathIcon.WidthProperty, LoadingIndicator.LARGE_INDICATOR_SIZE);
      largeIconStyle.Add(PathIcon.HeightProperty, LoadingIndicator.LARGE_INDICATOR_SIZE);
      largeSizeStyle.Add(largeIconStyle);
      Add(largeSizeStyle);
      
      var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(LoadingIndicator.SizeTypeProperty, SizeType.Middle));
      var middleIconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
      middleIconStyle.Add(PathIcon.WidthProperty, LoadingIndicator.MIDDLE_INDICATOR_SIZE);
      middleIconStyle.Add(PathIcon.HeightProperty, LoadingIndicator.MIDDLE_INDICATOR_SIZE);
      middleSizeStyle.Add(middleIconStyle);
      Add(middleSizeStyle);
      
      var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(LoadingIndicator.SizeTypeProperty, SizeType.Small));
      var smallIconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
      smallIconStyle.Add(PathIcon.WidthProperty, LoadingIndicator.SMALL_INDICATOR_SIZE);
      smallIconStyle.Add(PathIcon.HeightProperty, LoadingIndicator.SMALL_INDICATOR_SIZE);
      smallSizeStyle.Add(smallIconStyle);
      Add(smallSizeStyle);
   }

   private void BuildDotSizeStyle(Style commonStyle)
   {
      var largeSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(LoadingIndicator.SizeTypeProperty, SizeType.Large));
      largeSizeStyle.Add(LoadingIndicator.DotSizeProperty, LoadingIndicatorResourceKey.DotSizeLG);
      commonStyle.Add(largeSizeStyle);
      
      var middleSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(LoadingIndicator.SizeTypeProperty, SizeType.Middle));
      middleSizeStyle.Add(LoadingIndicator.DotSizeProperty, LoadingIndicatorResourceKey.DotSize);
      commonStyle.Add(middleSizeStyle);
      
      var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(LoadingIndicator.SizeTypeProperty, SizeType.Small));
      smallSizeStyle.Add(LoadingIndicator.DotSizeProperty, LoadingIndicatorResourceKey.DotSizeSM);
      commonStyle.Add(smallSizeStyle);
   }
}