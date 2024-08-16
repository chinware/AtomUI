using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class LineEditInnerBoxTheme : AddOnDecoratedInnerBoxTheme
{
   public LineEditInnerBoxTheme() : base(typeof(LineEditInnerBox)) {}
   
   protected override void BuildRightAddOnItems(StackPanel layout, INameScope scope)
   {
      BuildClearButton(layout, scope);
      BuildRevealButton(layout, scope);
   }
   
   protected virtual void BuildRevealButton(StackPanel addOnLayout, INameScope scope)
   {
      var checkedIcon = new PathIcon()
      {
         Kind = "EyeTwoTone"
      };
      
      TokenResourceBinder.CreateGlobalTokenBinding(checkedIcon, PathIcon.PrimaryFilledBrushProperty,
                                                   AddOnDecoratedBoxResourceKey.ActiveBorderColor);
      TokenResourceBinder.CreateGlobalTokenBinding(checkedIcon, PathIcon.SecondaryFilledBrushProperty,
                                                   GlobalResourceKey.ColorPrimaryBgHover);

      var unCheckedIcon = new PathIcon()
      {
         Kind = "EyeInvisibleOutlined"
      };

   
      TokenResourceBinder.CreateGlobalTokenBinding(unCheckedIcon, PathIcon.NormalFilledBrushProperty,
                                                   GlobalResourceKey.ColorTextQuaternary);
      TokenResourceBinder.CreateGlobalTokenBinding(unCheckedIcon, PathIcon.ActiveFilledBrushProperty,
                                                   GlobalResourceKey.ColorTextTertiary);
      TokenResourceBinder.CreateGlobalTokenBinding(unCheckedIcon, PathIcon.SelectedFilledBrushProperty,
                                                   GlobalResourceKey.ColorText);

      var revealButton = new ToggleIconButton()
      {
         Name = RevealButtonPart,
         CheckedIcon = checkedIcon,
         UnCheckedIcon = unCheckedIcon,
      };
      TokenResourceBinder.CreateGlobalTokenBinding(revealButton, ToggleIconButton.IconHeightProperty, GlobalResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(revealButton, ToggleIconButton.IconWidthProperty, GlobalResourceKey.IconSize);
      revealButton.RegisterInNameScope(scope);
      CreateTemplateParentBinding(revealButton, ToggleIconButton.IsVisibleProperty,
                                  LineEditInnerBox.IsRevealButtonVisibleProperty);
      CreateTemplateParentBinding(revealButton, ToggleIconButton.IsCheckedProperty,
                                  LineEditInnerBox.IsRevealButtonCheckedProperty, BindingMode.TwoWay);
      addOnLayout.Children.Add(revealButton);
   }
}