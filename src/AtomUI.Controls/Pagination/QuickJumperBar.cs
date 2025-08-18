using System.Reactive.Disposables;
using AtomUI.Controls.PaginationLang;
using AtomUI.Controls.Themes;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

internal class QuickJumpArgs
{
    public QuickJumpArgs(int pageNumber)
    {
        PageNumber = pageNumber;
    }
    public int PageNumber { get; set; }
}

internal class QuickJumperBar : TemplatedControl, IResourceBindingManager
{
    public event EventHandler<QuickJumpArgs>? JumpRequest;
    
    public static readonly DirectProperty<QuickJumperBar, string?> JumpToTextProperty =
        AvaloniaProperty.RegisterDirect<QuickJumperBar,  string?>(nameof(JumpToText),
            o => o.JumpToText,
            (o, v) => o.JumpToText = v);
    
    public static readonly DirectProperty<QuickJumperBar, string?> PageTextProperty =
        AvaloniaProperty.RegisterDirect<QuickJumperBar,  string?>(nameof(PageText),
            o => o.PageText,
            (o, v) => o.PageText = v);
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<QuickJumperBar>();
    
    private string? _jumpToText;
    public string? JumpToText
    {
        get => _jumpToText;
        set => SetAndRaise(JumpToTextProperty, ref _jumpToText, value);
    }
    
    private string? _pageText;
    public string? PageText
    {
        get => _pageText;
        set => SetAndRaise(PageTextProperty, ref _pageText, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable
    {
        get => _resourceBindingsDisposable;
        set => _resourceBindingsDisposable = value;
    }
    private CompositeDisposable? _resourceBindingsDisposable;
    
    private LineEdit? _lineEdit;
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _lineEdit = e.NameScope.Find<LineEdit>(PaginationQuickJumperBarThemeConstants.PageLineEditPart);
        this.AddResourceBindingDisposable(LanguageResourceBinder.CreateBinding(this, JumpToTextProperty, PaginationLangResourceKey.JumpToText));
        this.AddResourceBindingDisposable(LanguageResourceBinder.CreateBinding(this, PageTextProperty, PaginationLangResourceKey.PageText));

        if (_lineEdit != null)
        {
            _lineEdit.KeyUp += HandleLineEditKeyUp;
        }
    }

    private void HandleLineEditKeyUp(object? sender, KeyEventArgs e)
    {
        if (sender is LineEdit lineEdit)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(lineEdit.Text?.Trim(), out var pageNumber))
                {
                    JumpRequest?.Invoke(this, new QuickJumpArgs(pageNumber));
                }
                lineEdit.Clear();
            }
        }
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
}