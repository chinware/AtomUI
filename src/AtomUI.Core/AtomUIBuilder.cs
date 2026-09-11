using AtomUI.Localization;
using AtomUI.Theme;
using Avalonia;

namespace AtomUI;

internal sealed class AtomUIBuilder : IAtomUIBuilder
{
    private readonly Dictionary<string, object> _extensionStates = new(StringComparer.Ordinal);
    private readonly List<AtomUIOwnedServiceRegistration> _ownedServiceRegistrations = [];
    private readonly HashSet<string> _ownedServiceIds = new(StringComparer.Ordinal);

    internal AtomUIBuilder(Application application)
    {
        ArgumentNullException.ThrowIfNull(application);
        ThemeManagerBuilder = new ThemeManagerBuilder(application);
        LocalizationBuilder = new LocalizationBuilder();
    }

    public IThemeManagerBuilder Theme => ThemeManagerBuilder;

    public ILocalizationBuilder Localization => LocalizationBuilder;

    internal ThemeManagerBuilder ThemeManagerBuilder { get; }

    internal LocalizationBuilder LocalizationBuilder { get; }

    internal IReadOnlyList<AtomUIOwnedServiceRegistration> OwnedServiceRegistrations => _ownedServiceRegistrations;

    internal TState GetOrAddExtensionState<TState>(string id, Func<TState> factory)
        where TState : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(factory);
        if (_extensionStates.TryGetValue(id, out var current))
        {
            return (TState)current;
        }

        var created = factory();
        _extensionStates.Add(id, created);
        return created;
    }

    internal void AddOwnedService(
        string id,
        Func<Application, IAtomUIOwnedService> factory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(factory);
        if (!_ownedServiceIds.Add(id))
        {
            return;
        }

        _ownedServiceRegistrations.Add(new AtomUIOwnedServiceRegistration(id, factory));
    }
}
