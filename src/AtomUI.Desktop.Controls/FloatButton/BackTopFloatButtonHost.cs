using System.Reactive.Disposables;
using AtomUI.Controls.Commons;

namespace AtomUI.Desktop.Controls;

public class BackTopFloatButtonHost : AbstractBackTopFloatButtonHost
{
    protected override AbstractFloatButton NotifyCreateFloatButton(CompositeDisposable disposables)
    {
        var floatButton = new BackTopFloatButton();
        
        floatButton[!BackTopFloatButton.IconProperty]             = this[!IconProperty];
        floatButton[!BackTopFloatButton.TooltipProperty]          = this[!TooltipProperty];
        floatButton[!BackTopFloatButton.TooltipColorProperty]     = this[!TooltipColorProperty];
        floatButton[!BackTopFloatButton.ButtonTypeProperty]       = this[!ButtonTypeProperty];
        floatButton[!BackTopFloatButton.ShapeProperty]            = this[!ShapeProperty];
        floatButton[!BackTopFloatButton.HrefProperty]             = this[!HrefProperty];
        floatButton[!BackTopFloatButton.IsMotionEnabledProperty]  = this[!IsMotionEnabledProperty];
        floatButton[!BackTopFloatButton.PlacementProperty]        = this[!PlacementProperty];
        floatButton[!BackTopFloatButton.FloatOffsetXProperty]     = this[!FloatOffsetXProperty];
        floatButton[!BackTopFloatButton.FloatOffsetYProperty]     = this[!FloatOffsetYProperty];
        floatButton[!BackTopFloatButton.ToTopDurationProperty]    = this[!ToTopDurationProperty];
        floatButton[!BackTopFloatButton.TargetProperty]           = this[!TargetProperty];
        floatButton[!BackTopFloatButton.VisibilityHeightProperty] = this[!VisibilityHeightProperty];
        floatButton[!BackTopFloatButton.MotionDurationProperty]   = this[!MotionDurationProperty];
        
        return floatButton;
    }
}