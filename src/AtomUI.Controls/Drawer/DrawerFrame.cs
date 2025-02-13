using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public abstract class DrawerFrame : TemplatedControl
{
    [Content]
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
    public static readonly StyledProperty<object?> ContentProperty = AvaloniaProperty
        .Register<DrawerFrame, object?>(nameof(Content));

    public IDataTemplate? ContentTemplate
    {
        get => GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }
    public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty = AvaloniaProperty
        .Register<DrawerFrame, IDataTemplate?>(nameof(ContentTemplate));

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
    public static readonly StyledProperty<object?> HeaderProperty = AvaloniaProperty
        .Register<DrawerFrame, object?>(nameof(Header));

    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }
    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty = AvaloniaProperty
        .Register<DrawerFrame, IDataTemplate?>(nameof(HeaderTemplate));

    public ICommand? ConfirmCommand
    {
        get => GetValue(ConfirmCommandProperty);
        set => SetValue(ConfirmCommandProperty, value);
    }
    public static readonly StyledProperty<ICommand?> ConfirmCommandProperty = AvaloniaProperty
        .Register<DrawerFrame, ICommand?>(nameof(ConfirmCommand));

    protected DrawerFrame()
    {
        var header = new ContentControl()
        {
            [!ContentControl.ContentProperty]         = this[!HeaderProperty],
            [!ContentControl.ContentTemplateProperty] = this[!HeaderTemplateProperty],
            FontWeight                                = FontWeight.Medium,
            Margin                                    = new Thickness(16),
            VerticalAlignment                         = VerticalAlignment.Center,
            FontSize                                  = 16,
        };
        Grid.SetColumn(header, 0);

        var buttons = BuildButtons();
        var stackPanel = new StackPanel()
        {
            Orientation         = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing             = 16,
            Margin              = new Thickness(16),
        };
        foreach (var button in buttons)
        {
            stackPanel.Children.Add(button);
        }
        Grid.SetColumn(stackPanel, 1);
        
        var grid1 = new Grid()
        {
            Children = { header ,stackPanel },
            VerticalAlignment = VerticalAlignment.Top,
        };
        grid1.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        grid1.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        
        var border = new Border()
        {
            BorderThickness = new Thickness(0, 0, 0, 0.5),
            BorderBrush     = Brushes.LightGray,
            Child           = grid1,
        };
        Grid.SetRow(border, 0);

        var content = new ContentControl()
        {
            [!ContentControl.ContentProperty]         = this[!ContentProperty],
            [!ContentControl.ContentTemplateProperty] = this[!ContentTemplateProperty],
        };
        Grid.SetRow(content, 1);

        var grid2 = new Grid()
        {
            Children = { border, content },
            MinWidth = 378,
        };
        grid2.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        grid2.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        
        VisualChildren.Add(grid2);
        ((ISetLogicalParent)grid2).SetParent(this);
    }

    protected abstract IEnumerable<Control> BuildButtons();
}

public class ConfirmDrawerFrame : DrawerFrame
{
    protected override IEnumerable<Control> BuildButtons()
    {
        yield return new Button()
        {
            Content = "Cancel",
            Command = new CloseDrawerCommand(this),
        };
        yield return new Button()
        {
            Content                                     = "Ok",
            ButtonType                                  = ButtonType.Primary,
            [!Avalonia.Controls.Button.CommandProperty] = this[!ConfirmCommandProperty]
        };
    }
}

public class SubmitDrawerFrame : DrawerFrame
{
    protected override IEnumerable<Control> BuildButtons()
    {
        yield return new Button()
        {
            Content = "Cancel",
            Command = new CloseDrawerCommand(this),
        };
        yield return new Button()
        {
            Content                                     = "Submit",
            ButtonType                                  = ButtonType.Primary,
            [!Avalonia.Controls.Button.CommandProperty] = this[!ConfirmCommandProperty]
        };
    }
}