using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Controls.Utils;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class SelectShowCase : ReactiveUserControl<SelectViewModel>
{
    public SelectShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is SelectViewModel viewModel)
            {
                InitializeBasicOptions(viewModel);
                InitializeRandomOptions(viewModel);
                InitializeMaxTagCountOptions(viewModel);
                viewModel.SelectOptionsAsyncLoader = new SelectOptionsAsyncLoader();

                this.OneWayBind(viewModel, vm => vm.SelectOptionsAsyncLoader, v => v.AsyncLoadSelect.OptionsLoader)
                    .DisposeWith(disposables);
                
                this.OneWayBind(viewModel, vm => vm.BasicSelectedOptions, v => v.DefaultSelectedSelect.OptionsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.DefaultSelectedOptions, v => v.DefaultSelectedSelect.SelectedOptions)
                    .DisposeWith(disposables);
                
                this.OneWayBind(viewModel, vm => vm.RandomOptions, v => v.MultiSelect1.OptionsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.RandomOptions, v => v.MultiSelect2.OptionsSource)
                    .DisposeWith(disposables);
                
                this.OneWayBind(viewModel, vm => vm.RandomOptions, v => v.TagsModeSelect.OptionsSource)
                    .DisposeWith(disposables);
                
                this.OneWayBind(viewModel, vm => vm.RandomOptions, v => v.SizeSelect1.OptionsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.RandomOptions, v => v.SizeSelect2.OptionsSource)
                    .DisposeWith(disposables);
                
                this.OneWayBind(viewModel, vm => vm.MaxTagCountOptions, v => v.MaxTagSelect1.OptionsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.MaxTagCountOptions, v => v.MaxTagSelect2.OptionsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.MaxTagCountOptions, v => v.MaxTagSelect3.OptionsSource)
                    .DisposeWith(disposables);
                
                Disposable.Create(() =>
                {
                    viewModel.SelectOptionsAsyncLoader = null;
                    viewModel.BasicSelectedOptions     = null;
                    viewModel.RandomOptions            = null;
                    viewModel.MaxTagCountOptions       = null;
                    viewModel.DefaultSelectedOptions   = null;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
        CustomSearchSelect.Filter = new CustomFilter();
    }
    
    private void InitializeBasicOptions(SelectViewModel viewModel)
    {
        viewModel.BasicSelectedOptions = [
            new SelectOption()
            {
                Header  = "Jack",
                Content = "jack",
            },
            new SelectOption()
            {
                Header  = "Lucy",
                Content = "lucy",
            },
            new SelectOption()
            {
                Header  = "Yiminghe",
                Content = "yiminghe",
            },
            new SelectOption()
            {
                Header    = "Disabled",
                Content   = "disabled",
                IsEnabled = false
            }
        ];
        viewModel.DefaultSelectedOptions = [viewModel.BasicSelectedOptions[2]];
    }

    private void InitializeRandomOptions(SelectViewModel viewModel)
    {
        var options = new List<SelectOption>();
        for (var i = 10; i < 36; i++)
        {
            var base36Str = ConvertToBase36(i);
            options.Add(new SelectOption 
            {
                Header  = base36Str + i,
                Content = base36Str + i
            });
        }
        viewModel.RandomOptions = options;
    }
    
    private void InitializeMaxTagCountOptions(SelectViewModel viewModel)
    {
        var options = new List<SelectOption>();
        for (var i = 10; i < 36; i++)
        {
            var base36Str = ConvertToBase36(i);
            options.Add(new SelectOption 
            {
                Header  = $"Long label: {base36Str + i}",
                Content = base36Str + i
            });
        }
        viewModel.MaxTagCountOptions = options;
    }
    
    public static string ConvertToBase36(int num)
    {
        if (num == 0) return "0";
        const string chars  = "0123456789abcdefghijklmnopqrstuvwxyz";
        string       result = "";
        while (num > 0)
        {
            int remainder = num % 36;
            result =  chars[remainder] + result;
            num    /= 36;
        }
        return result;
    }

    private void HandleSizeTypeChanged(object? sender, OptionCheckedChangedEventArgs e)
    {
        if (DataContext is SelectViewModel viewModel)
        {
            if (e.CheckedOption.Tag is SizeType sizeType)
            {
                viewModel.SelectSizeType = sizeType;
            }
        }
    }
}

public record CustomOption : SelectOption
{
    public string? Description { get; init; }
    public string? Emoji { get; init; }
}

public class CustomFilter : IValueFilter
{
    public bool Filter(object? value, object? filterValue)
    {
        if (value is string valueStr && filterValue is string filterValueStr)
        {
            return valueStr.Contains(filterValueStr, StringComparison.Ordinal);
        }
        return false;
    }
}