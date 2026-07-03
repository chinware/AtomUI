using System;
using System.IO;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Window;

public class LinuxWindowFixAotTests
{
    [Fact]
    public void Linux_Window_And_TitleBar_Popup_Fixes_Do_Not_Add_Aot_Unsafe_Code()
    {
        var source = string.Join(
            Environment.NewLine,
            File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Window.cs")),
            File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/WindowChromeManager.cs")),
            File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/WindowsWindowChromeManager.cs")),
            File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/LinuxWindowChromeManager.cs")),
            File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Popup/LinuxCsdPopupSupport.cs")),
            File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Menu/Menu.cs")),
            File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Menu/MenuItem.cs")),
            File.ReadAllText(GetRepoFile("src/AtomUI.Native/Linux/ClickThroughShadowExtensions.cs")));

        foreach (var unsafePattern in s_aotUnsafePatterns)
        {
            source.ShouldNotContain(unsafePattern);
        }
    }

    private static readonly string[] s_aotUnsafePatterns =
    [
        "using System.Reflection",
        "Assembly.GetTypes",
        "GetCustomAttribute",
        "Activator.CreateInstance",
        "Expression.Compile",
        "MakeGenericType",
        "new Binding(",
        "ReflectionBinding",
        "WhenAnyValue",
        "ToProperty",
        "OneWayBind",
        "BindCommand",
        "ReactiveUserControl",
        "RequiresUnreferencedCode",
        "RequiresDynamicCode",
        "UnconditionalSuppressMessage"
    ];

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
