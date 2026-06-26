namespace AtomUI.Docs.LLMsGenerator.Tests;

internal static class TestRepository
{
    public static string RootPath { get; } = FindRootPath();

    private static string FindRootPath()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "AtomUI.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Unable to locate AtomUI repository root.");
    }
}
