using System.ComponentModel;
using AtomUI.Controls.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Data;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AtomListView = AtomUI.Desktop.Controls.ListView;
using AtomPagination = AtomUI.Desktop.Controls.Pagination;
using AtomSteps = AtomUI.Desktop.Controls.Steps;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.SelectionSemantics;

public class ControlledStateBindingTests
{
    static ControlledStateBindingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void P2_Controlled_State_Properties_Have_TwoWay_Metadata()
    {
        AssertTwoWayAndDataValidation(AtomListView.SelectedItemsProperty, typeof(AtomListView));
        AssertTwoWay(AtomPagination.CurrentPageProperty, typeof(AtomPagination));
        AssertTwoWay(AtomPagination.PageSizeProperty, typeof(AtomPagination));
        AssertTwoWay(AtomSteps.CurrentStepProperty, typeof(AtomSteps));
    }

    [Fact]
    public void Pagination_CurrentPage_DefaultBindingMode_Updates_ViewModel()
    {
        var viewModel = new ControlledStateBindingViewModel
        {
            CurrentPage = 1
        };
        var pagination = new AtomPagination
        {
            Total = 100
        };
        pagination.Bind(
            AtomPagination.CurrentPageProperty,
            new Binding(nameof(ControlledStateBindingViewModel.CurrentPage))
            {
                Source = viewModel
            });

        pagination.CurrentPage.ShouldBe(1);

        pagination.SetCurrentValue(AtomPagination.CurrentPageProperty, 2);
        Dispatcher.UIThread.RunJobs();

        viewModel.CurrentPage.ShouldBe(2);
    }

    [Fact]
    public void Pagination_PageSize_DefaultBindingMode_Updates_ViewModel()
    {
        var viewModel = new ControlledStateBindingViewModel
        {
            PageSize = 10
        };
        var pagination = new AtomPagination
        {
            Total = 100
        };
        pagination.Bind(
            AtomPagination.PageSizeProperty,
            new Binding(nameof(ControlledStateBindingViewModel.PageSize))
            {
                Source = viewModel
            });

        pagination.PageSize.ShouldBe(10);

        pagination.SetCurrentValue(AtomPagination.PageSizeProperty, 20);
        Dispatcher.UIThread.RunJobs();

        viewModel.PageSize.ShouldBe(20);
    }

    [Fact]
    public void Steps_CurrentStep_DefaultBindingMode_Updates_ViewModel()
    {
        var viewModel = new ControlledStateBindingViewModel
        {
            CurrentStep = 0
        };
        var steps = new AtomSteps();
        steps.Bind(
            AtomSteps.CurrentStepProperty,
            new Binding(nameof(ControlledStateBindingViewModel.CurrentStep))
            {
                Source = viewModel
            });

        steps.CurrentStep.ShouldBe(0);

        steps.SetCurrentValue(AtomSteps.CurrentStepProperty, 2);
        Dispatcher.UIThread.RunJobs();

        viewModel.CurrentStep.ShouldBe(2);
    }

    [Fact]
    public void Steps_SelectedIndex_Updates_CurrentStep_And_ViewModel()
    {
        var viewModel = new ControlledStateBindingViewModel
        {
            CurrentStep = 0
        };
        var steps = new AtomSteps
        {
            IsItemClickable = true,
            Items =
            {
                new StepsItem { Header = "First" },
                new StepsItem { Header = "Second" },
                new StepsItem { Header = "Third" }
            }
        };
        steps.Bind(
            AtomSteps.CurrentStepProperty,
            new Binding(nameof(ControlledStateBindingViewModel.CurrentStep))
            {
                Source = viewModel
            });

        ShowInWindow(steps, () =>
        {
            steps.SelectedIndex = 2;
            Dispatcher.UIThread.RunJobs();

            steps.CurrentStep.ShouldBe(2);
            viewModel.CurrentStep.ShouldBe(2);
        });
    }

    [Fact]
    public void ListView_SelectedItems_DefaultBindingMode_Updates_ViewModel_When_Property_Replaced()
    {
        var first = new ListItemData { Content = "First" };
        var second = new ListItemData { Content = "Second" };
        var replacement = new System.Collections.ArrayList { second };
        var viewModel = new ControlledStateBindingViewModel
        {
            SelectedItems = new System.Collections.ArrayList { first }
        };
        var listView = new AtomListView
        {
            ItemsSource = new[] { first, second },
            SelectionMode = SelectionMode.Multiple
        };
        listView.Bind(
            AtomListView.SelectedItemsProperty,
            new Binding(nameof(ControlledStateBindingViewModel.SelectedItems))
            {
                Source = viewModel
            });

        listView.SelectedItems.ShouldBeSameAs(viewModel.SelectedItems);

        listView.SelectedItems = replacement;
        Dispatcher.UIThread.RunJobs();

        viewModel.SelectedItems.ShouldBeSameAs(replacement);
    }

    private static void AssertTwoWay(AvaloniaProperty property, Type ownerType)
    {
        var metadata = property.GetMetadata(ownerType);

        metadata.DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
    }

    private static void AssertTwoWayAndDataValidation(AvaloniaProperty property, Type ownerType)
    {
        var metadata = property.GetMetadata(ownerType);

        metadata.DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
        metadata.EnableDataValidation.ShouldBe(true);
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width = 640,
            Height = 240,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }

    private sealed class ControlledStateBindingViewModel : INotifyPropertyChanged
    {
        private int _currentPage;
        private int _pageSize;
        private int _currentStep;
        private System.Collections.IList? _selectedItems;

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage == value)
                {
                    return;
                }

                _currentPage = value;
                RaisePropertyChanged(nameof(CurrentPage));
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (_pageSize == value)
                {
                    return;
                }

                _pageSize = value;
                RaisePropertyChanged(nameof(PageSize));
            }
        }

        public int CurrentStep
        {
            get => _currentStep;
            set
            {
                if (_currentStep == value)
                {
                    return;
                }

                _currentStep = value;
                RaisePropertyChanged(nameof(CurrentStep));
            }
        }

        public System.Collections.IList? SelectedItems
        {
            get => _selectedItems;
            set
            {
                if (ReferenceEquals(_selectedItems, value))
                {
                    return;
                }

                _selectedItems = value;
                RaisePropertyChanged(nameof(SelectedItems));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
