namespace AtomUI.Localization;

internal static class LanguageResourceKeys
{
    internal static object FlowDirection { get; } = new FlowDirectionResourceKey();

    private sealed class FlowDirectionResourceKey
    {
        public override string ToString() => "AtomUI.Localization.FlowDirection";
    }
}
