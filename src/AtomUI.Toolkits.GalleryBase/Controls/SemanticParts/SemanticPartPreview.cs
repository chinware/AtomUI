using System.Globalization;
using System.Text;
using AtomUI.Localization;
using AtomUI.Theme;
using AtomUI.Theme.Schema;
using AtomUI.Toolkits.GalleryBase.Localization;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Toolkits.GalleryBase.Controls;

[PseudoClasses(CompactPseudoClass)]
public class SemanticPartPreview : TemplatedControl, IDisposable
{
    private const string CompactPseudoClass = ":compact";
    private const double CompactWidth = 820;

    public static readonly StyledProperty<Control?> PreviewContentProperty =
        AvaloniaProperty.Register<SemanticPartPreview, Control?>(nameof(PreviewContent));

    public static readonly StyledProperty<Control?> SemanticOwnerProperty =
        AvaloniaProperty.Register<SemanticPartPreview, Control?>(nameof(SemanticOwner));

    public static readonly StyledProperty<Type?> SemanticOwnerTypeProperty =
        AvaloniaProperty.Register<SemanticPartPreview, Type?>(nameof(SemanticOwnerType));

    internal static readonly DirectProperty<SemanticPartPreview, IReadOnlyList<SemanticPartPreviewItem>> ItemsProperty =
        AvaloniaProperty.RegisterDirect<SemanticPartPreview, IReadOnlyList<SemanticPartPreviewItem>>(
            nameof(Items),
            preview => preview.Items);

    internal static readonly DirectProperty<SemanticPartPreview, Control?> InfoContentProperty =
        AvaloniaProperty.RegisterDirect<SemanticPartPreview, Control?>(
            nameof(InfoContent),
            preview => preview.InfoContent);

    internal static readonly DirectProperty<SemanticPartPreview, Control?> CodeContentProperty =
        AvaloniaProperty.RegisterDirect<SemanticPartPreview, Control?>(
            nameof(CodeContent),
            preview => preview.CodeContent);

    private IReadOnlyList<SemanticPartPreviewItem> _items = Array.Empty<SemanticPartPreviewItem>();
    private Control? _infoContent;
    private Control? _codeContent;
    private ControlSemanticDescriptor? _controlDescriptor;
    private SemanticPartRegistry? _registry;
    private Control? _effectiveOwner;
    private SemanticPartPreviewItem? _hoveredPart;
    private SemanticPartPreviewItem? _pinnedPart;
    private SemanticPartHighlightSession? _activeHighlightSession;
    private GalleryCodeViewer? _codeViewer;
    private SemanticPartPreviewInfoControl? _infoView;
    private SemanticPartPreviewCodeExampleControl? _codeView;
    private bool _highlightUpdateQueued;
    private bool _isDisposed;

    public SemanticPartPreview()
    {
        PartDescriptions.CollectionChanged += (_, _) => ResetPresentation();
    }

    public Control? PreviewContent
    {
        get => GetValue(PreviewContentProperty);
        set => SetValue(PreviewContentProperty, value);
    }

    public Control? SemanticOwner
    {
        get => GetValue(SemanticOwnerProperty);
        set => SetValue(SemanticOwnerProperty, value);
    }

    public Type? SemanticOwnerType
    {
        get => GetValue(SemanticOwnerTypeProperty);
        set => SetValue(SemanticOwnerTypeProperty, value);
    }

    public AvaloniaList<SemanticPartDescription> PartDescriptions { get; } = [];

    public AvaloniaList<Visual> AdditionalRoots { get; } = [];

    internal IReadOnlyList<SemanticPartPreviewItem> Items
    {
        get => _items;
        private set => SetAndRaise(ItemsProperty, ref _items, value);
    }

    internal Control? InfoContent
    {
        get => _infoContent;
        private set => SetAndRaise(InfoContentProperty, ref _infoContent, value);
    }

    internal Control? CodeContent
    {
        get => _codeContent;
        private set => SetAndRaise(CodeContentProperty, ref _codeContent, value);
    }

    internal bool IsPreviewActive { get; private set; }

    internal SemanticPartHighlightSession? ActiveHighlightSession => _activeHighlightSession;

    internal GalleryCodeViewer? CodeViewer => _codeViewer;

    internal SemanticPartPreviewItem? EffectivePart => _pinnedPart ?? _hoveredPart;

