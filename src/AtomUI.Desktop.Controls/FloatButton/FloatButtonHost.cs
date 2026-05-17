using System.Reactive.Disposables;
using AtomUI.Controls.Commons;
using AtomUI.Data;
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

        disposables.Add(BindUtils.RelayBind(this, IconProperty, floatButton, AbstractFloatButton.IconProperty));
        disposables.Add(BindUtils.RelayBind(this, TooltipProperty, floatButton, AbstractFloatButton.TooltipProperty));
        disposables.Add(BindUtils.RelayBind(this, TooltipColorProperty, floatButton, AbstractFloatButton.TooltipColorProperty));
        disposables.Add(BindUtils.RelayBind(this, ButtonTypeProperty, floatButton, AbstractFloatButton.ButtonTypeProperty));
        disposables.Add(BindUtils.RelayBind(this, ShapeProperty, floatButton, AbstractFloatButton.ShapeProperty));
        disposables.Add(BindUtils.RelayBind(this, HrefProperty, floatButton, AbstractFloatButton.HrefProperty));
        disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, floatButton, AbstractFloatButton.IsMotionEnabledProperty));
        disposables.Add(BindUtils.RelayBind(this, PlacementProperty, floatButton, AbstractFloatButton.PlacementProperty));
        disposables.Add(BindUtils.RelayBind(this, FloatOffsetXProperty, floatButton, AbstractFloatButton.FloatOffsetXProperty));
        disposables.Add(BindUtils.RelayBind(this, FloatOffsetYProperty, floatButton, AbstractFloatButton.FloatOffsetYProperty));
        disposables.Add(BindUtils.RelayBind(this, DescriptionProperty, floatButton, AbstractFloatButton.ContentProperty));
        disposables.Add(BindUtils.RelayBind(this, DescriptionTemplateProperty, floatButton, AbstractFloatButton.ContentTemplateProperty));
        disposables.Add(BindUtils.RelayBind(this, IsBadgeEnabledProperty, floatButton, AbstractFloatButton.IsBadgeEnabledProperty));
        disposables.Add(BindUtils.RelayBind(this, IsDotBadgeProperty, floatButton, AbstractFloatButton.IsDotBadgeProperty));
        disposables.Add(BindUtils.RelayBind(this, BadgeCountProperty, floatButton, AbstractFloatButton.BadgeCountProperty));
        disposables.Add(BindUtils.RelayBind(this, BadgeColorProperty, floatButton, AbstractFloatButton.BadgeColorProperty));
        disposables.Add(BindUtils.RelayBind(this, BadgeOffsetProperty, floatButton, AbstractFloatButton.BadgeOffsetProperty));
        disposables.Add(BindUtils.RelayBind(this, BadgeOverflowCountProperty, floatButton, AbstractFloatButton.BadgeOverflowCountProperty));
        return floatButton;
    }
}
