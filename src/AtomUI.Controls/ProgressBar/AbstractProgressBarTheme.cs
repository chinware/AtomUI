using AtomUI.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;


public class AbstractProgressBarTheme : ControlTheme
{
   public const string MainContainerPart = "PART_MainContainer";
   public const string PercentageLabelPart = "PART_PercentageLabel";
   public const string SuccessCompletedIconPart = "PART_SuccessCompletedIcon";
   public const string ExceptionCompletedIconPart = "PART_ExceptionCompletedIcon";
   public const string LayoutTransformControlPart = "PART_LayoutTransformControl";
   
   public AbstractProgressBarTheme(Type targetType) : base(targetType) {}
   
   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<AbstractProgressBar>((bar, scope) =>
      {
         var mainContainer = new Canvas()
         {
            Name = MainContainerPart
         };
         mainContainer.RegisterInNameScope(scope);
         NotifyBuildControlTemplate(bar, scope, mainContainer);
         return mainContainer;
      });
   }

   protected virtual void NotifyBuildControlTemplate(AbstractProgressBar bar, INameScope scope, Canvas container)
   {
      var percentageLabel = new Label
      {
         Name = PercentageLabelPart,
         Padding = new Thickness(0),
         Margin = new Thickness(0),
         VerticalContentAlignment = VerticalAlignment.Center,
      };
      percentageLabel.RegisterInNameScope(scope);
      CreateTemplateParentBinding(percentageLabel, Label.IsEnabledProperty, AbstractProgressBar.IsEnabledProperty);
      var layoutTransform = new LayoutTransformControl()
      {
         Name = LayoutTransformControlPart,
         Child = percentageLabel,
      };
      layoutTransform.RegisterInNameScope(scope);
      container.Children.Add(layoutTransform);
   }

   protected override void BuildStyles()
   {
      base.BuildStyles();
      var enabledStyle = new Style(selector => selector.Nesting().PropertyEquals(AbstractProgressBar.IsEnabledProperty, true));
      BuildCommonStyle(enabledStyle);
      BuildStatusStyle(enabledStyle);
      BuildInfoLabelStyle(enabledStyle);
      Add(enabledStyle);
      BuildDisabledStyle();
   }

   private void BuildCommonStyle(Style enabledStyle)
   {
      enabledStyle.Add(AbstractProgressBar.ForegroundProperty, GlobalResourceKey.ColorTextLabel);
      enabledStyle.Add(AbstractProgressBar.GrooveBrushProperty, ProgressBarResourceKey.RemainingColor);
   }

   private void BuildStatusStyle(Style enabledStyle)
   {
      // 异常状态
      var exceptionStatusStyle = new Style(selector => selector.Nesting().PropertyEquals(AbstractProgressBar.StatusProperty, ProgressStatus.Exception));
      {
         var exceptionIconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>().Name(ExceptionCompletedIconPart));
         exceptionIconStyle.Add(PathIcon.IsVisibleProperty, true);
         exceptionStatusStyle.Add(exceptionIconStyle);
        
         var successIconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>().Name(SuccessCompletedIconPart));
         successIconStyle.Add(PathIcon.IsVisibleProperty, false);
         exceptionStatusStyle.Add(successIconStyle);
         
         var infoLabelStyle = new Style(selector => selector.Nesting().Template().OfType<LayoutTransformControl>());
         infoLabelStyle.Add(LayoutTransformControl.IsVisibleProperty, false);
         
         exceptionStatusStyle.Add(AbstractProgressBar.IndicatorBarBrushProperty, GlobalResourceKey.ColorError);
         
         exceptionStatusStyle.Add(infoLabelStyle);
      }
      Add(exceptionStatusStyle);
      
      // 成功状态
      var successStatusStyle = new Style(selector => selector.Nesting().PropertyEquals(AbstractProgressBar.StatusProperty, ProgressStatus.Success));
      {
         var exceptionIconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>().Name(ExceptionCompletedIconPart));
         exceptionIconStyle.Add(PathIcon.IsVisibleProperty, false);
         successStatusStyle.Add(exceptionIconStyle);
         
         var successIconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>().Name(SuccessCompletedIconPart));
         successIconStyle.Add(PathIcon.IsVisibleProperty, true);
         successStatusStyle.Add(successIconStyle);
         
         var infoLabelStyle = new Style(selector => selector.Nesting().Template().OfType<LayoutTransformControl>());
         infoLabelStyle.Add(LayoutTransformControl.IsVisibleProperty, false);

         successStatusStyle.Add(AbstractProgressBar.IndicatorBarBrushProperty, GlobalResourceKey.ColorSuccess);
         
         successStatusStyle.Add(infoLabelStyle);
      }
      Add(successStatusStyle);
      
      // 正常状态
      var normalStatusStyle = new Style(selector => selector.Nesting().PropertyEquals(AbstractProgressBar.StatusProperty, ProgressStatus.Normal));
      {
         {
            var exceptionIconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>().Name(ExceptionCompletedIconPart));
            exceptionIconStyle.Add(PathIcon.IsVisibleProperty, false);
            normalStatusStyle.Add(exceptionIconStyle);
         }
         {
            var successIconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>().Name(SuccessCompletedIconPart));
            successIconStyle.Add(PathIcon.IsVisibleProperty, false);
            normalStatusStyle.Add(successIconStyle);
         }
         {
            var completedStyle = new Style(selector => selector.Nesting().Class(AbstractProgressBar.CompletedPC));
            completedStyle.Add(AbstractProgressBar.IndicatorBarBrushProperty, GlobalResourceKey.ColorSuccess);
            var successIconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>().Name(SuccessCompletedIconPart));
            successIconStyle.Add(PathIcon.IsVisibleProperty, true);
            completedStyle.Add(successIconStyle);
            normalStatusStyle.Add(completedStyle);
         }
         
         normalStatusStyle.Add(AbstractProgressBar.IndicatorBarBrushProperty, ProgressBarResourceKey.DefaultColor);
      }
      enabledStyle.Add(normalStatusStyle);
   }

   private void BuildInfoLabelStyle(Style enabledStyle)
   {
      var completedStyle = new Style(selector => selector.Nesting().Class(AbstractProgressBar.CompletedPC));
      var infoLabelStyle = new Style(selector => selector.Nesting().Template().OfType<LayoutTransformControl>());
      infoLabelStyle.Add(LayoutTransformControl.IsVisibleProperty, false);
      completedStyle.Add(infoLabelStyle);
      enabledStyle.Add(completedStyle);
   }

   private void BuildDisabledStyle()
   {
      var disableStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disableStyle.Add(AbstractProgressBar.GrooveBrushProperty, GlobalResourceKey.ColorBgContainerDisabled);
      disableStyle.Add(AbstractProgressBar.IndicatorBarBrushProperty, GlobalResourceKey.ControlItemBgActiveDisabled);
      disableStyle.Add(AbstractProgressBar.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      Add(disableStyle);
   }
}