using System.Linq;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme.Resources;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Notifications;

public class NotificationCardThemeTests
{
    static NotificationCardThemeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Notification_Default_Expiration_Matches_Ant_Default_Duration()
    {
        var notification = new Notification("Notification Title", "Notification body");

        notification.Expiration.ShouldBe(TimeSpan.FromSeconds(4.5));
    }

    [Fact]
    public void Open_Notification_Without_Type_Does_Not_Create_Default_Icon()
    {
        using var manager = new WindowNotificationManager
        {
            IsMotionEnabled = false
        };

        ShowInWindow(manager, window =>
        {
            manager.Show(new Notification(
                title: "Notification Title",
                content: "Notification body",
                expiration: TimeSpan.Zero));
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            var card = manager.GetVisualDescendants().OfType<NotificationCard>().Single();
            card.Icon.ShouldBeNull();

            var iconPresenter = GetSemanticIconPresenter(card);
            iconPresenter.IsVisible.ShouldBeFalse();
        });
    }

    [Fact]
    public void Typed_Notification_Keeps_Ant_Type_Icon()
    {
        using var manager = new WindowNotificationManager
        {
            IsMotionEnabled = false
        };

        ShowInWindow(manager, window =>
        {
            manager.Show(new Notification(
                title: "Notification Title",
                content: "Notification body",
                type: NotificationType.Information,
                expiration: TimeSpan.Zero));
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            var card = manager.GetVisualDescendants().OfType<NotificationCard>().Single();
            card.Icon.ShouldBeOfType<InfoCircleFilled>();

            var iconPresenter = GetSemanticIconPresenter(card);
            iconPresenter.IsVisible.ShouldBeTrue();
        });
    }

    [Fact]
    public void NotificationType_Change_From_Default_To_Information_Creates_Type_Icon()
    {
        using var manager = new WindowNotificationManager();
        var card = new NotificationCard(manager)
        {
            Title           = "Notification Title",
            Content         = "Notification body",
            IsMotionEnabled = false
        };

        ShowInWindow(card, () =>
        {
            card.Icon.ShouldBeNull();

            card.NotificationType = NotificationType.Information;
            Dispatcher.UIThread.RunJobs();

            card.Icon.ShouldBeOfType<InfoCircleFilled>();

            var iconPresenter = GetSemanticIconPresenter(card);
            iconPresenter.IsVisible.ShouldBeTrue();
        });
    }

    [Fact]
    public void NotificationType_Change_From_Information_To_Default_Removes_Template_Type_Icon()
    {
        using var manager = new WindowNotificationManager();
        var card = new NotificationCard(manager)
        {
            Title            = "Notification Title",
            Content          = "Notification body",
            NotificationType = NotificationType.Information,
            IsMotionEnabled  = false
        };

        ShowInWindow(card, () =>
        {
            card.Icon.ShouldBeOfType<InfoCircleFilled>();

            card.NotificationType = NotificationType.Default;
            Dispatcher.UIThread.RunJobs();

            card.Icon.ShouldBeNull();

            var iconPresenter = GetSemanticIconPresenter(card);
            iconPresenter.IsVisible.ShouldBeFalse();
        });
    }

    [Fact]
    public void Custom_Icon_Is_Not_Replaced_By_NotificationType_Change()
    {
        using var manager = new WindowNotificationManager();
        var customIcon = new CloseOutlined();
        var card = new NotificationCard(manager)
        {
            Title           = "Notification Title",
            Content         = "Notification body",
            Icon            = customIcon,
            IsMotionEnabled = false
        };

        ShowInWindow(card, () =>
        {
            card.NotificationType = NotificationType.Success;
            Dispatcher.UIThread.RunJobs();

            card.Icon.ShouldBeSameAs(customIcon);

            var iconPresenter = GetSemanticIconPresenter(card);
            iconPresenter.IsVisible.ShouldBeTrue();
        });
    }

    [Fact]
    public void Close_Button_Uses_Ant_Notification_Hover_And_Pressed_Visuals()
    {
        using var manager = new WindowNotificationManager();
        var card = new NotificationCard(manager)
        {
            Title           = "Notification Title",
            Content         = "Notification body",
            IsMotionEnabled = false
        };

        ShowInWindow(card, () =>
        {
            var expectedSize = GetThemeResource<double>(NotificationTokenKind.NotificationCloseButtonSize);
            var closeButton  = card.GetVisualDescendants()
                                   .OfType<IconButton>()
                                   .Single(item => item.Name == "PART_CloseButton");

            closeButton.IsMotionEnabled = false;
            closeButton.Width.ShouldBe(expectedSize, 0.5);
            closeButton.Height.ShouldBe(expectedSize, 0.5);

            BrushShouldHaveSameColor(closeButton.IconBrush, GetThemeColor(SharedTokenKind.ColorIcon));

            ((IPseudoClasses)closeButton.Classes).Set(":pointerover", true);
            Dispatcher.UIThread.RunJobs();

            BrushShouldHaveSameColor(closeButton.Background, GetThemeColor(SharedTokenKind.ColorBgTextHover));
            BrushShouldHaveSameColor(closeButton.IconBrush, GetThemeColor(SharedTokenKind.ColorIconHover));

            ((IPseudoClasses)closeButton.Classes).Set(":pressed", true);
            Dispatcher.UIThread.RunJobs();

            BrushShouldHaveSameColor(closeButton.Background, GetThemeColor(SharedTokenKind.ColorBgTextActive));
            BrushShouldHaveSameColor(closeButton.IconBrush, GetThemeColor(SharedTokenKind.ColorIconHover));
        });
    }

