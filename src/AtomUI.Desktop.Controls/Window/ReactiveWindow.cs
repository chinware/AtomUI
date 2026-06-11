using Avalonia;
using Avalonia.Interactivity;
using ReactiveUI;
using System;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// A ReactiveUI <see cref="Window"/> that implements the <see cref="IViewFor"/> interface and will
/// activate your ViewModel automatically if the view model implements <see cref="IActivatableViewModel"/>. When
/// the DataContext property changes, this class will update the ViewModel property with the new DataContext value,
/// and vice versa.
/// </summary>
/// <typeparam name="TViewModel">ViewModel type.</typeparam>
public class ReactiveWindow<TViewModel> : Window, IViewFor<TViewModel> where TViewModel : class
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1002",
        Justification = "Generic avalonia property is expected here.")]
    public static readonly StyledProperty<TViewModel?> ViewModelProperty = 
        AvaloniaProperty.Register<ReactiveWindow<TViewModel>, TViewModel?>(nameof(ViewModel));

    private IDisposable? _viewModelActivationDisposable;
    private object? _activatedViewModel;
    private bool _isViewActivated;
    private bool _isUpdatingViewModelActivation;
    private bool _hasPendingViewModelActivationUpdate;

    /// <summary>
    /// The ViewModel.
    /// </summary>
    public TViewModel? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == DataContextProperty)
        {
            if (ReferenceEquals(change.OldValue, ViewModel)
                && change.NewValue is null or TViewModel)
            {
                SetCurrentValue(ViewModelProperty, change.NewValue);
            }
        }
        else if (change.Property == ViewModelProperty)
        {
            if (ReferenceEquals(change.OldValue, DataContext))
            {
                SetCurrentValue(DataContextProperty, change.NewValue);
            }

            if (_isViewActivated && !ReferenceEquals(change.OldValue, change.NewValue))
            {
                UpdateViewModelActivation();
            }
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (_isViewActivated)
        {
            return;
        }

        _isViewActivated = true;
        UpdateViewModelActivation();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        if (!_isViewActivated)
        {
            base.OnUnloaded(e);
            return;
        }

        _isViewActivated = false;
        UpdateViewModelActivation();

        base.OnUnloaded(e);
    }

    private void UpdateViewModelActivation()
    {
        if (_isUpdatingViewModelActivation)
        {
            _hasPendingViewModelActivationUpdate = true;
            return;
        }

        _isUpdatingViewModelActivation = true;
        try
        {
            do
            {
                _hasPendingViewModelActivationUpdate = false;
                var targetViewModel = _isViewActivated ? ViewModel : null;
                if (ReferenceEquals(_activatedViewModel, targetViewModel))
                {
                    continue;
                }

                DeactivateViewModel();

                targetViewModel = _isViewActivated ? ViewModel : null;
                if (targetViewModel is IActivatableViewModel activatableViewModel)
                {
                    var activationDisposable = activatableViewModel.Activator.Activate();
                    _activatedViewModel = targetViewModel;
                    _viewModelActivationDisposable = activationDisposable;
                }
            } while (_hasPendingViewModelActivationUpdate);
        }
        finally
        {
            _isUpdatingViewModelActivation = false;
        }
    }

    private void DeactivateViewModel()
    {
        var activationDisposable = _viewModelActivationDisposable;
        _viewModelActivationDisposable = null;
        _activatedViewModel = null;
        activationDisposable?.Dispose();
    }
}
