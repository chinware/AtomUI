using System.Reflection;
using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using AtomQRCode = AtomUI.Desktop.Controls.QRCode;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static readonly PropertyInfo QRCodeBitmapProperty =
        typeof(AtomQRCode).BaseType?.GetProperty("Bitmap", BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("QRCode Bitmap property was not found.");

    private static bool RunQRCodeStateVerification()
    {
        var failures = new List<string>();
        VerifyQRCodeBitmapLifecycle(failures);
        VerifyQRCodeStatusLayerLifecycle(failures);
        VerifyQRCodeCustomStatusContentLifecycle(failures);
        VerifyQRCodeIconLayerLifecycle(failures);
        VerifyQRCodeRefreshSubscriptionLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("QRCode state verification passed.");
            return true;
        }

        Console.Error.WriteLine("QRCode state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyQRCodeBitmapLifecycle(ICollection<string> failures)
    {
        var qrCode = new AtomQRCode
        {
            Value = "https://atomui.net"
        };

        using var realized = RealizeControl(qrCode);
        var firstBitmap = GetQRCodeBitmap(qrCode);
        Expect(firstBitmap != null,
            "QRCode should create a bitmap when Value is non-empty.",
            failures);

        qrCode.ApplyTemplate();
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(firstBitmap, GetQRCodeBitmap(qrCode)),
            "QRCode should not regenerate the bitmap when the render key did not change.",
            failures);

        qrCode.Value = string.Empty;
        RefreshLayout(realized.Window);
        Expect(GetQRCodeBitmap(qrCode) == null,
            "QRCode should clear its bitmap when Value becomes empty.",
            failures);

        qrCode.Value = "https://atomui.net/changed";
        RefreshLayout(realized.Window);
        Expect(GetQRCodeBitmap(qrCode) != null,
            "QRCode should recreate the bitmap after Value becomes non-empty again.",
            failures);
    }

    private static void VerifyQRCodeStatusLayerLifecycle(ICollection<string> failures)
    {
        var qrCode = new AtomQRCode
        {
            Value = "https://atomui.net"
        };

        using var realized = RealizeControl(qrCode);
        Expect(FindVisualByName<Panel>(qrCode, "LoadingLayout") == null,
            "Active QRCode should not create the default loading layer.",
            failures);
        Expect(FindVisualByName<Panel>(qrCode, "ExpiredLayout") == null,
            "Active QRCode should not create the default expired layer.",
            failures);
        Expect(FindVisualByName<Panel>(qrCode, "ScannedLayout") == null,
            "Active QRCode should not create the default scanned layer.",
            failures);

        qrCode.Status = QRCodeStatus.Loading;
        RefreshLayout(realized.Window);
        var loadingLayer = FindVisualByName<Panel>(qrCode, "LoadingLayout");
        Expect(loadingLayer != null,
            "Loading QRCode should create the loading layer.",
            failures);
        Expect(qrCode.GetSelfAndVisualDescendants().Any(visual =>
                   IsQRCodeTypeOrDerived(visual.GetType(), "AtomUI.Desktop.Controls.Spin")),
            "Default loading QRCode should create a Spin only while loading.",
            failures);

        qrCode.Status = QRCodeStatus.Scanned;
        RefreshLayout(realized.Window);
        Expect(loadingLayer?.GetVisualParent() == null,
            "Released loading layer should not keep a visual parent.",
            failures);
        Expect(FindVisualByName<Panel>(qrCode, "ScannedLayout") != null,
            "Scanned QRCode should create the scanned layer.",
            failures);

        qrCode.Status = QRCodeStatus.Active;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<Panel>(qrCode, "ScannedLayout") == null,
            "Active QRCode should release the scanned layer.",
            failures);
    }

    private static void VerifyQRCodeCustomStatusContentLifecycle(ICollection<string> failures)
    {
        var loadingContent = new StackPanel
        {
            Name = "CustomLoadingContent"
        };
        var qrCode = new AtomQRCode
        {
            Value          = "https://atomui.net",
            LoadingContent = loadingContent
        };

        using var realized = RealizeControl(qrCode);
        Expect(loadingContent.GetVisualParent() == null,
            "Inactive custom loading content should not be parented.",
            failures);

        qrCode.Status = QRCodeStatus.Loading;
        RefreshLayout(realized.Window);
        Expect(loadingContent.GetVisualParent() is ContentPresenter,
            "Custom loading content should be parented by a ContentPresenter while loading.",
            failures);

        qrCode.Status = QRCodeStatus.Active;
        RefreshLayout(realized.Window);
        Expect(loadingContent.GetVisualParent() == null,
            "Custom loading content should be released when QRCode returns to Active.",
            failures);
    }

    private static void VerifyQRCodeIconLayerLifecycle(ICollection<string> failures)
    {
        var qrCode = new AtomQRCode
        {
            Value = "https://atomui.net"
        };

        using var realized = RealizeControl(qrCode);
        Expect(FindVisualByName<Border>(qrCode, "ImageFrame") == null,
            "QRCode without Icon should not create ImageFrame.",
            failures);

        qrCode.Icon = QRCodeDemoIcon;
        RefreshLayout(realized.Window);
        var imageFrame = FindVisualByName<Border>(qrCode, "ImageFrame");
        Expect(imageFrame != null,
            "QRCode should create ImageFrame when Icon is set.",
            failures);

        qrCode.Icon = null;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<Border>(qrCode, "ImageFrame") == null,
            "QRCode should release ImageFrame when Icon is cleared.",
            failures);
        Expect(imageFrame?.GetVisualParent() == null,
            "Released ImageFrame should not keep a visual parent.",
            failures);
    }

    private static void VerifyQRCodeRefreshSubscriptionLifecycle(ICollection<string> failures)
    {
        var qrCode = new AtomQRCode
        {
            Value  = "https://atomui.net",
            Status = QRCodeStatus.Expired
        };
        var refreshCount = 0;
        qrCode.RefreshRequested += (_, _) => refreshCount++;

        using var realized = RealizeControl(qrCode);
        ClickRefreshButton(qrCode);
        Expect(refreshCount == 1,
            $"Expired QRCode refresh button should raise RefreshRequested once. Actual: {refreshCount}.",
            failures);

        qrCode.ApplyTemplate();
        RefreshLayout(realized.Window);
        ClickRefreshButton(qrCode);
        Expect(refreshCount == 2,
            $"QRCode should not duplicate RefreshRequested subscriptions after re-template. Actual: {refreshCount}.",
            failures);

        var expiredLayer = FindVisualByName<Panel>(qrCode, "ExpiredLayout");
        qrCode.Status = QRCodeStatus.Active;
        RefreshLayout(realized.Window);
        Expect(expiredLayer?.GetVisualParent() == null,
            "Released expired layer should not keep a visual parent.",
            failures);
    }

    private static Bitmap? GetQRCodeBitmap(AtomQRCode qrCode)
    {
        return (Bitmap?)QRCodeBitmapProperty.GetValue(qrCode);
    }

    private static bool IsQRCodeTypeOrDerived(Type type, string fullName)
    {
        while (true)
        {
            if (type.FullName == fullName)
            {
                return true;
            }

            if (type.BaseType is null)
            {
                return false;
            }

            type = type.BaseType;
        }
    }

    private static void ClickRefreshButton(AtomQRCode qrCode)
    {
        var refreshButton = FindVisualByName<Button>(qrCode, "PART_RefreshButton");
        refreshButton?.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
    }
}
