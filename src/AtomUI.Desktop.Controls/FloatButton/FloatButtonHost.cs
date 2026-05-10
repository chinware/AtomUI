using System.Reactive.Disposables;
using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class FloatButtonHost : AbstractFloatButtonHost
{
    public FloatButtonHost()
    {
        this.RegisterTokenResourceScope(FloatButtonToken.ScopeProvider);
    }
    
    protected override AbstractFloatButton CreateFloatButton(CompositeDisposable disposables)
    {
        var floatButton = new FloatButton();

        floatButton[!AbstractFloatButton.IconProperty]               = this[!IconProperty];
        floatButton[!AbstractFloatButton.TooltipProperty]            = this[!TooltipProperty];
        floatButton[!AbstractFloatButton.TooltipColorProperty]       = this[!TooltipColorProperty];
        floatButton[!AbstractFloatButton.ButtonTypeProperty]         = this[!ButtonTypeProperty];
        floatButton[!AbstractFloatButton.ShapeProperty]              = this[!ShapeProperty];
        floatButton[!AbstractFloatButton.HrefProperty]               = this[!HrefProperty];
        floatButton[!AbstractFloatButton.IsMotionEnabledProperty]    = this[!IsMotionEnabledProperty];
        floatButton[!AbstractFloatButton.PlacementProperty]          = this[!PlacementProperty];
        floatButton[!AbstractFloatButton.FloatOffsetXProperty]       = this[!FloatOffsetXProperty];
        floatButton[!AbstractFloatButton.FloatOffsetYProperty]       = this[!FloatOffsetYProperty];
        floatButton[!AbstractFloatButton.ContentProperty]            = this[!DescriptionProperty];
        floatButton[!AbstractFloatButton.ContentTemplateProperty]    = this[!DescriptionTemplateProperty];
        floatButton[!AbstractFloatButton.IsBadgeEnabledProperty]     = this[!IsBadgeEnabledProperty];
        floatButton[!AbstractFloatButton.IsDotBadgeProperty]         = this[!IsDotBadgeProperty];
        floatButton[!AbstractFloatButton.BadgeCountProperty]         = this[!BadgeCountProperty];
        floatButton[!AbstractFloatButton.BadgeColorProperty]         = this[!BadgeColorProperty];
        floatButton[!AbstractFloatButton.BadgeOffsetProperty]        = this[!BadgeOffsetProperty];
        floatButton[!AbstractFloatButton.BadgeOverflowCountProperty] = this[!BadgeOverflowCountProperty];
        return floatButton;
    }
}