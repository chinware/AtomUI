using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using AvaloniaControlList = Avalonia.Controls.Controls;

namespace AtomUI.Demo.Desktop;

public class ShowCasePanel : Control
{
    private bool _initialized;
    private StackPanel _leftContainer = default!;
    private StackPanel _rightContainer = default!;

    public string? Id { get; protected set; }

    [Content] public AvaloniaControlList Children { get; } = new();

    protected void SetupUi()
    {
        var mainLayout = new UniformGrid
        {
            Rows    = 1,
            Columns = 2,
            Margin  = new Thickness(0)
        };
        _leftContainer = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 10,
            Margin      = new Thickness(0, 0, 10, 0)
        };
        _rightContainer = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 10
        };
        mainLayout.Children.Add(_leftContainer);
        mainLayout.Children.Add(_rightContainer);

        for (var i = 0; i < Children.Count; ++i)
        {
            var control = Children[i];
            if (i % 2 == 0)
            {
                _leftContainer.Children.Add(control);
            }
            else
            {
                _rightContainer.Children.Add(control);
            }
        }

        var scrollView = new ScrollViewer
        {
            Content = mainLayout
        };
        LogicalChildren.Add(scrollView);
        VisualChildren.Add(scrollView);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        if (!_initialized)
        {
            SetupUi();
            NotifyShowCaseLayoutReady();
            _initialized = true;
        }

        base.OnAttachedToLogicalTree(e);
    }

    internal virtual void NotifyAboutToActive()
    {
    }

    internal virtual void NotifyActivated()
    {
    }

    internal virtual void NotifyAboutToDeactivated()
    {
    }

    internal virtual void NotifyDeactivated()
    {
    }

    protected virtual void NotifyShowCaseLayoutReady()
    {
    }
}