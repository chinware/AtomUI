using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

public class AbstractLineProgressTheme : AbstractProgressBarTheme
{
   public AbstractLineProgressTheme(Type targetType) : base(targetType) {}
   
   protected override void NotifyBuildControlTemplate(AbstractProgressBar bar, INameScope scope, Canvas container)
   {
      base.NotifyBuildControlTemplate(bar, scope, container);
      CreateCompletedIcons(scope, container);
   }

   private void CreateCompletedIcons(INameScope scope, Canvas container)
   {
      var exceptionCompletedIcon = new PathIcon
      {
         Name = ExceptionCompletedIconPart,
         Kind = "CloseCircleFilled",
         HorizontalAlignment = HorizontalAlignment.Left
      };
      exceptionCompletedIcon.RegisterInNameScope(scope);
      BindUtils.CreateGlobalTokenBinding(exceptionCompletedIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorError);
      BindUtils.CreateGlobalTokenBinding(exceptionCompletedIcon, PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ControlItemBgActiveDisabled);
      
      var successCompletedIcon = new PathIcon
      {
         Name = SuccessCompletedIconPart,
         Kind = "CheckCircleFilled",
         HorizontalAlignment = HorizontalAlignment.Left
      };
      successCompletedIcon.RegisterInNameScope(scope);
      BindUtils.CreateGlobalTokenBinding(successCompletedIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorSuccess);
      BindUtils.CreateGlobalTokenBinding(successCompletedIcon, PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ControlItemBgActiveDisabled);
      
      container.Children.Add(exceptionCompletedIcon);
      container.Children.Add(successCompletedIcon);
   }
   
   protected override void BuildStyles()
   {
      base.BuildStyles();
      
      BuildSizeTypeStyle();
   }
   
   private void BuildSizeTypeStyle()
   {
      var largeSizeTypeStyle = new Style(selector => selector.Nesting().PropertyEquals(AbstractProgressBar.EffectiveSizeTypeProperty, SizeType.Large));
      // icon
      {
         var completedIconsStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         completedIconsStyle.Add(PathIcon.WidthProperty, ProgressBarResourceKey.LineInfoIconSize);
         completedIconsStyle.Add(PathIcon.HeightProperty, ProgressBarResourceKey.LineInfoIconSize);
         largeSizeTypeStyle.Add(completedIconsStyle);
      }
      largeSizeTypeStyle.Add(Label.FontSizeProperty, GlobalResourceKey.FontSize);
      
      Add(largeSizeTypeStyle);
      
      var middleTypeStyle = new Style(selector => selector.Nesting().PropertyEquals(AbstractProgressBar.EffectiveSizeTypeProperty, SizeType.Middle));
      // icon
      {
         var completedIconsStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         completedIconsStyle.Add(PathIcon.WidthProperty, ProgressBarResourceKey.LineInfoIconSizeSM);
         completedIconsStyle.Add(PathIcon.HeightProperty, ProgressBarResourceKey.LineInfoIconSizeSM);
         middleTypeStyle.Add(completedIconsStyle);
      }
      
      middleTypeStyle.Add(Label.FontSizeProperty, GlobalResourceKey.FontSizeSM);
      Add(middleTypeStyle);

      var smallTypeStyle = new Style(selector => selector.Nesting().PropertyEquals(AbstractProgressBar.EffectiveSizeTypeProperty, SizeType.Small));
      // icon
      {
         var completedIconsStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
         completedIconsStyle.Add(PathIcon.WidthProperty, ProgressBarResourceKey.LineInfoIconSizeSM);
         completedIconsStyle.Add(PathIcon.HeightProperty, ProgressBarResourceKey.LineInfoIconSizeSM);
         smallTypeStyle.Add(completedIconsStyle);
      }
      smallTypeStyle.Add(Label.FontSizeProperty, GlobalResourceKey.FontSizeSM);
      Add(smallTypeStyle);
   }
}