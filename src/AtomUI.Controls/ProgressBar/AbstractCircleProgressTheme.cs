using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

public class AbstractCircleProgressTheme : AbstractProgressBarTheme
{
   public AbstractCircleProgressTheme(Type targetType) : base(targetType) {}
   
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
         Kind = "CloseOutlined",
         HorizontalAlignment = HorizontalAlignment.Center,
         VerticalAlignment = VerticalAlignment.Center
      };
      exceptionCompletedIcon.RegisterInNameScope(scope);
      BindUtils.CreateGlobalTokenBinding(exceptionCompletedIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorError);
      BindUtils.CreateGlobalTokenBinding(exceptionCompletedIcon, PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ControlItemBgActiveDisabled);
      
      var successCompletedIcon = new PathIcon
      {
         Name = SuccessCompletedIconPart,
         Kind = "CheckOutlined",
         HorizontalAlignment = HorizontalAlignment.Center,
         VerticalAlignment = VerticalAlignment.Center,
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

      var commonStyle = new Style(selector => selector.Nesting());
      {
         var labelStyle = new Style(selector => selector.Nesting().Template().OfType<Label>());
         labelStyle.Add(Label.HorizontalAlignmentProperty, HorizontalAlignment.Center);
         labelStyle.Add(Label.VerticalAlignmentProperty, VerticalAlignment.Center);
         commonStyle.Add(labelStyle);
      }
      var labelVisibleStyle =
         new Style(selector => selector.Nesting()
                                       .PropertyEquals(AbstractProgressBar.PercentLabelVisibleProperty, true));
      {
         var labelStyle = new Style(selector => selector.Nesting().Template().OfType<Label>());
         labelStyle.Add(Label.IsVisibleProperty, true);
         labelVisibleStyle.Add(labelStyle);
      }
      commonStyle.Add(labelVisibleStyle);
      
      var labelInVisibleStyle =
         new Style(selector => selector.Nesting()
                                       .PropertyEquals(AbstractProgressBar.PercentLabelVisibleProperty, false));
      {
         var labelStyle = new Style(selector => selector.Nesting().Template().OfType<Label>());
         labelStyle.Add(Label.IsVisibleProperty, false);
         labelInVisibleStyle.Add(labelStyle);
      }
      commonStyle.Add(labelInVisibleStyle);
      
      Add(commonStyle);
   }
}