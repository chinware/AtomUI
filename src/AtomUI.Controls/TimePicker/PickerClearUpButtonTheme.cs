using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class PickerClearUpButtonTheme : BaseControlTheme
{
   public const string ClearButtonPart = "PART_ClearButton";
   
   public PickerClearUpButtonTheme() : base(typeof(PickerClearUpButton))
   {
   }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<PickerClearUpButton>((indicator, scope) =>
      {
         var container = new Panel();
         BuildClearButton(container, scope);
         BuildClockIcon(container, scope);
         return container;
      });
   }

   private void BuildClockIcon(Panel layout, INameScope scope)
   {
      var clockIcon = new PathIcon()
      {
         Kind = "ClockCircleOutlined"
      };
      TokenResourceBinder.CreateGlobalTokenBinding(clockIcon, PathIcon.HeightProperty, GlobalTokenResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(clockIcon, PathIcon.WidthProperty, GlobalTokenResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(clockIcon, PathIcon.NormalFilledBrushProperty,
                                                   GlobalTokenResourceKey.ColorTextQuaternary);
      CreateTemplateParentBinding(clockIcon, IconButton.IsVisibleProperty,
                                  PickerClearUpButton.IsInClearModeProperty, BindingMode.Default,
         BoolConverters.Not);
      layout.Children.Add(clockIcon);
   }

   private void BuildClearButton(Panel layout, INameScope scope)
   {
      var closeIcon = new PathIcon()
      {
         Kind = "CloseCircleFilled"
      };
      var clearButton = new IconButton()
      {
         Name = ClearButtonPart,
         Icon = closeIcon
      };

      TokenResourceBinder.CreateGlobalTokenBinding(clearButton, IconButton.IconHeightProperty, GlobalTokenResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(clearButton, IconButton.IconWidthProperty, GlobalTokenResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.NormalFilledBrushProperty,
                                                   GlobalTokenResourceKey.ColorTextQuaternary);
      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.ActiveFilledBrushProperty,
                                                   GlobalTokenResourceKey.ColorTextTertiary);
      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.SelectedFilledBrushProperty,
                                                   GlobalTokenResourceKey.ColorText);

      clearButton.RegisterInNameScope(scope);
      CreateTemplateParentBinding(clearButton, IconButton.IsVisibleProperty,
                                  PickerClearUpButton.IsInClearModeProperty);
      layout.Children.Add(clearButton);
   }

   protected override void BuildStyles()
   {
      
   }
}