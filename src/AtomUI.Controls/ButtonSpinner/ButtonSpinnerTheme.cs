using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ButtonSpinnerTheme : BaseControlTheme
{
   public const string DecoratedBoxPart = "PART_DecoratedBox";
   public const string SpinnerInnerBoxPart = "PART_SpinnerInnerBox";
   public const string IncreaseButtonPart = "PART_IncreaseButton";
   public const string DecreaseButtonPart = "PART_DecreaseButton";
   
   public ButtonSpinnerTheme() : base(typeof(ButtonSpinner)) {}
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<ButtonSpinner>((buttonSpinner, scope) =>
      {
         var decoratedBox = BuildSpinnerDecoratedBox(buttonSpinner, scope);
         var innerBox = BuildSpinnerContent(buttonSpinner, scope);
         decoratedBox.Content = innerBox;
         innerBox.RegisterInNameScope(scope);
         return decoratedBox;
      });
   }
   
   protected virtual ButtonSpinnerDecoratedBox BuildSpinnerDecoratedBox(ButtonSpinner buttonSpinner, INameScope scope)
   {
      var decoratedBox = new ButtonSpinnerDecoratedBox
      {
         Name = DecoratedBoxPart,
         Focusable = true
      };
      CreateTemplateParentBinding(decoratedBox, ButtonSpinnerDecoratedBox.StyleVariantProperty, ButtonSpinner.StyleVariantProperty);
      CreateTemplateParentBinding(decoratedBox, ButtonSpinnerDecoratedBox.SizeTypeProperty, ButtonSpinner.SizeTypeProperty);
      CreateTemplateParentBinding(decoratedBox, ButtonSpinnerDecoratedBox.StatusProperty, ButtonSpinner.StatusProperty);
      CreateTemplateParentBinding(decoratedBox, ButtonSpinnerDecoratedBox.LeftAddOnProperty, ButtonSpinner.LeftAddOnProperty);
      CreateTemplateParentBinding(decoratedBox, ButtonSpinnerDecoratedBox.RightAddOnProperty, ButtonSpinner.RightAddOnProperty);
      CreateTemplateParentBinding(decoratedBox, ButtonSpinnerDecoratedBox.ShowButtonSpinnerProperty, ButtonSpinner.ShowButtonSpinnerProperty);
      CreateTemplateParentBinding(decoratedBox, ButtonSpinnerDecoratedBox.ButtonSpinnerLocationProperty, ButtonSpinner.ButtonSpinnerLocationProperty);
      decoratedBox.RegisterInNameScope(scope);
      return decoratedBox;
   }
   
    protected virtual ButtonSpinnerInnerBox BuildSpinnerContent(ButtonSpinner buttonSpinner, INameScope scope)
   {
      var spinnerInnerBox = new ButtonSpinnerInnerBox
      {
         Name = SpinnerInnerBoxPart,
      };
      CreateTemplateParentBinding(spinnerInnerBox, ButtonSpinnerInnerBox.ContentProperty, ButtonSpinner.ContentProperty);
      CreateTemplateParentBinding(spinnerInnerBox, ButtonSpinnerInnerBox.ContentTemplateProperty, ButtonSpinner.ContentTemplateProperty);
      CreateTemplateParentBinding(spinnerInnerBox, ButtonSpinnerInnerBox.ShowButtonSpinnerProperty, ButtonSpinner.ShowButtonSpinnerProperty);
      CreateTemplateParentBinding(spinnerInnerBox, ButtonSpinnerInnerBox.ButtonSpinnerLocationProperty, ButtonSpinner.ButtonSpinnerLocationProperty);
      
      var spinnerLayout = new UniformGrid()
      {
         Columns = 1,
         Rows = 2
      };

      TokenResourceBinder.CreateTokenBinding(spinnerLayout, StackPanel.WidthProperty, ButtonSpinnerResourceKey.HandleWidth);
      
      var increaseButtonIcon = new PathIcon()
      {
         Kind = "UpOutlined"
      };

      TokenResourceBinder.CreateGlobalTokenBinding(increaseButtonIcon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorPrimaryHover);
      TokenResourceBinder.CreateGlobalTokenBinding(increaseButtonIcon, PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorPrimaryActive);
      
      var increaseButton = new IconButton()
      {
         Name = IncreaseButtonPart,
         Icon = increaseButtonIcon,
         VerticalAlignment = VerticalAlignment.Stretch,
         HorizontalAlignment = HorizontalAlignment.Stretch,
      };
      
      TokenResourceBinder.CreateTokenBinding(increaseButton, IconButton.IconWidthProperty, ButtonSpinnerResourceKey.HandleIconSize);
      TokenResourceBinder.CreateTokenBinding(increaseButton, IconButton.IconHeightProperty, ButtonSpinnerResourceKey.HandleIconSize);
      increaseButton.RegisterInNameScope(scope);

      var decreaseButtonIcon = new PathIcon()
      {
         Kind = "DownOutlined"
      };
      
      TokenResourceBinder.CreateGlobalTokenBinding(decreaseButtonIcon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorPrimaryHover);
      TokenResourceBinder.CreateGlobalTokenBinding(decreaseButtonIcon, PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorPrimaryActive);
      
      var decreaseButton = new IconButton()
      {
         Name = DecreaseButtonPart,
         Icon = decreaseButtonIcon,
         VerticalAlignment = VerticalAlignment.Stretch,
         HorizontalAlignment = HorizontalAlignment.Stretch,
      };
      
      decreaseButton.RegisterInNameScope(scope);
      TokenResourceBinder.CreateTokenBinding(decreaseButton, IconButton.IconWidthProperty, ButtonSpinnerResourceKey.HandleIconSize);
      TokenResourceBinder.CreateTokenBinding(decreaseButton, IconButton.IconHeightProperty, ButtonSpinnerResourceKey.HandleIconSize);
      
      spinnerLayout.Children.Add(increaseButton);
      spinnerLayout.Children.Add(decreaseButton);

      spinnerInnerBox.SpinnerContent = spinnerLayout;
      
      return spinnerInnerBox;
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      
      Add(commonStyle);
   }
}