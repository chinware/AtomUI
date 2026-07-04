using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using AtomUI.Controls.Data;
using AtomUI.Desktop.Controls.Data;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.Filtering;

public class DataGridColumnFilterBindingTests
{
    static DataGridColumnFilterBindingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Filters_Can_Be_Assigned_From_ViewModel_Source()
    {
        var filters = new ObservableCollection<DataGridFilterItem>
        {
            new() { Text = "London", Value = "London" },
            new() { Text = "New York", Value = "New York" }
        };

        var column = new DataGridTextColumn
        {
            Header  = "Address",
            Binding = new Binding(nameof(FilterRow.Address)),
            Filters = filters
        };

        column.Filters.ShouldBeSameAs(filters);
    }

    [Fact]
    public void Filters_Can_Project_Generated_Filter_Item_Dto()
    {
        var parent = new FilterOption
        {
            Label = "Europe",
            Code  = "Europe"
        };
        parent.Children.Add(new FilterOption
        {
            Label = "London",
            Code  = "London"
        });

        var column = new DataGridTextColumn
        {
            Filters                  = new ObservableCollection<FilterOption> { parent },
            FilterTextMemberPath     = nameof(FilterOption.Label),
            FilterValueMemberPath    = nameof(FilterOption.Code),
            FilterChildrenMemberPath = nameof(FilterOption.Children)
        };

        var filterItem = column.GetEffectiveFilterItems().Single();

        filterItem.Text.ShouldBe("Europe");
        filterItem.Value.ShouldBe("Europe");
        filterItem.Children.Count.ShouldBe(1);
        filterItem.Children[0].Text.ShouldBe("London");
        filterItem.Children[0].Value.ShouldBe("London");
    }


