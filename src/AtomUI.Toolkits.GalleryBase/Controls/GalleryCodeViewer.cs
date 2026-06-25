using System.Diagnostics;
using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.VisualTree;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;
using AtomUIContextMenu = AtomUI.Desktop.Controls.ContextMenu;
using AtomUIMenuItem = AtomUI.Desktop.Controls.MenuItem;

namespace AtomUI.Toolkits.GalleryBase.Controls;

public sealed partial class GalleryCodeViewer : UserControl, IDisposable
{
    public static readonly StyledProperty<string?> CodeTextProperty =
        AvaloniaProperty.Register<GalleryCodeViewer, string?>(nameof(CodeText));

    public static readonly StyledProperty<string> LanguageProperty =
        AvaloniaProperty.Register<GalleryCodeViewer, string>(nameof(Language), "text");

    public static readonly StyledProperty<bool> ShowLineNumbersProperty =
        AvaloniaProperty.Register<GalleryCodeViewer, bool>(nameof(ShowLineNumbers), true);

    public static readonly StyledProperty<ThemeName> LightSyntaxThemeProperty =
        AvaloniaProperty.Register<GalleryCodeViewer, ThemeName>(nameof(LightSyntaxTheme), ThemeName.LightPlus);

    public static readonly StyledProperty<ThemeName> DarkSyntaxThemeProperty =
        AvaloniaProperty.Register<GalleryCodeViewer, ThemeName>(nameof(DarkSyntaxTheme), ThemeName.DarkPlus);

    private readonly TextEditor _editor;
    private readonly AtomUIContextMenu _editorContextMenu = new();
    private readonly AtomUIMenuItem _copyMenuItem = new()
    {
        Header = "Copy",
        IsEnabled = false
    };

    private TextMate.Installation? _textMateInstallation;
    private RegistryOptions? _registryOptions;
    private ScrollBar? _horizontalScrollBar;
    private Application? _subscribedApplication;
    private IThemeManager? _subscribedThemeManager;
    private IDisposable? _themeVariantSubscription;
    private ThemeName? _currentSyntaxTheme;
    private bool _isDisposed;

    public GalleryCodeViewer()
    {
        InitializeComponent();
        _editor = this.FindControl<TextEditor>("PART_Editor")!;
        _editor.LayoutUpdated += HandleEditorLayoutUpdated;
        _editor.TextArea.SelectionChanged += HandleEditorSelectionChanged;
        ActualThemeVariantChanged += HandleActualThemeVariantChanged;
        ConfigureEditorContextMenu();
        EnsureTextMateInstalled();
        UpdateText();
        ApplyGrammar();
    }

    public string? CodeText
    {
        get => GetValue(CodeTextProperty);
        set => SetValue(CodeTextProperty, value);
    }

