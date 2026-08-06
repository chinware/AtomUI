using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace AtomUIGallery.Tests.Localization;

internal static class XliffTestDocument
{
    public static IReadOnlyDictionary<string, string> Read(string relativePath)
    {
        var path = Path.Combine(FindRepositoryRoot(), relativePath);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Could not find repository file: {relativePath}", path);
        }

        var document = XDocument.Load(path);
        XNamespace xliff = "urn:oasis:names:tc:xliff:document:2.0";
        var values = document.Descendants(xliff + "unit")
                             .ToDictionary(
                                 unit => (string?)unit.Attribute("name") ?? throw new InvalidDataException(
                                     $"XLIFF unit in '{relativePath}' has no name."),
                                 unit => unit.Descendants(xliff + "target").SingleOrDefault()?.Value ??
                                         unit.Descendants(xliff + "source").Single().Value,
                                 StringComparer.Ordinal);
        return new ReadOnlyDictionary<string, string>(values);
    }

    private static string FindRepositoryRoot()
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

        throw new DirectoryNotFoundException("The AtomUI repository root could not be located.");
    }
}
