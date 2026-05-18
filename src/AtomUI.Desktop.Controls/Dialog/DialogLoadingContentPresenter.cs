using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

internal class DialogLoadingContentPresenter : ContentControl
{
    public static readonly StyledProperty<bool> IsLoadingProperty =
        Dialog.IsLoadingProperty.AddOwner<DialogLoadingContentPresenter>();

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    private Panel? _rootLayout;
    private Skeleton? _skeleton;

    public DialogLoadingContentPresenter()
    {
        this.RegisterTokenResourceScope(DialogToken.ScopeProvider);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ReleaseSkeleton();
        _rootLayout = e.NameScope.Find<Panel>("PART_RootLayout");
        ConfigureSkeleton();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ConfigureSkeleton();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ReleaseSkeleton();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsLoadingProperty)
        {
            ConfigureSkeleton();
        }
    }

    private void ConfigureSkeleton()
    {
        if (IsLoading)
        {
            EnsureSkeleton();
        }
        else
        {
            ReleaseSkeleton();
        }
    }

    private void EnsureSkeleton()
    {
        if (_rootLayout is null || _skeleton != null)
        {
            return;
        }

        _skeleton = new Skeleton
        {
            IsLoading    = true,
            IsActive     = true,
            ParagraphRows = 4,
            IsShowTitle  = false,
            ClipToBounds = false
        };
        _rootLayout.Children.Insert(0, _skeleton);
    }

    private void ReleaseSkeleton()
    {
        if (_skeleton is null)
        {
            return;
        }

        if (_skeleton.Parent is Panel parent)
        {
            parent.Children.Remove(_skeleton);
        }
        _skeleton = null;
    }
}