    internal void ActivatePreview()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        IsPreviewActive = true;
        EnsurePresentation();
        if (EffectivePart is not null)
        {
            QueueHighlightUpdate();
        }
    }

    internal void DeactivatePreview()
    {
        IsPreviewActive = false;
        _hoveredPart = null;
        _highlightUpdateQueued = false;
        ReleaseHighlightSession();
    }

    internal void SetHoveredPart(SemanticPartPreviewItem item, bool isHovered)
    {
        EnsureKnownItem(item);
        if (isHovered)
        {
            _hoveredPart = item;
        }
        else if (ReferenceEquals(_hoveredPart, item))
        {
            _hoveredPart = null;
        }

        QueueHighlightUpdate();
    }

    internal void TogglePinnedPart(SemanticPartPreviewItem item)
    {
        EnsureKnownItem(item);
        _pinnedPart = ReferenceEquals(_pinnedPart, item) ? null : item;
        foreach (var candidate in Items)
        {
            candidate.IsPinned = ReferenceEquals(candidate, _pinnedPart);
        }
        QueueHighlightUpdate();
    }

    internal void ShowPartInfo(SemanticPartPreviewItem item)
    {
        EnsureKnownItem(item);
        var code = item.CodeSnippet ?? BuildCodeSnippet(item.Descriptor);
        _codeViewer ??= new GalleryCodeViewer
        {
            Language = "xml",
            ShowLineNumbers = false
        };
        _codeViewer.CodeText = code;
        _infoView ??= new SemanticPartPreviewInfoControl();
        _infoView.Show(item);
        _codeView ??= new SemanticPartPreviewCodeExampleControl();
        _codeView.Show(_codeViewer);
        InfoContent = _infoView;
        CodeContent = _codeView;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        PseudoClasses.Set(CompactPseudoClass, finalSize.Width < CompactWidth);
        return base.ArrangeOverride(finalSize);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == PreviewContentProperty ||
            ((change.Property == SemanticOwnerProperty ||
              change.Property == SemanticOwnerTypeProperty) &&
             IsSemanticOwnerStateResolvable()))
        {
            ResetPresentation();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DeactivatePreview();
        base.OnDetachedFromVisualTree(e);
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        DeactivatePreview();
        _infoView?.Clear();
        _infoView = null;
        _codeView?.Clear();
        _codeView = null;
        _codeViewer?.Dispose();
        _codeViewer = null;
        InfoContent = null;
        CodeContent = null;
        Items = Array.Empty<SemanticPartPreviewItem>();
        _controlDescriptor = null;
        _registry = null;
        _effectiveOwner = null;
        _pinnedPart = null;
        _isDisposed = true;
    }

    private void EnsurePresentation()
    {
        if (_controlDescriptor is not null)
        {
            return;
        }

        var owner = SemanticOwner ?? PreviewContent ??
                    throw new InvalidOperationException(
                        $"{nameof(SemanticPartPreview)} requires {nameof(SemanticOwner)} or {nameof(PreviewContent)}.");
        var ownerType = SemanticOwnerType ?? owner.GetType();
        var registry = Application.Current?.GetThemeManager()?.SemanticParts ?? SemanticPartRegistry.Empty;
        if (!registry.TryGetControl(ownerType, out var descriptor))
        {
            throw new InvalidOperationException(
                $"No Semantic Part descriptor is registered for '{ownerType.FullName}'.");
        }
        if (!descriptor.ControlType.IsAssignableFrom(owner.GetType()))
        {
            throw new InvalidOperationException(
                $"Semantic owner '{owner.GetType().FullName}' is not compatible with descriptor owner " +
                $"'{descriptor.ControlType.FullName}'.");
        }

        var descriptions = BuildDescriptions(descriptor);
        var partsByPath = descriptor.Parts.ToDictionary(static part => part.Path, StringComparer.Ordinal);
        var orderedParts = PartDescriptions
                           .Select(description => partsByPath[description.Path])
                           .Concat(descriptor.Parts.Where(part => !descriptions.ContainsKey(part.Path)));
        _effectiveOwner = owner;
        _registry = registry;
        _controlDescriptor = descriptor;
        Items = orderedParts.Select(part =>
                            {
                                descriptions.TryGetValue(part.Path, out var description);
                                return new SemanticPartPreviewItem(
                                    part,
                                    description?.Description ?? GetFallbackDescription(part),
                                    description?.CodeSnippet);
                            })
                            .ToArray();
    }

    private Dictionary<string, SemanticPartDescription> BuildDescriptions(ControlSemanticDescriptor descriptor)
    {
        var result = new Dictionary<string, SemanticPartDescription>(StringComparer.Ordinal);
        foreach (var description in PartDescriptions)
        {
            ArgumentNullException.ThrowIfNull(description);
            ArgumentException.ThrowIfNullOrWhiteSpace(description.Path);
            if (!result.TryAdd(description.Path, description))
            {
                throw new InvalidOperationException(
                    $"Semantic Part description path '{description.Path}' is duplicated.");
            }
        }

        var knownPaths = descriptor.Parts.Select(static part => part.Path).ToHashSet(StringComparer.Ordinal);
        foreach (var path in result.Keys)
        {
            if (!knownPaths.Contains(path))
            {
                throw new InvalidOperationException(
                    $"Semantic Part description path '{path}' does not exist on " +
                    $"'{descriptor.ControlType.FullName}'.");
            }
        }
        return result;
    }

    private void ResetPresentation()
    {
        ReleaseHighlightSession();
        _infoView?.Clear();
        _codeView?.Clear();
        InfoContent = null;
        CodeContent = null;
        _controlDescriptor = null;
        _registry = null;
        _effectiveOwner = null;
        _hoveredPart = null;
        _pinnedPart = null;
        Items = Array.Empty<SemanticPartPreviewItem>();
        if (IsPreviewActive && !_isDisposed)
        {
            EnsurePresentation();
        }
    }

    private bool IsSemanticOwnerStateResolvable()
    {
        return SemanticOwner is not { } owner ||
               SemanticOwnerType is not { } ownerType ||
               ownerType.IsAssignableFrom(owner.GetType());
    }

    private void QueueHighlightUpdate()
    {
        if (!IsPreviewActive || _isDisposed || _highlightUpdateQueued)
        {
            return;
        }

        _highlightUpdateQueued = true;
        Dispatcher.UIThread.Post(ApplyQueuedHighlightUpdate, DispatcherPriority.Render);
    }

    private void ApplyQueuedHighlightUpdate()
    {
        if (!_highlightUpdateQueued)
        {
            return;
        }

        _highlightUpdateQueued = false;
        ReleaseHighlightSession();
        if (!IsPreviewActive ||
            EffectivePart is not { } item ||
            _effectiveOwner is not { } owner ||
            _registry is not { } registry)
        {
            return;
        }

        _activeHighlightSession = SemanticPartHighlightSession.Start(
            owner,
            item.Descriptor,
            registry,
            AdditionalRoots);
    }

    private void ReleaseHighlightSession()
    {
        _activeHighlightSession?.Dispose();
        _activeHighlightSession = null;
    }

    private void EnsureKnownItem(SemanticPartPreviewItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (!Items.Contains(item))
        {
            throw new ArgumentException("The Semantic Part item does not belong to this Preview.", nameof(item));
        }
    }

    private static string GetFallbackDescription(SemanticPartDescriptor part)
    {
        return string.Equals(part.Path, "root", StringComparison.Ordinal)
            ? GetLocalized(
                SemanticPartPreviewLangResourceKind.RootFallbackDescription,
                "The control root.")
            : FormatLocalized(
                SemanticPartPreviewLangResourceKind.PartFallbackDescription,
                "The {0} semantic region.",
                part.Name);
    }

    private static string GetLocalized(
        SemanticPartPreviewLangResourceKind resourceKind,
        string fallback)
    {
        return Application.Current?.GetLocalizer()?.Get(resourceKind) ?? fallback;
    }

    private static string FormatLocalized(
        SemanticPartPreviewLangResourceKind resourceKind,
        string fallback,
        params object?[] arguments)
    {
        return Application.Current?.GetLocalizer()?.Format(resourceKind, arguments) ??
               string.Format(CultureInfo.CurrentCulture, fallback, arguments);
    }

    private string BuildCodeSnippet(SemanticPartDescriptor part)
    {
        var ownerType = _controlDescriptor?.ControlType ?? SemanticOwnerType ?? _effectiveOwner?.GetType() ??
                        throw new InvalidOperationException("Semantic owner metadata is unavailable.");
        const string atomPrefix = "atom";
        var targetType = GetSetterTargetType(part.ContractType, atomPrefix);
        var ownerSelector = $"{atomPrefix}|{ownerType.Name}";

        var builder = new StringBuilder();
        builder.AppendLine("<Styles xmlns=\"https://github.com/avaloniaui\"");
        builder.AppendLine("        xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
        builder.Append($"        xmlns:{atomPrefix}=\"https://atomui.net\"");
        builder.AppendLine(">");
        if (string.Equals(part.Path, "root", StringComparison.Ordinal))
        {
            builder.AppendLine($"  <Style Selector=\"{ownerSelector}\"");
            builder.AppendLine($"         x:SetterTargetType=\"{targetType}\">");
            builder.AppendLine("    <Setter Property=\"Opacity\" Value=\"1\" />");
            builder.AppendLine("  </Style>");
        }
        else
        {
            var styleTypeName = part.StyleType?.Name ??
                                $"{ownerType.Name}{ToPartTypeName(part.Path)}Style";
            builder.AppendLine($"  <Style Selector=\"{ownerSelector}\">");
            builder.AppendLine($"    <{atomPrefix}:{styleTypeName} x:SetterTargetType=\"{targetType}\">");
            builder.AppendLine("      <Setter Property=\"Opacity\" Value=\"1\" />");
            builder.AppendLine($"    </{atomPrefix}:{styleTypeName}>");
            builder.AppendLine("  </Style>");
        }
        builder.Append("</Styles>");
        return builder.ToString();
    }

    private static string GetSetterTargetType(Type contractType, string atomPrefix)
    {
        return contractType.Namespace?.StartsWith("Avalonia.", StringComparison.Ordinal) == true
            ? contractType.Name
            : $"{atomPrefix}:{contractType.Name}";
    }

    private static string ToPartTypeName(string path)
    {
        return string.Concat(path.Split('.').Select(static segment =>
            segment.Length == 0
                ? string.Empty
                : char.ToUpperInvariant(segment[0]) + segment.Substring(1)));
    }
}
