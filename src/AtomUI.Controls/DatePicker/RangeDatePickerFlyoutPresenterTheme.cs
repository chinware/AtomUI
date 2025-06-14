using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class RangeDatePickerFlyoutPresenterTheme : ArrowDecoratedBoxTheme
{
    public const string PickerPresenterContentPart = "PART_PickerPresenterContent";
    
    public RangeDatePickerFlyoutPresenterTheme()
        : this(typeof(RangeDatePickerFlyoutPresenter))
    {
    }
    
    protected RangeDatePickerFlyoutPresenterTheme(Type targetType) : base(targetType)
    {
    }

    protected override Control BuildContent(INameScope scope)
    {
        var datePickerPresenter = new RangeDatePickerPresenter()
        {
            Name = PickerPresenterContentPart,
        };
        datePickerPresenter.RegisterInNameScope(scope);
        return datePickerPresenter;
    }
}