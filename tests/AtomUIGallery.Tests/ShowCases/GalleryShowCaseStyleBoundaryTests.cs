using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class GalleryShowCaseStyleBoundaryTests
{
    [Fact]
    public void Gallery_ShowCases_Only_Enter_Templates_From_Owned_Style_Scopes()
    {
        var repositoryRoot = GetRepositoryRoot();
        var showCasesRoot = Path.Combine(repositoryRoot, "controlgallery", "AtomUIGallery", "ShowCases");
        var violations = new List<string>();

        foreach (var path in Directory.EnumerateFiles(showCasesRoot, "*.axaml", SearchOption.AllDirectories))
        {
            var document = XDocument.Load(path);
            foreach (var style in document.Descendants().Where(element => element.Name.LocalName == "Style"))
            {
                var selector = style.Attribute("Selector")?.Value;
                if (string.IsNullOrWhiteSpace(selector) ||
                    !selector.Contains("/template/", StringComparison.Ordinal))
                {
                    continue;
                }

                var ownerTheme = style.Ancestors()
                                      .FirstOrDefault(element => element.Name.LocalName == "ControlTheme");
                if ((ownerTheme is null || !TargetsGalleryOwnedControl(ownerTheme)) &&
                    !TargetsGalleryOwnedControlStyles(style, selector))
                {
                    violations.Add(
                        $"{Path.GetRelativePath(repositoryRoot, path)}: template selector is not owned by a Gallery control theme or its own Styles: {selector}");
                    continue;
                }

                foreach (var branch in selector.Split(','))
                {
                    if (CountTemplateBoundaries(branch) > 1)
                    {
                        violations.Add(
                            $"{Path.GetRelativePath(repositoryRoot, path)}: selector crosses multiple template boundaries: {branch.Trim()}");
                    }
                }
            }
        }

        foreach (var path in Directory.EnumerateFiles(showCasesRoot, "*.cs", SearchOption.AllDirectories))
        {
            if (File.ReadAllText(path).Contains(".Template()", StringComparison.Ordinal))
            {
                violations.Add($"{Path.GetRelativePath(repositoryRoot, path)}: .Template()");
            }
        }

        violations.ShouldBeEmpty(string.Join(Environment.NewLine, violations));
    }

    private static bool TargetsGalleryOwnedControl(XElement controlTheme)
    {
        var targetType = controlTheme.Attribute("TargetType")?.Value;
        var separatorIndex = targetType?.IndexOf(':') ?? -1;
        if (separatorIndex <= 0)
        {
            return false;
        }

        var prefix = targetType![..separatorIndex];
        var controlNamespace = controlTheme.GetNamespaceOfPrefix(prefix)?.NamespaceName;
        return controlNamespace?.StartsWith("using:AtomUIGallery", StringComparison.Ordinal) == true;
    }

    private static bool TargetsGalleryOwnedControlStyles(XElement style, string selector)
    {
        var stylesProperty = style.Ancestors()
                                  .FirstOrDefault(element => element.Name.LocalName.EndsWith(".Styles", StringComparison.Ordinal));
        var styleHost = stylesProperty?.Parent;
        if (stylesProperty is null ||
            styleHost is null ||
            stylesProperty.Name.LocalName != $"{styleHost.Name.LocalName}.Styles")
        {
            return false;
        }

        XNamespace xaml = "http://schemas.microsoft.com/winfx/2006/xaml";
        var className = styleHost.Attribute(xaml + "Class")?.Value;
        if (className?.StartsWith("AtomUIGallery.", StringComparison.Ordinal) != true)
        {
            return false;
        }

        return selector.Split(',').All(branch => TargetsStyleHostType(branch, styleHost));
    }

    private static bool TargetsStyleHostType(string selectorBranch, XElement styleHost)
    {
        var templateIndex = selectorBranch.IndexOf("/template/", StringComparison.Ordinal);
        if (templateIndex <= 0)
        {
            return false;
        }

        var ownerSelector = selectorBranch[..templateIndex].Trim();
        var prefixEnd = ownerSelector.IndexOf('|');
        if (prefixEnd <= 0)
        {
            return false;
        }

        var typeStart = prefixEnd + 1;
        var typeEnd = ownerSelector.IndexOfAny(['.', '#', ':', '[', ' '], typeStart);
        if (typeEnd < 0)
        {
            typeEnd = ownerSelector.Length;
        }

        var prefix = ownerSelector[..prefixEnd];
        var targetType = ownerSelector[typeStart..typeEnd];
        return targetType == styleHost.Name.LocalName &&
               styleHost.GetNamespaceOfPrefix(prefix) == styleHost.Name.Namespace;
    }

    private static int CountTemplateBoundaries(string selector)
    {
        var count = 0;
        var index = 0;
        while ((index = selector.IndexOf("/template/", index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += "/template/".Length;
        }

        return count;
    }

    private static string GetRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "AtomUI.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate repository root.");
    }
}
