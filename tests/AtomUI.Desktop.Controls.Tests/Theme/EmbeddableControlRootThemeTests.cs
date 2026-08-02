using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Embedding;
using Avalonia.Media;
using Avalonia.Styling;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class EmbeddableControlRootThemeTests
{
    public EmbeddableControlRootThemeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Prepare_Assigns_A_Concrete_System_Bar_Brush()
    {
        var root = new EmbeddableThemeProbe();
        Application.Current!.TryFindResource(typeof(EmbeddableControlRoot), out var resource)
                   .ShouldBeTrue();
        root.Theme = resource.ShouldBeAssignableTo<ControlTheme>();
        var host = new Avalonia.Controls.Window
        {
            Content = root
        };

        try
        {
            host.Show();
            host.UpdateLayout();

            root.GetValue(TopLevel.SystemBarColorProperty)
                .ShouldBeOfType<SolidColorBrush>();
        }
        finally
        {
            host.Close();
        }
    }

    private sealed class EmbeddableThemeProbe : Control
    {
        protected override Type StyleKeyOverride => typeof(EmbeddableControlRoot);
    }
}
