using AtomUI.Controls;
using AtomUI.Desktop.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AtomUI.Desktop.Controls;

internal class QuickJumpArgs
{
    public QuickJumpArgs(int pageNumber)
    {
        PageNumber = pageNumber;
    }
    public int PageNumber { get; set; }
}

internal class QuickJumperBar : TemplatedControl
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
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<QuickJumperBar>();
    
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
    
    private LineEdit? _lineEdit;
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _lineEdit = e.NameScope.Find<LineEdit>(PaginationQuickJumperBarThemeConstants.PageLineEditPart);

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
}