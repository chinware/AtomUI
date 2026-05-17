using System.Reactive.Disposables;
using AtomUI.Controls.Commons;
using AtomUI.Data;

namespace AtomUI.Desktop.Controls;

public class BackTopFloatButtonHost : AbstractBackTopFloatButtonHost
{
    protected override AbstractFloatButton CreateFloatButton(CompositeDisposable disposables)
    {
        var floatButton = new BackTopFloatButton();

        disposables.Add(BindUtils.RelayBind(this, IconProperty, floatButton, BackTopFloatButton.IconProperty));
        disposables.Add(BindUtils.RelayBind(this, TooltipProperty, floatButton, BackTopFloatButton.TooltipProperty));
        disposables.Add(BindUtils.RelayBind(this, TooltipColorProperty, floatButton, BackTopFloatButton.TooltipColorProperty));
        disposables.Add(BindUtils.RelayBind(this, ButtonTypeProperty, floatButton, BackTopFloatButton.ButtonTypeProperty));
        disposables.Add(BindUtils.RelayBind(this, ShapeProperty, floatButton, BackTopFloatButton.ShapeProperty));
        disposables.Add(BindUtils.RelayBind(this, HrefProperty, floatButton, BackTopFloatButton.HrefProperty));
        disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, floatButton, BackTopFloatButton.IsMotionEnabledProperty));
        disposables.Add(BindUtils.RelayBind(this, PlacementProperty, floatButton, BackTopFloatButton.PlacementProperty));
        disposables.Add(BindUtils.RelayBind(this, FloatOffsetXProperty, floatButton, BackTopFloatButton.FloatOffsetXProperty));
        disposables.Add(BindUtils.RelayBind(this, FloatOffsetYProperty, floatButton, BackTopFloatButton.FloatOffsetYProperty));
        disposables.Add(BindUtils.RelayBind(this, ToTopDurationProperty, floatButton, BackTopFloatButton.ToTopDurationProperty));
        disposables.Add(BindUtils.RelayBind(this, TargetProperty, floatButton, BackTopFloatButton.TargetProperty));
        disposables.Add(BindUtils.RelayBind(this, VisibilityHeightProperty, floatButton, BackTopFloatButton.VisibilityHeightProperty));
        disposables.Add(BindUtils.RelayBind(this, MotionDurationProperty, floatButton, BackTopFloatButton.MotionDurationProperty));
        
        return floatButton;
    }
}
