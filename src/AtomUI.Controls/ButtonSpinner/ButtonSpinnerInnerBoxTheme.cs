using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ButtonSpinnerInnerBoxTheme : AddOnDecoratedInnerBoxTheme
{
   public const string SpinnerMainLayoutPart = "PART_SpinnerMainLayout";
   public const string SpinnerHandlePart = "PART_SpinnerHandle";
   
   public ButtonSpinnerInnerBoxTheme() : base(typeof(ButtonSpinnerInnerBox)) {}
   public ButtonSpinnerInnerBoxTheme(Type targetType) : base(targetType) {}
   
   protected override Panel BuildBoxMainLayout(AddOnDecoratedInnerBox decoratedBox, INameScope scope)
   {
      var baseMainLayout = base.BuildBoxMainLayout(decoratedBox, scope);
      var contentPresenter = new ContentPresenter()
      {
         Name = SpinnerHandlePart
      };
      contentPresenter.RegisterInNameScope(scope);
      CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, ButtonSpinnerInnerBox.SpinnerContentProperty);
      CreateTemplateParentBinding(contentPresenter, ContentPresenter.IsVisibleProperty, ButtonSpinnerInnerBox.ShowButtonSpinnerProperty);
      
      var spinnerMainLayout = new DockPanel
      {
         Name = SpinnerMainLayoutPart,
         LastChildFill = true,
      };
      spinnerMainLayout.Children.Add(contentPresenter);
      spinnerMainLayout.Children.Add(baseMainLayout);
      
      return spinnerMainLayout;
   }
   
   protected override void BuildStyles()
   {
      base.BuildStyles();
      
      var commonStyle = new Style(selector => selector.Nesting());
      
      var largeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(ButtonSpinnerInnerBox.SizeTypeProperty, SizeType.Large));
      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ButtonSpinnerInnerBoxTheme.ContentPresenterPart));
         contentPresenterStyle.Add(ContentPresenter.LineHeightProperty, GlobalResourceKey.FontHeightLG);
         largeStyle.Add(contentPresenterStyle);
      }
      commonStyle.Add(largeStyle);

      var middleStyle =
         new Style(selector => selector.Nesting().PropertyEquals(ButtonSpinnerInnerBox.SizeTypeProperty, SizeType.Middle));
      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ButtonSpinnerInnerBoxTheme.ContentPresenterPart));
         contentPresenterStyle.Add(ContentPresenter.LineHeightProperty, GlobalResourceKey.FontHeight);
         middleStyle.Add(contentPresenterStyle);
      }
      commonStyle.Add(middleStyle);

      var smallStyle =
         new Style(selector => selector.Nesting().PropertyEquals(ButtonSpinnerInnerBox.SizeTypeProperty, SizeType.Small));
      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ButtonSpinnerInnerBoxTheme.ContentPresenterPart));
         contentPresenterStyle.Add(ContentPresenter.LineHeightProperty, GlobalResourceKey.FontHeightSM);
         smallStyle.Add(contentPresenterStyle);
      }
      commonStyle.Add(smallStyle);
      
      // spinner 的位置
      var leftPositionStyle = new Style(selector => selector.Nesting().PropertyEquals(ButtonSpinnerInnerBox.ButtonSpinnerLocationProperty, Location.Left));
      {
         var handleStyle = new Style(selector => selector.Nesting().Template().Name(SpinnerHandlePart));
         handleStyle.Add(DockPanel.DockProperty, Dock.Left);
         leftPositionStyle.Add(handleStyle);
      }
      commonStyle.Add(leftPositionStyle);

      var rightPositionStyle = new Style(selector => selector.Nesting().PropertyEquals(ButtonSpinnerInnerBox.ButtonSpinnerLocationProperty, Location.Right));
      {
         var handleStyle = new Style(selector => selector.Nesting().Template().Name(SpinnerHandlePart));
         handleStyle.Add(DockPanel.DockProperty, Dock.Right);
         rightPositionStyle.Add(handleStyle);
      }
      commonStyle.Add(rightPositionStyle);
      Add(commonStyle);
   }
}