using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class LoadingIndicatorAdornerTheme : BaseControlTheme
{
    public const string LoadingIndicatorPart = "PART_LoadingIndicator";
    public const string MainContainerPart = "PART_MainContainer";

    public LoadingIndicatorAdornerTheme()
        : base(typeof(LoadingIndicatorAdorner))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<LoadingIndicatorAdorner>((adorner, scope) =>
        {
            var mainContainer = new Canvas
            {
                Name = MainContainerPart
            };
            var loadingIndicator = new LoadingIndicator
            {
                Name = LoadingIndicatorPart
            };
            mainContainer.Children.Add(loadingIndicator);
            mainContainer.RegisterInNameScope(scope);
            loadingIndicator.RegisterInNameScope(scope);
            return mainContainer;
        });
    }
}