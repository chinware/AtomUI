namespace AtomUI.Localization;

internal static class LanguageRuntimeResourceKeys
{
    internal static object FlowDirection { get; } = new FlowDirectionResourceKey();

    private sealed class FlowDirectionResourceKey
    {
        public override string ToString() => "AtomUI.Localization.FlowDirection";
    }
}
