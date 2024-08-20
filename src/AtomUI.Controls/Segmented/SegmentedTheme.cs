using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class SegmentedTheme : BaseControlTheme
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
      commonStyle.Add(Segmented.PaddingProperty, SegmentedTokenResourceKey.TrackPadding);
      commonStyle.Add(Segmented.BackgroundProperty, SegmentedTokenResourceKey.TrackBg);
      commonStyle.Add(Segmented.HorizontalAlignmentProperty, HorizontalAlignment.Left);
      commonStyle.Add(Segmented.SelectedThumbBgProperty, SegmentedTokenResourceKey.ItemSelectedBg);
      commonStyle.Add(Segmented.SelectedThumbBoxShadowsProperty, GlobalTokenResourceKey.BoxShadowsTertiary);
      var expandingStyle = new Style(selector => selector.Nesting().PropertyEquals(Segmented.IsExpandingProperty, true));
      expandingStyle.Add(Segmented.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
      commonStyle.Add(expandingStyle);
      Add(commonStyle);

      BuildSizeTypeStyle();
   }

   private void BuildSizeTypeStyle()
   {
      var largeSizeTypeStyle = new Style(selector => selector.Nesting().PropertyEquals(Segmented.SizeTypeProperty, SizeType.Large));
      largeSizeTypeStyle.Add(Segmented.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusLG);
      largeSizeTypeStyle.Add(Segmented.SelectedThumbCornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
      Add(largeSizeTypeStyle);
      
      var middleSizeTypeStyle = new Style(selector => selector.Nesting().PropertyEquals(Segmented.SizeTypeProperty, SizeType.Middle));
      middleSizeTypeStyle.Add(Segmented.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
      middleSizeTypeStyle.Add(Segmented.SelectedThumbCornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
      Add(middleSizeTypeStyle);
      
      var smallSizeTypeStyle = new Style(selector => selector.Nesting().PropertyEquals(Segmented.SizeTypeProperty, SizeType.Small));
      smallSizeTypeStyle.Add(Segmented.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
      smallSizeTypeStyle.Add(Segmented.SelectedThumbCornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusXS);
      Add(smallSizeTypeStyle);
   }
}