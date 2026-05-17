using AtomUI.Controls.Commons;
using Avalonia;

namespace AtomUI.Desktop.Controls;

using ToolTipControl = AtomUI.Desktop.Controls.ToolTip;

public class BackTopFloatButton : AbstractBackTopFloatButton
{
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TooltipProperty)
        {
            ToolTipControl.SetTip(this, Tooltip);
        }
        else if (change.Property == TooltipColorProperty)
        {
            ToolTipControl.SetColor(this, TooltipColor);
        }
    }
    
    private protected override void ConfigureBadge()
    {
        if (!IsBadgeEnabled)
        {
            ClearBadge();
            return;
        }

        var badgeLayout = EnsureBadgeLayout();
        if (badgeLayout == null)
        {
            return;
        }

        if (IsDotBadge)
        {
            if (_badge is DotBadgeAdorner)
            {
                CalculateBadgePosition();
                return;
            }

            var dotBadge = new DotBadgeAdorner();
            dotBadge[!DotBadgeAdorner.BadgeDotColorProperty]   = this[!BadgeEffectiveColorProperty];
            dotBadge[!DotBadgeAdorner.IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
            AttachBadge(dotBadge, badgeLayout);
        }
        else
        {
            if (_badge is CountBadgeAdorner)
            {
                CalculateBadgePosition();
                return;
            }

            var countBadge = new CountBadgeAdorner();
            countBadge[!CountBadgeAdorner.BadgeColorProperty]      = this[!BadgeEffectiveColorProperty];
            countBadge[!CountBadgeAdorner.IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
            countBadge[!CountBadgeAdorner.CountProperty]           = this[!BadgeCountProperty];
            countBadge[!CountBadgeAdorner.OverflowCountProperty]   = this[!BadgeOverflowCountProperty];
            AttachBadge(countBadge, badgeLayout);
        }
    }
}
