using System.ComponentModel;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

public class ImagePreviewer : AbstractImagePreviewer
{
    public static readonly StyledProperty<object?> CoverIndicatorContentProperty =
        AvaloniaProperty.Register<ImagePreviewer, object?>(nameof(CoverIndicatorContent));

    public static readonly StyledProperty<IDataTemplate?> CoverIndicatorContentTemplateProperty =
        AvaloniaProperty.Register<ImagePreviewer, IDataTemplate?>(nameof(CoverIndicatorContentTemplate));

    public static readonly StyledProperty<bool> IsShowCoverMaskProperty =
        AvaloniaProperty.Register<ImagePreviewer, bool>(nameof(IsShowCoverMask), true);

    public static readonly DirectProperty<ImagePreviewer, ImageLoadState> CoverLoadStateProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewer, ImageLoadState>(
            nameof(CoverLoadState),
            control => control.CoverLoadState);

    public static readonly DirectProperty<ImagePreviewer, ImageLoadError?> CoverLoadErrorProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewer, ImageLoadError?>(
            nameof(CoverLoadError),
            control => control.CoverLoadError);

    public static readonly DirectProperty<ImagePreviewer, ImageLoadProgress?> CoverLoadProgressProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewer, ImageLoadProgress?>(
            nameof(CoverLoadProgress),
            control => control.CoverLoadProgress);

    public static readonly DirectProperty<ImagePreviewer, bool> IsCoverLoadingProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewer, bool>(
            nameof(IsCoverLoading),
            control => control.IsCoverLoading);

    public static readonly DirectProperty<ImagePreviewer, bool> IsCoverLoadedProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewer, bool>(
            nameof(IsCoverLoaded),
            control => control.IsCoverLoaded);

    public static readonly DirectProperty<ImagePreviewer, bool> IsCoverFailedProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewer, bool>(
            nameof(IsCoverFailed),
            control => control.IsCoverFailed);

    internal static readonly DirectProperty<ImagePreviewer, IImage?> EffectiveCoverImageProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewer, IImage?>(
            nameof(EffectiveCoverImage),
            control => control.EffectiveCoverImage);

    private ImagePreviewEntry? _coverEntry;
    private IImage? _effectiveCoverImage;
    private ImageLoadState _coverLoadState;
    private ImageLoadError? _coverLoadError;
    private ImageLoadProgress? _coverLoadProgress;
    private bool _isCoverLoading;
    private bool _isCoverLoaded;
    private bool _isCoverFailed;

    [DependsOn(nameof(CoverIndicatorContentTemplate))]
    public object? CoverIndicatorContent
    {
        get => GetValue(CoverIndicatorContentProperty);
        set => SetValue(CoverIndicatorContentProperty, value);
    }

    public IDataTemplate? CoverIndicatorContentTemplate
    {
        get => GetValue(CoverIndicatorContentTemplateProperty);
        set => SetValue(CoverIndicatorContentTemplateProperty, value);
    }

    public bool IsShowCoverMask
    {
        get => GetValue(IsShowCoverMaskProperty);
        set => SetValue(IsShowCoverMaskProperty, value);
    }

    public ImageLoadState CoverLoadState => _coverLoadState;

    public ImageLoadError? CoverLoadError => _coverLoadError;

    public ImageLoadProgress? CoverLoadProgress => _coverLoadProgress;

    public bool IsCoverLoading => _isCoverLoading;

    public bool IsCoverLoaded => _isCoverLoaded;

    public bool IsCoverFailed => _isCoverFailed;

    internal IImage? EffectiveCoverImage => _effectiveCoverImage;

    public void ReloadCover()
    {
        if (_coverEntry is null)
        {
            return;
        }
        var (width, height) = GetCoverDecodeSize();
        RequestThumbnailLoad(
            _coverEntry,
            width,
            height,
            ImageRequestPriority.High,
            reload: true);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == EffectiveItemsProperty ||
            change.Property == CoverIndexProperty)
        {
            ConfigureCoverEntry();
        }
    }

    private protected override void OnEffectiveItemsChanged()
    {
        ConfigureCoverEntry();
    }

    private protected override void RequestClosedStateLoads()
    {
        ConfigureCoverEntry();
        RequestCoverLoadIfNeeded();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var arranged = base.ArrangeOverride(finalSize);
        RequestCoverLoadIfNeeded();
        return arranged;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        SetCoverEntry(null);
        base.OnDetachedFromVisualTree(e);
    }

    private void ConfigureCoverEntry()
    {
        var entry = EffectiveItems is { Count: > 0 } entries
            ? entries[ClampIndex(CoverIndex, entries.Count)]
            : null;
        SetCoverEntry(entry);
    }

    private void SetCoverEntry(ImagePreviewEntry? entry)
    {
        if (ReferenceEquals(_coverEntry, entry))
        {
            UpdateCoverState();
            RequestCoverLoadIfNeeded();
            return;
        }
        if (_coverEntry is not null)
        {
            _coverEntry.PropertyChanged -= HandleCoverEntryPropertyChanged;
        }
        _coverEntry = entry;
        if (_coverEntry is not null)
        {
            _coverEntry.PropertyChanged += HandleCoverEntryPropertyChanged;
        }
        UpdateCoverState();
        RequestCoverLoadIfNeeded();
    }

    private void RequestCoverLoadIfNeeded()
    {
        if (_coverEntry is null)
        {
            return;
        }
        var (width, height) = GetCoverDecodeSize();
        if (width == 0 && height == 0)
        {
            return;
        }
        RequestThumbnailLoad(_coverEntry, width, height, ImageRequestPriority.High);
    }

    private void HandleCoverEntryPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName is nameof(ImagePreviewEntry.ThumbnailImage) or
            nameof(ImagePreviewEntry.ThumbnailState) or
            nameof(ImagePreviewEntry.ThumbnailError) or
            nameof(ImagePreviewEntry.ThumbnailProgress))
        {
            UpdateCoverState();
        }
    }

    private void UpdateCoverState()
    {
        var state = _coverEntry?.ThumbnailState ?? ImageLoadState.Idle;
        SetAndRaise(EffectiveCoverImageProperty, ref _effectiveCoverImage, _coverEntry?.ThumbnailImage);
        SetAndRaise(CoverLoadStateProperty, ref _coverLoadState, state);
        SetAndRaise(CoverLoadErrorProperty, ref _coverLoadError, _coverEntry?.ThumbnailError);
        SetAndRaise(CoverLoadProgressProperty, ref _coverLoadProgress, _coverEntry?.ThumbnailProgress);
        SetAndRaise(IsCoverLoadingProperty, ref _isCoverLoading, state == ImageLoadState.Loading);
        SetAndRaise(IsCoverLoadedProperty, ref _isCoverLoaded, state == ImageLoadState.Loaded);
        SetAndRaise(IsCoverFailedProperty, ref _isCoverFailed, state == ImageLoadState.Failed);
    }

    private (int Width, int Height) GetCoverDecodeSize()
    {
        var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
        var logicalWidth = double.IsNaN(CoverWidth) ? Bounds.Width : CoverWidth;
        var logicalHeight = double.IsNaN(CoverHeight) ? Bounds.Height : CoverHeight;
        return (Quantize(logicalWidth * scaling), Quantize(logicalHeight * scaling));
    }

    private static int Quantize(double value)
    {
        if (!double.IsFinite(value) || value <= 0)
        {
            return 0;
        }
        return checked((int)(Math.Ceiling(value / 16) * 16));
    }
}
