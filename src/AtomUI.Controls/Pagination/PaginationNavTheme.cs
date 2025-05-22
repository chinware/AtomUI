using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class PaginationNavTheme : BaseControlTheme
{
    public const string FramePart = "PART_Frame";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    
    public PaginationNavTheme()
        : base(typeof(PaginationNav))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<PaginationNav>((paginationNav, scope) =>
        {
            var frame = new Border
            {
                Name         = FramePart,
                ClipToBounds = true
            };
            var itemsPresenter = new ItemsPresenter
            {
                Name = ItemsPresenterPart,
                ItemsPanel = new FuncTemplate<Panel?>(() =>
                {
                    var panel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal
                    };
                    paginationNav.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(panel, StackPanel.SpacingProperty, SharedTokenKey.MarginXS));
                    return panel;
                })
            };
            itemsPresenter.RegisterInNameScope(scope);
            frame.Child = itemsPresenter;
            
            CreateTemplateParentBinding(frame, Border.CornerRadiusProperty,
                TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(frame, Decorator.PaddingProperty, TemplatedControl.PaddingProperty);
            return frame;
        });
    }
}