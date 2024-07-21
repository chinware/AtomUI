using AtomUI.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class SegmentedTheme : ControlTheme
{
   public const string MainContainerPart = "PART_MainContainer";
   public SegmentedTheme()
      : base(typeof(Segmented))
   {
   }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<Segmented>((segmented, scope) =>
      {
         var mainContainer = new Canvas()
         {
            Name = MainContainerPart,
            Background = new SolidColorBrush(Colors.Transparent)
         };
         mainContainer.RegisterInNameScope(scope);
         return mainContainer;
      });
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(Segmented.PaddingProperty, SegmentedResourceKey.TrackPadding);
      commonStyle.Add(Segmented.BackgroundProperty, SegmentedResourceKey.TrackBg);
      commonStyle.Add(Segmented.HorizontalAlignmentProperty, HorizontalAlignment.Left);
      commonStyle.Add(Segmented.SelectedThumbBgProperty, SegmentedResourceKey.ItemSelectedBg);
      commonStyle.Add(Segmented.SelectedThumbBoxShadowsProperty, GlobalResourceKey.BoxShadowsTertiary);
      var expandingStyle = new Style(selector => selector.Nesting().PropertyEquals(Segmented.IsExpandingProperty, true));
      expandingStyle.Add(Segmented.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
      commonStyle.Add(expandingStyle);
      Add(commonStyle);

      BuildSizeTypeStyle();
   }

   private void BuildSizeTypeStyle()
   {
      var largeSizeTypeStyle = new Style(selector => selector.Nesting().PropertyEquals(Segmented.SizeTypeProperty, SizeType.Large));
      largeSizeTypeStyle.Add(Segmented.CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
      largeSizeTypeStyle.Add(Segmented.SelectedThumbCornerRadiusProperty, GlobalResourceKey.BorderRadius);
      Add(largeSizeTypeStyle);
      
      var middleSizeTypeStyle = new Style(selector => selector.Nesting().PropertyEquals(Segmented.SizeTypeProperty, SizeType.Middle));
      middleSizeTypeStyle.Add(Segmented.CornerRadiusProperty, GlobalResourceKey.BorderRadius);
      middleSizeTypeStyle.Add(Segmented.SelectedThumbCornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      Add(middleSizeTypeStyle);
      
      var smallSizeTypeStyle = new Style(selector => selector.Nesting().PropertyEquals(Segmented.SizeTypeProperty, SizeType.Small));
      smallSizeTypeStyle.Add(Segmented.CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      smallSizeTypeStyle.Add(Segmented.SelectedThumbCornerRadiusProperty, GlobalResourceKey.BorderRadiusXS);
      Add(smallSizeTypeStyle);
   }
}