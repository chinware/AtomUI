using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

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
        ClearLineEditPart();
        _lineEdit = e.NameScope.Find<LineEdit>("PART_PageLineEdit");

        if (_lineEdit != null)
        {
            _lineEdit.KeyUp -= HandleLineEditKeyUp;
            _lineEdit.KeyUp += HandleLineEditKeyUp;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ClearLineEditPart();
    }

    private void ClearLineEditPart()
    {
        if (_lineEdit is not null)
        {
            _lineEdit.KeyUp -= HandleLineEditKeyUp;
            _lineEdit = null;
        }
    }

    private void HandleLineEditKeyUp(object? sender, KeyEventArgs e)
    {
        if (sender is LineEdit lineEdit)
        {
            if (e.Key == Key.Enter)
            {
                if (lineEdit.Text is { } text && int.TryParse(text.AsSpan().Trim(), out var pageNumber))
                {
                    JumpRequest?.Invoke(this, new QuickJumpArgs(pageNumber));
                }
                lineEdit.Clear();
            }
        }
    }
}
