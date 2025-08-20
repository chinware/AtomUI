using AtomUI.Controls.Themes;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using AvaloniaButton = Avalonia.Controls.Button;

namespace AtomUI.Controls;

internal class ColorSlider : AbstractColorSlider
{
    private AvaloniaButton? _decreaseButton;
    private AvaloniaButton? _increaseButton;
    private IDisposable? _decreaseButtonPressDispose;
    private IDisposable? _decreaseButtonReleaseDispose;
    private IDisposable? _increaseButtonSubscription;
    private IDisposable? _increaseButtonReleaseDispose;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _decreaseButtonPressDispose?.Dispose();
        _decreaseButtonReleaseDispose?.Dispose();
        _increaseButtonSubscription?.Dispose();
        _increaseButtonReleaseDispose?.Dispose();

        Track = e.NameScope.Find<ColorPickerSliderTrack>(ColorSliderThemeConstants.TrackPart);

        if (Track != null)
        {
            Track.IgnoreThumbDrag = true;

            _decreaseButton = e.NameScope.Find<AvaloniaButton>(ColorSliderThemeConstants.DecreaseButtonPart);
            _increaseButton = e.NameScope.Find<AvaloniaButton>(ColorSliderThemeConstants.IncreaseButtonPart);

            if (_decreaseButton != null)
            {
                _decreaseButtonPressDispose = _decreaseButton.AddDisposableHandler(PointerPressedEvent, TrackPressed, RoutingStrategies.Tunnel);
                _decreaseButtonReleaseDispose = _decreaseButton.AddDisposableHandler(PointerReleasedEvent, TrackReleased, RoutingStrategies.Tunnel);
            }

            if (_increaseButton != null)
            {
                _increaseButtonSubscription = _increaseButton.AddDisposableHandler(PointerPressedEvent, TrackPressed, RoutingStrategies.Tunnel);
                _increaseButtonReleaseDispose = _increaseButton.AddDisposableHandler(PointerReleasedEvent, TrackReleased, RoutingStrategies.Tunnel);
            }
        }
    }
}