using Avalonia;

namespace AtomUI.Controls;

public static class ImageLoadingBuilderExtensions
{
    private const string StateId = "AtomUI.Controls.ImageLoading";
    private const string ServiceId = "AtomUI.Controls.ImageLoader";

    public static IAtomUIBuilder UseImageLoading(
        this IAtomUIBuilder builder,
        Action<ImageLoadingOptionsBuilder>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var atomBuilder = builder as AtomUIBuilder ??
            throw new InvalidOperationException("Image loading requires the AtomUI application builder.");
        var state = atomBuilder.GetOrAddExtensionState(StateId, static () => new ImageLoadingRegistrationState());
        configure?.Invoke(state.Options);
        atomBuilder.AddOwnedService(ServiceId, state.BuildLoader);
        return builder;
    }

    internal static IAtomUIBuilder AddImageCodec<TCodec>(
        this IAtomUIBuilder builder,
        Func<TCodec> factory)
        where TCodec : ImageCodec
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(factory);
        builder.UseImageLoading();
        var atomBuilder = (AtomUIBuilder)builder;
        var state = atomBuilder.GetOrAddExtensionState(StateId, static () => new ImageLoadingRegistrationState());
        state.AddCodec(factory);
        return builder;
    }

    private sealed class ImageLoadingRegistrationState
    {
        private readonly List<Func<ImageCodec>> _codecFactories = [static () => new RasterImageCodec()];
        private bool _built;

        internal ImageLoadingOptionsBuilder Options { get; } = new();

        internal void AddCodec(Func<ImageCodec> factory)
        {
            if (_built)
            {
                throw new InvalidOperationException("Image codec registration is frozen.");
            }
            _codecFactories.Add(factory);
        }

        internal ImageLoader BuildLoader(Application application)
        {
            if (_built)
            {
                throw new InvalidOperationException("Image loader has already been built.");
            }
            _built = true;
            var applicationName = application.GetType().Assembly.GetName().Name;
            return new ImageLoader(
                Options.Build(applicationName),
                _codecFactories.Select(factory => factory()));
        }
    }
}

public static class ImageLoadingApplicationExtensions
{
    public static IImageLoader GetImageLoader(this Application application)
    {
        ArgumentNullException.ThrowIfNull(application);
        return ImageLoaderStore.Get(application) ?? throw new InvalidOperationException(
            "Image loading is not registered for this Application. Call UseImageLoading(), UseCommonControls(), or UseDesktopControls() inside UseAtomUI().");
    }

    public static IImageLoader? TryGetImageLoader(this Application application)
    {
        ArgumentNullException.ThrowIfNull(application);
        return ImageLoaderStore.Get(application);
    }
}
