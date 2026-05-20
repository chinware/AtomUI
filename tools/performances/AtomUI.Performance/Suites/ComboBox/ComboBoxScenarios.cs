using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using System.Reflection;

namespace AtomUI.Performance;

using AtomComboBox = AtomUI.Desktop.Controls.ComboBox;
using AtomComboBoxItem = AtomUI.Desktop.Controls.ComboBoxItem;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateComboBoxScenarios()
    {
        return
        [
            new PerfScenario("ComboBox.FourItems.Closed", _ => CreateComboBoxWithItems(4)),
            new PerfScenario("ComboBox.FortyItems.Closed", _ => CreateComboBoxWithItems(40)),
            new PerfScenario("ComboBox.ItemsSource.Template.Closed", _ => new AtomComboBox
            {
                Width           = 300,
                PlaceholderText = "Please select",
                ShouldUseOverlayPopup = false,
                ItemsSource     = CreateComboBoxData(8),
                ItemTemplate    = new FuncDataTemplate<ComboBoxOptionData>((item, _) =>
                    new TextBlock
                    {
                        Text              = item?.Text,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                    })
            }),
            new PerfScenario("ComboBox.ContentRightAddOn.Closed", _ => CreateComboBoxWithItems(4, ".com")),
            new PerfScenario("ComboBox.FourItems.PopupMaterialized", _ => MaterializeComboBoxPopupAfterLoaded(CreateComboBoxWithItems(4))),
            new PerfScenario("ComboBox.FortyItems.PopupMaterialized", _ => MaterializeComboBoxPopupAfterLoaded(CreateComboBoxWithItems(40)))
        ];
    }

    private static AtomComboBox CreateComboBoxWithItems(int itemCount, object? contentRightAddOn = null)
    {
        var comboBox = new AtomComboBox
        {
            Width             = 300,
            PlaceholderText   = "Please select",
            ShouldUseOverlayPopup = false,
            ContentRightAddOn = contentRightAddOn
        };

        foreach (var item in CreateComboBoxData(itemCount))
        {
            comboBox.Items.Add(new AtomComboBoxItem
            {
                Content = item.Text
            });
        }

        return comboBox;
    }

    private static List<ComboBoxOptionData> CreateComboBoxData(int itemCount)
    {
        var items = new List<ComboBoxOptionData>(itemCount);
        for (var i = 0; i < itemCount; i++)
        {
            items.Add(new ComboBoxOptionData($"Option {i + 1}"));
        }
        return items;
    }

    private static AtomComboBox MaterializeComboBoxPopupAfterLoaded(AtomComboBox comboBox)
    {
        void HandleLoaded(object? sender, RoutedEventArgs args)
        {
            comboBox.Loaded -= HandleLoaded;
            MaterializeLazyComboBoxPopupContentForTest(comboBox);
        }

        comboBox.Loaded += HandleLoaded;
        return comboBox;
    }

    private static void MaterializeLazyComboBoxPopupContentForTest(AtomComboBox comboBox)
    {
        var method = comboBox.GetType().GetMethod(
            "EnsurePopupContent",
            BindingFlags.Instance | BindingFlags.NonPublic);
        method?.Invoke(comboBox, null);
    }

    private sealed record ComboBoxOptionData(string Text);
}
