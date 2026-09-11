using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// A masonry (waterfall) layout control that organizes children of uneven heights into columns
/// using stable-column assignments by default, with classic shortest-column reflow available
/// through <see cref="LayoutStrategy"/>.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Masonry"/> derives from <see cref="ItemsControl"/> and supports two content styles:
/// placing arbitrary <see cref="Control"/> children directly (the child itself acts as its container,
/// no extra visual layer), or binding a data collection via <see cref="ItemsControl.ItemsSource"/>
/// (the base class generates <c>ContentPresenter</c> containers). Container generation, collection
/// change synchronization and container recycling are provided by the base class.
/// </para>
/// <para>
/// The layout engine is the internal <c>MasonryPanel</c> assembled as the <c>ItemsPanel</c> by
/// the control theme; it is not exposed to developers.
/// </para>
/// </remarks>
public class Masonry : ItemsControl
{
    #region 公共属性定义

    /// <summary>
    /// Defines the <see cref="ColumnCount"/> property.
    /// </summary>
    public static readonly StyledProperty<int> ColumnCountProperty =
        AvaloniaProperty.Register<Masonry, int>(nameof(ColumnCount));

    /// <summary>
    /// Defines the <see cref="ColumnInfo"/> property.
    /// </summary>
    public static readonly StyledProperty<ResponsiveInt?> ColumnInfoProperty =
        AvaloniaProperty.Register<Masonry, ResponsiveInt?>(nameof(ColumnInfo));

    /// <summary>
    /// Defines the <see cref="MinColumnWidth"/> property.
    /// </summary>
    public static readonly StyledProperty<double> MinColumnWidthProperty =
        AvaloniaProperty.Register<Masonry, double>(nameof(MinColumnWidth), 320d);

    /// <summary>
    /// Defines the <see cref="MaxColumnCount"/> property.
    /// </summary>
    public static readonly StyledProperty<int> MaxColumnCountProperty =
        AvaloniaProperty.Register<Masonry, int>(nameof(MaxColumnCount), 4);

    /// <summary>
    /// Defines the <see cref="ColumnGap"/> property.
    /// </summary>
    public static readonly StyledProperty<double> ColumnGapProperty =
        AvaloniaProperty.Register<Masonry, double>(nameof(ColumnGap), 16d);

    /// <summary>
    /// Defines the <see cref="RowGap"/> property.
    /// </summary>
    public static readonly StyledProperty<double> RowGapProperty =
        AvaloniaProperty.Register<Masonry, double>(nameof(RowGap), 16d);

    /// <summary>
    /// Defines the <see cref="Gutter"/> property.
    /// </summary>
    public static readonly StyledProperty<ResponsiveGutter?> GutterProperty =
        AvaloniaProperty.Register<Masonry, ResponsiveGutter?>(nameof(Gutter));

    /// <summary>
    /// Defines the <see cref="LayoutStrategy"/> property.
    /// </summary>
    public static readonly StyledProperty<MasonryLayoutStrategy> LayoutStrategyProperty =
        AvaloniaProperty.Register<Masonry, MasonryLayoutStrategy>(
            nameof(LayoutStrategy),
            MasonryLayoutStrategy.StableColumns);

    /// <summary>
    /// Defines the attached <c>Masonry.Column</c> property, which pins a child to a specific
    /// column. A <c>null</c> value (the default) delegates automatic placement to
    /// <see cref="LayoutStrategy"/>.
    /// </summary>
    public static readonly AttachedProperty<int?> ColumnProperty =
        AvaloniaProperty.RegisterAttached<Masonry, Control, int?>("Column");

    /// <summary>
    /// Defines the attached <c>Masonry.Span</c> property, which controls how a child occupies
    /// columns. Defaults to <see cref="MasonryItemSpan.Auto"/>.
    /// </summary>
    public static readonly AttachedProperty<MasonryItemSpan> SpanProperty =
        AvaloniaProperty.RegisterAttached<Masonry, Control, MasonryItemSpan>("Span", MasonryItemSpan.Auto);

    /// <summary>
    /// Gets or sets the fixed number of columns. When greater than zero, Masonry uses a fixed
    /// column count. When less than or equal to zero, Masonry computes the effective column count
    /// from <see cref="MinColumnWidth"/>, <see cref="MaxColumnCount"/> and the available width.
    /// Defaults to <c>0</c> (container adaptive).
    /// </summary>
    public int ColumnCount
    {
        get => GetValue(ColumnCountProperty);
        set => SetValue(ColumnCountProperty, value);
    }

    /// <summary>
    /// Gets or sets the responsive column count. When the current breakpoint is configured,
    /// this value takes precedence over <see cref="ColumnCount"/>.
    /// </summary>
    public ResponsiveInt? ColumnInfo
    {
        get => GetValue(ColumnInfoProperty);
        set => SetValue(ColumnInfoProperty, value);
    }