    public string Language
    {
        get => GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    public bool ShowLineNumbers
    {
        get => GetValue(ShowLineNumbersProperty);
        set => SetValue(ShowLineNumbersProperty, value);
    }

    public ThemeName LightSyntaxTheme
    {
        get => GetValue(LightSyntaxThemeProperty);
        set => SetValue(LightSyntaxThemeProperty, value);
    }

    public ThemeName DarkSyntaxTheme
    {
        get => GetValue(DarkSyntaxThemeProperty);
        set => SetValue(DarkSyntaxThemeProperty, value);
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _textMateInstallation?.Dispose();
        _textMateInstallation = null;
        _registryOptions      = null;
        ReleaseThemeVariantSubscriptions();
        _editor.LayoutUpdated -= HandleEditorLayoutUpdated;
        _editor.TextArea.SelectionChanged -= HandleEditorSelectionChanged;
        _editor.Document      = null;
        _copyMenuItem.Click -= HandleCopyMenuItemClick;
        _editorContextMenu.Opened -= HandleEditorContextMenuOpened;
        _editorContextMenu.Close();
        if (ReferenceEquals(_editor.ContextMenu, _editorContextMenu))
        {
            _editor.ContextMenu = null;
        }
        ActualThemeVariantChanged -= HandleActualThemeVariantChanged;
        _horizontalScrollBar  = null;
        _currentSyntaxTheme   = null;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CodeTextProperty)
        {
            UpdateText();
        }
        else if (change.Property == LanguageProperty)
        {
            ApplyGrammar();
        }
        else if (change.Property == ShowLineNumbersProperty)
        {
            _editor.ShowLineNumbers = ShowLineNumbers;
            UpdateHorizontalScrollBarGutterInset();
        }
        else if (change.Property == LightSyntaxThemeProperty ||
                 change.Property == DarkSyntaxThemeProperty)
        {
            ApplySyntaxTheme();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SubscribeThemeVariantChanges();
        ApplySyntaxTheme();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ReleaseThemeVariantSubscriptions();
        base.OnDetachedFromVisualTree(e);
    }

    private void HandleEditorLayoutUpdated(object? sender, EventArgs e)
    {
        UpdateHorizontalScrollBarGutterInset();
    }

    private void HandleEditorSelectionChanged(object? sender, EventArgs e)
    {
        UpdateCopyMenuItemState();
    }

    private void HandleEditorContextMenuOpened(object? sender, EventArgs e)
    {
        UpdateCopyMenuItemState();
    }

    private async void HandleCopyMenuItemClick(object? sender, RoutedEventArgs e)
    {
        e.Handled = true;
        try
        {
            await CopySelectedCodeToClipboardAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error copying Gallery source code: {ex.Message}");
        }
    }

    private void HandleActualThemeVariantChanged(object? sender, EventArgs e)
    {
        ApplySyntaxTheme();
    }

    private void HandleApplicationActualThemeVariantChanged(object? sender, EventArgs e)
    {
        ApplySyntaxTheme();
    }

    private void HandleThemeManagerThemeVariantChanged(ThemeVariant themeVariant)
    {
        ApplySyntaxTheme();
    }

    private void UpdateHorizontalScrollBarGutterInset()
    {
        var horizontalScrollBar = GetHorizontalScrollBar();
        if (horizontalScrollBar is null)
        {
            return;
        }

        var gutterWidth = GetTextAreaLeftMarginsRightEdge();
        var currentMargin = horizontalScrollBar.Margin;
        if (Math.Abs(currentMargin.Left - gutterWidth) <= 0.5)
        {
            return;
        }

        horizontalScrollBar.Margin = new Thickness(
            gutterWidth,
            currentMargin.Top,
            currentMargin.Right,
            currentMargin.Bottom);
    }

    private ScrollBar? GetHorizontalScrollBar()
    {
        if (_horizontalScrollBar is { } horizontalScrollBar &&
            TopLevel.GetTopLevel(horizontalScrollBar) == TopLevel.GetTopLevel(_editor))
        {
            return horizontalScrollBar;
        }

        _horizontalScrollBar = _editor.GetVisualDescendants()
                                      .OfType<ScrollBar>()
                                      .FirstOrDefault(scrollBar => scrollBar.Orientation == Orientation.Horizontal);
        return _horizontalScrollBar;
    }

    private double GetTextAreaLeftMarginsRightEdge()
    {
        if (!ShowLineNumbers || !_editor.ShowLineNumbers)
        {
            return 0;
        }

        var rightEdge = 0d;
        foreach (var margin in _editor.TextArea.LeftMargins)
        {
            var point = margin.TranslatePoint(new Point(margin.Bounds.Width, 0), _editor);
            if (point is { } value)
            {
                rightEdge = Math.Max(rightEdge, value.X);
            }
        }

        return rightEdge;
    }

    private void UpdateText()
    {
        _editor.Text = CodeText ?? string.Empty;
    }

    private void ConfigureEditorContextMenu()
    {
        _copyMenuItem.Click += HandleCopyMenuItemClick;
        _editorContextMenu.Opened += HandleEditorContextMenuOpened;
        _editorContextMenu.Items.Add(_copyMenuItem);
        _editor.ContextMenu = _editorContextMenu;
        UpdateCopyMenuItemState();
    }

    private void UpdateCopyMenuItemState()
    {
        _copyMenuItem.IsEnabled = !string.IsNullOrEmpty(GetSelectedCodeText());
    }

    private string GetSelectedCodeText()
    {
        return _editor.TextArea.Selection.IsEmpty
            ? string.Empty
            : _editor.TextArea.Selection.GetText();
    }

    private async Task CopySelectedCodeToClipboardAsync()
    {
        var selectedText = GetSelectedCodeText();
        if (string.IsNullOrEmpty(selectedText))
        {
            return;
        }

        var clipboard = TopLevel.GetTopLevel(_editor)?.Clipboard;
        if (clipboard is null)
        {
            return;
        }

        var item = new DataTransferItem();
        item.SetText(selectedText);

        var dataTransfer = new DataTransfer();
        dataTransfer.Add(item);
        await clipboard.SetDataAsync(dataTransfer);
    }

    private void EnsureTextMateInstalled()
    {
        if (_textMateInstallation is not null)
        {
            return;
        }

        var syntaxTheme = ResolveSyntaxTheme();
        _registryOptions = new RegistryOptions(syntaxTheme);
        _textMateInstallation = _editor.InstallTextMate(_registryOptions);
        _currentSyntaxTheme = syntaxTheme;
    }

    private void ApplySyntaxTheme()
    {
        if (_isDisposed)
        {
            return;
        }

        EnsureTextMateInstalled();
        if (_registryOptions is null || _textMateInstallation is null)
        {
            return;
        }

        var syntaxTheme = ResolveSyntaxTheme();
        if (_currentSyntaxTheme == syntaxTheme)
        {
            return;
        }

        _textMateInstallation.SetTheme(_registryOptions.LoadTheme(syntaxTheme));
        _currentSyntaxTheme = syntaxTheme;
    }

    private ThemeName ResolveSyntaxTheme()
    {
        return ResolveIsDarkSyntaxTheme()
            ? DarkSyntaxTheme
            : LightSyntaxTheme;
    }

    private bool ResolveIsDarkSyntaxTheme()
    {
        if (ActualThemeVariant == ThemeVariant.Dark)
        {
            return true;
        }

        if (ActualThemeVariant == ThemeVariant.Light)
        {
            return false;
        }

        return Application.Current?.IsDarkThemeMode() == true;
    }

    private void SubscribeThemeVariantChanges()
    {
        var application = Application.Current;
        var themeManager = application?.GetThemeManager();
        if (ReferenceEquals(_subscribedApplication, application))
        {
            if (ReferenceEquals(_subscribedThemeManager, themeManager))
            {
                return;
            }
        }

        ReleaseThemeVariantSubscriptions();
        _subscribedApplication = application;
        if (_subscribedApplication is not null)
        {
            _subscribedApplication.ActualThemeVariantChanged += HandleApplicationActualThemeVariantChanged;
        }

        _subscribedThemeManager = themeManager;
        _themeVariantSubscription = themeManager?.BindingSource
                                                .GetObservable(IThemeManager.ThemeVariantProperty)
                                                .Subscribe(HandleThemeManagerThemeVariantChanged);
    }

    private void ReleaseThemeVariantSubscriptions()
    {
        if (_subscribedApplication is not null)
        {
            _subscribedApplication.ActualThemeVariantChanged -= HandleApplicationActualThemeVariantChanged;
            _subscribedApplication = null;
        }

        _themeVariantSubscription?.Dispose();
        _themeVariantSubscription = null;
        _subscribedThemeManager = null;
    }

    private void ApplyGrammar()
    {
        EnsureTextMateInstalled();

        var scope = ResolveTextMateScope(Language);
        if (scope is not null)
        {
            _textMateInstallation?.SetGrammar(scope);
        }
    }

    private string? ResolveTextMateScope(string language)
    {
        var normalized = language.Trim().ToLowerInvariant();
        var extension = normalized switch
        {
            "axaml"  => ".xml",
            "xml"    => ".xml",
            "csharp" => ".cs",
            "cs"     => ".cs",
            _        => normalized.StartsWith(".") ? normalized : "." + normalized
        };

        return _registryOptions?.GetScopeByExtension(extension);
    }
}
