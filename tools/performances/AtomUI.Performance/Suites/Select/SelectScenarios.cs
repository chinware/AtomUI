using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateSelectScenarios()
    {
        return
        [
            new PerfScenario("Select.Default.Single.Closed", _ => new Select
            {
                OptionsSource = CreateSelectOptions()
            }),
            new PerfScenario("Select.Single.AllowClear.Empty", _ => new Select
            {
                IsAllowClear = true,
                OptionsSource = CreateSelectOptions()
            }),
            new PerfScenario("Select.Single.AllowClear.Selected", _ =>
            {
                var options = CreateSelectOptions();
                return new Select
                {
                    IsAllowClear  = true,
                    OptionsSource = options,
                    SelectedOption = options[0]
                };
            }),
            new PerfScenario("Select.Single.Filter.Closed", _ => new Select
            {
                IsFilterEnabled = true,
                OptionsSource   = CreateSelectOptions()
            }),
            new PerfScenario("Select.Single.Filter.PopupMaterialized", _ => MaterializePopupAfterLoaded(new Select
            {
                IsFilterEnabled = true,
                OptionsSource   = CreateSelectOptions()
            })),
            new PerfScenario("Select.Multiple.Empty", _ => new Select
            {
                Mode          = SelectMode.Multiple,
                OptionsSource = CreateSelectOptions()
            }),
            new PerfScenario("Select.Multiple.Selected", _ =>
            {
                var options = CreateSelectOptions();
                return new Select
                {
                    Mode            = SelectMode.Multiple,
                    OptionsSource   = options,
                    SelectedOptions = options.Take(2).ToList<ISelectOption>()
                };
            }),
            new PerfScenario("Select.Tags.Empty", _ => new Select
            {
                Mode          = SelectMode.Tags,
                OptionsSource = CreateSelectOptions()
            }),
            new PerfScenario("Select.MaxCountIndicator", _ =>
            {
                var options = CreateSelectOptions();
                return new Select
                {
                    Mode                    = SelectMode.Multiple,
                    IsShowMaxCountIndicator = true,
                    MaxCount                = 3,
                    OptionsSource           = options,
                    SelectedOptions         = options.Take(2).ToList<ISelectOption>()
                };
            }),
            new PerfScenario("Select.ContentRightAddOn", _ => new Select
            {
                ContentRightAddOn = new Avalonia.Controls.TextBlock { Text = "ms" },
                OptionsSource     = CreateSelectOptions()
            }),
            new PerfScenario("Select.AsyncLoading", _ =>
            {
                var select = new Select
                {
                    OptionsSource = CreateSelectOptions()
                };
                SetSelectLoadingForTest(select, true);
                return select;
            }),
            new PerfScenario("TreeSelect.Default.Closed", _ => new TreeSelect
            {
                ItemsSource = CreateTreeNodes()
            }),
            new PerfScenario("TreeSelect.Filter.DefaultExpandAll", _ => new TreeSelect
            {
                IsFilterEnabled   = true,
                IsDefaultExpandAll = true,
                ItemsSource        = CreateTreeNodes()
            })
        ];
    }

    private static List<SelectOption> CreateSelectOptions()
    {
        return
        [
            new SelectOption { Header = "Apple", Content = "apple" },
            new SelectOption { Header = "Orange", Content = "orange" },
            new SelectOption { Header = "Banana", Content = "banana" },
            new SelectOption { Header = "Pear", Content = "pear" },
            new SelectOption { Header = "Grape", Content = "grape" },
            new SelectOption { Header = "Mango", Content = "mango" }
        ];
    }

    private static T MaterializePopupAfterLoaded<T>(T select)
        where T : AbstractSelect
    {
        void HandleLoaded(object? sender, RoutedEventArgs args)
        {
            select.Loaded -= HandleLoaded;
            MaterializeLazyPopupContentForTest(select);
        }

        select.Loaded += HandleLoaded;
        return select;
    }

    private static List<ITreeItemNode> CreateTreeNodes()
    {
        return
        [
            new TreeItemNode
            {
                Header = "Parent 1",
                Value  = "parent-1",
                Children =
                [
                    new TreeItemNode
                    {
                        Header = "Child 1-1",
                        Value  = "child-1-1",
                        Children =
                        [
                            new TreeItemNode { Header = "Leaf 1-1-1", Value = "leaf-1-1-1" },
                            new TreeItemNode { Header = "Leaf 1-1-2", Value = "leaf-1-1-2" }
                        ]
                    },
                    new TreeItemNode { Header = "Child 1-2", Value = "child-1-2" }
                ]
            },
            new TreeItemNode
            {
                Header = "Parent 2",
                Value  = "parent-2",
                Children =
                [
                    new TreeItemNode { Header = "Leaf 2-1", Value = "leaf-2-1" },
                    new TreeItemNode { Header = "Leaf 2-2", Value = "leaf-2-2" }
                ]
            }
        ];
    }

}