    [Fact]
    public void SelectedFilterValues_Rebuilds_FilterDescription_When_Set_After_Attach()
    {
        var column = new DataGridTextColumn
        {
            Header           = "Address",
            Binding          = new Binding(nameof(FilterRow.Address)),
            FilterMemberPath = nameof(FilterRow.Address),
            Filters = new ObservableCollection<DataGridFilterItem>
            {
                new() { Text = "London", Value = "London" },
                new() { Text = "New York", Value = "New York" }
            }
        };
        var trailingColumn = new DataGridTextColumn
        {
            Header  = "Trailing",
            Binding = new Binding(nameof(FilterRow.Address))
        };
        var (window, grid) = CreateShownWindow(column);
        grid.Columns.Add(trailingColumn);
        Dispatcher.UIThread.RunJobs();

        try
        {
            column.SelectedFilterValues = new ObservableCollection<object> { "London" };
            Dispatcher.UIThread.RunJobs();

            var filter = GetSingleFilterDescription(grid);
            filter.PropertyPath.ShouldBe(nameof(FilterRow.Address));
            filter.FilterConditions.ShouldBe(["London"]);

            column.SelectedFilterValues = new ObservableCollection<object> { "New York" };
            Dispatcher.UIThread.RunJobs();

            filter = GetSingleFilterDescription(grid);
            filter.FilterConditions.ShouldBe(["New York"]);

            column.SelectedFilterValues = new ObservableCollection<object>();
            Dispatcher.UIThread.RunJobs();

            grid.CollectionView!.FilterDescriptions.ShouldBeEmpty();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void SelectedFilterValues_CollectionMutation_Rebuilds_FilterDescription()
    {
        var selectedValues = new ObservableCollection<object>();
        var column = new DataGridTextColumn
        {
            Header               = "Address",
            Binding              = new Binding(nameof(FilterRow.Address)),
            FilterMemberPath     = nameof(FilterRow.Address),
            SelectedFilterValues = selectedValues,
            Filters = new ObservableCollection<DataGridFilterItem>
            {
                new() { Text = "London", Value = "London" },
                new() { Text = "New York", Value = "New York" }
            }
        };
        var (window, grid) = CreateShownWindow(column);

        try
        {
            selectedValues.Add("London");
            Dispatcher.UIThread.RunJobs();

            var filter = GetSingleFilterDescription(grid);
            filter.FilterConditions.ShouldBe(["London"]);

            selectedValues.Clear();
            Dispatcher.UIThread.RunJobs();

            grid.CollectionView!.FilterDescriptions.ShouldBeEmpty();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Filters_CollectionMutation_Prunes_SelectedFilterValues()
    {
        var selectedValues = new ObservableCollection<object> { "London", "New York" };
        var filters = new ObservableCollection<DataGridFilterItem>
        {
            new() { Text = "London", Value = "London" },
            new() { Text = "New York", Value = "New York" }
        };
        var column = new DataGridTextColumn
        {
            Header               = "Address",
            Binding              = new Binding(nameof(FilterRow.Address)),
            FilterMemberPath     = nameof(FilterRow.Address),
            SelectedFilterValues = selectedValues,
            Filters              = filters
        };
        var (window, grid) = CreateShownWindow(column);

        try
        {
            GetSingleFilterDescription(grid).FilterConditions.ShouldBe(["London", "New York"]);

            filters.RemoveAt(0);
            Dispatcher.UIThread.RunJobs();

            column.SelectedFilterValues.ShouldNotBeNull();
            column.SelectedFilterValues.Cast<object>().ShouldBe(["New York"]);
            GetSingleFilterDescription(grid).FilterConditions.ShouldBe(["New York"]);

            filters.Clear();
            Dispatcher.UIThread.RunJobs();

            column.SelectedFilterValues.Cast<object>().ShouldBeEmpty();
            grid.CollectionView!.FilterDescriptions.ShouldBeEmpty();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }


    [Fact]
    public void Column_Filter_Method_Updates_SelectedFilterValues()
    {
        var column = new DataGridTextColumn
        {
            Header           = "Address",
            Binding          = new Binding(nameof(FilterRow.Address)),
            FilterMemberPath = nameof(FilterRow.Address),
            Filters = new ObservableCollection<DataGridFilterItem>
            {
                new() { Text = "London", Value = "London" },
                new() { Text = "New York", Value = "New York" }
            }
        };
        var (window, grid) = CreateShownWindow(column);

        try
        {
            column.Filter(["London"]);
            Dispatcher.UIThread.RunJobs();

            column.SelectedFilterValues.ShouldNotBeNull();
            column.SelectedFilterValues.Cast<object>().ShouldBe(["London"]);
            GetSingleFilterDescription(grid).FilterConditions.ShouldBe(["London"]);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Column_Filter_Method_Mutates_Existing_SelectedFilterValues_Source()
    {
        var selectedValues = new ObservableCollection<object>();
        var column = new DataGridTextColumn
        {
            Header               = "Address",
            Binding              = new Binding(nameof(FilterRow.Address)),
            FilterMemberPath     = nameof(FilterRow.Address),
            SelectedFilterValues = selectedValues,
            Filters = new ObservableCollection<DataGridFilterItem>
            {
                new() { Text = "London", Value = "London" },
                new() { Text = "New York", Value = "New York" }
            }
        };
        var (window, grid) = CreateShownWindow(column);

        try
        {
            column.Filter(["New York"]);
            Dispatcher.UIThread.RunJobs();

            column.SelectedFilterValues.ShouldBeSameAs(selectedValues);
            selectedValues.ShouldBe(["New York"]);
            GetSingleFilterDescription(grid).FilterConditions.ShouldBe(["New York"]);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Column_Filter_Method_Reapplies_Existing_SelectedFilterValues_When_Projection_Missing()
    {
        var selectedValues = new ObservableCollection<object> { "London" };
        var column = new DataGridTextColumn
        {
            Header               = "Address",
            Binding              = new Binding(nameof(FilterRow.Address)),
            FilterMemberPath     = nameof(FilterRow.Address),
            SelectedFilterValues = selectedValues,
            Filters = new ObservableCollection<DataGridFilterItem>
            {
                new() { Text = "London", Value = "London" },
                new() { Text = "New York", Value = "New York" }
            }
        };
        var (window, grid) = CreateShownWindow(column);

        try
        {
            var filterDescriptions = grid.CollectionView!.FilterDescriptions;
            filterDescriptions.ShouldNotBeNull();
            filterDescriptions.Clear();
            Dispatcher.UIThread.RunJobs();
            filterDescriptions.ShouldBeEmpty();

            column.Filter(["London"]);
            Dispatcher.UIThread.RunJobs();

            column.SelectedFilterValues.ShouldBeSameAs(selectedValues);
            selectedValues.ShouldBe(["London"]);
            GetSingleFilterDescription(grid).FilterConditions.ShouldBe(["London"]);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void ClearFilters_Clears_SelectedFilterValues_And_FilterDescriptions()
    {
        var column = new DataGridTextColumn
        {
            Header               = "Address",
            Binding              = new Binding(nameof(FilterRow.Address)),
            FilterMemberPath     = nameof(FilterRow.Address),
            SelectedFilterValues = new ObservableCollection<object> { "London" },
            Filters = new ObservableCollection<DataGridFilterItem>
            {
                new() { Text = "London", Value = "London" },
                new() { Text = "New York", Value = "New York" }
            }
        };
        var (window, grid) = CreateShownWindow(column);

        try
        {
            GetSingleFilterDescription(grid).FilterConditions.ShouldBe(["London"]);

            grid.ClearFilters();
            Dispatcher.UIThread.RunJobs();

            column.SelectedFilterValues.ShouldNotBeNull();
            column.SelectedFilterValues.Cast<object>().ShouldBeEmpty();
            grid.CollectionView!.FilterDescriptions.ShouldBeEmpty();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Column_Filter_Bindings_Use_OwningGrid_DataContext()
    {
        var firstFilters = new ObservableCollection<DataGridFilterItem>
        {
            new() { Text = "London", Value = "London" },
            new() { Text = "New York", Value = "New York" }
        };
        var firstSelectedValues = new ObservableCollection<object> { "London" };
        var firstViewModel = new FilterBindingViewModel
        {
            AddressFilters    = firstFilters,
            SelectedAddresses = firstSelectedValues
        };
        var column = new DataGridTextColumn
        {
            Header           = "Address",
            Binding          = new Binding(nameof(FilterRow.Address)),
            FilterMemberPath = nameof(FilterRow.Address)
        };
        column.Bind(DataGridColumn.FiltersProperty, new Binding(nameof(FilterBindingViewModel.AddressFilters)));
        column.Bind(DataGridColumn.SelectedFilterValuesProperty, new Binding(nameof(FilterBindingViewModel.SelectedAddresses)));

        var (window, grid) = CreateShownWindow(column, firstViewModel);

        try
        {
            column.Filters.ShouldBeSameAs(firstFilters);
            column.SelectedFilterValues.ShouldBeSameAs(firstSelectedValues);
            GetSingleFilterDescription(grid).FilterConditions.ShouldBe(["London"]);

            var secondFilters = new ObservableCollection<DataGridFilterItem>
            {
                new() { Text = "Tokyo", Value = "Tokyo" }
            };
            var secondSelectedValues = new ObservableCollection<object> { "Tokyo" };
            grid.DataContext = new FilterBindingViewModel
            {
                AddressFilters    = secondFilters,
                SelectedAddresses = secondSelectedValues
            };
            Dispatcher.UIThread.RunJobs();

            column.Filters.ShouldBeSameAs(secondFilters);
            column.SelectedFilterValues.ShouldBeSameAs(secondSelectedValues);
            GetSingleFilterDescription(grid).FilterConditions.ShouldBe(["Tokyo"]);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Column_Filter_Method_Updates_Bound_SelectedFilterValues_Source()
    {
        var viewModel = new FilterBindingViewModel
        {
            AddressFilters =
            [
                new() { Text = "London", Value = "London" },
                new() { Text = "New York", Value = "New York" }
            ],
            SelectedAddresses = new ObservableCollection<object>()
        };
        var column = new DataGridTextColumn
        {
            Header           = "Address",
            Binding          = new Binding(nameof(FilterRow.Address)),
            FilterMemberPath = nameof(FilterRow.Address)
        };
        column.Bind(DataGridColumn.FiltersProperty, new Binding(nameof(FilterBindingViewModel.AddressFilters)));
        column.Bind(DataGridColumn.SelectedFilterValuesProperty, new Binding(nameof(FilterBindingViewModel.SelectedAddresses)));

        var (window, grid) = CreateShownWindow(column, viewModel);

        try
        {
            column.Filter(["New York"]);
            Dispatcher.UIThread.RunJobs();

            viewModel.SelectedAddresses.ShouldNotBeNull();
            viewModel.SelectedAddresses.Cast<object>().ShouldBe(["New York"]);
            column.SelectedFilterValues.ShouldBeSameAs(viewModel.SelectedAddresses);
            GetSingleFilterDescription(grid).FilterConditions.ShouldBe(["New York"]);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Column_Removal_Releases_Filter_Source_Subscriptions()
    {
        var filters = new TrackedObservableCollection<DataGridFilterItem>
        {
            new() { Text = "London", Value = "London" }
        };
        var selectedValues = new TrackedObservableCollection<object> { "London" };
        var column = new DataGridTextColumn
        {
            Header               = "Address",
            Binding              = new Binding(nameof(FilterRow.Address)),
            FilterMemberPath     = nameof(FilterRow.Address),
            Filters              = filters,
            SelectedFilterValues = selectedValues
        };
        var leadingColumn = new DataGridTextColumn
        {
            Header  = "Leading",
            Binding = new Binding(nameof(FilterRow.Address))
        };
        var grid = new global::AtomUI.Desktop.Controls.DataGrid
        {
            AutoGenerateColumns  = false,
            CanUserFilterColumns = true,
            Width                = 480,
            Height               = 240
        };
        grid.Columns.Add(leadingColumn);
        grid.Columns.Add(column);

        var window = new Window
        {
            Width   = 520,
            Height  = 280,
            Content = grid
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();

        try
        {
            var filterSubscriptionCount = filters.CollectionChangedHandlerCount;
            filterSubscriptionCount.ShouldBeGreaterThan(0);
            selectedValues.CollectionChangedHandlerCount.ShouldBe(1);

            grid.Columns.Remove(column);
            Dispatcher.UIThread.RunJobs();

            filters.CollectionChangedHandlerCount.ShouldBeLessThan(filterSubscriptionCount);
            selectedValues.CollectionChangedHandlerCount.ShouldBe(0);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static (Window Window, global::AtomUI.Desktop.Controls.DataGrid Grid) CreateShownWindow(
        DataGridTextColumn column,
        object? dataContext = null)
    {
        var grid = new global::AtomUI.Desktop.Controls.DataGrid
        {
            AutoGenerateColumns   = false,
            CanUserFilterColumns  = true,
            DataContext           = dataContext,
            ItemsSource           = new ObservableCollection<FilterRow>
            {
                new("London"),
                new("New York"),
                new("Tokyo")
            },
            Width                 = 480,
            Height                = 240
        };
        grid.Columns.Add(column);

        var window = new Window
        {
            Width   = 520,
            Height  = 280,
            Content = grid
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        return (window, grid);
    }

    private static DataGridFilterDescription GetSingleFilterDescription(global::AtomUI.Desktop.Controls.DataGrid grid)
    {
        var filterDescriptions = grid.CollectionView!.FilterDescriptions;
        filterDescriptions.ShouldNotBeNull();
        filterDescriptions.Count.ShouldBe(1);
        return filterDescriptions[0];
    }

}

[GenerateDataMemberAccessors]
internal sealed partial record FilterRow(string Address);

[GenerateDataMemberAccessors]
internal sealed partial class FilterOption
{
    public string Label { get; set; } = string.Empty;
    public object Code { get; set; } = string.Empty;
    public ObservableCollection<FilterOption> Children { get; set; } = new();
}

internal sealed class FilterBindingViewModel
{
    public ObservableCollection<DataGridFilterItem>? AddressFilters { get; set; }
    public IList? SelectedAddresses { get; set; }
}

internal sealed class TrackedObservableCollection<T> : ObservableCollection<T>
{
    public int CollectionChangedHandlerCount { get; private set; }

    public override event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add
        {
            CollectionChangedHandlerCount++;
            base.CollectionChanged += value;
        }
        remove
        {
            CollectionChangedHandlerCount--;
            base.CollectionChanged -= value;
        }
    }
}
