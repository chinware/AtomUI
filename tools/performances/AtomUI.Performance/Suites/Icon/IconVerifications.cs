using System.Diagnostics;
using System.Reflection;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunAntDesignMetadataVerification()
    {
        var failures = new List<string>();
        var calculateGeometryBounds = typeof(Icon).GetMethod(
            "CalculateGeometryBounds",
            BindingFlags.Instance | BindingFlags.NonPublic);
        var calculateZoomToFit = typeof(AntDesignIcon).GetMethod(
            "CalculateZoomToFit",
            BindingFlags.Static | BindingFlags.NonPublic);
        if (calculateGeometryBounds == null || calculateZoomToFit == null)
        {
            Console.Error.WriteLine("AntDesign metadata verification failed: reflection target not found.");
            return false;
        }

        var iconTypes = typeof(AntDesignIcon).Assembly
                                             .GetTypes()
                                             .Where(type => type is { IsClass: true, IsAbstract: false } &&
                                                            type != typeof(AntDesignIcon) &&
                                                            type.Namespace == typeof(AntDesignIcon).Namespace &&
                                                            type.IsSubclassOf(typeof(AntDesignIcon)))
                                             .OrderBy(type => type.Name, StringComparer.Ordinal)
                                             .ToList();

        foreach (var iconType in iconTypes)
        {
            var icon = (AntDesignIcon)Activator.CreateInstance(iconType)!;
            var hasGeneratedMetadata = GetGeneratedMetadataProperty<bool>(icon, "HasGeneratedGeometryMetadata");
            if (!hasGeneratedMetadata)
            {
                failures.Add($"{iconType.Name}: generated metadata is disabled.");
                continue;
            }

            var calculatedBounds = (Rect)calculateGeometryBounds.Invoke(icon, null)!;
            var generatedBounds  = GetGeneratedMetadataProperty<Rect>(icon, "GeneratedGeometryBounds");
            if (!AreClose(calculatedBounds, generatedBounds))
            {
                failures.Add($"{iconType.Name}: generated bounds {generatedBounds} != calculated bounds {calculatedBounds}.");
            }

            var generatedViewBox = GetGeneratedMetadataProperty<Rect>(icon, "GeneratedViewBox");
            var expectedZoom     = (Matrix)calculateZoomToFit.Invoke(null, [generatedViewBox, calculatedBounds])!;
            var generatedZoom    = GetGeneratedMetadataProperty<Matrix>(icon, "GeneratedZoomMatrix");
            if (!AreClose(expectedZoom, generatedZoom))
            {
                failures.Add($"{iconType.Name}: generated zoom {generatedZoom} != expected zoom {expectedZoom}.");
            }
        }

        if (failures.Count == 0)
        {
            Console.WriteLine($"AntDesign metadata verification passed. Checked {iconTypes.Count} icons.");
            return true;
        }

        Console.Error.WriteLine("AntDesign metadata verification failed:");
        foreach (var failure in failures.Take(20))
        {
            Console.Error.WriteLine($"- {failure}");
        }
        if (failures.Count > 20)
        {
            Console.Error.WriteLine($"- ... {failures.Count - 20} more failures.");
        }
        return false;
    }

    private static bool RunIconHiddenSlotsVerification()
    {
        var failures = new List<string>();
        VerifyMenuItemIndicatorSlot(failures);
        VerifyToggleIconButtonSlot(failures);
        VerifySelectHandleIndicatorSlots(failures);
        VerifyNavMenuIndicatorSlots(failures);
        VerifyButtonLoadingIconSlot(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Icon hidden slot verification passed.");
            return true;
        }

        Console.Error.WriteLine("Icon hidden slot verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static bool RunIconProviderCacheVerification()
    {
        var failures = new List<string>();
        var cacheType = typeof(Icon).Assembly.GetType("AtomUI.Controls.IconProviderCache");
        if (cacheType is null)
        {
            Console.Error.WriteLine("Icon provider cache verification failed: cache type not found.");
            return false;
        }

        AntDesignIconProvider.ClearAllCache();
        _ = new AntDesignIconProvider(AntDesignIconKind.SearchOutlined).ProvideValue(null!);

        if (!ContainsCacheKey(cacheType, "TypeToCreator", typeof(SearchOutlined)))
        {
            failures.Add("TypeToCreator did not cache SearchOutlined after provider creation.");
        }

        if (!ContainsCacheKey(cacheType, "EnumTypeToIconTypes", typeof(AntDesignIconKind)))
        {
            failures.Add("EnumTypeToIconTypes did not track AntDesignIconKind after provider creation.");
        }

        AntDesignIconProvider.ClearCache();

        if (ContainsCacheKey(cacheType, "TypeToCreator", typeof(SearchOutlined)))
        {
            failures.Add("ClearCache did not remove SearchOutlined creator from TypeToCreator.");
        }

        if (ContainsCacheKey(cacheType, "EnumTypeToIconTypes", typeof(AntDesignIconKind)))
        {
            failures.Add("ClearCache did not remove AntDesignIconKind from EnumTypeToIconTypes.");
        }

        _ = new AntDesignIconProvider(AntDesignIconKind.SearchOutlined).ProvideValue(null!);
        AntDesignIconProvider.ClearAllCache();
        if (GetCacheCount(cacheType, "TypeToCreator") != 0 ||
            GetCacheCount(cacheType, "EnumTypeToIconTypes") != 0)
        {
            failures.Add("ClearAllCache did not empty provider creator tracking caches.");
        }

        if (failures.Count == 0)
        {
            Console.WriteLine("Icon provider cache verification passed.");
            return true;
        }

        Console.Error.WriteLine("Icon provider cache verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static bool ContainsCacheKey(Type cacheType, string fieldName, object key)
    {
        var field = cacheType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        var cache = field?.GetValue(null);
        var containsKey = cache?.GetType().GetMethod("ContainsKey");
        return containsKey != null && (bool)containsKey.Invoke(cache, [key])!;
    }

    private static int GetCacheCount(Type cacheType, string fieldName)
    {
        var field = cacheType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        var cache = field?.GetValue(null);
        var count = cache?.GetType().GetProperty("Count")?.GetValue(cache);
        return count is int value ? value : -1;
    }

    private static T GetGeneratedMetadataProperty<T>(AntDesignIcon icon, string propertyName)
    {
        var property = icon.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic) ??
                       typeof(AntDesignIcon).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
        Debug.Assert(property != null);
        return (T)property.GetValue(icon)!;
    }

    private static bool AreClose(Rect left, Rect right)
    {
        return AreClose(left.X, right.X) &&
               AreClose(left.Y, right.Y) &&
               AreClose(left.Width, right.Width) &&
               AreClose(left.Height, right.Height);
    }

    private static bool AreClose(Matrix left, Matrix right)
    {
        return AreClose(left.M11, right.M11) &&
               AreClose(left.M12, right.M12) &&
               AreClose(left.M21, right.M21) &&
               AreClose(left.M22, right.M22) &&
               AreClose(left.M31, right.M31) &&
               AreClose(left.M32, right.M32);
    }

    private static bool AreClose(double left, double right)
    {
        return Math.Abs(left - right) <= 0.000001;
    }

    private static void VerifyMenuItemIndicatorSlot(ICollection<string> failures)
    {
        var menuItem = new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "Leaf item"
        };
        using var realized = RealizeControl(menuItem);
        var host = FindVisualByName<Panel>(menuItem, "PART_MenuIndicatorIconHost");
        Expect(host != null, "MenuItem should create the indicator host.", failures);
        Expect(FindVisualByName<RightOutlined>(menuItem, "MenuIndicatorIcon") == null,
            "Leaf MenuItem should not create MenuIndicatorIcon.", failures);
        Expect(host?.Children.Count == 0,
            $"Leaf MenuItem indicator host should be empty, actual {host?.Children.Count}.", failures);

        menuItem.Items.Add(new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "Runtime child"
        });
        RefreshLayout(realized.Window);
        var indicator = FindVisualByName<RightOutlined>(menuItem, "MenuIndicatorIcon");
        Expect(indicator != null, "MenuItem with submenu should create MenuIndicatorIcon.", failures);
        Expect(host?.Children.OfType<RightOutlined>().SingleOrDefault() != null,
            "MenuItem indicator should be attached to PART_MenuIndicatorIconHost.", failures);

        menuItem.Items.Clear();
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<RightOutlined>(menuItem, "MenuIndicatorIcon") == null,
            "MenuItem should remove MenuIndicatorIcon when submenu items are cleared.", failures);
        Expect(host?.Children.Count == 0,
            $"MenuItem indicator host should be empty after clearing submenu items, actual {host?.Children.Count}.",
            failures);
        Expect(indicator?.GetVisualParent() == null,
            "Removed MenuIndicatorIcon should not keep a visual parent.", failures);
    }

    private static void VerifyToggleIconButtonSlot(ICollection<string> failures)
    {
        var button = CreateToggleIconButton();
        using var realized = RealizeControl(button);
        Expect(button.GetSelfAndVisualDescendants().OfType<IconPresenter>().Count() == 1,
            "ToggleIconButton should create exactly one IconPresenter.", failures);
        Expect(button.GetSelfAndVisualDescendants().OfType<EyeInvisibleOutlined>().SingleOrDefault() != null,
            "Unchecked ToggleIconButton should show UnCheckedIcon.", failures);
        Expect(button.GetSelfAndVisualDescendants().OfType<EyeOutlined>().SingleOrDefault() == null,
            "Unchecked ToggleIconButton should not attach CheckedIcon.", failures);

        var uncheckedIcon = button.UnCheckedIcon;
        button.SetCurrentValue(ToggleButton.IsCheckedProperty, true);
        RefreshLayout(realized.Window);
        Expect(button.GetSelfAndVisualDescendants().OfType<IconPresenter>().Count() == 1,
            "Checked ToggleIconButton should keep a single IconPresenter.", failures);
        Expect(button.GetSelfAndVisualDescendants().OfType<EyeOutlined>().SingleOrDefault() != null,
            "Checked ToggleIconButton should show CheckedIcon.", failures);
        Expect(button.GetSelfAndVisualDescendants().OfType<EyeInvisibleOutlined>().SingleOrDefault() == null,
            "Checked ToggleIconButton should detach UnCheckedIcon.", failures);
        Expect((uncheckedIcon as Visual)?.GetVisualParent() == null,
            "Detached ToggleIconButton UnCheckedIcon should not keep a visual parent.", failures);
    }

    private static void VerifySelectHandleIndicatorSlots(ICollection<string> failures)
    {
        var openIcon = new DownOutlined();
        var handle = new SelectHandle
        {
            OpenIndicator = openIcon,
            LoadingIcon = new LoadingOutlined
            {
                LoadingAnimation = IconAnimation.Spin
            }
        };
        using var realized = RealizeControl(handle);
        var indicator = FindVisualByName<IconPresenter>(handle, "OpenIndicator");
        Expect(indicator != null,
            "SelectHandle should create one shared indicator presenter.", failures);
        Expect(ReferenceEquals(indicator?.Icon, openIcon) && indicator.IsVisible,
            "SelectHandle default state should show OpenIndicator icon.", failures);
        Expect(FindVisualByName<IconPresenter>(handle, "LoadingIndicator") == null,
            "SelectHandle default state should not create LoadingIndicator.", failures);
        Expect(FindVisualByName<SearchOutlined>(handle, "SearchIndicator") == null,
            "SelectHandle default state should not create a dedicated SearchIndicator visual.", failures);
        Expect(FindVisualByName<InputClearIconButton>(handle, "PART_ClearButton") is { IsVisible: false },
            "SelectHandle default state should keep clear button hidden.", failures);

        handle.SetCurrentValue(SelectHandle.IsFilterEnabledProperty, true);
        handle.SetCurrentValue(SelectHandle.IsDropDownOpenProperty, true);
        RefreshLayout(realized.Window);
        indicator = FindVisualByName<IconPresenter>(handle, "OpenIndicator");
        Expect(indicator != null && ReferenceEquals(indicator.Icon, handle.FilterIndicator) && indicator.IsVisible,
            "SelectHandle filter-open state should show FilterIndicator in the shared presenter.", failures);
        Expect(openIcon.GetVisualParent() == null,
            "Detached SelectHandle OpenIndicator icon should not keep a visual parent.", failures);
        Expect(handle.FilterIndicator is SearchOutlined { } searchIcon && searchIcon.GetVisualParent() == indicator,
            "SelectHandle filter-open state should attach the theme SearchOutlined through the shared presenter.", failures);

        handle.SetCurrentValue(SelectHandle.IsLoadingProperty, true);
        RefreshLayout(realized.Window);
        indicator = FindVisualByName<IconPresenter>(handle, "OpenIndicator");
        Expect(indicator != null && ReferenceEquals(indicator.Icon, handle.FilterIndicator),
            "SelectHandle loading plus filter-open state should keep FilterIndicator visible.", failures);

        handle.SetCurrentValue(SelectHandle.IsDropDownOpenProperty, false);
        RefreshLayout(realized.Window);
        indicator = FindVisualByName<IconPresenter>(handle, "OpenIndicator");
        Expect(indicator != null && ReferenceEquals(indicator.Icon, handle.LoadingIcon) && indicator.IsVisible,
            "SelectHandle loading state should show LoadingIcon in the shared presenter.", failures);

        handle.SetCurrentValue(SelectHandle.IsLoadingProperty, false);
        handle.SetCurrentValue(SelectHandle.IsAllowClearProperty, true);
        handle.SetCurrentValue(SelectHandle.IsSelectionEmptyProperty, false);
        handle.SetCurrentValue(SelectHandle.IsInputHoverProperty, true);
        RefreshLayout(realized.Window);
        var clearButton = FindVisualByName<InputClearIconButton>(handle, "PART_ClearButton");
        var clearRequested = false;
        handle.ClearRequested += (_, _) => clearRequested = true;
        clearButton?.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent, clearButton));
        Expect(clearRequested, "SelectHandle clear button should raise ClearRequested.", failures);
        Expect(clearButton is { IsVisible: true }, "SelectHandle clear state should show clear button.", failures);
        Expect(indicator is { IsVisible: false },
            "SelectHandle clear state should hide the shared indicator presenter.", failures);
        Expect(FindVisualByName<SearchOutlined>(handle, "SearchIndicator") == null,
            "SelectHandle clear state should not create a dedicated SearchIndicator visual.", failures);

        handle.SetCurrentValue(SelectHandle.IsInputHoverProperty, false);
        RefreshLayout(realized.Window);
        indicator = FindVisualByName<IconPresenter>(handle, "OpenIndicator");
        Expect(clearButton is { IsVisible: false },
            "SelectHandle should hide clear button when clear state ends.", failures);
        Expect(indicator is { IsVisible: true },
            "SelectHandle should restore the shared indicator when clear state ends.", failures);
    }

    private static void VerifyNavMenuIndicatorSlots(ICollection<string> failures)
    {
        var inlineHeader = new InlineNavMenuItemHeader
        {
            Header = "Leaf item"
        };
        using var inlineRealized = RealizeControl(inlineHeader);
        var inlineHost = FindVisualByName<Panel>(inlineHeader, "PART_MenuIndicatorIconHost");
        Expect(inlineHost != null, "InlineNavMenuItemHeader should create the indicator host.", failures);
        Expect(FindVisualByName<RightOutlined>(inlineHeader, "MenuIndicatorIcon") == null,
            "Leaf InlineNavMenuItemHeader should not create MenuIndicatorIcon.", failures);

        inlineHeader.HasSubMenu = true;
        RefreshLayout(inlineRealized.Window);
        var inlineIndicator = FindVisualByName<RightOutlined>(inlineHeader, "MenuIndicatorIcon");
        Expect(inlineIndicator != null,
            "InlineNavMenuItemHeader with submenu should create MenuIndicatorIcon.", failures);

        inlineHeader.HasSubMenu = false;
        RefreshLayout(inlineRealized.Window);
        Expect(FindVisualByName<RightOutlined>(inlineHeader, "MenuIndicatorIcon") == null,
            "InlineNavMenuItemHeader should remove MenuIndicatorIcon when HasSubMenu becomes false.", failures);
        Expect(inlineIndicator?.GetVisualParent() == null,
            "Removed InlineNavMenuItemHeader indicator should not keep a visual parent.", failures);

        var horizontalHeader = new HorizontalNavMenuItemHeader
        {
            Header     = "Top parent item",
            HasSubMenu = true,
            IsTopLevel = true
        };
        using var horizontalRealized = RealizeControl(horizontalHeader);
        Expect(FindVisualByName<RightOutlined>(horizontalHeader, "MenuIndicatorIcon") == null,
            "Top-level HorizontalNavMenuItemHeader should not create MenuIndicatorIcon.", failures);

        horizontalHeader.IsTopLevel = false;
        RefreshLayout(horizontalRealized.Window);
        Expect(FindVisualByName<RightOutlined>(horizontalHeader, "MenuIndicatorIcon") != null,
            "Nested HorizontalNavMenuItemHeader with submenu should create MenuIndicatorIcon.", failures);
    }

    private static void VerifyButtonLoadingIconSlot(ICollection<string> failures)
    {
        var button = new AtomUI.Desktop.Controls.Button
        {
            Content = "Button"
        };
        using var realized = RealizeControl(button);
        Expect(FindVisualByName<LoadingOutlined>(button, "PART_LoadingIcon") == null,
            "Non-loading Button should not create PART_LoadingIcon.", failures);

        button.SetCurrentValue(AtomUI.Desktop.Controls.Button.IsLoadingProperty, true);
        RefreshLayout(realized.Window);
        var loadingIcon = FindVisualByName<LoadingOutlined>(button, "PART_LoadingIcon");
        Expect(loadingIcon != null, "Loading Button should create PART_LoadingIcon.", failures);

        button.SetCurrentValue(AtomUI.Desktop.Controls.Button.IsLoadingProperty, false);
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<LoadingOutlined>(button, "PART_LoadingIcon") == null,
            "Button should remove PART_LoadingIcon when loading ends.", failures);
        Expect(loadingIcon?.GetVisualParent() == null,
            "Removed Button loading icon should not keep a visual parent.", failures);
    }
}