    [Fact]
    public void Progress_Bar_Gradient_Uses_Primary_Border_Hover_To_Primary()
    {
        var progressBrush = GetThemeResource<IBrush>(NotificationTokenKind.NotificationProgressBg);
        var gradient      = progressBrush.ShouldBeAssignableTo<IGradientBrush>();

        gradient.GradientStops.Count.ShouldBe(2);
        gradient.GradientStops[0].Color.ShouldBe(GetThemeColor(SharedTokenKind.ColorPrimaryBorderHover));
        gradient.GradientStops[0].Offset.ShouldBe(0);
        gradient.GradientStops[1].Color.ShouldBe(GetThemeColor(SharedTokenKind.ColorPrimary));
        gradient.GradientStops[1].Offset.ShouldBe(1);
    }

    [Fact]
    public void Internal_Spacing_Tokens_Are_Reduced_By_One_Third()
    {
        const double ratio = 2d / 3d;

        var compactHorizontalPadding = GetThemeResource<double>(SharedTokenKind.UniformlyPaddingLG) * ratio;
        var compactVerticalPadding   = GetThemeResource<double>(SharedTokenKind.UniformlyPaddingMD) * ratio;
        var compactTitleGap          = GetThemeResource<double>(SharedTokenKind.UniformlyMarginXS) * ratio;
        var compactIconGap           = GetThemeResource<double>(SharedTokenKind.UniformlyMarginSM) * ratio;

        var notificationPadding = GetThemeResource<Thickness>(NotificationTokenKind.NotificationPadding);
        ThicknessShouldBe(
            notificationPadding,
            new Thickness(compactHorizontalPadding, compactVerticalPadding, compactHorizontalPadding, 0));

        var contentMargin = GetThemeResource<Thickness>(NotificationTokenKind.NotificationContentMargin);
        ThicknessShouldBe(contentMargin, new Thickness(0, 0, 0, compactVerticalPadding));

        var headerMargin = GetThemeResource<Thickness>(NotificationTokenKind.HeaderMargin);
        ThicknessShouldBe(headerMargin, new Thickness(0, 0, 0, compactTitleGap));

        var iconMargin = GetThemeResource<Thickness>(NotificationTokenKind.NotificationIconMargin);
        ThicknessShouldBe(iconMargin, new Thickness(0, 0, compactIconGap, 0));
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        ShowInWindow(content, _ => assertion());
    }

    private static void ShowInWindow(Control content, Action<AvaloniaWindow> assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 500,
            Height  = 300,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            content.ApplyTemplate();
            window.UpdateLayout();
            assertion(window);
        }
        finally
        {
            window.Close();
        }
    }

    private static T GetThemeResource<T>(object key)
    {
        var application = Application.Current;
        application.ShouldNotBeNull();
        application!.TryGetResource(key, application.ActualThemeVariant, out var value).ShouldBeTrue();
        value.ShouldBeAssignableTo<T>();
        return (T)value!;
    }

    private static IconPresenter GetSemanticIconPresenter(NotificationCard card)
    {
        return card.GetVisualDescendants()
                   .OfType<IconPresenter>()
                   .Single(item => item.Name == "IconPresenter" && ReferenceEquals(item.Icon, card.Icon));
    }

    private static Color GetThemeColor(object key)
    {
        var application = Application.Current;
        application.ShouldNotBeNull();
        application!.TryGetResource(key, application.ActualThemeVariant, out var value).ShouldBeTrue();
        return ToColor(value);
    }

    private static void BrushShouldHaveSameColor(IBrush? actual, Color expected)
    {
        actual.ShouldNotBeNull();
        ToColor(actual).ShouldBe(expected);
    }

    private static void ThicknessShouldBe(Thickness actual, Thickness expected)
    {
        actual.Left.ShouldBe(expected.Left, 0.001);
        actual.Top.ShouldBe(expected.Top, 0.001);
        actual.Right.ShouldBe(expected.Right, 0.001);
        actual.Bottom.ShouldBe(expected.Bottom, 0.001);
    }

    private static Color ToColor(object? value)
    {
        return value switch
        {
            Color color => color,
            ISolidColorBrush brush => brush.Color,
            _ => throw new ShouldAssertException($"Expected color or solid brush, got {value?.GetType().FullName ?? "<null>"}")
        };
    }
}
