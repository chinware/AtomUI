using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;

namespace AtomUIGallery.Controls;

public class GalleryReactiveUserControl<TViewModel> : UserControl, IViewFor<TViewModel>
    where TViewModel : class
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("AvaloniaProperty", "AVP1002",
        Justification = "Generic avalonia property is expected here.")]
    public static readonly StyledProperty<TViewModel?> ViewModelProperty =
        AvaloniaProperty.Register<GalleryReactiveUserControl<TViewModel>, TViewModel?>(nameof(ViewModel));

    private readonly List<ActivationRegistration> _activationRegistrations = [];
    private CompositeDisposable? _activationDisposables;
    private IDisposable? _viewModelActivationDisposable;
    private object? _activatedViewModel;
    private bool _isViewActivated;
    private bool _isUpdatingViewModelActivation;
    private bool _hasPendingViewModelActivationUpdate;

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

    public IDisposable WhenActivated(Action<CompositeDisposable> activationBlock)
    {
        ArgumentNullException.ThrowIfNull(activationBlock);

        var registration = new ActivationRegistration(this, activationBlock);
        _activationRegistrations.Add(registration);
        try
        {
            if (_isViewActivated && _activationDisposables is not null)
            {
                registration.Activate(_activationDisposables);
            }
        }
        catch
        {
            registration.Dispose();
            throw;
        }

        return registration;
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
        _activationDisposables = new CompositeDisposable();
        try
        {
            foreach (var activationRegistration in _activationRegistrations.ToArray())
            {
                activationRegistration.Activate(_activationDisposables);
            }
        }
        catch
        {
            _isViewActivated = false;
            UpdateViewModelActivation();
            DisposeViewActivation();
            throw;
        }
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        if (_isViewActivated)
        {
            _isViewActivated = false;
            UpdateViewModelActivation();
            DisposeViewActivation();
        }

        base.OnUnloaded(e);
    }

    private void DisposeViewActivation()
    {
        var activationDisposables = _activationDisposables;
        _activationDisposables = null;
        activationDisposables?.Dispose();
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

    private sealed class ActivationRegistration : IDisposable
    {
        private readonly GalleryReactiveUserControl<TViewModel> _owner;
        private readonly Action<CompositeDisposable> _activationBlock;
        private CompositeDisposable? _activeDisposables;
        private CompositeDisposable? _viewDisposables;
        private bool _disposed;

        public ActivationRegistration(
            GalleryReactiveUserControl<TViewModel> owner,
            Action<CompositeDisposable> activationBlock)
        {
            _owner           = owner;
            _activationBlock = activationBlock;
        }

        public void Activate(CompositeDisposable viewDisposables)
        {
            if (_disposed)
            {
                return;
            }

            DisposeActiveDisposables();

            var activeDisposables = new CompositeDisposable();
            _activeDisposables = activeDisposables;
            _viewDisposables   = viewDisposables;
            viewDisposables.Add(activeDisposables);
            try
            {
                _activationBlock(activeDisposables);
            }
            catch
            {
                DisposeActiveDisposables();
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _owner._activationRegistrations.Remove(this);
            DisposeActiveDisposables();
        }

        private void DisposeActiveDisposables()
        {
            var activeDisposables = _activeDisposables;
            var viewDisposables   = _viewDisposables;
            _activeDisposables = null;
            _viewDisposables   = null;

            if (activeDisposables is null)
            {
                return;
            }

            if (viewDisposables?.Remove(activeDisposables) != true)
            {
                activeDisposables.Dispose();
            }
        }
    }
}
