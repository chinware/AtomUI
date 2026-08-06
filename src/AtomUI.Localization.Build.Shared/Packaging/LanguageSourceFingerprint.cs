using System.Security.Cryptography;
using System.Text;

namespace AtomUI.Localization.Build;

internal static class LanguageSourceFingerprint
{
    internal static string Compute(XliffDocumentModel document)
    {
        var builder = new StringBuilder();
        foreach (var unit in document.File.Units
                                     .Where(static unit => !unit.IsObsolete)
                                     .OrderBy(static unit => unit.Id))
        {
            builder.Append(unit.Id)
                   .Append('\0')
                   .Append(unit.Name)
                   .Append('\0')
                   .Append(unit.Source)
                   .Append('\n');
        }

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(builder.ToString()));
        var result = new StringBuilder(hash.Length * 2);
        foreach (var value in hash)
        {
            result.Append(value.ToString("x2"));
        }
        return result.ToString();
    }
}
