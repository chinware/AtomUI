using System.ComponentModel;
using AtomUI.Theme;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Schema;

namespace AtomUI.Registration;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class AotTrimControlPackageRegistrationBuilder
{
    private readonly IAtomUIBuilder _builder;
    private readonly IControlThemesProvider _provider;
    private readonly Func<ControlTokenIdentity, bool>? _includeIdentity;
    private readonly Func<IReadOnlyList<ControlThemeAssetDescriptor>,
        IReadOnlyList<ControlThemeAssetDescriptor>>? _selectAssets;
    private readonly HashSet<string> _enteredUnits = new(StringComparer.Ordinal);
    private readonly List<ControlTokenDescriptor> _controls = new();
    private readonly List<ThemeAssetFragment> _themeAssets = new();
    private readonly List<Action<IControlThemesProvider>> _packageSharedThemeAssets = new();
    private readonly List<Action<IControlThemesProvider>> _unitThemeResources = new();
    private bool _registered;

    public AotTrimControlPackageRegistrationBuilder(
        IAtomUIBuilder builder,
        IControlThemesProvider provider,
        Func<ControlTokenIdentity, bool>? includeIdentity = null,
        Func<IReadOnlyList<ControlThemeAssetDescriptor>,
            IReadOnlyList<ControlThemeAssetDescriptor>>? selectAssets = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(provider);

        _builder = builder;
        _provider = provider;
        _includeIdentity = includeIdentity;
        _selectAssets = selectAssets;
    }

    public bool TryEnterUnit(string unitId)
    {
        ThrowIfRegistered();
        ArgumentException.ThrowIfNullOrWhiteSpace(unitId);
        return _enteredUnits.Add(unitId);
    }

    public void AddControl(ControlTokenDescriptor descriptor)
    {
        ThrowIfRegistered();
        ArgumentNullException.ThrowIfNull(descriptor);
        if (_includeIdentity is null || _includeIdentity(descriptor.Identity))
        {
            _controls.Add(descriptor);
        }
    }

    public void AddThemeAsset(
        ControlThemeAssetDescriptor descriptor,
        Action<IControlThemesProvider> addResource)
    {
        ThrowIfRegistered();
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(addResource);
        if (!Includes(descriptor.OwnerIdentity) ||
            descriptor.ReferencedControlIdentities.Any(identity => !Includes(identity)))
        {
            return;
        }

        _themeAssets.Add(new ThemeAssetFragment(descriptor, addResource));
    }

    public void AddPackageSharedThemeAsset(Action<IControlThemesProvider> addResource)
    {
        ThrowIfRegistered();
        ArgumentNullException.ThrowIfNull(addResource);
        _packageSharedThemeAssets.Add(addResource);
    }

    public void AddUnitThemeResource(Action<IControlThemesProvider> addResource)
    {
        ThrowIfRegistered();
        ArgumentNullException.ThrowIfNull(addResource);
        _unitThemeResources.Add(addResource);
    }

    public void Register()
    {
        ThrowIfRegistered();
        _registered = true;

        var availableAssets = _themeAssets.Select(static fragment => fragment.Descriptor).ToArray();
        var selectedAssets = _selectAssets is null
            ? availableAssets
            : _selectAssets(availableAssets);
        ArgumentNullException.ThrowIfNull(selectedAssets);

        var selectedUris = new HashSet<string>(StringComparer.Ordinal);
        foreach (var descriptor in selectedAssets)
        {
            var uri = descriptor.AssetUri.AbsoluteUri;
            if (!availableAssets.Any(asset =>
                    string.Equals(asset.AssetUri.AbsoluteUri, uri, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException(
                    $"Theme asset selector returned '{uri}', which is not part of generated package '{_provider.Id}'.");
            }
            selectedUris.Add(uri);
        }

        foreach (var addResource in _packageSharedThemeAssets)
        {
            addResource(_provider);
        }
        foreach (var addResource in _unitThemeResources)
        {
            addResource(_provider);
        }
        foreach (var fragment in _themeAssets)
        {
            if (selectedUris.Contains(fragment.Descriptor.AssetUri.AbsoluteUri))
            {
                fragment.AddResource(_provider);
            }
        }

        _builder.Theme.AddControlPackage(new ControlPackageRegistration(
            _provider.Id,
            _controls,
            selectedAssets,
            _provider));
    }

    private bool Includes(ControlTokenIdentity identity)
    {
        return _includeIdentity is null || _includeIdentity(identity);
    }

    private void ThrowIfRegistered()
    {
        if (_registered)
        {
            throw new InvalidOperationException(
                $"AOT/Trim control package '{_provider.Id}' has already been registered.");
        }
    }

    private sealed record ThemeAssetFragment(
        ControlThemeAssetDescriptor Descriptor,
        Action<IControlThemesProvider> AddResource);
}
