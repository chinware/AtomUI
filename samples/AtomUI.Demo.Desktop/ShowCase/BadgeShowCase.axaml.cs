using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class BadgeShowCase : UserControl
{
    public static readonly StyledProperty<double> DynamicBadgeCountProperty =
        AvaloniaProperty.Register<ProgressBarShowCase, double>(nameof(DynamicBadgeCount), 5);

    public static readonly StyledProperty<bool> DynamicDotBadgeVisibleProperty =
        AvaloniaProperty.Register<ProgressBarShowCase, bool>(nameof(DynamicDotBadgeVisible), true);

    public static readonly StyledProperty<bool> StandaloneSwitchCheckedProperty =
        AvaloniaProperty.Register<ProgressBarShowCase, bool>(nameof(StandaloneSwitchChecked), true);

    public static readonly StyledProperty<double> StandaloneBadgeCount1Property =
        AvaloniaProperty.Register<ProgressBarShowCase, double>(nameof(StandaloneBadgeCount1), 11);

    public static readonly StyledProperty<double> StandaloneBadgeCount2Property =
        AvaloniaProperty.Register<ProgressBarShowCase, double>(nameof(StandaloneBadgeCount2), 25);

    public static readonly StyledProperty<double> StandaloneBadgeCount3Property =
        AvaloniaProperty.Register<ProgressBarShowCase, double>(nameof(StandaloneBadgeCount3), 109);

    public double DynamicBadgeCount
    {
        get => GetValue(DynamicBadgeCountProperty);
        set => SetValue(DynamicBadgeCountProperty, value);
    }

    public bool DynamicDotBadgeVisible
    {
        get => GetValue(DynamicDotBadgeVisibleProperty);
        set => SetValue(DynamicDotBadgeVisibleProperty, value);
    }

    public bool StandaloneSwitchChecked
    {
        get => GetValue(StandaloneSwitchCheckedProperty);
        set => SetValue(StandaloneSwitchCheckedProperty, value);
    }

    public double StandaloneBadgeCount1
    {
        get => GetValue(StandaloneBadgeCount1Property);
        set => SetValue(StandaloneBadgeCount1Property, value);
    }

    public double StandaloneBadgeCount2
    {
        get => GetValue(StandaloneBadgeCount2Property);
        set => SetValue(StandaloneBadgeCount2Property, value);
    }

    public double StandaloneBadgeCount3
    {
        get => GetValue(StandaloneBadgeCount3Property);
        set => SetValue(StandaloneBadgeCount3Property, value);
    }

    public BadgeShowCase()
    {
        DataContext = this;
        InitializeComponent();
    }

    public void AddDynamicBadgeCount()
    {
        DynamicBadgeCount += 1;
    }

    public void SubDynamicBadgeCount()
    {
        var value = DynamicBadgeCount;
        value             -= 1;
        value             =  Math.Max(value, 0);
        DynamicBadgeCount =  value;
    }

    public void RandomDynamicBadgeCount()
    {
        var random = new Random();
        DynamicBadgeCount = random.Next(0, 110);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        base.OnPropertyChanged(args);
        if (args.Property == StandaloneSwitchCheckedProperty)
        {
            var isChecked = args.GetNewValue<bool>();
            if (isChecked)
            {
                StandaloneBadgeCount1 = 11;
                StandaloneBadgeCount2 = 25;
                StandaloneBadgeCount3 = 109;
            }
            else
            {
                StandaloneBadgeCount1 = 0;
                StandaloneBadgeCount2 = 0;
                StandaloneBadgeCount3 = 0;
            }
        }
    }
}