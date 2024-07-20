using AtomUI.Icon;
using AtomUI.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;


public class AbstractProgressBarTheme : ControlTheme
{
   public const string PercentageLabelPart = "PART_PercentageLabel";
   public const string SuccessCompletedIconPart = "PART_SuccessCompletedIcon";
   public const string ExceptionCompletedIconPart = "PART_ExceptionCompletedIcon";
   public const string LayoutTransformControlPart = "PART_LayoutTransformControl";
   
   public AbstractProgressBarTheme(Type targetType) : base(targetType) {}
   
   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<AbstractProgressBar>((bar, scope) =>
      {
         var mainContainer = new Canvas();
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
      BuildCommonStyle();
      BuildInfoLabelStyle();
      BuildStatusStyle();
      BuildDisabledStyle();
   }

   private void BuildCommonStyle()
   {
      var enabledStyle = new Style(selector => selector.Nesting().PropertyEquals(AbstractProgressBar.IsEnabledProperty, true));
      enabledStyle.Add(AbstractProgressBar.ForegroundProperty, GlobalResourceKey.ColorTextLabel);
      enabledStyle.Add(AbstractProgressBar.GrooveBrushProperty, ProgressBarResourceKey.RemainingColor);
      Add(enabledStyle);
   }

   private void BuildStatusStyle()
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
      var normalOrActiveStatusStyle = new Style(selector => Selectors.Or(selector.Nesting().PropertyEquals(AbstractProgressBar.StatusProperty, ProgressStatus.Normal),
                                                                         selector.Nesting().PropertyEquals(AbstractProgressBar.StatusProperty, ProgressStatus.Active)));
      {
         {
            var exceptionIconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>().Name(ExceptionCompletedIconPart));
            exceptionIconStyle.Add(PathIcon.IsVisibleProperty, false);
            normalOrActiveStatusStyle.Add(exceptionIconStyle);
         }
         {
            var successIconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>().Name(SuccessCompletedIconPart));
            successIconStyle.Add(PathIcon.IsVisibleProperty, false);
            normalOrActiveStatusStyle.Add(successIconStyle);
         }
         {
            var completedStyle = new Style(selector => selector.Nesting().Class(AbstractProgressBar.CompletedPC));
            completedStyle.Add(AbstractProgressBar.IndicatorBarBrushProperty, GlobalResourceKey.ColorSuccess);
            var successIconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>().Name(SuccessCompletedIconPart));
            successIconStyle.Add(PathIcon.IsVisibleProperty, true);
            completedStyle.Add(successIconStyle);
            normalOrActiveStatusStyle.Add(completedStyle);
         }
         
         normalOrActiveStatusStyle.Add(AbstractProgressBar.IndicatorBarBrushProperty, ProgressBarResourceKey.DefaultColor);
      }
      Add(normalOrActiveStatusStyle);
   }

   private void BuildInfoLabelStyle()
   {
      var completedStyle = new Style(selector => selector.Nesting().Class(AbstractProgressBar.CompletedPC));
      var infoLabelStyle = new Style(selector => selector.Nesting().Template().OfType<LayoutTransformControl>());
      infoLabelStyle.Add(LayoutTransformControl.IsVisibleProperty, false);
      completedStyle.Add(infoLabelStyle);
      Add(completedStyle);
   }

   private void BuildDisabledStyle()
   {
      var disableStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      disableStyle.Add(AbstractProgressBar.GrooveBrushProperty, GlobalResourceKey.ColorBgContainerDisabled);
      disableStyle.Add(AbstractProgressBar.IndicatorBarBrushProperty, GlobalResourceKey.ControlItemBgActiveDisabled);
      disableStyle.Add(AbstractProgressBar.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      var statusIconStyle = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
      statusIconStyle.Add(PathIcon.IconModeProperty, IconMode.Disabled);
      disableStyle.Add(statusIconStyle);
      Add(disableStyle);
   }
}