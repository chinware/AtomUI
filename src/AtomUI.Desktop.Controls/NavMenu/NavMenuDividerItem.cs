using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace AtomUI.Desktop.Controls;

internal sealed class NavMenuDividerItem : TemplatedControl
{
    internal static readonly DirectProperty<NavMenuDividerItem, NavMenuMode> ModeProperty =
        AvaloniaProperty.RegisterDirect<NavMenuDividerItem, NavMenuMode>(
            nameof(Mode),
            item => item.Mode,
            (item, value) => item.Mode = value);

    internal static readonly DirectProperty<NavMenuDividerItem, bool> IsTopLevelProperty =
        AvaloniaProperty.RegisterDirect<NavMenuDividerItem, bool>(
            nameof(IsTopLevel),
            item => item.IsTopLevel,
            (item, value) => item.IsTopLevel = value);

    internal static readonly DirectProperty<NavMenuDividerItem, bool> IsDarkStyleProperty =
        AvaloniaProperty.RegisterDirect<NavMenuDividerItem, bool>(
            nameof(IsDarkStyle),
            item => item.IsDarkStyle,
            (item, value) => item.IsDarkStyle = value);

    internal static readonly DirectProperty<NavMenuDividerItem, Orientation> OrientationProperty =
        AvaloniaProperty.RegisterDirect<NavMenuDividerItem, Orientation>(
            nameof(Orientation),
            item => item.Orientation);

    private NavMenuMode _mode;
    private bool _isTopLevel;
    private bool _isDarkStyle;
    private Orientation _orientation = Orientation.Horizontal;
    private CompositeDisposable? _entryBindingDisposables;

    internal NavMenuMode Mode
    {
        get => _mode;
        set
        {
            if (SetAndRaise(ModeProperty, ref _mode, value))
            {
                UpdateOrientation();
            }
        }
    }

    internal bool IsTopLevel
    {
        get => _isTopLevel;
        set
        {
            if (SetAndRaise(IsTopLevelProperty, ref _isTopLevel, value))
            {
                UpdateOrientation();
            }
        }
    }

    internal bool IsDarkStyle
    {
        get => _isDarkStyle;
        set => SetAndRaise(IsDarkStyleProperty, ref _isDarkStyle, value);
    }

    internal Orientation Orientation
    {
        get => _orientation;
        private set => SetAndRaise(OrientationProperty, ref _orientation, value);
    }

    internal CompositeDisposable ResetEntryBindingDisposables()
    {
        _entryBindingDisposables?.Dispose();
        _entryBindingDisposables = new CompositeDisposable();
        return _entryBindingDisposables;
    }

    internal void ClearEntryBindingDisposables()
    {
        _entryBindingDisposables?.Dispose();
        _entryBindingDisposables = null;
    }

    internal void ClearEntryContext()
    {
        ClearValue(ModeProperty);
        ClearValue(IsTopLevelProperty);
        ClearValue(IsDarkStyleProperty);
        Mode       = default;
        IsTopLevel = false;
        IsDarkStyle = false;
    }

    private void UpdateOrientation()
    {
        Orientation = Mode == NavMenuMode.Horizontal && IsTopLevel
            ? Orientation.Vertical
            : Orientation.Horizontal;
    }
}