    /// <summary>
    /// Gets or sets the target minimum width of a single column used by the container-adaptive
    /// column count computation. Defaults to <c>320</c>.
    /// </summary>
    public double MinColumnWidth
    {
        get => GetValue(MinColumnWidthProperty);
        set => SetValue(MinColumnWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the upper bound for the adaptive column count to prevent runaway column
    /// counts on wide screens. Defaults to <c>4</c>.
    /// </summary>
    public int MaxColumnCount
    {
        get => GetValue(MaxColumnCountProperty);
        set => SetValue(MaxColumnCountProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal gap between columns. Negative, <see cref="double.NaN"/>
    /// and infinite values are normalized to a valid non-negative value in the effective state.
    /// Defaults to <c>16</c>.
    /// </summary>
    public double ColumnGap
    {
        get => GetValue(ColumnGapProperty);
        set => SetValue(ColumnGapProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical gap between rows. Negative, <see cref="double.NaN"/>
    /// and infinite values are normalized to a valid non-negative value in the effective state.
    /// Defaults to <c>16</c>.
    /// </summary>
    public double RowGap
    {
        get => GetValue(RowGapProperty);
        set => SetValue(RowGapProperty, value);
    }

    /// <summary>
    /// Gets or sets the responsive horizontal and vertical gutter. When the current breakpoint is
    /// configured, this value takes precedence over <see cref="ColumnGap"/> and <see cref="RowGap"/>.
    /// </summary>
    public ResponsiveGutter? Gutter
    {
        get => GetValue(GutterProperty);
        set => SetValue(GutterProperty, value);
    }

    /// <summary>
    /// Gets or sets the strategy used to assign automatic items to columns. The default
    /// <see cref="MasonryLayoutStrategy.StableColumns"/> keeps existing items in their columns
    /// while the effective column count is unchanged. <see cref="MasonryLayoutStrategy.Reflow"/>
    /// recomputes automatic assignments from the current shortest column on each layout calculation.
    /// </summary>
    public MasonryLayoutStrategy LayoutStrategy
    {
        get => GetValue(LayoutStrategyProperty);
        set => SetValue(LayoutStrategyProperty, value);
    }

    /// <summary>Gets the value of the attached <c>Masonry.Column</c> property.</summary>
    public static int? GetColumn(Control element) => element.GetValue(ColumnProperty);

    /// <summary>Sets the value of the attached <c>Masonry.Column</c> property.</summary>
    public static void SetColumn(Control element, int? value) => element.SetValue(ColumnProperty, value);

    /// <summary>Gets the value of the attached <c>Masonry.Span</c> property.</summary>
    public static MasonryItemSpan GetSpan(Control element) => element.GetValue(SpanProperty);

    /// <summary>Sets the value of the attached <c>Masonry.Span</c> property.</summary>
    public static void SetSpan(Control element, MasonryItemSpan value) => element.SetValue(SpanProperty, value);

    #endregion

    #region 公共事件定义

    /// <summary>
    /// Raised when the effective column assignment of children changes. The event only notifies
    /// column assignment changes, not pixel-level arrange rectangles, and is dispatched outside
    /// of the Avalonia layout pass to prevent re-entrancy.
    /// </summary>
    public event EventHandler<MasonryLayoutChangedEventArgs>? LayoutChanged;

    #endregion

    static Masonry()
    {
        // Layout properties live on Masonry; the internal MasonryPanel reads them via
        // RelativeSource binding assembled by the control theme. A change here must trigger
        // a re-measure of MasonryPanel (the ItemsPanel).
        AffectsMeasure<Masonry>(
            ColumnCountProperty,
            ColumnInfoProperty,
            MinColumnWidthProperty,
            MaxColumnCountProperty,
            ColumnGapProperty,
            RowGapProperty,
            GutterProperty,
            LayoutStrategyProperty);
        ColumnProperty.Changed.AddClassHandler<Control>(HandleItemLayoutPropertyChanged);
        SpanProperty.Changed.AddClassHandler<Control>(HandleItemLayoutPropertyChanged);
    }

    /// <summary>
    /// Called by the internal layout engine after a layout pass produced a new effective column
    /// assignment. Dispatched outside the layout pass to avoid re-entrancy.
    /// </summary>
    internal void NotifyLayoutChanged(IReadOnlyList<MasonryItemLayout> items)
    {
        Dispatcher.Post(() => LayoutChanged?.Invoke(this, new MasonryLayoutChangedEventArgs(items)));
    }

    private static void HandleItemLayoutPropertyChanged(Control control, AvaloniaPropertyChangedEventArgs args)
    {
        if (control.GetVisualParent() is MasonryPanel panel)
        {
            panel.InvalidateStableAssignments();
        }
    }

}
