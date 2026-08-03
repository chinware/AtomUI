using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

[PseudoClasses(LunarCalendarCellPseudoClass.Weekend, LunarCalendarCellPseudoClass.Holiday, LunarCalendarCellPseudoClass.Workday)]
internal sealed class LunarCalendarViewCell : CalendarViewCell
{
    internal static readonly DirectProperty<LunarCalendarViewCell, LunarCalendarCellContext?> LunarContextProperty =
        AvaloniaProperty.RegisterDirect<LunarCalendarViewCell, LunarCalendarCellContext?>(
            nameof(LunarContext),
            owner => owner.LunarContext);

    internal static readonly DirectProperty<LunarCalendarViewCell, string> SecondaryTextProperty =
        AvaloniaProperty.RegisterDirect<LunarCalendarViewCell, string>(
            nameof(SecondaryText),
            owner => owner.SecondaryText);

    internal static readonly DirectProperty<LunarCalendarViewCell, LunarCalendarSecondaryContentKind> SecondaryContentKindProperty =
        AvaloniaProperty.RegisterDirect<LunarCalendarViewCell, LunarCalendarSecondaryContentKind>(
            nameof(SecondaryContentKind),
            owner => owner.SecondaryContentKind);

    internal static readonly DirectProperty<LunarCalendarViewCell, bool> ShowSecondaryContentProperty =
        AvaloniaProperty.RegisterDirect<LunarCalendarViewCell, bool>(
            nameof(ShowSecondaryContent),
            owner => owner.ShowSecondaryContent);

    internal static readonly DirectProperty<LunarCalendarViewCell, bool> ShowMarkerProperty =
        AvaloniaProperty.RegisterDirect<LunarCalendarViewCell, bool>(
            nameof(ShowMarker),
            owner => owner.ShowMarker);

    private LunarCalendarCellContext? _lunarContext;
    private string _secondaryText = string.Empty;
    private LunarCalendarSecondaryContentKind _secondaryContentKind;
    private bool _showSecondaryContent;
    private bool _showMarker;
    private Control? _secondaryPresenter;
    private Control? _secondaryTextControl;
    private Control? _marker;

    internal LunarCalendarCellContext? LunarContext
    {
        get => _lunarContext;
        private set => SetAndRaise(LunarContextProperty, ref _lunarContext, value);
    }

    internal string SecondaryText
    {
        get => _secondaryText;
        private set => SetAndRaise(SecondaryTextProperty, ref _secondaryText, value);
    }

    internal LunarCalendarSecondaryContentKind SecondaryContentKind
    {
        get => _secondaryContentKind;
        private set => SetAndRaise(SecondaryContentKindProperty, ref _secondaryContentKind, value);
    }

    internal bool ShowSecondaryContent
    {
        get => _showSecondaryContent;
        private set => SetAndRaise(ShowSecondaryContentProperty, ref _showSecondaryContent, value);
    }

    internal bool ShowMarker
    {
        get => _showMarker;
        private set => SetAndRaise(ShowMarkerProperty, ref _showMarker, value);
    }

    protected override bool HasDefaultSecondaryContent => ShowSecondaryContent;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == CellTemplateProperty || change.Property == FullCellTemplateProperty)
        {
            UpdateContentVisibility();
        }

        base.OnPropertyChanged(change);
        if (change.Property == ShowSecondaryContentProperty || change.Property == ShowMarkerProperty)
        {
            UpdateVisualPartVisibility();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _secondaryPresenter = e.NameScope.Find<Control>("PART_SecondaryPresenter");
        _secondaryTextControl = e.NameScope.Find<Control>("PART_SecondaryText");
        _marker = e.NameScope.Find<Control>("PART_Marker");
        UpdateVisualPartVisibility();
    }

    internal void ApplyLunarContext(LunarCalendarCellContext context, bool highlightWeekends)
    {
        LunarContext = context;
        SecondaryText = context.SecondaryText;
        SecondaryContentKind = context.SecondaryContentKind;
        PseudoClasses.Set(LunarCalendarCellPseudoClass.Weekend, highlightWeekends && context.IsWeekend);
        PseudoClasses.Set(LunarCalendarCellPseudoClass.Holiday, context.IsHoliday);
        PseudoClasses.Set(LunarCalendarCellPseudoClass.Workday, context.IsAdjustedWorkday);
        UpdateContentVisibility();
    }

    internal void ClearLunarContext()
    {
        LunarContext = null;
        SecondaryText = string.Empty;
        SecondaryContentKind = default;
        ShowSecondaryContent = false;
        ShowMarker = false;
        PseudoClasses.Set(LunarCalendarCellPseudoClass.Weekend, false);
        PseudoClasses.Set(LunarCalendarCellPseudoClass.Holiday, false);
        PseudoClasses.Set(LunarCalendarCellPseudoClass.Workday, false);
    }

    private void UpdateContentVisibility()
    {
        ShowSecondaryContent = LunarContext is not null &&
                               Model?.Kind is not CalendarViewCellKind.Week &&
                               CellTemplate is null &&
                               FullCellTemplate is null;
        ShowMarker = ShowSecondaryContent &&
                     LunarContext is { IsHoliday: true } or { IsAdjustedWorkday: true };
    }

    private void UpdateVisualPartVisibility()
    {
        if (_secondaryPresenter is not null)
        {
            _secondaryPresenter.IsVisible = ShowSecondaryContent;
        }

        if (_marker is not null)
        {
            _marker.IsVisible = ShowMarker;
        }

        if (_secondaryTextControl is not null)
        {
            _secondaryTextControl.IsVisible = ShowSecondaryContent;
        }
    }
}
