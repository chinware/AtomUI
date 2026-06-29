using System.Diagnostics;
using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.TextMate;
using TextMateSharp.Model;
using TextMateSharp.Grammars;
using TextMateSharp.Registry;
using TextMateSharp.Themes;
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

    private GalleryTextMateInstallation? _textMateInstallation;
    private RegistryOptions? _registryOptions;
    private ScrollBar? _horizontalScrollBar;
    private Application? _subscribedApplication;
    private IThemeManager? _subscribedThemeManager;
    private IDisposable? _themeVariantSubscription;
    private ThemeName? _currentSyntaxTheme;
    private bool _isHorizontalScrollBarInsetUpdatePending;
    private bool _isUpdatingScrollBarInset;
    private bool _isDisposed;

    public GalleryCodeViewer()
    {
        InitializeComponent();
        _editor = this.FindControl<TextEditor>("PART_Editor")!;
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
        CancelHorizontalScrollBarGutterInsetUpdate();
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
            RequestHorizontalScrollBarGutterInsetUpdate();
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
        RequestHorizontalScrollBarGutterInsetUpdate();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        CancelHorizontalScrollBarGutterInsetUpdate();
        ReleaseThemeVariantSubscriptions();
        base.OnDetachedFromVisualTree(e);
    }

    private void HandleEditorLayoutUpdated(object? sender, EventArgs e)
    {
        CancelHorizontalScrollBarGutterInsetUpdate();
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

    private void RequestHorizontalScrollBarGutterInsetUpdate()
    {
        if (_isDisposed || _isHorizontalScrollBarInsetUpdatePending)
        {
            return;
        }

        _isHorizontalScrollBarInsetUpdatePending = true;
        _editor.LayoutUpdated += HandleEditorLayoutUpdated;
    }

    private void CancelHorizontalScrollBarGutterInsetUpdate()
    {
        if (!_isHorizontalScrollBarInsetUpdatePending)
        {
            return;
        }

        _isHorizontalScrollBarInsetUpdatePending = false;
        _editor.LayoutUpdated -= HandleEditorLayoutUpdated;
    }

    private void UpdateHorizontalScrollBarGutterInset()
    {
        // Mutating the scroll bar Margin triggers another layout pass, so this method must only
        // run from the one-shot request path above. Drag selection auto-scroll produces repeated
        // LayoutUpdated ticks, and a permanent handler can spin the UI thread.
        if (_isUpdatingScrollBarInset)
        {
            return;
        }

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

        _isUpdatingScrollBarInset = true;
        try
        {
            horizontalScrollBar.Margin = new Thickness(
                gutterWidth,
                currentMargin.Top,
                currentMargin.Right,
                currentMargin.Bottom);
        }
        finally
        {
            _isUpdatingScrollBarInset = false;
        }
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

        // Sum the left margins' own widths instead of projecting a point through TranslatePoint.
        // The left gutter is fixed and does not scroll, but TranslatePoint's result shifts with
        // the horizontal scroll offset and with mid-layout bounds, so the computed value jitters
        // during a drag-scroll and never converges against the 0.5 tolerance above.
        var width = 0d;
        foreach (var margin in _editor.TextArea.LeftMargins)
        {
            width += margin.Bounds.Width;
        }

        return width;
    }

    private void UpdateText()
    {
        _editor.Text = CodeText ?? string.Empty;
        RequestHorizontalScrollBarGutterInsetUpdate();
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
        _copyMenuItem.IsEnabled = !_editor.TextArea.Selection.IsEmpty;
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
        _textMateInstallation = new GalleryTextMateInstallation(_editor, _registryOptions);
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

    private sealed class GalleryTextMateInstallation : IDisposable
    {
        private readonly object _lock = new();
        private readonly Registry _textMateRegistry;
        private readonly TextEditor _editor;
        private readonly TextMateColoringTransformer _transformer;
        private readonly bool _ownsTransformer;
        private Action<Exception>? _exceptionHandler;
        private GalleryTextEditorModel? _editorModel;
        private IGrammar? _grammar;
        private TMModel? _tmModel;
        private ReadOnlyDictionary<string, string>? _themeColorsDictionary;
        private bool _isDisposed;

        public GalleryTextMateInstallation(TextEditor editor,
                                           IRegistryOptions registryOptions,
                                           Action<Exception>? exceptionHandler = null)
        {
            RegistryOptions = registryOptions ?? throw new ArgumentNullException(nameof(registryOptions));
            _editor = editor ?? throw new ArgumentNullException(nameof(editor));
            _exceptionHandler = exceptionHandler;
            _textMateRegistry = new Registry(registryOptions);
            _transformer = _editor.TextArea.TextView.LineTransformers
                                  .OfType<TextMateColoringTransformer>()
                                  .FirstOrDefault() ??
                           new TextMateColoringTransformer(_editor.TextArea.TextView, _exceptionHandler);

            if (!_editor.TextArea.TextView.LineTransformers.Contains(_transformer))
            {
                _editor.TextArea.TextView.LineTransformers.Add(_transformer);
                _ownsTransformer = true;
            }

            SetTheme(registryOptions.GetDefaultTheme());
            _editor.DocumentChanged += HandleEditorDocumentChanged;
            HandleEditorDocumentChanged(_editor, EventArgs.Empty);
        }

        public IRegistryOptions RegistryOptions { get; }

        public void SetGrammar(string scopeName)
        {
            ThrowIfDisposed();
            lock (_lock)
            {
                ThrowIfDisposed();
                SetGrammarInternal(_textMateRegistry.LoadGrammar(scopeName));
                ForceTokenizeDocument();
            }
            _editor.TextArea.TextView.Redraw();
        }

        public bool TryGetThemeColor(string colorKey, out string? colorString)
        {
            ThrowIfDisposed();
            return (_themeColorsDictionary ?? throw new ObjectDisposedException(nameof(GalleryTextMateInstallation)))
                .TryGetValue(colorKey, out colorString);
        }

        public void SetTheme(IRawTheme theme)
        {
            ThrowIfDisposed();
            lock (_lock)
            {
                ThrowIfDisposed();
                _textMateRegistry.SetTheme(theme);
                var textMateTheme = _textMateRegistry.GetTheme();
                _transformer.SetTheme(textMateTheme);
                _tmModel?.InvalidateLine(0);
                _editorModel?.InvalidateViewPortLines();
                _themeColorsDictionary = textMateTheme.GetGuiColorDictionary();
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            GalleryTextEditorModel? editorModel;
            TMModel? tmModel;
            lock (_lock)
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
                editorModel = _editorModel;
                _editorModel = null;
                tmModel = _tmModel;
                _tmModel = null;
                _grammar = null;
                _themeColorsDictionary = null;
                _exceptionHandler = null;
            }

            _editor.DocumentChanged -= HandleEditorDocumentChanged;
            editorModel?.Dispose();
            DisposeTMModel(tmModel);

            if (_ownsTransformer)
            {
                _editor.TextArea.TextView.LineTransformers.Remove(_transformer);
                _transformer.Dispose();
            }
            else
            {
                _transformer.SetModel(null, null);
            }
        }

        private void HandleEditorDocumentChanged(object? sender, EventArgs e)
        {
            if (_isDisposed || _editor.Document is null)
            {
                return;
            }

            lock (_lock)
            {
                if (_isDisposed || _editor.Document is null)
                {
                    return;
                }

                try
                {
                    _editorModel?.Dispose();
                    DisposeTMModel(_tmModel);
                    _editorModel = new GalleryTextEditorModel(
                        _editor.TextArea.TextView,
                        _editor.Document,
                        _exceptionHandler);
                    _tmModel = new TMModel(_editorModel);
                    _tmModel.SetGrammar(_grammar);
                    _transformer.SetModel(_editor.Document, _tmModel);
                    _tmModel.AddModelTokensChangedListener(_transformer);
                    ForceTokenizeDocument();
                }
                catch (Exception ex)
                {
                    _exceptionHandler?.Invoke(ex);
                }
            }
        }

        private void SetGrammarInternal(IGrammar grammar)
        {
            _grammar = grammar;
            _transformer.SetGrammar(_grammar);
        }

        private void ForceTokenizeDocument()
        {
            _editorModel?.ForceTokenizeAllLines();
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(GalleryTextMateInstallation));
            }
        }

        private void DisposeTMModel(TMModel? tmModel)
        {
            if (tmModel is null)
            {
                return;
            }

            tmModel.RemoveModelTokensChangedListener(_transformer);
            tmModel.Dispose();
        }
    }

    private sealed class GalleryTextEditorModel : AbstractLineList, IDisposable
    {
        private readonly TextDocument _document;
        private readonly AvaloniaEdit.Rendering.TextView _textView;
        private readonly Action<Exception>? _exceptionHandler;
        private DocumentSnapshot _documentSnapshot;
        private InvalidLineRange? _invalidRange;
        private bool _isViewportTokenizationPending;
        private int _pendingViewportStartLine = -1;
        private int _pendingViewportEndLine = -1;
        private bool _isDisposed;

        public GalleryTextEditorModel(AvaloniaEdit.Rendering.TextView textView,
                                      TextDocument document,
                                      Action<Exception>? exceptionHandler)
        {
            _textView = textView;
            _document = document;
            _exceptionHandler = exceptionHandler;
            _documentSnapshot = new DocumentSnapshot(_document);
            for (var i = 0; i < _document.LineCount; i++)
            {
                AddLine(i);
            }

            _document.Changing += HandleDocumentChanging;
            _document.Changed += HandleDocumentChanged;
            _document.UpdateFinished += HandleDocumentUpdateFinished;
            _textView.ScrollOffsetChanged += HandleTextViewScrollOffsetChanged;
        }

        public override void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _document.Changing -= HandleDocumentChanging;
            _document.Changed -= HandleDocumentChanged;
            _document.UpdateFinished -= HandleDocumentUpdateFinished;
            _textView.ScrollOffsetChanged -= HandleTextViewScrollOffsetChanged;
        }

        public override void UpdateLine(int lineIndex)
        {
        }

        public override int GetNumberOfLines()
        {
            return _documentSnapshot.LineCount;
        }

        public override LineText GetLineTextIncludingTerminators(int lineIndex)
        {
            return _documentSnapshot.GetLineTextIncludingTerminatorAsMemory(lineIndex);
        }

        public override int GetLineLength(int lineIndex)
        {
            return _documentSnapshot.GetLineLength(lineIndex);
        }

        public void InvalidateViewPortLines()
        {
            if (_textView.VisualLinesValid && _textView.VisualLines.Count != 0)
            {
                InvalidateLineRange(GetFirstVisibleLineIndex(), GetLastVisibleLineIndex());
            }
        }

        public void ForceTokenizeAllLines()
        {
            var lineCount = _documentSnapshot.LineCount;
            if (lineCount > 0)
            {
                ForceTokenization(0, lineCount - 1);
            }
        }

        private void HandleTextViewScrollOffsetChanged(object? sender, EventArgs e)
        {
            try
            {
                RequestViewportTokenization();
            }
            catch (Exception ex)
            {
                _exceptionHandler?.Invoke(ex);
            }
        }

        private void RequestViewportTokenization()
        {
            if (_isDisposed || !_textView.VisualLinesValid || _textView.VisualLines.Count == 0)
            {
                return;
            }

            var startLine = GetFirstVisibleLineIndex();
            var endLine = GetLastVisibleLineIndex();
            if (_pendingViewportStartLine < 0)
            {
                _pendingViewportStartLine = startLine;
                _pendingViewportEndLine = endLine;
            }
            else
            {
                _pendingViewportStartLine = Math.Min(_pendingViewportStartLine, startLine);
                _pendingViewportEndLine = Math.Max(_pendingViewportEndLine, endLine);
            }

            if (_isViewportTokenizationPending)
            {
                return;
            }

            _isViewportTokenizationPending = true;
            Avalonia.Threading.Dispatcher.UIThread.Post(
                ProcessPendingViewportTokenization,
                DispatcherPriority.Background);
        }

        private void ProcessPendingViewportTokenization()
        {
            if (_isDisposed)
            {
                return;
            }

            var startLine = _pendingViewportStartLine;
            var endLine = _pendingViewportEndLine;
            _pendingViewportStartLine = -1;
            _pendingViewportEndLine = -1;
            _isViewportTokenizationPending = false;
            if (startLine >= 0 && endLine >= startLine)
            {
                ForceTokenization(startLine, endLine);
            }
        }

        private void HandleDocumentChanging(object? sender, DocumentChangeEventArgs e)
        {
            try
            {
                if (e.RemovalLength <= 0)
                {
                    return;
                }

                var startLine = _document.GetLineByOffset(e.Offset).LineNumber - 1;
                var endLine = _document.GetLineByOffset(e.Offset + e.RemovalLength).LineNumber - 1;
                for (var line = endLine; line > startLine; line--)
                {
                    RemoveLine(line);
                }
                _documentSnapshot.RemoveLines(startLine, endLine);
            }
            catch (Exception ex)
            {
                _exceptionHandler?.Invoke(ex);
            }
        }

        private void HandleDocumentChanged(object? sender, DocumentChangeEventArgs e)
        {
            try
            {
                var startLine = _document.GetLineByOffset(e.Offset).LineNumber - 1;
                var endLine = startLine;
                if (e.InsertionLength > 0)
                {
                    endLine = _document.GetLineByOffset(e.Offset + e.InsertionLength).LineNumber - 1;
                    for (var line = startLine; line < endLine; line++)
                    {
                        AddLine(line);
                    }
                }

                _documentSnapshot.Update(e);
                SetInvalidRange(startLine == 0 ? startLine : startLine - 1, endLine);
            }
            catch (Exception ex)
            {
                _exceptionHandler?.Invoke(ex);
            }
        }

        private void SetInvalidRange(int startLine, int endLine)
        {
            if (!_document.IsInUpdate)
            {
                InvalidateLineRange(startLine, endLine);
            }
            else if (_invalidRange is null)
            {
                _invalidRange = new InvalidLineRange(startLine, endLine);
            }
            else
            {
                _invalidRange.SetInvalidRange(startLine, endLine);
            }
        }

        private void HandleDocumentUpdateFinished(object? sender, EventArgs e)
        {
            if (_invalidRange is null)
            {
                return;
            }

            try
            {
                var startLine = Math.Clamp(_invalidRange.StartLine, 0, _documentSnapshot.LineCount - 1);
                var endLine = Math.Clamp(_invalidRange.EndLine, 0, _documentSnapshot.LineCount - 1);
                InvalidateLineRange(startLine, endLine);
            }
            finally
            {
                _invalidRange = null;
            }
        }

        private int GetFirstVisibleLineIndex()
        {
            return _textView.VisualLines[0].FirstDocumentLine.LineNumber - 1;
        }

        private int GetLastVisibleLineIndex()
        {
            return _textView.VisualLines[^1].LastDocumentLine.LineNumber - 1;
        }

        private sealed class InvalidLineRange
        {
            public InvalidLineRange(int startLine, int endLine)
            {
                StartLine = startLine;
                EndLine = endLine;
            }

            public int StartLine { get; private set; }

            public int EndLine { get; private set; }

            public void SetInvalidRange(int startLine, int endLine)
            {
                StartLine = Math.Min(StartLine, startLine);
                EndLine = Math.Max(EndLine, endLine);
            }
        }
    }
}
