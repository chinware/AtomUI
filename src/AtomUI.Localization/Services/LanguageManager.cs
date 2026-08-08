using System.Collections.ObjectModel;
using Avalonia.Threading;

namespace AtomUI.Localization;

internal sealed class LanguageManager : ILanguageManager, IDisposable
{
    private readonly LanguageContext _context;
    private readonly IReadOnlyDictionary<LanguageTag, LanguageSnapshot> _snapshots;
    private readonly IReadOnlyDictionary<LanguageTag, LanguageDefinition> _definitions;
    private readonly ReadOnlyCollection<LanguageDefinition> _supportedLanguages;
    private readonly LanguageResourceProvider _resourceProvider;
    private readonly Func<bool> _checkAccess;
    private int _disposed;

    internal LanguageManager(
        LanguageContext context,
        IReadOnlyDictionary<LanguageTag, LanguageSnapshot> snapshots,
        IReadOnlyList<LanguageDefinition> supportedLanguages,
        LanguageResourceProvider resourceProvider,
        Func<bool>? checkAccess = null)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        ArgumentNullException.ThrowIfNull(snapshots);
        ArgumentNullException.ThrowIfNull(supportedLanguages);
        _resourceProvider = resourceProvider ?? throw new ArgumentNullException(nameof(resourceProvider));
        _checkAccess = checkAccess ?? (static () => Dispatcher.UIThread.CheckAccess());

        if (supportedLanguages.Count == 0)
        {
            throw new LanguageConfigurationException("At least one supported language is required.");
        }

        var definitionArray = new LanguageDefinition[supportedLanguages.Count];
        var definitionsByTag = new Dictionary<LanguageTag, LanguageDefinition>();
        var snapshotCopies = new Dictionary<LanguageTag, LanguageSnapshot>();
        for (var index = 0; index < supportedLanguages.Count; index++)
        {
            var definition = supportedLanguages[index] ?? throw new LanguageConfigurationException(
                "A supported language definition cannot be null.");
            if (!definitionsByTag.TryAdd(definition.Tag, definition))
            {
                throw new LanguageConfigurationException(
                    $"Language definition '{definition.Tag.Value}' is registered more than once.");
            }
            if (!snapshots.TryGetValue(definition.Tag, out var snapshot))
            {
                throw new LanguageConfigurationException(
                    $"Supported language '{definition.Tag.Value}' has no prebuilt Snapshot.");
            }
            if (snapshot.RequestedLanguage != definition.Tag)
            {
                throw new LanguageConfigurationException(
                    $"Snapshot '{snapshot.RequestedLanguage.Value}' cannot be registered for language " +
                    $"'{definition.Tag.Value}'.");
            }

            definitionArray[index] = definition;
            snapshotCopies.Add(definition.Tag, snapshot);
        }

        var currentLanguage = context.Current.State.CurrentLanguage;
        if (!definitionsByTag.ContainsKey(currentLanguage))
        {
            throw new LanguageConfigurationException(
                $"Initial language '{currentLanguage.Value}' is not supported.");
        }

        _supportedLanguages = Array.AsReadOnly(definitionArray);
        _definitions = new ReadOnlyDictionary<LanguageTag, LanguageDefinition>(definitionsByTag);
        _snapshots = new ReadOnlyDictionary<LanguageTag, LanguageSnapshot>(snapshotCopies);
    }

    public LanguageState Current => _context.Current.State;

    public IReadOnlyList<LanguageDefinition> SupportedLanguages => _supportedLanguages;

    public event EventHandler<LanguageChangedEventArgs>? LanguageChanged;

    public LanguageChangeResult ChangeLanguage(LanguageTag language)
    {
        VerifyAccess();
        ThrowIfDisposed();
        if (language == default)
        {
            throw new ArgumentException("A valid language tag is required.", nameof(language));
        }
        if (!_definitions.TryGetValue(language, out var definition))
        {
            throw new LanguageNotSupportedException(language);
        }

        var oldRevision = _context.Current;
        if (oldRevision.State.CurrentLanguage == language)
        {
            return LanguageChangeResult.NoOp(oldRevision.State);
        }

        var newState = new LanguageState(
            language,
            definition.FormattingCulture,
            definition.TextDirection,
            checked(oldRevision.State.Revision + 1));
        var newRevision = new LanguageRevision(_snapshots[language], newState);
        var result = LanguageChangeResult.Committed(oldRevision.State, newState);

        _context.Publish(newRevision);
        PublishResourcesChanged();
        PublishLanguageChanged(new LanguageChangedEventArgs(result));
        return result;
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0)
        {
            LanguageChanged = null;
        }
    }

    private void VerifyAccess()
    {
        if (!_checkAccess())
        {
            throw new InvalidOperationException("Language changes must be committed on the UI thread.");
        }
    }

    private void ThrowIfDisposed()
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new InvalidOperationException("LanguageManager has been disposed.");
        }
    }

    private void PublishResourcesChanged()
    {
        try
        {
            _resourceProvider.PublishResourcesChanged();
        }
        catch (Exception exception)
        {
            LocalizationLogger.LogPublishFailure(
                this,
                nameof(LanguageResourceProvider),
                exception);
        }
    }

    private void PublishLanguageChanged(LanguageChangedEventArgs args)
    {
        var handlers = LanguageChanged;
        if (handlers is null)
        {
            return;
        }

        foreach (var invocation in handlers.GetInvocationList())
        {
            if (invocation is not EventHandler<LanguageChangedEventArgs> handler)
            {
                continue;
            }

            try
            {
                handler(this, args);
            }
            catch (Exception exception)
            {
                LocalizationLogger.LogPublishFailure(
                    this,
                    nameof(LanguageChanged),
                    exception);
            }
        }
    }
}
